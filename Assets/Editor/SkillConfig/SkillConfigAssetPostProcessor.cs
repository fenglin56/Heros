using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class SkillConfig : AssetPostprocessor
{
    public static readonly string EW_RESOURCE_SKILL_CONFIG_FOLDER = "Assets/Data/SkillConfig/Res";
    public static readonly string EW_ASSET_SKILL_CONFIG_FOLDER = "Assets/Data/SkillConfig/Data";
	
	public static readonly string ASSET_SKILL_ICON_FOLDER = "Assets/Prefab/GUI/ItemIcon/SkillICon";


    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            OnPostprocessSkillConfig();
            OnPostprocessSkillCameraConfig();
        }
        
    }


    private static void OnPostprocessSkillCameraConfig()
    {
        string path = System.IO.Path.Combine(EW_RESOURCE_SKILL_CONFIG_FOLDER, "SkillCamera.xml");
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

            List<SkillCameraData> tempList = new List<SkillCameraData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                SkillCameraData data = new SkillCameraData();
                data._CameraID = Convert.ToInt32(sheet["CameraID"][i]);
                data._CameraDuration = Convert.ToSingle(sheet["CameraDuration"][i]);
                data._StartType = Convert.ToInt32(sheet["CameraStartType"][i]);
                
                string[] startOffset = Convert.ToString(sheet["CameraStartPos"][i]).Split('+');
                data._CameraOffset = new Vector3(Convert.ToInt32(startOffset[1]), Convert.ToInt32(startOffset[2]), Convert.ToInt32(startOffset[3]));

                string[] targetOffset = Convert.ToString(sheet["CameraTargetPos"][i]).Split('+');
                data._TargetOffset = new Vector3(Convert.ToInt32(targetOffset[1]), Convert.ToInt32(targetOffset[2]), Convert.ToInt32(targetOffset[3]));

                data._CameraType = Convert.ToInt32(sheet["CameraMoveType"][i]);

                string[] cameraParamsStr = Convert.ToString(sheet["CameraMoveNumber"][i]).Split('|');
                data._CameraParams = new SkillCameraParam[cameraParamsStr.Length];

                for (int j = 0; j < cameraParamsStr.Length; j++)
                {
                    data._CameraParams[j] = new SkillCameraParam();
                    string[] itemParamStr = Convert.ToString(cameraParamsStr[j]).Split('+');
                    if (itemParamStr.Length == 3)
                    {
                        data._CameraParams[j]._EquA = Convert.ToSingle(itemParamStr[1]);
                        data._CameraParams[j]._EquB = Convert.ToSingle(itemParamStr[2]);
                    }
                }

                data._CameraResetTime = Convert.ToSingle(sheet["CameraEndTime"][i]);
                string[] shakeTimeStr = Convert.ToString(sheet["ShakeStartTime"][i]).Split('+');
                data._ShakeStartTime = new float[shakeTimeStr.Length];
                for (int j = 0; j < shakeTimeStr.Length; j++)
                {
                    data._ShakeStartTime[j] = Convert.ToSingle(shakeTimeStr[j]);
                }

                data._ShakeAnimName = Convert.ToString(sheet["ShakeDoc"][i]).Split('+');

                tempList.Add(data);
            }

            CreateSkillCameraDataBase(tempList);
        }
    }

   private static void OnPostprocessSkillConfig()
    {
        string path = System.IO.Path.Combine(EW_RESOURCE_SKILL_CONFIG_FOLDER, "SkillConfig.xml");
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

            List<SkillConfigData> tempList = new List<SkillConfigData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                SkillConfigData data = new SkillConfigData();
                data.m_skillId = Convert.ToInt32(sheet["skill_id"][i]);
                data.m_unlockLevel = Convert.ToInt32(sheet["unlock_level"][i]);
                data.m_UpdateInterval = Convert.ToInt32(sheet["UpdateInterval"][i]);
                data.m_vocation = Convert.ToInt32(sheet["vocation"][i]);
                data.m_skillType = Convert.ToInt32(sheet["skill_type"][i]);
                data.m_maxLv = Convert.ToInt32(sheet["max_level"][i]);

                string iconPrefabpath = Convert.ToString(sheet["icon"][i]);
                data.m_iconName = iconPrefabpath;
                string pathRes = System.IO.Path.Combine(ASSET_SKILL_ICON_FOLDER, iconPrefabpath + ".prefab");
                data.m_icon = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));
                data.icon_circle = Convert.ToString(sheet["icon_circle"][i]);
                string icon_CirclepathRes = System.IO.Path.Combine(ASSET_SKILL_ICON_FOLDER, data.icon_circle + ".prefab");
                data.Icon_CirclePrefab = (GameObject)AssetDatabase.LoadAssetAtPath(icon_CirclepathRes, typeof(GameObject));
                data.m_name = Convert.ToString(sheet["name"][i]);
                data.m_descSimple = Convert.ToString(sheet["desc_simple"][i]);
                data.m_desc = Convert.ToString(sheet["desc"][i]);
                data.m_chantTime = Convert.ToSingle(sheet["chant_time"][i]);
                data.m_range = Convert.ToInt32(sheet["range"][i]);
                data.m_element = Convert.ToInt32(sheet["element"][i]);

                data.m_mightParams = new float[4];

                string mightStr = Convert.ToString(sheet["Interface_Power"][i]);
                string[] splitmightStrs = mightStr.Split("+".ToCharArray());

                for (int j = 0; j < 4; j++)
                {
                    data.m_mightParams[j] = Convert.ToSingle(splitmightStrs[j]);
                }

                data.m_manaConsumeParams = new float[4];

                string manaConsumeStr = Convert.ToString(sheet["mana_comsume"][i]);
                string[] splitManaStrs = manaConsumeStr.Split("+".ToCharArray());
                if (splitManaStrs.Length != 4)
                {
                    Debug.LogError("Skill Data Error:  wrong mana consume params count");
                }
                for (int j = 0; j < 4; j++)
                {
                    data.m_manaConsumeParams[j] = Convert.ToSingle(splitManaStrs[j]);
                }


                data.m_upgradeMsg1 = Convert.ToString(sheet["upgrade_msg_1"][i]);
                data.m_upgradeMsg2 = Convert.ToString(sheet["upgrade_msg_2"][i]);
                data.m_upgradeMsg3 = Convert.ToString(sheet["upgrade_msg_3"][i]);
                data.m_upgradeMsg4 = Convert.ToString(sheet["upgrade_msg_4"][i]);

                //skill point
                string skillPointStr = Convert.ToString(sheet["upgrade_skill_point"][i]);
                string[] splitSkillPointStrs = skillPointStr.Split("+".ToCharArray());
                if (splitSkillPointStrs.Length != 4)
                {
                    Debug.LogError("Skill Data Error:  wrong skill point params count");
                }
                data.m_upgradeSkillPointParams = new float[4];
                for (int j = 0; j < 4; j++)
                {
                    data.m_upgradeSkillPointParams[j] = Convert.ToSingle(splitSkillPointStrs[j]);
                }

                //upgrade money
                string upgradeMoneyStr = Convert.ToString(sheet["upgrade_money"][i]);
                string[] splitMoneyStrs = upgradeMoneyStr.Split("+".ToCharArray());
                //if(splitMoneyStrs.Length != 4)
                //{
                //    Debug.LogError("Skill Data Error:  wrong skill upgrade money params count");	
                //}
                data.m_upgradeMoneyParams = new int[splitMoneyStrs.Length];
                for (int j = 0; j < data.m_upgradeMoneyParams.Length; j++)
                {
                    data.m_upgradeMoneyParams[j] = Convert.ToInt32(splitMoneyStrs[j]);
                }

                //upgrade item
                string upgradeItemStr = Convert.ToString(sheet["upgrade_item"][i]);
                string[] splitUpgradeItemStrs = upgradeItemStr.Split("+".ToCharArray());
                data.m_upgradeItemId = Convert.ToInt32(splitUpgradeItemStrs[0]);
                data.m_upgradeItemCount = Convert.ToInt32(splitUpgradeItemStrs[1]);

                //upgrade param
                string upgradeParamStr = Convert.ToString(sheet["upgrade_param"][i]);
                string[] splitUpgradeParamStrs = upgradeParamStr.Split("+".ToCharArray());
                if (splitUpgradeParamStrs.Length != 4)
                {
                    Debug.LogError("Skill Data Error:  wrong skill upgrade params count");
                }
                data.m_upgradeParams = new float[4];
                for (int j = 0; j < 4; j++)
                {
                    data.m_upgradeParams[j] = Convert.ToSingle(splitUpgradeParamStrs[j]);
                }

                //ugprade hurt param
                string upgradeHurtParamStr = Convert.ToString(sheet["upgrade_hurt_param"][i]);
                string[] splitUpgradeHurtParamStrs = upgradeHurtParamStr.Split("+".ToCharArray());
                if (splitUpgradeHurtParamStrs.Length != 4)
                {
                    Debug.LogError("Skill Data Error:  wrong skill upgrade hurt params count");
                }
                data.m_upgradeHurtParams = new float[4];
                for (int j = 0; j < 4; j++)
                {
                    data.m_upgradeHurtParams[j] = Convert.ToSingle(splitUpgradeHurtParamStrs[j]);
                }
                data.m_breakLevel = Convert.ToInt32(sheet["break_level"][i]);
				data.m_skillAttacktimes = Convert.ToInt32(sheet["SkillAttacktimes"][i]);
				var skillDamages = Convert.ToString(sheet["SkillDamage"][i]).Split('+');
				data.m_skillDamageList = new List<int>();
				foreach(string str in skillDamages)
				{
					data.m_skillDamageList.Add(int.Parse(str));
				}
//                data.m_skillDamage = new float[4];
//                data.m_skillDamage[0] = float.Parse(skillDamages[0]);
//                data.m_skillDamage[1] = float.Parse(skillDamages[1]);
//                data.m_skillDamage[2] = float.Parse(skillDamages[2]);
//                data.m_skillDamage[3] = float.Parse(skillDamages[3]);

				data.m_coolDown = Convert.ToSingle(sheet["cd"][i]);
                data.m_triggerType = Convert.ToInt32(sheet["trigger_type"][i]);
                data.m_triggerTarget = Convert.ToInt32(sheet["trigger_target"][i]);


                //trigger range
                data.m_triggerRange = new int[2];
                string triggerRangeStr = Convert.ToString(sheet["trigger_range"][i]);
                if ("0".Equals(triggerRangeStr))
                {
                    data.m_triggerRange[0] = 0;
                    data.m_triggerRange[1] = 0;
                }
                else
                {
                    string[] splitTriggerRangeStrs = triggerRangeStr.Split("+".ToCharArray());
                    data.m_triggerRange[0] = Convert.ToInt32(splitTriggerRangeStrs[0]);
                    data.m_triggerRange[1] = Convert.ToInt32(splitTriggerRangeStrs[1]);

                }
                data.m_directionParam = Convert.ToInt32(sheet["direction_param"][i]);

                //launch range
                data.m_launchRange = new int[2];
                string launchRangeStr = Convert.ToString(sheet["launch_range"][i]);
                string[] splitLaunchRangeStrs = launchRangeStr.Split("+".ToCharArray());
                data.m_launchRange[0] = Convert.ToInt32(splitLaunchRangeStrs[0]);
                data.m_launchRange[1] = Convert.ToInt32(splitLaunchRangeStrs[1]);


                //aciton
                string actionStr = Convert.ToString(sheet["action_id"][i]);
                string[] splitActionStrs = actionStr.Split("+".ToCharArray());

                data.m_actionId = new int[splitActionStrs.Length];
                for (int j = 0; j < splitActionStrs.Length; j++)
                {
                    data.m_actionId[j] = Convert.ToInt32(splitActionStrs[j]);
                }

                //bullet
                string bulletGroupStr = Convert.ToString(sheet["bullet_id"][i]);
                string[] splitBulletGroupStrs = bulletGroupStr.Split("|".ToCharArray());
                data.m_bulletGroups = new BulletGroup[splitBulletGroupStrs.Length];
				for (int j = 0; j < splitBulletGroupStrs.Length; j++)
                {
                    data.m_bulletGroups[j] = new BulletGroup();
                    string[] bulletStrs = splitBulletGroupStrs[j].Split("+".ToCharArray());
                    data.m_bulletGroups[j].m_bulletId = Convert.ToInt32(bulletStrs[0]);
                    data.m_bulletGroups[j].m_delay = Convert.ToInt32(bulletStrs[1]);
                }
				//strengthen bullet
				string bulletGroupStr1 = Convert.ToString(sheet["SkillStrengthen_bullet_id"][i]);
				if(!bulletGroupStr1.Equals("0"))
				{
					string[] splitBulletGroupStrs1 = bulletGroupStr1.Split("|".ToCharArray());
					data.m_bulletStrengGroups = new BulletGroup[splitBulletGroupStrs1.Length];
					for (int j = 0; j < splitBulletGroupStrs1.Length; j++)
					{
						data.m_bulletStrengGroups[j] = new BulletGroup();
						string[] bulletStrs = splitBulletGroupStrs1[j].Split("+".ToCharArray());
						data.m_bulletStrengGroups[j].m_bulletId = Convert.ToInt32(bulletStrs[0]);
						data.m_bulletStrengGroups[j].m_delay = Convert.ToInt32(bulletStrs[1]);
					}
				}

                //sfx
                string skillSfxStr = Convert.ToString(sheet["SkillVoice"][i]);
                string[] splitSkillSfxGroupStrs = skillSfxStr.Split('+');
                data.m_skillSfxGroup = new string[splitSkillSfxGroupStrs.Length];
                for (int j = 0; j < splitSkillSfxGroupStrs.Length; j++)
                {
                    data.m_skillSfxGroup[j] = splitSkillSfxGroupStrs[j];
                }
                data.m_affectTarget = Convert.ToInt32(sheet["affect_target"][i]);

                //uiEffect
                string[] uiEffectStr = Convert.ToString(sheet["UIEffect"][i]).Split('+');
                string[] effectStartTimeStr = Convert.ToString(sheet["EffectStartTime"][i]).Split('+');
                string[] effectDurationStr = Convert.ToString(sheet["EffectDuration"][i]).Split('+');
                string[] effectStartPosStr = Convert.ToString(sheet["EffectStartPos"][i]).Split('|');
                data.m_UIEffectGroupList = new List<UIEffectGroup>();
                for (int j = 0; j < uiEffectStr.Length; j++)
                {
                    UIEffectGroup uiEffectGroup = new UIEffectGroup();
                    uiEffectGroup._UIEffectPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Effects/Skill/" + uiEffectStr[j] + ".prefab", typeof(GameObject));
                    uiEffectGroup._EffectStartTime = Convert.ToInt32(effectStartTimeStr[j]) / 1000f;
                    uiEffectGroup._EffectDuration = Convert.ToInt32(effectDurationStr[j]) / 1000f;
                    string[] posStr = Convert.ToString(effectStartPosStr[j]).Split('+');
                    uiEffectGroup._EffectStartPos = new Vector3(Convert.ToUInt32(posStr[1]), Convert.ToUInt32(posStr[2]), 0);
                    if (uiEffectGroup._UIEffectPrefab != null)
                    {
                        data.m_UIEffectGroupList.Add(uiEffectGroup);
                    }
                }

                string[] cameraOffsetStr = Convert.ToString(sheet["CameraRangePos"][i]).Split('+');
                data.cameraRangeOffset = new Vector3(Convert.ToUInt32(cameraOffsetStr[1]), 0, Convert.ToUInt32(cameraOffsetStr[2]));
                data.skillCameraRange = Convert.ToSingle(sheet["CameraRange"][i]);

                string[] cameraStr = Convert.ToString(sheet["SkillCameraID"][i]).Split('+');
				data.cameraIdList = new int[cameraStr.Length];
                for (int j = 0; j < cameraStr.Length; j++)
                {
					data.cameraIdList[j] = Convert.ToInt32(cameraStr[j]);
                }

				string[] hinTextArr = Convert.ToString(sheet["HintText"][i]).Split('+');
				data.skillText = hinTextArr[0];
				data.HintTextList = new List<int>();
				for(int k = 1 ; k < hinTextArr.Length;k++)
				{
					data.HintTextList.Add(int.Parse(hinTextArr[k]));
				}

                data.m_IsSirenSkill = Convert.ToInt32(sheet["SirenSkill"][i]) == 1;//1Ϊtrue

                data.AutoDirecting = Convert.ToInt32(sheet["AutoDirecting"][i])==1;
				data.energy_comsumeParam = Convert.ToInt32(sheet["energy_comsumeParam"][i]);

				data.energy_comsume = Convert.ToSingle(sheet["energy_comsume"][i]);
				string energyComsumeStr = Convert.ToString(sheet["energy_comsume"][i]);
				data.energyComsumePrefab = (GameObject)Resources.LoadAssetAtPath("Assets/Prefab/GUI/IconPrefab/SkillSPIcon/SP_"+energyComsumeStr+".prefab",typeof(GameObject));
                var comboSkill = Convert.ToString(sheet["ComboSkill"][i]);
                if (comboSkill != "0")
                {
                    var comboSkillSplitVal = comboSkill.Split('+');
                    data.ComboSkill = new int[comboSkillSplitVal.Length];
                    for (int k = 0; k < comboSkillSplitVal.Length; k++)
                    {
                        data.ComboSkill[k] = int.Parse(comboSkillSplitVal[k]);
                    }
                }

				data.PreSkill = Convert.ToInt32(sheet["PreSkill"][i]);
				data.PostSkill = Convert.ToInt32(sheet["PostSkill"][i]);
				data.SkillTreeID = Convert.ToInt32(sheet["SkillTreeID"][i]);
				data.Advanced_item = Convert.ToString(sheet["Advanced_item"][i]);
				string[] advItemStr = data.Advanced_item.Split('+');
				data.advItemID = int.Parse(advItemStr[0]);
				data.advItemCount = int.Parse(advItemStr[1]);
				data.SkillStrengthen = Convert.ToInt32(sheet["SkillStrengthen"][i]);

				data.SkillStrengthen_Money = Convert.ToString(sheet["SkillStrengthen_Money"][i]);
				if(!data.SkillStrengthen_Money.Equals("0"))
				{
					string[] strengMonStr = data.SkillStrengthen_Money.Split('+');
					foreach(string str in strengMonStr)
					{
						data.skillStrMoneyList.Add(int.Parse(str));
					}
				}
				data.SkillStrengthen_bullet_id = Convert.ToString(sheet["SkillStrengthen_bullet_id"][i]);
				data.SkillStrengthen_Text = Convert.ToString(sheet["SkillStrengthen_Text"][i]);
				data.SkillStrengthen_Damage = Convert.ToString(sheet["SkillStrengthen_Damage"][i]);
				if(!data.SkillStrengthen_Damage.Equals("0"))
				{
					string[] strengDameStr = data.SkillStrengthen_Damage.Split('+');
					foreach(string str in strengDameStr)
					{
						data.skillStrDamegeList.Add(int.Parse(str));
					}
				}
                data.FatherSkill = Convert.ToInt32(sheet["FatherSkill"][i]);

                tempList.Add(data);
            }

            CreateSkillConfigDataBase(tempList);
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

    private static void CreateSkillCameraDataBase(List<SkillCameraData> list)
    {
        string fileName = typeof(SkillCameraConfigDataBase).Name;
        string path = System.IO.Path.Combine(EW_ASSET_SKILL_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            SkillCameraConfigDataBase database = (SkillCameraConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(SkillCameraConfigDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new SkillCameraData[list.Count];

            list.CopyTo(database._dataTable);


            EditorUtility.SetDirty(database);
        }
        else
        {
            SkillCameraConfigDataBase database = ScriptableObject.CreateInstance<SkillCameraConfigDataBase>();

            database._dataTable = new SkillCameraData[list.Count];

            list.CopyTo(database._dataTable);
            AssetDatabase.CreateAsset(database, path);
        }

    }

    private static void CreateSkillConfigDataBase(List<SkillConfigData> list)
    {
        string fileName = typeof(SkillConfigDataBase).Name;
        string path = System.IO.Path.Combine(EW_ASSET_SKILL_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            SkillConfigDataBase database = (SkillConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(SkillConfigDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new SkillConfigData[list.Count];
			
			list.CopyTo(database._dataTable);
			
          
            EditorUtility.SetDirty(database);
        }
        else
        {
            SkillConfigDataBase database = ScriptableObject.CreateInstance<SkillConfigDataBase>();

            database._dataTable = new SkillConfigData[list.Count];

            list.CopyTo(database._dataTable);
            AssetDatabase.CreateAsset(database, path);
        }

    }
}
