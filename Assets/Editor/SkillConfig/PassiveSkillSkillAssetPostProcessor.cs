using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class PassiveSkillSkillAssetPostProcessor : AssetPostprocessor
{

    public static readonly string EW_RESOURCE_SKILL_CONFIG_FOLDER = "Assets/Data/SkillConfig/Res";
    public static readonly string EW_ASSET_SKILL_CONFIG_FOLDER = "Assets/Data/SkillConfig/Data";

    public static readonly string SkillIconPrefabPath = "Assets/Prefab/GUI/ItemIcon/SkillICon";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {

        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
			string path = System.IO.Path.Combine(EW_RESOURCE_SKILL_CONFIG_FOLDER, "PlayerPassiveSkill.xml");
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

                List<PassiveSkillData> tempList = new List<PassiveSkillData>();

                for (int i = 2; i < levelIds.Length; i++)
                {
                    PassiveSkillData data = new PassiveSkillData();

                    data.SkillID = Convert.ToInt32(sheet["SkillID"][i]);
                    data.SkillLevel = Convert.ToInt32(sheet["SkillLevel"][i]);
                    data.MaxLevel = Convert.ToInt32(sheet["MaxLevel"][i]);
                    data.SkillName = Convert.ToString(sheet["SkillName"][i]);
                    data.SkillIcon = Convert.ToString(sheet["SkillIcon"][i]);
                    data.SkillDis = Convert.ToString(sheet["SkillDis"][i]);

                    string iconPath = System.IO.Path.Combine(SkillIconPrefabPath, data.SkillIcon + ".prefab");
                    data.SkillIconPrefab = AssetDatabase.LoadAssetAtPath(iconPath,typeof(GameObject)) as GameObject;

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

    private static void CreateBulletDataBase(List<PassiveSkillData> list)
    {
        string fileName = typeof(PassiveSkillDataBase).Name;
        string path = System.IO.Path.Combine(EW_ASSET_SKILL_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            PassiveSkillDataBase database = (PassiveSkillDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PassiveSkillDataBase));

            if (null == database)
            {
                return;
            }
            database._dataTable = new PassiveSkillData[list.Count];
            list.CopyTo(database._dataTable);
            EditorUtility.SetDirty(database);
        }
        else
        {
            PassiveSkillDataBase database = ScriptableObject.CreateInstance<PassiveSkillDataBase>();

            database._dataTable = new PassiveSkillData[list.Count];

            list.CopyTo(database._dataTable);
            AssetDatabase.CreateAsset(database, path);
        }

    }
}
