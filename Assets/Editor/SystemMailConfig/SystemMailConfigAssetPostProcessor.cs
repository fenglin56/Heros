using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class SystemMailConfigAssetPostProcessor : AssetPostprocessor {
    public static readonly string RESOURCE_UI_CONFIG_FOLDER = "Assets/Data/SystemMailConfig/Res";
    public static readonly string ASSET_UI_CONFIG_FOLDER = "Assets/Data/SystemMailConfig/Data";
    
    
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
                                               string[] movedAssets, string[] movedFromPath)
    {
        
        if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
        {
            string path = System.IO.Path.Combine(RESOURCE_UI_CONFIG_FOLDER, "SystemMail.xml");
            TextReader tr = new StreamReader(path);
            string text = tr.ReadToEnd();
            
            if(text == null)
            {
                Debug.LogError("Player level config file not exist");
                return; 
            }
            else
            {
                XmlSpreadSheetReader.ReadSheet(text);
                XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
                string[] keys =  XmlSpreadSheetReader.Keys;
                
                object[] levelIds = sheet[keys[0]];
                
                List<SystemMailConfigData > tempList = new List<SystemMailConfigData>();
                
                for(int i = 2; i< levelIds.Length; i++ )
                {
                    SystemMailConfigData data= new SystemMailConfigData();
                    
                    data.MailType=Convert.ToInt32(sheet["MailType"][i]);
                    data.MailTitle=Convert.ToString(sheet["MailTitle"][i]);
                    data.MailText=Convert.ToString(sheet["MailText"][i]);
                    tempList.Add(data);
                }
           
                CreateSceneConfigDataBase(tempList);
                
            }
        }
    }
    
    
    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach(string file in files)
        {
            if (file.Contains(RESOURCE_UI_CONFIG_FOLDER))
            {
                fileModified = true;    
                break;
            }
        }
        return fileModified;
    }
    

    private static void CreateSceneConfigDataBase(List<SystemMailConfigData> list)
    {
        string fileName = typeof(SystemMailConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_UI_CONFIG_FOLDER, fileName + ".asset");
        Debug.Log("22");
        if(File.Exists(path))
        {
            SystemMailConfigDataBase database = (SystemMailConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(SystemMailConfigDataBase));

            if(null == database)
            {
                return;
            }
            
            database.SystemMailConfigDataList = new SystemMailConfigData[list.Count];
            
            for(int i = 0; i < list.Count; i++)
            {
                database.SystemMailConfigDataList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            SystemMailConfigDataBase database = ScriptableObject.CreateInstance<SystemMailConfigDataBase>();
            
            database.SystemMailConfigDataList = new SystemMailConfigData[list.Count];
            
            for(int i = 0; i < list.Count; i++)
            {
                database.SystemMailConfigDataList[i] = list[i];
                
            }
            AssetDatabase.CreateAsset(database, path);
        }
}
}