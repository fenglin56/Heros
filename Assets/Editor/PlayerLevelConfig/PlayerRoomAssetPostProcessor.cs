using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;
using UnityEditor;
using System;

public class PlayerRoomAssetPostProcessor : AssetPostprocessor
{
    private static readonly string RESOURCE_TRAP_CONFIG_FOLDER = "Assets/Data/PlayerConfig/Res";
    private static readonly string ASSET_TRAP_CONFIG_FOLDER = "Assets/Data/PlayerConfig/Data";    

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {        
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            string path = System.IO.Path.Combine(RESOURCE_TRAP_CONFIG_FOLDER, "PlayerRoom.xml");
            TextReader tr = new StreamReader(path);
            string text = tr.ReadToEnd();

            if (text == null)
            {
                Debug.LogError("Player Room file not exist");
                return;
            }
            else
            {                
                XmlSpreadSheetReader.ReadSheet(text);
                XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
                string[] keys = XmlSpreadSheetReader.Keys;

                object[] levelIds = sheet[keys[0]];

                List<PlayerRoomAccoutConfigData> tempList = new List<PlayerRoomAccoutConfigData>();

               
                for (int i = 0; i < levelIds.Length; i++)
                {
                    if (0 == i || 1 == i) continue;
                    PlayerRoomAccoutConfigData data = new PlayerRoomAccoutConfigData();

                    data._roomTypeID = Convert.ToInt32(sheet["RoomTypeID"][i]);
                    data._basicsParam = Convert.ToInt32(sheet["BasicsParam"][i]);
                    data._guestAddition = Convert.ToInt32(sheet["GuestAddition"][i]);
                    data._ownerAddition = Convert.ToInt32(sheet["OwnerAddition"][i]);
                    string roomLevelStr = Convert.ToString(sheet["RoomLevel"][i]);
                    string[] roomLevelInfo = roomLevelStr.Split('+');
                    data._roomLevel = Convert.ToInt32(roomLevelInfo[1]);//取上限
                    data._upperLimit = Convert.ToInt32(sheet["UpperLimit"][i]);
                    string[] cameraPos = Convert.ToString(sheet["Camera"][i]).Split('+');
                    data._camera = new Vector3(Convert.ToSingle(cameraPos[1]), Convert.ToSingle(cameraPos[2]), Convert.ToSingle(cameraPos[3]));
                    //data._damageID = Convert.ToInt32(sheet["BoxID"][i]);
                    //data._damageName = Convert.ToString(sheet["BoxName"][i]);
                    //data._sirenPos = Convert.ToString(sheet["SirenPos"][i]);
                    data.SirenPosInfoList = new List<SirenPosInfo>();
                    string sirenPos = Convert.ToString(sheet["SirenPos"][i]);
                    string[] everySirenPos = sirenPos.Split('|');
                    everySirenPos.ApplyAllItem(p =>
                        {
                            string[] posInfo = p.Split('+');
                            Vector3 pos = new Vector3() { x = Convert.ToSingle(posInfo[1]), y = Convert.ToSingle(posInfo[2]), z = Convert.ToSingle(posInfo[3]) };
                            data.SirenPosInfoList.Add(new SirenPosInfo()
                            {
                                sirenID = Convert.ToInt32(posInfo[0]),
                                sirenPos = pos
                            });
                        });

                    data.PlayerPosList = new List<Vector3>();
                    data.PlayerAngleList = new List<Vector3>();
                    string playerPosStr = Convert.ToString(sheet["LandlordPos"][i]);
                    string playerHighAngleStr = Convert.ToString(sheet["LandlordPos3D"][i]);
                    string[] homerPos = playerPosStr.Split('+');
                    string[] homerHighAngle = playerHighAngleStr.Split('+');
                    Vector3 vHomerPos = new Vector3(Convert.ToSingle(homerPos[0]) / 10, Convert.ToSingle(homerHighAngle[0]) / 10, Convert.ToSingle(homerPos[1]) / -10);
                    Vector3 vHomerAngle = new Vector3(0, Convert.ToSingle(homerHighAngle[1]), 0);
                    data.PlayerPosList.Add(vHomerPos);
                    data.PlayerAngleList.Add(vHomerAngle);
                    for (int tenantNum = 1; tenantNum <= 5; tenantNum++)
                    {
                        string tenantPosStr = Convert.ToString(sheet["Tenant0" + tenantNum.ToString() + "Pos"][i]);
                        string tenantHighAngleStr = Convert.ToString(sheet["Tenant0" + tenantNum.ToString() + "Pos3D"][i]);
                        string[] tenantPos = tenantPosStr.Split('+');
                        string[] tenantHighAngle = tenantHighAngleStr.Split('+');
                        Vector3 vTenantPos = new Vector3(Convert.ToSingle(tenantPos[0]) / 10, Convert.ToSingle(tenantHighAngle[0]) / 10, Convert.ToSingle(tenantPos[1]) / -10);
                        Vector3 vTenantAngle = new Vector3(0, Convert.ToSingle(tenantHighAngle[1]), 0);
                        data.PlayerPosList.Add(vTenantPos);
                        data.PlayerAngleList.Add(vTenantAngle);
                    }
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


    private static void CreateSceneConfigDataBase(List<PlayerRoomAccoutConfigData> list)
    {
        string fileName = typeof(PlayerRoomAccoutConfigData).Name;
        string path = System.IO.Path.Combine(ASSET_TRAP_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            PlayerRoomAccoutConfigDataBase database = (PlayerRoomAccoutConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerRoomAccoutConfigDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new PlayerRoomAccoutConfigData[list.Count];
            list.CopyTo(database._dataTable);
            
            EditorUtility.SetDirty(database);
        }
        else
        {
            PlayerRoomAccoutConfigDataBase database = ScriptableObject.CreateInstance<PlayerRoomAccoutConfigDataBase>();

            database._dataTable = new PlayerRoomAccoutConfigData[list.Count];
            list.CopyTo(database._dataTable);
            
            AssetDatabase.CreateAsset(database, path);
        }

    }
}