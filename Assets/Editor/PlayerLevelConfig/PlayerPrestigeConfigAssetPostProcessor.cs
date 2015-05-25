using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;
using UnityEditor;
using System;

public class PlayerPrestigeConfigAssetPostProcessor : AssetPostprocessor
{
    private static readonly string RESOURCE_TRAP_CONFIG_FOLDER = "Assets/Data/PlayerConfig/Res";
    private static readonly string ASSET_TRAP_CONFIG_FOLDER = "Assets/Data/PlayerConfig/Data";
    //private static readonly string ASSET_TRAP_RES_CONFIG_FOLDER = "Assets/MapItems/Damages/Prefab";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {        
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            string path = System.IO.Path.Combine(RESOURCE_TRAP_CONFIG_FOLDER, "PlayerPrestige.xml");
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

                List<PlayerPrestigeConfigData> tempList = new List<PlayerPrestigeConfigData>();

                for (int i = 0; i < levelIds.Length; i++)
                {
                    if (0 == i || 1 == i) continue;
                    PlayerPrestigeConfigData data = new PlayerPrestigeConfigData();
                   
                    data._pvpLevel = Convert.ToInt32(sheet["PvpLevel"][i]);
                    data._pvpGroup = Convert.ToInt32(sheet["PvpGroup"][i]);
                    data._titleName = Convert.ToString(sheet["TitleName"][i]);
                    data._pvpExp = Convert.ToInt32(sheet["PvpExp"][i]);
                    data._pvpInsignia = Convert.ToInt32(sheet["PvpInsignia"][i]);
                    data._groupName = Convert.ToString(sheet["GroupName"][i]);
                    data._title_ID = Convert.ToString(sheet["Title_ID"][i]);
                    data._titlePrefab = (GameObject)Resources.LoadAssetAtPath("Assets/" + data._title_ID + ".prefab", typeof(GameObject)); 
                    //
                    if (2 == i)
                    {
                        string provocation = Convert.ToString(sheet["ProvocationWord"][i]);
                        data._ProvocationWord = provocation.Split('+');
                        string win = Convert.ToString(sheet["WinWord"][i]);
                        data._WinWord = win.Split('+');
                        string lose = Convert.ToString(sheet["LoseWord"][i]);
                        data._LoseWord = lose.Split('+');
                    }
                    //data._damageID = Convert.ToInt32(sheet["BoxID"][i]);
                    //data._damageName = Convert.ToString(sheet["BoxName"][i]);

                    //data._damagePrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                    //string correspondingItemIDStr = Convert.ToString(sheet["BoxGoodsDrop"][i]);
                    //string[] splitCorrespondingItemIDStrs = correspondingItemIDStr.Split("+".ToCharArray());
                    //data._correspondingItemID = Convert.ToInt32(splitCorrespondingItemIDStrs[0]);

                    tempList.Add(data);
                }

                CreateSceneConfigDataBase(tempList);
            }
        }
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


    private static void CreateSceneConfigDataBase(List<PlayerPrestigeConfigData> list)
    {
        string fileName = typeof(PlayerPrestigeConfigData).Name;
        string path = System.IO.Path.Combine(ASSET_TRAP_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            PlayerPrestigeConfigDataBase database = (PlayerPrestigeConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerPrestigeConfigDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new PlayerPrestigeConfigData[list.Count];
            list.CopyTo(database._dataTable);
            //for (int i = 0; i < list.Count; i++)
            //{
            //    database._dataTable[i] = new PlayerPrestigeConfigData();
            //    database._dataTable[i] = list[i];
            //}
            EditorUtility.SetDirty(database);
        }
        else
        {
            PlayerPrestigeConfigDataBase database = ScriptableObject.CreateInstance<PlayerPrestigeConfigDataBase>();

            database._dataTable = new PlayerPrestigeConfigData[list.Count];
            list.CopyTo(database._dataTable);
            //for (int i = 0; i < list.Count; i++)
            //{
            //    database._dataTable[i] = new PlayerPrestigeConfigData();
            //    database._dataTable[i] = list[i];
            //}
            AssetDatabase.CreateAsset(database, path);
        }

    }
}