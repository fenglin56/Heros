using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class MonsterConfig : AssetPostprocessor
{
    private static readonly string RESOURCE_MONSTER_CONFIG_FOLDER = "Assets/Data/MonsterConfig/Res";
    private static readonly string ASSET_MONSTER_CONFIG_FOLDER = "Assets/Data/MonsterConfig/Data";
    private static readonly string ASSET_TRAP_RES_CONFIG_FOLDER = "Assets/Enemy/";
	private static readonly string ASSET_BLOOD_EFFECT_FOLDER = "Assets/Effects/Prefab/";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            MonsterConfigPostprocess();
        }
    }

    private static void MonsterConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_MONSTER_CONFIG_FOLDER, "MonsterConfig.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("Monster config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<MonsterConfigData> tempList = new List<MonsterConfigData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                MonsterConfigData data = new MonsterConfigData();
                data._monsterID = Convert.ToInt32(sheet["dwMonsterID"][i]);
                data._szName = Convert.ToString(sheet["szName"][i]);
                data._moveSpeed = (float)Convert.ToDouble(sheet["nMoveSpd"][i]);
                string vectSkill = Convert.ToString(sheet["vectSkillID"][i]);

                string[] splitVectSkill = vectSkill.Split("|".ToCharArray());

                data._skillGroup = new SkillGroup[splitVectSkill.Length];
                for (int k = 0; k < splitVectSkill.Length; k++)
                {
                    string[] skillInfo = splitVectSkill[k].Split("+".ToCharArray());
                    data._skillGroup[k] = new SkillGroup()
                    {
                        _skillID = Convert.ToInt32(skillInfo[0]),
                        _skillLevel = Convert.ToInt32(skillInfo[1]),
                        _skillProbability = Convert.ToInt32(skillInfo[2])
                    };
                }
                data._skillID = data._skillGroup[0]._skillID;

                data.MonsterSubType = Convert.ToInt32(sheet["byMonsterSubType"][i]);
                data.Hurt_sfx = Convert.ToString(sheet["hurt_sfx"][i]); 
                ////Debug.Log(data.skillInfoData[skillInfoNum]._skillID);                            

                string prefabpath = Convert.ToString(sheet["dwDisplayID"][i]);
                string pathRes = System.IO.Path.Combine(ASSET_TRAP_RES_CONFIG_FOLDER, prefabpath + ".prefab");
                //data._MonsterPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));
				
				string bloodEffectPrefabPath = Convert.ToString(sheet["SpecialDeadEffect"][i]);
				string bloodPathRes = System.IO.Path.Combine(ASSET_BLOOD_EFFECT_FOLDER, bloodEffectPrefabPath + ".prefab");
				//data._monsterBloodEffectPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(bloodPathRes, typeof(GameObject));
				
                data._standUpTime = Convert.ToInt32(sheet["standup_time"][i]);
                data._flyHigh = (float)Convert.ToDouble(sheet["fly_high"][i]);
				data._fly_initial_high = (float)Convert.ToDouble(sheet["fly_initial_high"][i]);
				data._isShowGuideArrow = (int)Convert.ToInt32(sheet["IsShowGuideArrow"][i]);
                string[] nshared = Convert.ToString(sheet["nShared"][i]).Split('+');

                data.m_shieldpoint = (float)Convert.ToDouble(nshared[0]);
                data.m_breaktime = (float)Convert.ToDouble(nshared[1]);

                data._maxHP = Convert.ToInt32(sheet["nMaxHP"][i]);

                data._bornEffects = Convert.ToString(sheet["BornEffects"][i]);
                data._dialogMonsterName = Convert.ToString(sheet["DialogMonsterName"][i]);
                data._bornDialogue = Convert.ToString(sheet["BornDialogue"][i]);
                data._bornSound = Convert.ToString(sheet["BornSound"][i]);
                data._dialogPortrait = Convert.ToString(sheet["DialogPortrait"][i]);
                data._deadEffect = Convert.ToString(sheet["DeadEffect"][i]);
				string[] bornDialogueFullStr = Convert.ToString(sheet["BornDialogueFull"][i]).Split('|');
				int bDialogLength = bornDialogueFullStr.Length;
				data._BornDialogueFulls = new MonsterConfigData.BornDialogueFull[bDialogLength];
				for(int p = 0;p<bDialogLength;p++)
				{
					string[] bDialogStr = bornDialogueFullStr[p].Split('+');
					data._BornDialogueFulls[p] = new MonsterConfigData.BornDialogueFull();
					data._BornDialogueFulls[p].Portrait = Convert.ToString(bDialogStr[0]);
					data._BornDialogueFulls[p].MonsterName = Convert.ToString(bDialogStr[1]);
					data._BornDialogueFulls[p].Dialogue = Convert.ToString(bDialogStr[2]);
					data._BornDialogueFulls[p].BornDialoguePosition = Convert.ToInt32(bDialogStr[3]);
					data._BornDialogueFulls[p].Time = Convert.ToInt32(bDialogStr[4])/1000f;
                    data._BornDialogueFulls[p].protraitType = Convert.ToInt32(bDialogStr[5]);
				}

				data._deadEffect = Convert.ToString(sheet["DeadEffect"][i]);
                data._hitRadius = Convert.ToInt32(sheet["HitRadius"][i]);

                string[] cameraFixStr = Convert.ToString(sheet["CameraFix"][i]).Split('+');
                data._cameraFix_pos = new Vector3(Convert.ToInt32(cameraFixStr[1]), Convert.ToInt32(cameraFixStr[2]), Convert.ToInt32(cameraFixStr[3]));
                data._cameraFix_time = Convert.ToSingle(cameraFixStr[4]) / 1000f;
                data._cameraStay_time = Convert.ToSingle(cameraFixStr[5]) / 1000f;
                data._cameraBack_time = Convert.ToSingle(cameraFixStr[6]) / 1000f;

                if(cameraFixStr[7] == "0")
                {
                    data._blockPlayerToIdle = false;
                }
                else
                {
                    data._blockPlayerToIdle = true;
                }


				data._downTips=Convert.ToString(sheet["DownTips"][i]);
				data._upTips=Convert.ToString(sheet["UpTips"][i]);

				string[] deathBulletStr = Convert.ToString(sheet["DeathBullet"][i]).Split('+');
				data._DeathBullet = new int[deathBulletStr.Length];
				for(int index = 0;index<deathBulletStr.Length;index++)
				{
					data._DeathBullet[index] = Convert.ToInt32(deathBulletStr[index]);
				}
                tempList.Add(data);
            }

            CreateMonsterConfigDataBase(tempList);
        }
    }

    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_MONSTER_CONFIG_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }


    private static void CreateMonsterConfigDataBase(List<MonsterConfigData> list)
    {
        string fileName = typeof(MonsterConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_MONSTER_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            MonsterConfigDataBase database = (MonsterConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(MonsterConfigDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new MonsterConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                
                database._dataTable[i] = list[i];
               
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            MonsterConfigDataBase database = ScriptableObject.CreateInstance<MonsterConfigDataBase>();

            database._dataTable = new MonsterConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = list[i];

            }
            AssetDatabase.CreateAsset(database, path);
        }

    }
}
