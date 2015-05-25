using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;
public class ForgeRecipeConfigAssetPosProcessor : AssetPostprocessor {
    public static readonly string RESOURCE_UI_CONFIG_FOLDER = "Assets/Data/ForgeRecipeConfig/Res";
    public static readonly string ASSET_UI_CONFIG_FOLDER = "Assets/Data/ForgeRecipeConfig/Data";

    
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
                                               string[] movedAssets, string[] movedFromPath)
    {
        
        if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
        {
            string path = System.IO.Path.Combine(RESOURCE_UI_CONFIG_FOLDER, "ForgeRecipe.xml");
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
                
                List<ForgeRecipeData> tempList = new List<ForgeRecipeData>();
                
                for(int i = 2; i< levelIds.Length; i++ )
                {
                    ForgeRecipeData data= new ForgeRecipeData();

                    data.ForgeID=Convert.ToInt32(sheet["ForgeID"][i]);
                    data.ForgeEquipmentID=Convert.ToInt32(sheet["ForgeEquipmentID"][i]);
                    string[] str=Convert.ToString(sheet["ForgeProfession"][i]).Split('+');
                    data.ForgeProfession=new int[str.Length];
                    for(int j=0;j<str.Length;j++)
                    {
                        data.ForgeProfession[j]=Convert.ToInt32(str[j]);
                    }
                    string[] str1=Convert.ToString(sheet["ForgeCost"][i]).Split('|');
                    data.ForgeCost=new Recipe[str1.Length];
                    for(int j=0;j<str1.Length;j++)
                    {
                       string[] str2= str1[j].Split('+');
                        Recipe recipe=new Recipe(){RecipeID=Convert.ToInt32( str2[0]),count=Convert.ToInt32(str2[1])};
                        data.ForgeCost[j]=recipe;
                    }
                    data.ForgeIDS=Convert.ToString(sheet["ForgeIDS"][i]);
                    data.ForgeType=(UI.Forging.ForgingType)Convert.ToInt32(sheet["ForgeType"][i]);
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
    
    
    private static void CreateSceneConfigDataBase(List<ForgeRecipeData> list)
    {
        string fileName = typeof(ForgeRecipeDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_UI_CONFIG_FOLDER, fileName + ".asset");
        
        if(File.Exists(path))
        {
            ForgeRecipeDataBase database = (ForgeRecipeDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(ForgeRecipeDataBase));
            
            if(null == database)
            {
                return;
            }
            
            database.ForgeRecipeDataList = new ForgeRecipeData[list.Count];
            
            for(int i = 0; i < list.Count; i++)
            {
                database.ForgeRecipeDataList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            ForgeRecipeDataBase database = ScriptableObject.CreateInstance<ForgeRecipeDataBase>();
            
            database.ForgeRecipeDataList = new ForgeRecipeData[list.Count];
            
            for(int i = 0; i < list.Count; i++)
            {
                database.ForgeRecipeDataList[i] = list[i];

            }
            AssetDatabase.CreateAsset(database, path);
        }
        
    }
}
