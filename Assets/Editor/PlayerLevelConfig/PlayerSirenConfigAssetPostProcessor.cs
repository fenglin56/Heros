using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;
using UnityEditor;
using System;
using System.Linq;

public class PlayerSirenConfigAssetPostProcessor : AssetPostprocessor
{
    private static readonly string RESOURCE_TRAP_CONFIG_FOLDER = "Assets/Data/PlayerConfig/Res";
    private static readonly string ASSET_TRAP_CONFIG_FOLDER = "Assets/Data/PlayerConfig/Data";
	private static readonly string ASSET_Prefab_GUI_IconPrefab_SirenIcon_FOLDER = "Assets/Prefab/GUI/IconPrefab/SirenIcon";    

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {        
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            string path = System.IO.Path.Combine(RESOURCE_TRAP_CONFIG_FOLDER, "PlayerSirenConfig.xml");
            TextReader tr = new StreamReader(path);
            string text = tr.ReadToEnd();

            if (text == null)
            {
                Debug.LogError("Player level config file not exist");
                return;
            }
            else
            {                
                XmlSpreadSheetReader.ReadSheet(text);
                XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
                string[] keys = XmlSpreadSheetReader.Keys;

                object[] levelIds = sheet[keys[0]];

                List<PlayerSirenConfigData> tempList = new List<PlayerSirenConfigData>();
                List<PlayerSirenConfigData> tempWithoutList = new List<PlayerSirenConfigData>();
                /* 普通方法
                for (int i = 0; i < levelIds.Length; i++)
                {
                    if (0 == i || 1 == i) continue;
                    PlayerSirenConfigData data = new PlayerSirenConfigData();

                    data._sirenID = Convert.ToInt32(sheet["SirenID"][i]);
                    data._name = Convert.ToString(sheet["Name"][i]);
                    //data._damageID = Convert.ToInt32(sheet["BoxID"][i]);
                    //data._damageName = Convert.ToString(sheet["BoxName"][i]);

                    //data._damagePrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                    //string correspondingItemIDStr = Convert.ToString(sheet["BoxGoodsDrop"][i]);
                    //string[] splitCorrespondingItemIDStrs = correspondingItemIDStr.Split("+".ToCharArray());
                    //data._correspondingItemID = Convert.ToInt32(splitCorrespondingItemIDStrs[0]);

                    tempList.Add(data);
                }
                */
                for (int i = 0; i < levelIds.Length; i++)
                {
                    if (0 == i || 1 == i) continue;

                    for (int clone = 0; clone < 2; clone++)
                    {
                        List<PlayerSirenConfigData> temporaryList;
                        if (clone == 0)
                        {
                            temporaryList = tempList;
                        }
                        else
                        {
                            temporaryList = tempWithoutList;
                        }

                        //导表
                        int sirenID = Convert.ToInt32(sheet["SirenID"][i]);
                        var temp = temporaryList.SingleOrDefault(p => p._sirenID == sirenID);
                        if (temp == null)
                        {
                            temp = new PlayerSirenConfigData();

                            temp._sirenID = sirenID;
                            temp._name = Convert.ToString(sheet["Name"][i]);
                            temp._nameRes = Convert.ToString(sheet["NameRes"][i]);
                            temp._portraitID = Convert.ToString(sheet["PortraitID"][i]);
                            //temp._portraitPrefab = (GameObject)Resources.LoadAssetAtPath("Assets/Prefab/GUI/SirenHead/" + temp._portraitID + ".prefab", typeof(GameObject));

                            //temp._rejectAnim = Convert.ToString(sheet["RejectAnim"][i]);
                            //temp._shyAnim = Convert.ToString(sheet["ShyAnim"][i]);
                            //temp._temptationAnim = Convert.ToString(sheet["TemptationAnim"][i]);

                            string[] composeCost = Convert.ToString(sheet["ComposeCost"][i]).Split('+');
                            temp._composeCost_itemID = Convert.ToInt32(composeCost[0]);
                            temp._composeCost_itemNum = Convert.ToInt32(composeCost[1]);

                            temp._unlockTips = Convert.ToString(sheet["UnlockTips"][i]);

                            temp._growthMaxLevel = Convert.ToInt32(sheet["GrowthMaxLevel"][i]);

                            temp._defaultWordCd = Convert.ToInt32(sheet["DefaultWordCd"][i]);

                            temp._refiningColdTime = Convert.ToInt32(sheet["RefiningColdTime"][i]);

							string[] unlockStr = Convert.ToString(sheet["Unlock"][i]).Split('|');
							temp.Unlock = new UnlockCondition[unlockStr.Length];
							for(int p=0;p<unlockStr.Length;p++)
							{
								temp.Unlock[p] = new UnlockCondition();								
								string[] unlock = unlockStr[p].Split('+');
								temp.Unlock[p].Type = Convert.ToInt32(unlock[0]);
								temp.Unlock[p].Condition1 = Convert.ToInt32(unlock[1]);
								temp.Unlock[p].Condition2 = Convert.ToInt32(unlock[2]);
							}

							temp._SirenText = Convert.ToString(sheet["SirenText"][i]);
							temp._UnlockText = Convert.ToString(sheet["UnlockText"][i]);
							temp._SirenPrice = Convert.ToInt32(sheet["SirenPrice"][i]);
							temp._BattleVoice = Convert.ToString(sheet["BattleVoice"][i]);


                            temporaryList.Add(temp);
                        }

                        if (temp._sirenConfigDataList == null)
                        {
                            temp._sirenConfigDataList = new List<SirenConfigData>();
                        }

                        SirenConfigData data = new SirenConfigData();
                        data._dwDisplayID = Convert.ToString(sheet["dwDisplayID"][i]);
                        data._dzDisplayID = Convert.ToString(sheet["dzDisplayID"][i]);
                        if (clone == 0)
                        {                            
                            data._prefab = (GameObject)Resources.LoadAssetAtPath("Assets/Prefab/NPC/Prefab/" + data._dwDisplayID + ".prefab", typeof(GameObject));                            
                            data._dzPrefab = (GameObject)Resources.LoadAssetAtPath("Assets/Prefab/NPC/Prefab/" + data._dzDisplayID + ".prefab", typeof(GameObject));
                        	

							string titlePrefabStr = Convert.ToString(sheet["TitlePrefab"][i]);
							data._TitlePrefab = (GameObject)Resources.LoadAssetAtPath("Assets/Prefab/GUI/IconPrefab/SirenIcon/" + titlePrefabStr + ".prefab", typeof(GameObject));                            
							string namePrefabStr = Convert.ToString(sheet["NamePrefab"][i]);
							data._NamePrefab = (GameObject)Resources.LoadAssetAtPath("Assets/Prefab/GUI/IconPrefab/SirenIcon/" + namePrefabStr + ".prefab", typeof(GameObject));                            				

						}
                        data._defaultAnim = Convert.ToString(sheet["DefaultAnim"][i]);
                        data._fearAnim = Convert.ToString(sheet["FearAnim"][i]);
                        data._growthCost = Convert.ToInt32(sheet["GrowthCost"][i]);
                        data._growthEffect = Convert.ToString(sheet["GrowthEffect"][i]);
                        data._growthLevels = Convert.ToInt32(sheet["GrowthLevels"][i]);
                        data._touchAnim = Convert.ToString(sheet["TouchAnim"][i]);
                        data._touchWord = ParseString(Convert.ToString(sheet["TouchWord"][i]));
                        data._defaultWord = ParseString(Convert.ToString(sheet["DefaultWord"][i]));
                        data._successWord = ParseString(Convert.ToString(sheet["SuccessWord"][i]));
                        data._failWord = ParseString(Convert.ToString(sheet["FailWord"][i]));
                        data._sitEffect = Convert.ToInt32(sheet["SitEffect"][i]);
                        data._sirenPower = Convert.ToInt32(sheet["SirenPower"][i]);
                        data._sitEffectTips = Convert.ToString(sheet["SitEffectTips"][i]);

						data._BattleVoice = Convert.ToString(sheet["BattleVoice"][i]);

                        string posStr = Convert.ToString(sheet["CameraPosition"][i]);
                        string[] vector3 = posStr.Split('+');//A+1+2+3
                        data._cameraPosition = new Vector3(Convert.ToSingle(vector3[1]), Convert.ToSingle(vector3[2]), Convert.ToSingle(vector3[3]));

                        data._touchSound = Convert.ToString(sheet["TouchSound"][i]);

                        data._refiningShakeTime = Convert.ToInt32(sheet["RefiningShakeTime"][i]);
                        data._refiningShakeAttenuation = Convert.ToSingle(sheet["RefiningShakeAttenuation"][i]);
                        data._refiningShakeElasticity = Convert.ToSingle(sheet["RefiningShakeElasticity"][i]);
                        data._refiningShakeInitSpeed = Convert.ToSingle(sheet["RefiningShakeInitSpeed"][i]);

                        string sirenPosStr = Convert.ToString(sheet["SirenPosition"][i]);
                        string[] sirenVector3 = sirenPosStr.Split('+');//A+1+2+3
                        data._sirenPosition = new Vector3(Convert.ToSingle(sirenVector3[1]), Convert.ToSingle(sirenVector3[2]), Convert.ToSingle(sirenVector3[3]));

                        string[] refiningItemStr = Convert.ToString(sheet["RefiningItem"][i]).Split('+');
                        data._refiningItem_itemID = Convert.ToInt32(refiningItemStr[0]);
                        data._refiningItem_itemNum = Convert.ToInt32(refiningItemStr[1]);
						data._growthLvlLimit = Convert.ToInt32(sheet["GrowthLvlLimit"][i]);

						data._GrowthEffect = Convert.ToString(sheet["GrowthEffect"][i]);
						data._MaxGrowthEffect = Convert.ToString(sheet["MaxGrowthEffect"][i]);

						data._LevelUpText = Convert.ToString(sheet["LevelUpText"][i]);
						string[] sirenSkillIDStr = Convert.ToString(sheet["SirenSkillID"][i]).Split('|');
						int sirenSkillIDLength = sirenSkillIDStr.Length;
						data._SirenSkillIDs = new SirenSkillID[sirenSkillIDLength];
						for(int p = 0;p<sirenSkillIDLength;p++)
						{
							string[] strs = sirenSkillIDStr[p].Split('+');
							data._SirenSkillIDs[p] = new SirenSkillID();
							data._SirenSkillIDs[p].Vocation = Convert.ToInt32(strs[0]);
							data._SirenSkillIDs[p].SkillID = Convert.ToInt32(strs[1]);
						}
						//data._LevelUpExp = Convert.ToInt32(sheet["LevelUpExp"][i]);
						data._SirenSkillIDText = Convert.ToString(sheet["SirenSkillIDText"][i]);

						data.SirenBreakStage = Convert.ToInt32(sheet["SirenBreakStage"][i]);
						data.BreakStageMaxLevel = Convert.ToInt32(sheet["BreakStageMaxLevel"][i]);
						string[] breakConditionStr = Convert.ToString(sheet["BreakCondition"][i]).Split('|');//id+num|id+num
						data.BreakCondition = new SirenBreakCondition[breakConditionStr.Length];
						for(int p = 0;p<breakConditionStr.Length;p++)
						{
							string[] value =  breakConditionStr[p].Split('+');
							data.BreakCondition[p] = new SirenBreakCondition();
							if(value.Length >= 2)
							{
								data.BreakCondition[p].ItemID = Convert.ToInt32(value[0]);
								data.BreakCondition[p].ItemNum = Convert.ToInt32(value[1]);
							}
						}
						data.BreakDesc = Convert.ToString(sheet["BreakDesc"][i]);
						data.SirenTeamBuffID = Convert.ToInt32(sheet["SirenTeamBuffID"][i]);
						data.SirenTeamBuffText = Convert.ToString(sheet["SirenTeamBuffText"][i]);

                        temp._sirenConfigDataList.Add(data);

                    }                    
                }


                CreateSceneConfigDataBase(tempList);
                CreateWithOutResDataBase(tempWithoutList);
            }
        }
    }

    //
    private static SirenDialogConfigData ParseString(string str)
    {
        SirenDialogConfigData data = new SirenDialogConfigData();

        string[] wordInfoArray = str.Split('|');//ids , 位置, 行数
        data.IDS = wordInfoArray[0];
        string[] posArray = wordInfoArray[1].Split('+');
        data.Pos = new Vector2(Convert.ToSingle(posArray[0]), Convert.ToSingle(posArray[1]));
        data.Rows = Convert.ToInt32(wordInfoArray[2]);
        return data;
    }


    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_TRAP_CONFIG_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }


    private static void CreateSceneConfigDataBase(List<PlayerSirenConfigData> list)
    {
        string fileName = typeof(PlayerSirenConfigData).Name;
        string path = System.IO.Path.Combine(ASSET_TRAP_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            PlayerSirenConfigDataBase database = (PlayerSirenConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerSirenConfigDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new PlayerSirenConfigData[list.Count];
            list.CopyTo(database._dataTable);           

            EditorUtility.SetDirty(database);
        }
        else
        {
            PlayerSirenConfigDataBase database = ScriptableObject.CreateInstance<PlayerSirenConfigDataBase>();

            database._dataTable = new PlayerSirenConfigData[list.Count];
            list.CopyTo(database._dataTable);
            
            AssetDatabase.CreateAsset(database, path);
        }

    }

    private static void CreateWithOutResDataBase(List<PlayerSirenConfigData> list)
    {
        string fileName = typeof(PlayerSirenConfigData).Name + "WithOutRes";
        string path = System.IO.Path.Combine(ASSET_TRAP_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            PlayerSirenConfigDataBase database = (PlayerSirenConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerSirenConfigDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new PlayerSirenConfigData[list.Count];
            list.CopyTo(database._dataTable);

            EditorUtility.SetDirty(database);
        }
        else
        {
            PlayerSirenConfigDataBase database = ScriptableObject.CreateInstance<PlayerSirenConfigDataBase>();

            database._dataTable = new PlayerSirenConfigData[list.Count];
            list.CopyTo(database._dataTable);

            AssetDatabase.CreateAsset(database, path);
        }

    }
}