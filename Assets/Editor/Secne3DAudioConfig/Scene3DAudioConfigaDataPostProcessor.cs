using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;
public class Scene3DAudioConfigaDataPostProcessor : AssetPostprocessor {
    public static readonly string EW_RESOURCE_MULTILANGUAGE_FOLDER = "Assets/Data/Scene3DSoundConfig/Res";
    public static readonly string EW_ASSETS_MULTILANGUAGE_FOLDER = "Assets/Data/Scene3DSoundConfig/Data";
    
    
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
                                               string[] movedAssets, string[] movedFromPath)
    {
        if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
        {
            string path = System.IO.Path.Combine( EW_RESOURCE_MULTILANGUAGE_FOLDER, "Scene3DAudio.xml" );
            
            TextReader tr = new StreamReader(path);
            
            string text = tr.ReadToEnd();
            
            if(text == null)
            {
                Debug.LogError("Scene3DAudio file not exist");
                return; 
            }
            else
            {
                XmlSpreadSheetReader.ReadSheet(text);
                
                XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
                
                int langCount =  XmlSpreadSheetReader.Keys.Length - 1;
                
                string[] keys =  XmlSpreadSheetReader.Keys;
                
                object[] AudioIds = sheet[keys[0]];
                List<Scene3DAudioData> tempList = new List<Scene3DAudioData>();
                for(int i = 2; i< AudioIds.Length; i++ )
                {
                    Scene3DAudioData data = new Scene3DAudioData();
                    data.MapId = Convert.ToInt32(sheet["MapID"][i]);
                    string posStr = sheet["PointPos"][i].ToString();
                    string[] posXYZ=posStr.Split('+');
                    data.PointPos=new Vector3(int.Parse(posXYZ[1]),int.Parse(posXYZ[2]),int.Parse(posXYZ[3]));
                    data.Radius =Convert.ToInt32( sheet["Radius"][i]);
                    data.Sound=sheet["Sound"][i].ToString();
                    tempList.Add(data);
                }
                CreateScene3DAudioDataBase(tempList);
            }
        }
        
    }
    
    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach(string file in files)
        {
            if(file.Contains(EW_RESOURCE_MULTILANGUAGE_FOLDER))
            {
                fileModified = true;    
                break;
            }
        }
        return fileModified;
    }
    
    
    private static void CreateScene3DAudioDataBase(List<Scene3DAudioData> list)
    {
        string className = typeof(Scene3DAudioConfigaDataBase).Name;
        string path = System.IO.Path.Combine(EW_ASSETS_MULTILANGUAGE_FOLDER, className +".asset");
        
        if(File.Exists(path))
        {
            Scene3DAudioConfigaDataBase database = (Scene3DAudioConfigaDataBase)AssetDatabase.LoadAssetAtPath(path,typeof(Scene3DAudioConfigaDataBase));
            
            if(null == database)
            {
                return;
            }
            database.SoundList = new Scene3DAudioData[list.Count];
            
            for(int i = 0; i < list.Count; i++)
            {
                database.SoundList[i]= new Scene3DAudioData();
                database.SoundList[i].MapId= list[i].MapId;
                database.SoundList[i].PointPos= list[i].PointPos;
                database.SoundList[i].Radius= list[i].Radius;
                database.SoundList[i].Sound= list[i].Sound;
            }
            EditorUtility.SetDirty(database);
        }
        
        else
        {
            Scene3DAudioConfigaDataBase database = ScriptableObject.CreateInstance<Scene3DAudioConfigaDataBase>();
            
            database.SoundList = new Scene3DAudioData[list.Count];
            for(int i = 0; i < list.Count; i++)
            {
                database.SoundList[i] = new Scene3DAudioData();
                database.SoundList[i].MapId = list[i].MapId;
                database.SoundList[i].PointPos= list[i].PointPos;
                database.SoundList[i].Radius= list[i].Radius;
                database.SoundList[i].Sound= list[i].Sound;
            }
            AssetDatabase.CreateAsset(database, path);
        }
}
}