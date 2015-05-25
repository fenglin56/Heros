using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class BulletImpactAssetPostProcessor : AssetPostprocessor
{

	public static readonly string EW_RESOURCE_SKILL_CONFIG_FOLDER = "Assets/Data/SkillConfig/Res";
    public static readonly string EW_ASSET_SKILL_CONFIG_FOLDER = "Assets/Data/SkillConfig/Data";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            string path = System.IO.Path.Combine(EW_RESOURCE_SKILL_CONFIG_FOLDER, "BulletImpact.xml");
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

                List<BulletImpactData> tempList = new List<BulletImpactData>();

                for (int i = 2; i < levelIds.Length; i++)
                {
                    BulletImpactData data = new BulletImpactData();
					data.m_id = Convert.ToInt32(sheet["id"][i]);
					data.m_damage_type = Convert.ToInt32(sheet["damage_type"][i]);
					data.m_beatBackLevel = Convert.ToInt32(sheet["hit_lv"][i]);
					data.m_beatBackDir = Convert.ToInt32(sheet["hit_dir"][i]);
					data.m_beatBackDuration = Convert.ToSingle(sheet["hit_time"][i]);
					data.m_beatBackSpeed = Convert.ToSingle(sheet["hit_speed"][i]);
					data.m_beatBackAcceleration = Convert.ToSingle(sheet["hit_accspeed"][i]);
					data.m_beatFlyLevel = Convert.ToInt32(sheet["fly_level"][i]);
					data.m_beatFlySpeed = Convert.ToSingle(sheet["fly_speed"][i]);
					data.m_beatFlyAcceleration = Convert.ToSingle(sheet["fly_accspeed"][i]);
					data.m_beatFlyVerticalSpeed = Convert.ToSingle(sheet["fly_verspeed"][i]);

                    data.m_teleportLevel = Convert.ToSingle(sheet["teleport_level"][i]);
                    var desInfo = Convert.ToString(sheet["teleport_destination"][i]).Split('+');
                    data.m_teleportDestination = new Vector3(float.Parse(desInfo[1]), 0, float.Parse( desInfo[2]));
                    var areaInfo = Convert.ToString(sheet["teleport_area"][i]).Split('+');
                    data.m_teleportArea = new Vector3(float.Parse(areaInfo[1]), 0, float.Parse(areaInfo[2]));
                    data.m_teleportAngle = Convert.ToSingle(sheet["teleport_angle"][i]);

					data.m_affect_src = new int[3];
					for(int index = 0;index<data.m_affect_src.Length;index++)
					{
						data.m_affect_src[index] = Convert.ToInt32(sheet["affect_src"+(index+1).ToString()][i]);
					}
					data.m_affect_prop = new int[3];
					for(int index = 0;index<data.m_affect_prop.Length;index++)
					{
						data.m_affect_prop[index] = Convert.ToInt32(sheet["affect_prop"+(index+1).ToString()][i]);
					}

					tempList.Add(data);
                }

                CreateBulletImpactDataBase(tempList);
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
	
	private static void CreateBulletImpactDataBase(List<BulletImpactData> list)
    {
        string fileName = typeof(BulletImpactDataBase).Name;
        string path = System.IO.Path.Combine(EW_ASSET_SKILL_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            BulletImpactDataBase database = (BulletImpactDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(BulletImpactDataBase));

            if (null == database)
            {
                return;
            }
            database._dataTable = new BulletImpactData[list.Count];
			list.CopyTo(database._dataTable);
            EditorUtility.SetDirty(database);
        }
        else
        {
            BulletImpactDataBase database = ScriptableObject.CreateInstance<BulletImpactDataBase>();

            database._dataTable = new BulletImpactData[list.Count];

            list.CopyTo(database._dataTable);
            AssetDatabase.CreateAsset(database, path);
        }

    }
}
