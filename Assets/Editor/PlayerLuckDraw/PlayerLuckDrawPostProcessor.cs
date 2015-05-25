using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;




public class PlayerLuckDrawPostProcessor : AssetPostprocessor 
{
    public static readonly string RESOURCE_LUCK_DRAW_FOLDER = "Assets/Data/PlayerLuckDraw/Res";
    public static readonly string ASSET_LUCK_DRAW_FOLDER = "Assets/Data/PlayerLuckDraw/Data";
    public static readonly string ASSET_LUCK_DRAW_ICON_FOLDER = "Assets/Prefab/GUI/IconPrefab/LuckDrawIcon";



    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            string path = System.IO.Path.Combine(RESOURCE_LUCK_DRAW_FOLDER, "PlayerLuckDraw.xml");
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
                
                List<PlayerLuckDrawData> tempList = new List<PlayerLuckDrawData>();
                
                for (int i = 2; i < levelIds.Length; i++)
                {
                    PlayerLuckDrawData data = new PlayerLuckDrawData();
                    data.m_luckId = Convert.ToInt32(sheet["ID"][i]);


                    //string iocnPrefabPath = System.IO.Path.Combine(ASSET_LUCK_DRAW_ICON_FOLDER, iocnPrefabStr + ".prefab");
                    //data.m_iconPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(iocnPrefabPath, typeof(GameObject));

                    string tipPrefabStr = Convert.ToString(sheet["StatusTips"][i]);
                    string tipPrefabPath = System.IO.Path.Combine(ASSET_LUCK_DRAW_ICON_FOLDER, tipPrefabStr + ".prefab");
                    data.m_tipPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(tipPrefabPath, typeof(GameObject));
                    
                    data.m_goodsNum = new List<ItemCountGroup>();
                    string strGoodNum = Convert.ToString(sheet["GoodsNum"][i]);
                    string iocnPrefabStr = Convert.ToString(sheet["StatusPicture"][i]);

                    if(strGoodNum != "0")
                    {
                        string[] splitNums = strGoodNum.Split('+');
                        string[] splitIconStrs = iocnPrefabStr.Split('+');
                        for(int j = 0; j < splitNums.Length; j++)
                        {    
                            ItemCountGroup group = new ItemCountGroup();
                            string iocnPrefabPath = System.IO.Path.Combine(ASSET_LUCK_DRAW_ICON_FOLDER, splitIconStrs[j] + ".prefab");
                            group.m_icon = (GameObject)AssetDatabase.LoadAssetAtPath(iocnPrefabPath, typeof(GameObject));
                            group.m_itemCount = Convert.ToInt32(splitNums[j]);
                            data.m_goodsNum.Add(group);
                        }
                    }


                    tempList.Add(data);
                }
                CreateLuckDrawDataBase(tempList);



            }
        }
    }

    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_LUCK_DRAW_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }
    
    private static void CreateLuckDrawDataBase(List<PlayerLuckDrawData> list)
    {
        string fileName = typeof(PlayerLuckDrawDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_LUCK_DRAW_FOLDER, fileName + ".asset");
        
        if (File.Exists(path))
        {
            PlayerLuckDrawDataBase database = (PlayerLuckDrawDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerLuckDrawDataBase));
            
            if (null == database)
            {
                return;
            }
            database.m_dataTable = new PlayerLuckDrawData[list.Count];
            list.CopyTo(database.m_dataTable);
            EditorUtility.SetDirty(database);
        }
        else
        {
            PlayerLuckDrawDataBase database = ScriptableObject.CreateInstance<PlayerLuckDrawDataBase>();
            
            database.m_dataTable = new PlayerLuckDrawData[list.Count];
            
            list.CopyTo(database.m_dataTable);
            AssetDatabase.CreateAsset(database, path);
        }

    }

	
}
