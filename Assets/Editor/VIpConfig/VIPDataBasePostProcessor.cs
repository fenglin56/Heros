using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;
using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class VIPDataBasePostProcessor : AssetPostprocessor {
    public static readonly string RESOURCE_VIP_FOLDER = "Assets/Data/PlayerConfig/Res";
    private const string Data_VIP_FOLDER = "Assets/Data/PlayerConfig/Data";
    public static readonly string ASSET_VIP_ICON_FOLDER = "Assets/Prefab/GUI/IconPrefab/VIPIcon";
    
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            OnPostprocessVIP();
        }
        
    }

    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_VIP_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }


    private static void OnPostprocessVIP()
    {
        string path = System.IO.Path.Combine(RESOURCE_VIP_FOLDER, "VipPrivilege.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();
        
        if (text == null)
        {
            Debug.LogError("VIP file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;
            
            object[] levelIds = sheet[keys[0]];
            
            List<VIPConfigData> tempList = new List<VIPConfigData>();
            List<VipPrevillegeResData> previllegeList = new List<VipPrevillegeResData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                VIPConfigData data = new VIPConfigData();

                data.m_vipLevel = Convert.ToInt32(sheet["VipLevel"][i]);
                data.m_upgradeExp = Convert.ToInt32(sheet["UpgradeExp"][i]);
                data.m_freeDrugTimes =Convert.ToInt32(sheet["FreeDrug"][i]);
                data.m_energyPurchaseTimes = Convert.ToInt32(sheet["EnergyPurchaseTimes"][i]);
                data.m_mainEctypeRewardTimes = Convert.ToInt32(sheet["MainEctypeRewardTimes"][i]);
                data.m_ectypeExpBonus = Convert.ToInt32(sheet["EctypeExpBonus"][i]);
                data.m_luckDrawTimes = Convert.ToInt32(sheet["LuckDrawTimes"][i]);
                data.m_canEquipmentQuickStrengthen = Convert.ToBoolean(sheet["EquipmentQuickStrengthen"][i]);
                data.m_canEquipmentQuickUpgradeStar = Convert.ToBoolean(sheet["EquipmentQuickUpgradeStar"][i]);
                string iocnPrefabStr = Convert.ToString(sheet["VipBadge"][i]);
                string iocnPrefabPath = System.IO.Path.Combine(ASSET_VIP_ICON_FOLDER, iocnPrefabStr + ".prefab");
                data.m_vipEmblemPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(iocnPrefabPath, typeof(GameObject));
                data.m_RewardList = new List<VipLevelUpReward>();
                string strVipLevelRewardPrompt = Convert.ToString(sheet["VipLevelRewardPrompt"][i]);
				data.VipSweepNum = Convert.ToInt32(sheet["VipSweepNum"][i]);
                if(strVipLevelRewardPrompt != "0")
                {
                    string[] splitVipLevelRewardPromptStr = strVipLevelRewardPrompt.Split('|');

                    foreach(string str in splitVipLevelRewardPromptStr)
                    {
                        string[] splitItems = str.Split('+');
                        VipLevelUpReward reward = new VipLevelUpReward();
                        reward.m_vocation = Convert.ToInt32(splitItems[0]);
                        reward.m_itemID = Convert.ToInt32(splitItems[1]);
                        reward.m_itemCount = Convert.ToInt32(splitItems[2]);
                        data.m_RewardList.Add(reward);
                    }
                }


                tempList.Add(data);


          
                //previllege list
                VipPrevillegeResData pData = new VipPrevillegeResData();
                pData.m_vipLevel = Convert.ToInt32(sheet["VipLevel"][i]);
				string levelIocnPrefabStr = Convert.ToString(sheet["VipRes"][i]);
                string levelIocnPrefabPath = System.IO.Path.Combine(ASSET_VIP_ICON_FOLDER, levelIocnPrefabStr + ".prefab");
                pData.m_ipLevelIcon = (GameObject)AssetDatabase.LoadAssetAtPath(levelIocnPrefabPath, typeof(GameObject));

                string vipPrevilegeStr = Convert.ToString(sheet["VipPrivilegeDescription"][i]);
                string[] splitVipPrevilegeStr = vipPrevilegeStr.Split('|');
                pData.m_previllegeList = new List<PrevillegeItem>();


                if(vipPrevilegeStr != "0")
                {
                    foreach(string str in splitVipPrevilegeStr)
                    {
                        string[] splititems = str.Split('+');
                        PrevillegeItem item = new PrevillegeItem();

                        string previlegeIocnPrefabStr = splititems[0];
                        string previlegeIocnPrefabStrPath = System.IO.Path.Combine(ASSET_VIP_ICON_FOLDER, previlegeIocnPrefabStr + ".prefab");
                        item.m_icon =  (GameObject)AssetDatabase.LoadAssetAtPath(previlegeIocnPrefabStrPath, typeof(GameObject));

                        item.m_text = splititems[1];
                        pData.m_previllegeList.Add(item);
                    }
                }
                previllegeList.Add(pData);


            }

            CreateVIPConfigDataBase(tempList);
            CreateVipPrevillegeResDataBase(previllegeList);
        }



    }

    private static void CreateVIPConfigDataBase(List<VIPConfigData> list)
    {
        string fileName = typeof(VIPConfigDataBase).Name;
        string path = System.IO.Path.Combine(Data_VIP_FOLDER, fileName + ".asset");
        
        if (File.Exists(path))
        {
            VIPConfigDataBase database = (VIPConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(VIPConfigDataBase));
            
            if (null == database)
            {
                return;
            }
            
            database.m_dataTable = new VIPConfigData[list.Count];
            
            for (int i = 0; i < list.Count; i++)
            {
                database.m_dataTable[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            VIPConfigDataBase database = ScriptableObject.CreateInstance<VIPConfigDataBase>();
            database.m_dataTable = new VIPConfigData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.m_dataTable[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }

    private static void CreateVipPrevillegeResDataBase(List<VipPrevillegeResData> list)
    {
        string fileName = typeof(VipPrevillegeResDataBase).Name;
        string path = System.IO.Path.Combine(Data_VIP_FOLDER, fileName + ".asset");
        
        if (File.Exists(path))
        {
            VipPrevillegeResDataBase database = (VipPrevillegeResDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(VipPrevillegeResDataBase));
            
            if (null == database)
            {
                return;
            }
            
            database.m_dataTable = new VipPrevillegeResData[list.Count];
            
            for (int i = 0; i < list.Count; i++)
            {
                database.m_dataTable[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            VipPrevillegeResDataBase database = ScriptableObject.CreateInstance<VipPrevillegeResDataBase>();
            database.m_dataTable = new VipPrevillegeResData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.m_dataTable[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }
	
}
