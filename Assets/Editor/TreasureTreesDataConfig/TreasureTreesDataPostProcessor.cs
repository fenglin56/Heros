using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class TreasureTreesDataPostProcessor : AssetPostprocessor
{


    private static readonly string RESOURCE_TREASURETREES_CONFIG_FOLDER = "Assets/Data/TreasureTreesData/Res";
    private static readonly string ASSET_TREASURETREES_CONFIG_FOLDER = "Assets/Data/TreasureTreesData/Data";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            PostProcessorTreasureTreesData();
            PostProcessorFruitData();
        }
    }

    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_TREASURETREES_CONFIG_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }

    private static void PostProcessorTreasureTreesData()
    {
        string path = System.IO.Path.Combine(RESOURCE_TREASURETREES_CONFIG_FOLDER, "PlayerTree.xml");
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

            List<LocalTreasureTreesData> tempList = new List<LocalTreasureTreesData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                LocalTreasureTreesData data = new LocalTreasureTreesData();
                data.TreeID = Convert.ToInt32(sheet["TreeID"][i]);
                data.PositionID = Convert.ToInt32(sheet["PositionID"][i]);
                data.UnlockLevel = Convert.ToInt32(sheet["UnlockLevel"][i]);
                data.UnlockCost = Convert.ToInt32(sheet["UnlockCost"][i]);
                data.CreatFruit = Convert.ToString(sheet["CreatFruit"][i]);
                tempList.Add(data);
            }

            CreateConfigDataBase(tempList);
        }
    }

    private static void PostProcessorFruitData()
    {
        string path = System.IO.Path.Combine(RESOURCE_TREASURETREES_CONFIG_FOLDER, "PlayerFruit.xml");
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

            List<FruitData> tempList = new List<FruitData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                FruitData data = new FruitData();
                data.FruitID = Convert.ToInt32(sheet["FruitID"][i]);
                data.FruitName = Convert.ToString(sheet["FruitName"][i]);
                data.SeedModelID = Convert.ToString(sheet["SeedModelID"][i]);
                data.FlowerModelID = Convert.ToString(sheet["FlowerModelID"][i]);
                data.GrowModelID = Convert.ToString(sheet["GrowModelID"][i]);
                data.RipenModelID = Convert.ToString(sheet["RipenModelID"][i]);
                data.FruitLevel = Convert.ToInt32(sheet["FruitLevel"][i]);
                data.FlowerTime = Convert.ToInt32(sheet["FlowerTime"][i]);
                data.GrowTime = Convert.ToInt32(sheet["GrowTime"][i]);
                data.RipenTime = Convert.ToInt32(sheet["RipenTime"][i]);
                data.RewardType = Convert.ToInt32(sheet["RewardType"][i]);
                data.RewardNumber = Convert.ToString(sheet["RewardNumber"][i]);
                data.FruitReward = Convert.ToString(sheet["FruitReward"][i]).Split('+');
                tempList.Add(data);
            }

            CreateConfigDataBase(tempList);
        }
    }


    private static void CreateConfigDataBase(List<LocalTreasureTreesData> list)
    {
        string fileName = typeof(TreasureTreesDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_TREASURETREES_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            TreasureTreesDataBase database = (TreasureTreesDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(TreasureTreesDataBase));

            if (null == database)
            {
                return;
            }

            database.TreasureTreesDataList = new LocalTreasureTreesData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.TreasureTreesDataList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            TreasureTreesDataBase database = ScriptableObject.CreateInstance<TreasureTreesDataBase>();

            database.TreasureTreesDataList = new LocalTreasureTreesData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.TreasureTreesDataList[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }

    }
    private static void CreateConfigDataBase(List<FruitData> list)
    {
        string fileName = typeof(TreasureTreesDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_TREASURETREES_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            TreasureTreesDataBase database = (TreasureTreesDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(TreasureTreesDataBase));

            if (null == database)
            {
                return;
            }

            database.FruitDataList = new FruitData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.FruitDataList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            TreasureTreesDataBase database = ScriptableObject.CreateInstance<TreasureTreesDataBase>();

            database.FruitDataList = new FruitData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.FruitDataList[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }

    }

}
