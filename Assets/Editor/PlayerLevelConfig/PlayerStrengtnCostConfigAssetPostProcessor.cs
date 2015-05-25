using UnityEngine;
using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;
using UI.MainUI;


public class PlayerStrengtnCostConfigAssetPostProcessor : AssetPostprocessor {
    public static readonly string RESOURCE_DATA_FOLDER = "Assets/Data/PlayerConfig/Res";
    public static readonly string ASSET_DATA_FOLDER = "Assets/Data/PlayerConfig/Data";
    
    
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            OnPostprocessEquipment();
        }
        
    }
    
    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }
    
    
    private static void OnPostprocessEquipment()
    {
        
        string path = System.IO.Path.Combine(RESOURCE_DATA_FOLDER, "PlayerStrengthCost.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();
        
        if (text == null)
        {
            Debug.LogError("Equipment item file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;
            
            object[] levelIds = sheet[keys[0]];
            
            List<PlayerStrengthCost> tempList = new List<PlayerStrengthCost>();
            
            for (int i = 2; i < levelIds.Length; i++)
            {
                //if (0 == i) continue;
                PlayerStrengthCost data = new PlayerStrengthCost();
                data.GainType=(UpgradeType)Convert.ToInt32(sheet["GainType"][i]);
                data.GainLevel=Convert.ToInt32(sheet["GainLevel"][i]);
				data.lGoodsSubClass=(EquiptType)Convert.ToInt32(sheet["lGoodsSubClass"][i]);
                string[] UpgradeRequireStr=Convert.ToString(sheet["UpgradeRequires"][i]).Split('|');
                List<UpgradeRequire> UpgradeRequires=new List<UpgradeRequire>();
                foreach(string item in UpgradeRequireStr)
                {
                    UpgradeRequire ur=new UpgradeRequire();
                    string[] strs=item.Split('+');
                    ur.GoodsId=Convert.ToInt32( strs[0]);
                    ur.Count=Convert.ToInt32( strs[1]);
                    UpgradeRequires.Add(ur);
                }

                data.UpgradeRequires=UpgradeRequires;
                tempList.Add(data);
            }
            
            
            CreateMedicamentConfigDataList(tempList);
        }
        
    }
    
    
    static void CreateMedicamentConfigDataList(List<PlayerStrengthCost> list)
    {
        string fileName = typeof(PlayerStrengthCost).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_DATA_FOLDER, fileName + ".asset");
        
        if (File.Exists(path))
        {
            PlayerStrengthCostDataBase database = (PlayerStrengthCostDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerStrengthCostDataBase));
            
            if (null == database)
            {
                return;
            }
            
            database.PlayerStrengthCostList = new PlayerStrengthCost[list.Count];
            
            for (int i = 0; i < list.Count; i++)
            {
                database.PlayerStrengthCostList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            PlayerStrengthCostDataBase database = ScriptableObject.CreateInstance<PlayerStrengthCostDataBase>();
            database.PlayerStrengthCostList = new PlayerStrengthCost[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.PlayerStrengthCostList[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
}
