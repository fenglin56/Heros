using UnityEngine;
using System;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;
public class LinkConfigPostProcessor : AssetPostprocessor {
    public static readonly string RESOURCE_DATA_FOLDER = "Assets/Data/Link/Res";
    public static readonly string ASSET_DATA_FOLDER = "Assets/Data/Link/Data";
    public static readonly string ASSET_ITEM_DATA_ICON_FOLDER = "Assets/Prefab/GUI/IconPrefab/GetPathIcon";
    
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
        
        string path = System.IO.Path.Combine(RESOURCE_DATA_FOLDER, "LinkConfig.xml");
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
            
            List<LinkConfigItemData> tempList = new List<LinkConfigItemData>();
            
            for (int i = 2; i < levelIds.Length; i++)
            {
                //if (0 == i) continue;
                LinkConfigItemData data = new LinkConfigItemData();

                data.LinkID = Convert.ToString(sheet["LinkID"][i]);
                data.LinkType= (LinkType)Convert.ToInt32(sheet["LinkType"][i]);
                data.LinkPara= Convert.ToString(sheet["LinkPara"][i]);
				data.LinkChildren= Convert.ToInt32(sheet["LinkChildren"][i]);
                data.LinkName= Convert.ToString(sheet["LinkName"][i]);
                data.Des= Convert.ToString(sheet["Des"][i]);
                string[] LinkIconPaths= Convert.ToString(sheet["LinkIcon"][i]).Split('+');
                List<GameObject> prefabList=new List<GameObject>();
                foreach(string item in LinkIconPaths)
                {
                    string pathRes = System.IO.Path.Combine(ASSET_ITEM_DATA_ICON_FOLDER, item + ".prefab");
                   GameObject go= (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));
                    prefabList.Add(go);
                }

                data.LinkIcon= prefabList;
                tempList.Add(data);
            }
            
            
            CreateMedicamentConfigDataList(tempList);
        }
        
    }
    
    
    static void CreateMedicamentConfigDataList(List<LinkConfigItemData> list)
    {
        string fileName = typeof(LinkConfigItemData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_DATA_FOLDER, fileName + ".asset");
        
        if (File.Exists(path))
        {
            LinkConfigDaraBase database = (LinkConfigDaraBase)AssetDatabase.LoadAssetAtPath(path, typeof(LinkConfigDaraBase));
            
            if (null == database)
            {
                return;
            }
            
            database.LinkConfigItemList = new LinkConfigItemData[list.Count];
            
            for (int i = 0; i < list.Count; i++)
            {
                database.LinkConfigItemList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            LinkConfigDaraBase database = ScriptableObject.CreateInstance<LinkConfigDaraBase>();
            database.LinkConfigItemList = new LinkConfigItemData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.LinkConfigItemList[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
}
