using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class SoundAssetPostProcessor :  AssetPostprocessor{

    private static readonly string RESOURCE_SFX_FOLDER = "Assets/Data/SoundList/Res";
    private static readonly string ASSET_SFX_FOLDER = "Assets/Data/SoundList/Data";
    private static readonly string ASSET_SFX_CLIP_FOLDER = "Assets/Sound/SoundFX";
   
    
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            SfxPostprocess("Town");
            SfxPostprocess("Battle");
            SfxPostprocess("Common");
            SfxPostprocess("Login");
        }
    }

    private static void SfxPostprocess(string tabName)
    {
        string path = System.IO.Path.Combine(RESOURCE_SFX_FOLDER, "SFXSoundClipList.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();
        
        if (text == null)
        {
            Debug.LogError( tabName + " sfx config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text, tabName);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;
            
            object[] levelIds = sheet[keys[0]];
            
            List<SoundClip> tempList = new List<SoundClip>();
            
            for (int i = 2; i < levelIds.Length; i++)
            {
                SoundClip clip = new SoundClip();
                clip._name = Convert.ToString(sheet["Name"][i]);
                string clipPath = Convert.ToString(sheet["Clip"][i]);
                string pathRes = System.IO.Path.Combine(ASSET_SFX_CLIP_FOLDER, clipPath);

                clip._clip = AssetDatabase.LoadAssetAtPath(pathRes, typeof(AudioClip)) as AudioClip;
                tempList.Add(clip);
            }
            CreateSfxDataBase(tempList, tabName);
        }

       

    }


    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_SFX_FOLDER + "/SFXSoundClipList.xml"))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }

    private static void CreateSfxDataBase(List<SoundClip> list, string fileNameTitle)
    {
        string fileName = fileNameTitle + typeof(SoundClipList).Name;
        string path = System.IO.Path.Combine(ASSET_SFX_FOLDER, fileName + ".asset");
        
        if (File.Exists(path))
        {
            SoundClipList database = (SoundClipList)AssetDatabase.LoadAssetAtPath(path, typeof(SoundClipList));
            
            if (null == database)
            {
                return;
            }
            
            database._soundList = new SoundClip[list.Count];
            
            for (int i = 0; i < list.Count; i++)
            {
                
                database._soundList[i] = list[i];
                
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            SoundClipList database = ScriptableObject.CreateInstance<SoundClipList>();
            
            database._soundList = new SoundClip[list.Count];
            
            for (int i = 0; i < list.Count; i++)
            {
                database._soundList[i] = list[i];
                
            }
            AssetDatabase.CreateAsset(database, path);
        }
        
    }


}
