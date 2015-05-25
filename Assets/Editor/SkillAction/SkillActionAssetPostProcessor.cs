using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class SkillActionAssetPostProcessor : AssetPostprocessor 
{
	 public static readonly string EW_RESOURCE_SKILL_CONFIG_FOLDER = "Assets/Data/SkillConfig/Res";
     public static readonly string EW_ASSET_SKILL_CONFIG_FOLDER = "Assets/Data/SkillConfig/Data";

     public static readonly string ASSET_EFFECT_RES_FOLDER = "Assets/Effects";
	
	 private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {

        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            string path = System.IO.Path.Combine(EW_RESOURCE_SKILL_CONFIG_FOLDER, "SkillAction.xml");
            TextReader tr = new StreamReader(path);
            string text = tr.ReadToEnd();

            if (text == null)
            {
                Debug.LogError("Skill Action file not exist");
                return;
            }
            else
            {
                XmlSpreadSheetReader.ReadSheet(text);
                XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
                string[] keys = XmlSpreadSheetReader.Keys;

                object[] levelIds = sheet[keys[0]];

                List<SkillActionData> tempList = new List<SkillActionData>();

                for (int i = 2; i < levelIds.Length; i++)
                {
                    SkillActionData data = new SkillActionData();
					data.m_actionId = Convert.ToInt32(sheet["act_id"][i]);
					data.m_animationId = Convert.ToString(sheet["ani_id"][i]);
					data.m_startTime = Convert.ToSingle(sheet["start_time"][i]);
					
					string startPosStr = Convert.ToString(sheet["start_pos"][i]);
					string[] splitStartPosStrs = startPosStr.Split("+".ToCharArray());
					float posX = Convert.ToSingle(splitStartPosStrs[1]);
					float posZ = Convert.ToSingle(splitStartPosStrs[2]);
					data.m_startPos = new Vector2(posX, posZ);
					
					data.m_moveType = Convert.ToInt32(sheet["move_type"][i]);
					data.m_startSpeed = Convert.ToSingle(sheet["speed"][i]);
					data.m_acceleration = Convert.ToSingle(sheet["acceleration"][i]);
					data.m_angle = Convert.ToSingle(sheet["angle"][i]);
					data.m_duration = Convert.ToSingle(sheet["duration"][i]);
					data.m_threshold = Convert.ToInt32(sheet["threshold"][i]);
					data.m_startAngle = Convert.ToSingle(sheet["start_angle"][i]);
					data.m_endAngle = Convert.ToSingle(sheet["end_angle"][i]);
					
					//effect group
					string effectGroupStr = Convert.ToString(sheet["effect_group"][i]);
					string[] splitEffectGroupStrs = effectGroupStr.Split("+".ToCharArray());
					data.m_effectGroup = new int[splitEffectGroupStrs.Length];
					for(int j = 0; j < splitEffectGroupStrs.Length; j++)
					{
						data.m_effectGroup[j] = Convert.ToInt32(splitEffectGroupStrs[j]);	
					}
					//					

                    string effectPath = Convert.ToString(sheet["effect_resource"][i]);
                    data.m_effectPath = effectPath;
                    //string pathEffect = System.IO.Path.Combine(ASSET_EFFECT_RES_FOLDER, effectPath + ".prefab");
                    //data.m_effect_resource = (GameObject)AssetDatabase.LoadAssetAtPath(pathEffect, typeof(GameObject));
                    
                    data.m_effect_start_time = Convert.ToSingle(sheet["effect_start_time"][i]);
                    var posString = Convert.ToString(sheet["effect_start_pos"][i]);
                    string[] splitposString = posString.Split("+".ToCharArray());
                    float effectPosX = Convert.ToSingle(splitposString[1]);
                    float effectPosZ = Convert.ToSingle(splitposString[2]);
                    data.m_effect_start_pos = new Vector2(effectPosX, effectPosZ);
                    data.m_effect_start_angel = Convert.ToSingle(sheet["effect_start_angel"][i]);
                    data.m_effect_move_speed = Convert.ToSingle(sheet["effect_move_speed"][i]);
                    data.m_effect_move_accleration = Convert.ToSingle(sheet["effect_move_accleration"][i]);
                    data.m_effect_loop_time = Convert.ToInt32(sheet["effect_loop_time"][i]);
                    data.m_ani_followtype=Convert.ToByte(sheet["ani_followtype"][i]);
					
					string followOffsetStr = Convert.ToString(sheet["PositionOffset"][i]);
					string[] splitfollowOffsetStrs = followOffsetStr.Split("+".ToCharArray());
					float followPosX = Convert.ToSingle(splitfollowOffsetStrs[1]);
					float followPosY = Convert.ToSingle(splitfollowOffsetStrs[2]);
					data.m_followPositionOffset = new Vector2(followPosX, followPosY);
					
					data.m_invincible = Convert.ToInt32(sheet["Invincible"][i]);
					data.m_ironBody = Convert.ToInt32(sheet["IronBody"][i]);
					data.m_soundEffectName = Convert.ToString(sheet["music_name"][i]);
					data.m_sfxDelay = Convert.ToSingle(sheet["music_startime"][i]);
					data.IsIgnoreBlock = Convert.ToInt32(sheet["IsIgnoreBlock"][i])==1?true:false;
					data.m_sfxDelay /= 1000.0f;
					
                    tempList.Add(data);
                }

                CreateSkillActionDataBase(tempList);
            }
        }
    }
	
	private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(EW_RESOURCE_SKILL_CONFIG_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }
	
	private static void CreateSkillActionDataBase(List<SkillActionData> list)
    {
        string fileName = typeof(SkillActionDataBase).Name;
        string path = System.IO.Path.Combine(EW_ASSET_SKILL_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            SkillActionDataBase database = (SkillActionDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(SkillActionDataBase));

            if (null == database)
            {
                return;
            }
            database._dataTable = new SkillActionData[list.Count];
			list.CopyTo(database._dataTable);
            EditorUtility.SetDirty(database);
        }
        else
        {
            SkillActionDataBase database = ScriptableObject.CreateInstance<SkillActionDataBase>();

            database._dataTable = new SkillActionData[list.Count];

            list.CopyTo(database._dataTable);
            AssetDatabase.CreateAsset(database, path);
        }

    }
}


