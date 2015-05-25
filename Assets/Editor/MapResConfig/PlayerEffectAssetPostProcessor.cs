using UnityEngine;
using System.Collections;
using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;
public class PlayerEffectAssetPostProcessor : AssetPostprocessor 
{
    private static readonly string RESOURCE_PLAYER_EFFECT_CONFIG_FOLDER = "Assets/Data/MapResConfig/Res";
    private static readonly string ASSET_PLAYER_EFFECT_CONFIG_FOLDER = "Assets/Data/MapResConfig/Data";
	

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            PlayerEffectConfigPostprocess();
        }
    }

    private static void PlayerEffectConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_PLAYER_EFFECT_CONFIG_FOLDER, "resource_Players.xml");
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
            
            object[] Ids = sheet[keys[0]];
            List<string> tempPathList = new List<string>();
            List<string> tempSfxPathList = new List<string>();
            for(int i = 2; i < Ids.Length; i++)
            {
                string pathesStr = Convert.ToString(sheet["Players_Effects"][i]);
                string[] pathStrs = pathesStr.Split('+');
                tempPathList.AddRange(pathStrs);

                string sfxPathStr = Convert.ToString(sheet["Players_sound"][i]);
                string[] sfxPathStrs = sfxPathStr.Split('+');
                tempSfxPathList.AddRange(sfxPathStrs);

            }
            tempPathList = tempPathList.Distinct().ToList();
            CreatePlayerEffectDataBase(tempPathList, tempSfxPathList);
        }

    }













    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_PLAYER_EFFECT_CONFIG_FOLDER + "/resource_Players.xml"))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }

    private static void CreatePlayerEffectDataBase(List<string> pathList, List<string> sfxPathList)
    {
        string path = System.IO.Path.Combine(ASSET_PLAYER_EFFECT_CONFIG_FOLDER, "PlayerEffectDataBase.asset");
        if(File.Exists(path))
        {
            PlayerEffectDataBase dataBase = (PlayerEffectDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerEffectDataBase));

            dataBase.m_playerReses = new EffectObjGroup[pathList.Count];
            int i = 0;
            foreach(string ePath in pathList)
            {
                dataBase.m_playerReses[i] = new EffectObjGroup();
                dataBase.m_playerReses[i].m_effectPath = ePath;
                string effectObjPath = System.IO.Path.Combine("Assets/", ePath + ".prefab"); 
                dataBase.m_playerReses[i].m_effectPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(effectObjPath, typeof(GameObject));


                i++;
            }


            dataBase.m_playerSfxs = new SoundClip[sfxPathList.Count];
            i = 0;
            foreach(string sPath in sfxPathList)
            {
                dataBase.m_playerSfxs[i] = new SoundClip();
                dataBase.m_playerSfxs[i]._name = sPath;
                string pathSfx = System.IO.Path.Combine("Assets/Sound/SoundFX", sPath + ".wav");
                dataBase.m_playerSfxs[i]._clip = (AudioClip)AssetDatabase.LoadAssetAtPath(pathSfx, typeof(AudioClip));
                i++;
            }


            EditorUtility.SetDirty(dataBase);
        }
        else
        {
            PlayerEffectDataBase dataBase = ScriptableObject.CreateInstance<PlayerEffectDataBase>();

            dataBase.m_playerReses = new EffectObjGroup[pathList.Count];
            int i = 0;
            foreach(string ePath in pathList)
            {
                dataBase.m_playerReses[i] = new EffectObjGroup();
                dataBase.m_playerReses[i].m_effectPath = ePath;
                string effectObjPath = System.IO.Path.Combine("Assets/", ePath + ".prefab"); 
                dataBase.m_playerReses[i].m_effectPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(effectObjPath, typeof(GameObject));
                
                
                i++;
            }

            dataBase.m_playerSfxs = new SoundClip[sfxPathList.Count];
            i = 0;
            foreach(string sPath in sfxPathList)
            {
                dataBase.m_playerSfxs[i] = new SoundClip();
                dataBase.m_playerSfxs[i]._name = sPath;
                string pathSfx = System.IO.Path.Combine("Assets/Sound/SoundFX", sPath + ".wav");
                dataBase.m_playerSfxs[i]._clip = (AudioClip)AssetDatabase.LoadAssetAtPath(pathSfx, typeof(AudioClip));
                i++;
            }

            AssetDatabase.CreateAsset(dataBase, path);
        }

    }
}
