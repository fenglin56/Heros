using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class SirenSkillAssetPostProcessor : AssetPostprocessor
{

    public static readonly string EW_RESOURCE_SKILL_CONFIG_FOLDER = "Assets/Data/SkillConfig/Res";
    public static readonly string EW_ASSET_SKILL_CONFIG_FOLDER = "Assets/Data/SkillConfig/Data";

    //public static readonly string ASSET_BULLET_RES_FOLDER = "Assets/Bullets/Prefab";
    //public static readonly string ASSET_EFFECT_RES_FOLDER = "Assets/Effects";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {

        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            string path = System.IO.Path.Combine(EW_RESOURCE_SKILL_CONFIG_FOLDER, "SirenSkill.xml");
            TextReader tr = new StreamReader(path);
            string text = tr.ReadToEnd();

            if (text == null)
            {
                Debug.LogError("Bullet data file not exist");
                return;
            }
            else
            {
                XmlSpreadSheetReader.ReadSheet(text);
                XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
                string[] keys = XmlSpreadSheetReader.Keys;

                object[] levelIds = sheet[keys[0]];

                List<SirenSkillData> tempList = new List<SirenSkillData>();

                for (int i = 2; i < levelIds.Length; i++)
                {
                    SirenSkillData data = new SirenSkillData();

                    data._SirenSkill_id = Convert.ToInt32(sheet["SirenSkill_id"][i]);
                    data._vocation = Convert.ToInt32(sheet["vocation"][i]);

                    string[] sirenIDStr = Convert.ToString(sheet["SirenID"][i]).Split('+');
                    string[] skillIDStr = Convert.ToString(sheet["SkillID"][i]).Split('+');
                    string[] durationStr = Convert.ToString(sheet["Duration"][i]).Split('+');
                    string[] iconStr = Convert.ToString(sheet["icon"][i]).Split('+');
                    string[] icon_circleStr = Convert.ToString(sheet["icon_circle"][i]).Split('+');

                    data._SirenSkills = new SirenSkill[sirenIDStr.Length];
                    for (int m = 0; m < data._SirenSkills.Length; m++)
                    {
                        data._SirenSkills[m] = new SirenSkill();
                        data._SirenSkills[m]._SirenID = Convert.ToInt32(sirenIDStr[m]);
                        data._SirenSkills[m]._SkillID = Convert.ToInt32(skillIDStr[m]);
                        data._SirenSkills[m]._Duration = Convert.ToInt32(durationStr[m]) / 1000f;
                        data._SirenSkills[m]._icon = Convert.ToString(iconStr[m]);
                        data._SirenSkills[m]._icon_circle = Convert.ToString(icon_circleStr[m]);
                    }


                    //data.m_bulletId = Convert.ToInt32(sheet["bullet_id"][i]);

                    ////res
                    //string resourcePath = Convert.ToString(sheet["resource"][i]);
                    //string pathRes = System.IO.Path.Combine(ASSET_BULLET_RES_FOLDER, resourcePath + ".prefab");
                    //data.m_resource = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));
                    ////

                    //data.m_shapeParam1 = Convert.ToInt32(sheet["shape_1"][i]);
                    //data.m_shapeParam2 = Convert.ToInt32(sheet["shape_2"][i]);
                    //data.m_shapeParam3 = Convert.ToInt32(sheet["shape_3"][i]);

                    //data.m_startSpeed = Convert.ToSingle(sheet["speed"][i]);
                    //data.m_acceleration = Convert.ToSingle(sheet["acceleration"][i]);
                    //data.m_angle = Convert.ToSingle(sheet["angle"][i]);

                    //string initPosStr = Convert.ToString(sheet["init_pos"][i]);
                    //string[] splitInitPosStrs = initPosStr.Split("+".ToCharArray());
                    //float initPosX = Convert.ToSingle(splitInitPosStrs[1]);
                    //float initPosZ = Convert.ToSingle(splitInitPosStrs[2]);
                    //data.m_initPos = new Vector2(initPosX, initPosZ);

                    //data.m_lifeTime = Convert.ToSingle(sheet["life_time"][i]);
                    //data.m_overParam = Convert.ToInt32(sheet["over_param"][i]);
                    //data.m_bulletIdFollow = Convert.ToInt32(sheet["bullet_follow"][i]);
                    //data.m_monsterIdFollow = Convert.ToInt32(sheet["monster_follow"][i]);
                    //data.m_cauculateInverval = Convert.ToSingle(sheet["hit_interval"][i]) / 1000.0f;


                    //data.m_calculateId = Convert.ToInt32(sheet["calculate_id"][i]);
                    //data.m_sfx_id = Convert.ToString(sheet["sfx_id"][i]);
                    //data.m_hurtFlash = Convert.ToInt32(sheet["hurt_flash"][i]);
                    //data.m_hurtShakeTime = Convert.ToInt32(sheet["Hurt_ShakeTime"][i]);
                    //data.m_hurtShakeAttenuation = Convert.ToSingle(sheet["Hurt_ShakeAttenuation"][i]) / 1000.0f;
                    //data.m_hurtShakeInitSpeed = Convert.ToSingle(sheet["Hurt_ShakeInitSpeed"][i]);
                    //data.m_hurtShakeElasticity = Convert.ToSingle(sheet["Hurt_ShakeElasticity"][i]);

                    ////hurt effect
                    //string hurt_effectPath = Convert.ToString(sheet["hurt_effect_id"][i]);
                    //string pathHurtEffect = System.IO.Path.Combine(ASSET_EFFECT_RES_FOLDER, hurt_effectPath + ".prefab");
                    //data.m_hurt_effect = (GameObject)AssetDatabase.LoadAssetAtPath(pathHurtEffect, typeof(GameObject));


                    //string hurt_Ui_effectPath = Convert.ToString(sheet["hurt_UIeffect_id"][i]);
                    //string pathHurtUiEffect = System.IO.Path.Combine(ASSET_EFFECT_RES_FOLDER, hurt_Ui_effectPath + ".prefab");
                    //data.m_hurt_Ui_Effect = (GameObject)AssetDatabase.LoadAssetAtPath(pathHurtUiEffect, typeof(GameObject));

                    //data.m_hurtEffectRotationFlag = Convert.ToByte(sheet["hurt_effect_RotationFlag"][i]);

                    //data.m_followtype = Convert.ToByte(sheet["bul_followtype"][i]);

                    //data.m_mountType = Convert.ToByte(sheet["mount_type"][i]);

                    //data.m_breakType = Convert.ToByte(sheet["break_type"][i]);

                    ////born
                    //data.m_bornSfxId = Convert.ToString(sheet["sfx_born"][i]);
                    //data.m_bornShakeTime = Convert.ToInt32(sheet["BulletShakeTime"][i]);
                    //data.m_bornShakeAttenuation = Convert.ToSingle(sheet["BulletShakeAttenuation"][i]) / 1000.0f;
                    //data.m_bornShakeInitSpeed = Convert.ToSingle(sheet["BulletShakeInitSpeed"][i]);
                    //data.m_bornShakeElasticity = Convert.ToSingle(sheet["BulletShakeElasticity"][i]);

                    tempList.Add(data);
                }

                CreateBulletDataBase(tempList);
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

    private static void CreateBulletDataBase(List<SirenSkillData> list)
    {
        string fileName = typeof(SirenSkillDataBase).Name;
        string path = System.IO.Path.Combine(EW_ASSET_SKILL_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            SirenSkillDataBase database = (SirenSkillDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(SirenSkillDataBase));

            if (null == database)
            {
                return;
            }
            database._dataTable = new SirenSkillData[list.Count];
            list.CopyTo(database._dataTable);
            EditorUtility.SetDirty(database);
        }
        else
        {
            SirenSkillDataBase database = ScriptableObject.CreateInstance<SirenSkillDataBase>();

            database._dataTable = new SirenSkillData[list.Count];

            list.CopyTo(database._dataTable);
            AssetDatabase.CreateAsset(database, path);
        }

    }
}
