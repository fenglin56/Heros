using UnityEngine;
using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class MapResConfigAssetPostProcessor : AssetPostprocessor
{
    public class MonsterResGroup
    {
        public int monsterId;
        public string MonsterPath;
        public string MonsterBloodEffectPath;
    }


    public class MapResTempConfig
    {
        public int m_mapId;
        public Dictionary<int,MonsterResGroup> monsterGroupList;
        public List<string> effectPathList;
        public List<string> soundSfxList;
    }






    private static readonly string RESOURCE_MAPRES_CONFIG_FOLDER = "Assets/Data/MapResConfig/Res";
    private static readonly string ASSET_MAPRES_CONFIG_FOLDER = "Assets/Data/MapResConfig/Data";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            MapResConfigPostprocess();
        }
    }

    private static void MapResConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_MAPRES_CONFIG_FOLDER, "resource_in_map.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();
    
        if (text == null)
        {
            Debug.LogError("Map Res data file not exist");

            return;
        } 
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys =  XmlSpreadSheetReader.Keys;
            
            object[] mapIds = sheet[keys[0]];

            Dictionary<int, MapResTempConfig> mapResDic = new Dictionary<int, MapResTempConfig>();
            for(int i = 2; i < mapIds.Length; i++)
            {
                int mapId = Convert.ToInt32( sheet["Map_ID"][i]);


                if(mapResDic.ContainsKey(mapId))
                {
                    //read data
                    int monsterId = Convert.ToInt32(sheet["Monster_ID"][i]);

                    string monsterandBlood = Convert.ToString(sheet["Monster_Prefab"][i]);
                    string[] splitMonsandBlood = monsterandBlood.Split('+');
                    string monsterPath = splitMonsandBlood[0];
                    string monsterBloodPath = splitMonsandBlood[1];
                    string monsterEffectsStr = Convert.ToString(sheet["Monster_Effects"][i]);
                    string[] monsterEfectSplitStrs = monsterEffectsStr.Split('+');
                    List<string> monsterEffectsList = monsterEfectSplitStrs.ToList();
                    monsterEffectsList = monsterEffectsList.Distinct().ToList();

                    string monsterSfxStr = Convert.ToString(sheet["Monster_sound"][i]);
                    string[] monsterSfxSplitStrs = monsterSfxStr.Split('+');
                    List<string> monsterSfxList = monsterSfxSplitStrs.ToList();
                    monsterSfxList = monsterSfxList.Distinct().ToList();

                    //modify the exist config
                    MapResTempConfig config = mapResDic[mapId];

                    if(!config.monsterGroupList.ContainsKey(monsterId))
                    {
                        MonsterResGroup monsGroup = new MonsterResGroup();
                        monsGroup.monsterId = monsterId;
                        monsGroup.MonsterPath = monsterPath;
                        monsGroup.MonsterBloodEffectPath = monsterBloodPath;
                        config.monsterGroupList.Add(monsterId, monsGroup);
                        config.effectPathList = config.effectPathList.Union(monsterEffectsList).ToList();

                        if(monsterSfxList[0] != "0")
                        {
                            config.soundSfxList = config.soundSfxList.Union(monsterSfxList).ToList();
                        }
                    }

                }
                else
                {
                    //read data
                    int monsterId = Convert.ToInt32(sheet["Monster_ID"][i]);
                    string monsterandBlood = Convert.ToString(sheet["Monster_Prefab"][i]);
                    string[] splitMonsandBlood = monsterandBlood.Split('+');
                    string monsterPath = splitMonsandBlood[0];
                    string monsterBloodPath = splitMonsandBlood[1];

                    //string monsterPath = Convert.ToString(sheet["Monster_Prefab"][i]);
                    string monsterEffectsStr = Convert.ToString(sheet["Monster_Effects"][i]);
                    string[] monsterEfectSplitStrs = monsterEffectsStr.Split('+');
                    List<string> monsterEffectsList = monsterEfectSplitStrs.ToList();
                    monsterEffectsList = monsterEffectsList.Distinct().ToList();

                    string monsterSfxStr = Convert.ToString(sheet["Monster_sound"][i]);
                    string[] monsterSfxSplitStrs = monsterSfxStr.Split('+');
                    List<string> monsterSfxList = monsterSfxSplitStrs.ToList();
                    monsterSfxList = monsterSfxList.Distinct().ToList();

                    //new config
                    MapResTempConfig config = new MapResTempConfig();

                    config.m_mapId = mapId;
                    config.monsterGroupList = new Dictionary<int, MonsterResGroup>();

                    config.effectPathList = monsterEffectsList;
                    config.soundSfxList = new List<string>();
                    if(monsterSfxList[0] != "0")
                    {
                        config.soundSfxList = monsterSfxList;
                    }

                    MonsterResGroup monsGroup = new MonsterResGroup();
                    monsGroup.monsterId = monsterId;
                    monsGroup.MonsterPath = monsterPath;
                    monsGroup.MonsterBloodEffectPath = monsterBloodPath;
                    config.monsterGroupList.Add(monsterId, monsGroup);


                    mapResDic.Add(config.m_mapId, config);
                }

            }

            GreateMapResDataBases(mapResDic);

        }

    }

    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_MAPRES_CONFIG_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }

    private static void GreateMapResDataBases(Dictionary<int, MapResTempConfig> mapResDic)
    {
        foreach(int key in mapResDic.Keys)
        {
            MapResTempConfig config = mapResDic[key];
            string path = System.IO.Path.Combine(ASSET_MAPRES_CONFIG_FOLDER, config.m_mapId.ToString() + "MapResDataBase.asset");

            if(File.Exists(path))
            {
                MapResDataBase dataBase = (MapResDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(MapResDataBase));
                dataBase.m_monsters = new MonsterObjectGroup[config.monsterGroupList.Count];
                int i = 0;
                foreach(int mgkey in config.monsterGroupList.Keys)
                {
                    MonsterResGroup msg = config.monsterGroupList[mgkey];
                    dataBase.m_monsters[i] = new MonsterObjectGroup();
                    dataBase.m_monsters[i].m_monsterId = msg.monsterId;
                    string pathMonsterPrefab = System.IO.Path.Combine("Assets/", msg.MonsterPath + ".prefab");
                    dataBase.m_monsters[i].m_monsterPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathMonsterPrefab, typeof(GameObject));
					if(dataBase.m_monsters[i].m_monsterPrefab == null)
					{
						Debug.LogError("monster is null, path : " + pathMonsterPrefab);
					}

                    string pathMonsterBloodEffectPrefab = System.IO.Path.Combine("Assets/", msg.MonsterBloodEffectPath + ".prefab");
                    dataBase.m_monsters[i].m_monsterBloodEffectPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathMonsterBloodEffectPrefab, typeof(GameObject));

                    i++;
                }

                dataBase.m_effects = new EffectObjGroup[config.effectPathList.Count];
                i = 0;
                foreach(string ePath in config.effectPathList)
                {
                    dataBase.m_effects[i] = new EffectObjGroup();
                    dataBase.m_effects[i].m_effectPath = ePath;
                    string pathEffectPrefab = System.IO.Path.Combine("Assets/", ePath + ".prefab");
                    dataBase.m_effects[i].m_effectPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathEffectPrefab, typeof(GameObject));
					if(dataBase.m_effects[i].m_effectPrefab == null)
					{
						Debug.LogError("effect is null, path : " + pathEffectPrefab);
					}


                    i++;
                }

                dataBase.m_sfxClips = new SoundClip[config.soundSfxList.Count];
                i = 0;
                foreach(string sPath in config.soundSfxList)
                {
                    dataBase.m_sfxClips[i] = new SoundClip();
                    dataBase.m_sfxClips[i]._name = sPath;
                    string pathEffectPrefab = System.IO.Path.Combine("Assets/Sound/SoundFX", sPath + ".wav");
                    dataBase.m_sfxClips[i]._clip = (AudioClip)AssetDatabase.LoadAssetAtPath(pathEffectPrefab, typeof(AudioClip));
                    i++;
                }
                EditorUtility.SetDirty(dataBase);
            }
            else
            {
                MapResDataBase dataBase = ScriptableObject.CreateInstance<MapResDataBase>();
                dataBase.m_monsters = new MonsterObjectGroup[config.monsterGroupList.Count];
                int i = 0;
                foreach(int mgkey in config.monsterGroupList.Keys)
                {
                    MonsterResGroup msg = config.monsterGroupList[mgkey];
                    dataBase.m_monsters[i] = new MonsterObjectGroup();
                    dataBase.m_monsters[i].m_monsterId = msg.monsterId;
                    string pathMonsterPrefab = System.IO.Path.Combine("Assets/Enemy/", msg.MonsterPath + ".prefab");
                    dataBase.m_monsters[i].m_monsterPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathMonsterPrefab, typeof(GameObject));
                    string pathMonsterBloodEffectPrefab = System.IO.Path.Combine("Assets/", msg.MonsterBloodEffectPath + ".prefab");
                    dataBase.m_monsters[i].m_monsterBloodEffectPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathMonsterBloodEffectPrefab, typeof(GameObject));
					if(dataBase.m_monsters[i].m_monsterPrefab == null)
					{
						Debug.LogError("monster is null, path : " + pathMonsterPrefab);
					}
                    i++;
                }
                dataBase.m_effects = new EffectObjGroup[config.effectPathList.Count];
                i = 0;
                foreach(string ePath in config.effectPathList)
                {
                    dataBase.m_effects[i] = new EffectObjGroup();
                    dataBase.m_effects[i].m_effectPath = ePath;
                    string pathEffectPrefab = System.IO.Path.Combine("Assets/", ePath + ".prefab");
                    dataBase.m_effects[i].m_effectPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathEffectPrefab, typeof(GameObject));
					if(dataBase.m_effects[i].m_effectPrefab == null)
					{
						Debug.LogError("effect is null, path : " + pathEffectPrefab);
					}
                    i++;
                }

                dataBase.m_sfxClips = new SoundClip[config.soundSfxList.Count];
                i = 0;
                foreach(string sPath in config.soundSfxList)
                {
                    dataBase.m_sfxClips[i] = new SoundClip();
                    dataBase.m_sfxClips[i]._name = sPath;
                    string pathEffectPrefab = System.IO.Path.Combine("Assets/Sound/SoundFX", sPath + ".wav");
                    dataBase.m_sfxClips[i]._clip = (AudioClip)AssetDatabase.LoadAssetAtPath(pathEffectPrefab, typeof(AudioClip));
                    i++;
                }

                AssetDatabase.CreateAsset(dataBase, path);
            }
        }


    }

}
