using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class StroyLineConfig : AssetPostprocessor 
{
    private static readonly string RESOURCE_STROY_CONFIG_FOLDER = "Assets/Data/StroyLineConfig/Res";
    private static readonly string ASSET_STROY_CONFIG_FOLDER = "Assets/Data/StroyLineConfig/Data";
    private static readonly string ASSET_EFFECT_RES_FOLDER = "Assets/Effects";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
	
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            StroyLineConfigPostprocess();
            CameraGroupConfigPostprocess();
            StroyDialogConfigPostprocess();
            StroyActionConfigPostprocess();
            StroyCameraConfigPostprocess();
		}
	}

    private static void StroyCameraConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_STROY_CONFIG_FOLDER, "CameraConfig.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("stroyCamera config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<StroyCameraConfigData> tempList = new List<StroyCameraConfigData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                StroyCameraConfigData data = new StroyCameraConfigData();
                data._CameraID = Convert.ToInt32(sheet["CameraID"][i]);
                data._TargetType = Convert.ToInt32(sheet["TargetType"][i]);

                var targetPosString = Convert.ToString(sheet["TargetPos"][i]);
                string[] splitTarPosString = targetPosString.Split("+".ToCharArray());
                float posX = Convert.ToSingle(splitTarPosString[0]);
                float posZ = Convert.ToSingle(splitTarPosString[1]);
                data._TargetPos = new Vector2(posX, posZ);
                 
                string targetOffsetStr = Convert.ToString(sheet["TargetOffset"][i]);
                string[] splitTargetOffsetStrs = targetOffsetStr.Split('+');
                float OffsetX = Convert.ToSingle(splitTargetOffsetStrs[1]);
                float OffsetY = Convert.ToSingle(splitTargetOffsetStrs[2]);
                float OffsetZ = Convert.ToSingle(splitTargetOffsetStrs[3]);
                data._TargetOffset = new Vector3(OffsetX, OffsetY, OffsetZ);


                data._TargetID = Convert.ToInt32(sheet["TargetID"][i]);
                data._ActionTime = Convert.ToSingle(sheet["ActionTime"][i]);
                data._CameraMask = Convert.ToInt32(sheet["CameraChange"][i]);
                string parmsStr = Convert.ToString(sheet["Params"][i]);
                string[] splitCameraParmStr = parmsStr.Split("+".ToCharArray());

                data._Params = new CameraParam[3];
                for (int j = 0; j < 3; j++)
                {
                    data._Params[j] = new CameraParam();
                    data._Params[j]._EquA = Convert.ToSingle(splitCameraParmStr[j * 4]);
                    data._Params[j]._EquB = Convert.ToSingle(splitCameraParmStr[j * 4 + 1]);
                    data._Params[j]._EquC = Convert.ToSingle(splitCameraParmStr[j * 4 + 2]);
                    data._Params[j]._EquD = Convert.ToSingle(splitCameraParmStr[j * 4 + 3]);
                }
                data._MoveMode = Convert.ToInt32(sheet["ActionType"][i]);
                

                tempList.Add(data);
            }

            CreateStroyCameraConfigDataBase(tempList);
        }
    }

    private static void CreateStroyCameraConfigDataBase(List<StroyCameraConfigData> list)
    {
        string fileName = typeof(StroyCameraDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_STROY_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            StroyCameraDataBase database = (StroyCameraDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(StroyCameraDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new StroyCameraConfigData[list.Count];


            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            StroyCameraDataBase database = ScriptableObject.CreateInstance<StroyCameraDataBase>();

            database._dataTable = new StroyCameraConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }

    private static void StroyActionConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_STROY_CONFIG_FOLDER, "ActionConfig.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("stroyAction config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<StroyActionConfigData> tempList = new List<StroyActionConfigData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                StroyActionConfigData data = new StroyActionConfigData();
                data._ActionID = Convert.ToInt32(sheet["ActionID"][i]);
                data._ActionName = Convert.ToString(sheet["Name"][i]);
                var startPosString = Convert.ToString(sheet["StartPosition"][i]);
                string[] splitposString = startPosString.Split("+".ToCharArray());
                float posX = Convert.ToSingle(splitposString[0]);
                float posY = Convert.ToSingle(splitposString[1]);
                float posZ = Convert.ToSingle(splitposString[2]);
                data._StartPosition = new Vector3(posX, posY, posZ);

                data._ActionType = Convert.ToInt32(sheet["Type"][i]);
                data._StartAngle = Convert.ToSingle(sheet["StartAngle"][i]);
                data._ModelAngle = Convert.ToSingle(sheet["ModelAngle"][i]);
                data._Speed = Convert.ToSingle(sheet["Speed"][i]);
                data._Acceleration = Convert.ToSingle(sheet["Acceleration"][i]);
                data._Duration = Convert.ToSingle(sheet["Duration"][i]);
                
                string effectPath = Convert.ToString(sheet["EffectName"][i]);
                string pathEffect = System.IO.Path.Combine(ASSET_EFFECT_RES_FOLDER, effectPath + ".prefab");
                data._EffectGo = (GameObject)AssetDatabase.LoadAssetAtPath(pathEffect, typeof(GameObject));

                data._EffectStartTime = Convert.ToSingle(sheet["EffectTime"][i]);
                var effectPosString = Convert.ToString(sheet["EffectPosition"][i]);
                string[] effectposList = effectPosString.Split("+".ToCharArray());
                float effectposX = Convert.ToSingle(effectposList[1]);
                float effectposY = Convert.ToSingle(effectposList[2]);
                float effectposZ = Convert.ToSingle(effectposList[3]);
                data._EffectPosition = new Vector3(effectposX, effectposY, effectposZ);
                data._EffectStartAngle = Convert.ToSingle(sheet["EffectAngle"][i]);
                data._EffectLoopTimes = Convert.ToInt32(sheet["EffectLoopTime"][i]);

                data._SoundTime = Convert.ToSingle(sheet["SoundTime"][i]);
                data._SoundName = Convert.ToString(sheet["SoundName"][i]);

                tempList.Add(data);
            }

            CreateActionConfigDataBase(tempList);
        }
    }

    private static void StroyLineConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_STROY_CONFIG_FOLDER, "StroyLineConfig.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
			
		if(text == null)
		{
			Debug.LogError("stroyline config file not exist");
			return;	
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys =  XmlSpreadSheetReader.Keys;
				
			object[] levelIds = sheet[keys[0]];

            List<StroyLineConfigData> tempList = new List<StroyLineConfigData>();

			for(int i = 0; i< levelIds.Length; i++ )
			{
                if (0 == i || 1 == i) continue;
                StroyLineConfigData data = new StroyLineConfigData();
                data._StroyLineID = Convert.ToInt32(sheet["StroyLineID"][i]);
                data._SceneMapID = Convert.ToInt32(sheet["SceneMapID"][i]);
                data._BgMusic = Convert.ToString(sheet["BgMusic"][i]);
                data._TriggerVocation = Convert.ToInt32(sheet["TriggerProfession"][i]);
                data._TriggerCondition = Convert.ToInt32(sheet["TriggerCondition"][i]);
                data._EctypeID = Convert.ToInt32(sheet["EctypeID"][i]);

                string cameraStr = Convert.ToString(sheet["CameraGroup"][i]);
                string[] splitCameraStr = cameraStr.Split("+".ToCharArray());
                data._CameraGroup = new List<int>();//new int[splitCameraStr.Length];
				data.WeaponType = Convert.ToInt32(sheet["WeaponType"][i]);
                for (int j = 0; j < splitCameraStr.Length; j++ )
                {
                    data._CameraGroup.Add(Convert.ToInt32(splitCameraStr[j]));
                }

				tempList.Add(data);
			}

            CreateStroyLineConfigDataBase(tempList);
		}
    }

    private static void CameraGroupConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_STROY_CONFIG_FOLDER, "CameraGroupConfig.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("CameraGroup config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<CameraGroupConfigData> tempList = new List<CameraGroupConfigData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                CameraGroupConfigData data = new CameraGroupConfigData();
                data._CameraGroupID = Convert.ToInt32(sheet["CameraGroupID"][i]);

                data._ActionList = new NpcAction[8];
                for (int k = 0; k < 8; k++)
                {
                    string ActionStr = Convert.ToString(sheet["Action" + (k + 1)][i]);
					
					if(ActionStr.Length <= 1)
					{
						data._ActionList[k] = null;
						continue;
					}
                    string[] splitActionStr = ActionStr.Split("+".ToCharArray());
					
                    data._ActionList[k] = new NpcAction();
                    data._ActionList[k].NpcID = Convert.ToInt32(splitActionStr[0]);
                    data._ActionList[k].RoleResID = Convert.ToInt32(splitActionStr[1]);
                    data._ActionList[k].RoleType = Convert.ToInt32(splitActionStr[2]);
                    data._ActionList[k].AnimID = new List<int>();//new int[splitActionStr.Length - 3];

                    for (int h = 3; h < splitActionStr.Length; h++)
                    {
                        data._ActionList[k].AnimID.Add(Convert.ToInt32(splitActionStr[h]));
                    }
                }

                string cameraStr = Convert.ToString(sheet["CameraID"][i]);
                string[] splitCameraStr = cameraStr.Split("+".ToCharArray());
                data._CameraID = new List<int>();//new int[splitCameraStr.Length];
                for (int k = 0; k < splitCameraStr.Length; k++)
                {
                    data._CameraID.Add(Convert.ToInt32(splitCameraStr[k]));
                }

                string dialogStr = Convert.ToString(sheet["DialogGroupID"][i]);
                string[] splitDialogStr = dialogStr.Split("+".ToCharArray());
                data._DialogGroupID = new int[splitDialogStr.Length];
                for (int k = 0; k < splitDialogStr.Length; k++)
                {
                    data._DialogGroupID[k] = Convert.ToInt32(splitDialogStr[k]);
                }

                string effectPath = Convert.ToString(sheet["CameraGroupEff"][i]);
                string pathEffect = System.IO.Path.Combine(ASSET_EFFECT_RES_FOLDER, effectPath + ".prefab");
                data._EffectGo = (GameObject)AssetDatabase.LoadAssetAtPath(pathEffect, typeof(GameObject));

                data._IsCameraStartMask = Convert.ToBoolean(sheet["CameraChangeStar"][i]);
                data._IsCameraEndMask = Convert.ToBoolean(sheet["CameraChangeEnd"][i]);

                tempList.Add(data);
            }

            

            CreateCameraGroupConfigDataBase(tempList);
        }
    }


    private static void StroyDialogConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_STROY_CONFIG_FOLDER, "DialogConfig.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("CameraGroup config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<StroyDialogConfigData> tempList = new List<StroyDialogConfigData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                StroyDialogConfigData data = new StroyDialogConfigData();
                data._DialogID = Convert.ToInt32(sheet["DialogID"][i]);
                data._Content = Convert.ToString(sheet["Content"][i]);
                var viewOffsetString = Convert.ToString(sheet["ViewOffset"][i]);
                string[] viewposList = viewOffsetString.Split("+".ToCharArray());
                float viewoffsetX = Convert.ToSingle(viewposList[0]);
                float viewoffsetY = Convert.ToSingle(viewposList[1]);
                data._ViewOffset = new Vector3(viewoffsetX, viewoffsetY,0);
				data._NpcOrPlayer = Convert.ToInt32(sheet["TalkNPCID"][i]);
                data._DialogType = Convert.ToInt32(sheet["DialogType"][i]);
                data._NpcName = Convert.ToString(sheet["NpcName"][i]);
                data._NpcIconName = Convert.ToString(sheet["NpcIconName"][i]);
				//1玩家，2是npc
				if(data._NpcOrPlayer == 2)
				{
					data.npcIconPrefab = (GameObject)Resources.LoadAssetAtPath("Assets/Prefab/GUI/IconPrefab/StroyPersonHead/"+data._NpcIconName + ".prefab", typeof(GameObject));//Prefab/GUI/IconPrefab/QuickBuyIcon
				}
                tempList.Add(data);
            }

            CreateDialogConfigDataBase(tempList);
        }
    }




	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach(string file in files)
		{
            if (file.Contains(RESOURCE_STROY_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}


    private static void CreateStroyLineConfigDataBase(List<StroyLineConfigData> list)
	{
        string fileName = typeof(StroyLineDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_STROY_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            StroyLineDataBase database = (StroyLineDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(StroyLineDataBase));
			
			if(null == database)
			{
				return;
			}

            database._dataTable = new StroyLineConfigData[list.Count];
            

			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = new StroyLineConfigData();
                database._dataTable[i]._StroyLineID = list[i]._StroyLineID;
                database._dataTable[i]._SceneMapID = list[i]._SceneMapID;
                database._dataTable[i]._BgMusic = list[i]._BgMusic;
                database._dataTable[i]._TriggerVocation = list[i]._TriggerVocation;
                database._dataTable[i]._TriggerCondition = list[i]._TriggerCondition;
                database._dataTable[i]._EctypeID = list[i]._EctypeID;
                database._dataTable[i]._CameraGroup = list[i]._CameraGroup;
				database._dataTable[i].WeaponType = list[i].WeaponType;
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            StroyLineDataBase database = ScriptableObject.CreateInstance<StroyLineDataBase>();

            database._dataTable = new StroyLineConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = new StroyLineConfigData();
                database._dataTable[i]._StroyLineID = list[i]._StroyLineID;
                database._dataTable[i]._SceneMapID = list[i]._SceneMapID;
                database._dataTable[i]._BgMusic = list[i]._BgMusic;
                database._dataTable[i]._TriggerVocation = list[i]._TriggerVocation;
                database._dataTable[i]._TriggerCondition = list[i]._TriggerCondition;
                database._dataTable[i]._EctypeID = list[i]._EctypeID;
                database._dataTable[i]._CameraGroup = list[i]._CameraGroup;
				database._dataTable[i].WeaponType = list[i].WeaponType;
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}

    private static void CreateCameraGroupConfigDataBase(List<CameraGroupConfigData> list)
    {
        string fileName = typeof(CameraGroupDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_STROY_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            CameraGroupDataBase database = (CameraGroupDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(CameraGroupDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new CameraGroupConfigData[list.Count];
            
            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new CameraGroupConfigData();
                database._dataTable[i]._CameraGroupID = list[i]._CameraGroupID;
                database._dataTable[i]._CameraID = list[i]._CameraID;
                database._dataTable[i]._DialogGroupID = list[i]._DialogGroupID;
                database._dataTable[i]._EffectGo = list[i]._EffectGo;
                database._dataTable[i]._IsCameraStartMask = list[i]._IsCameraStartMask;
                database._dataTable[i]._IsCameraEndMask = list[i]._IsCameraEndMask;
				database._dataTable[i]._ActionList = new NpcAction[8];
				
				for (int k = 0; k < 8; k++)
                {
					if(list[i]._ActionList[k] == null)
						continue;
                    database._dataTable[i]._ActionList[k] = new NpcAction();
                    database._dataTable[i]._ActionList[k].NpcID = list[i]._ActionList[k].NpcID;
                    database._dataTable[i]._ActionList[k].RoleResID = list[i]._ActionList[k].RoleResID;
                    database._dataTable[i]._ActionList[k].RoleType = list[i]._ActionList[k].RoleType;
                    database._dataTable[i]._ActionList[k].AnimID = list[i]._ActionList[k].AnimID;
                }
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            CameraGroupDataBase database = ScriptableObject.CreateInstance<CameraGroupDataBase>();

            database._dataTable = new CameraGroupConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new CameraGroupConfigData();
                database._dataTable[i]._CameraGroupID = list[i]._CameraGroupID;
                database._dataTable[i]._CameraID = list[i]._CameraID;
                database._dataTable[i]._DialogGroupID = list[i]._DialogGroupID;
                database._dataTable[i]._EffectGo = list[i]._EffectGo;
                database._dataTable[i]._IsCameraStartMask = list[i]._IsCameraStartMask;
                database._dataTable[i]._IsCameraEndMask = list[i]._IsCameraEndMask;

                database._dataTable[i]._ActionList = new NpcAction[8];
                for (int k = 0; k < 8; k++)
                {
                    database._dataTable[i]._ActionList[k] = new NpcAction();
                    database._dataTable[i]._ActionList[k].NpcID = list[i]._ActionList[k].NpcID;
                    database._dataTable[i]._ActionList[k].RoleResID = list[i]._ActionList[k].RoleResID;
                    database._dataTable[i]._ActionList[k].RoleType = list[i]._ActionList[k].RoleType;
                    database._dataTable[i]._ActionList[k].AnimID = list[i]._ActionList[k].AnimID;
                }
            }
            AssetDatabase.CreateAsset(database, path);
        }

    }

    private static void CreateDialogConfigDataBase(List<StroyDialogConfigData> list)
    {
        string fileName = typeof(StroyLineDialogDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_STROY_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            StroyLineDialogDataBase database = (StroyLineDialogDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(StroyLineDialogDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new StroyDialogConfigData[list.Count];


            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new StroyDialogConfigData();
                database._dataTable[i]._DialogID = list[i]._DialogID;
                database._dataTable[i]._Content = list[i]._Content;
                database._dataTable[i]._ViewOffset = list[i]._ViewOffset;
				database._dataTable[i]._NpcOrPlayer = list[i]._NpcOrPlayer;
                database._dataTable[i]._DialogType = list[i]._DialogType;
                database._dataTable[i]._NpcName = list[i]._NpcName;
                database._dataTable[i]._NpcIconName = list[i]._NpcIconName;
				database._dataTable[i].npcIconPrefab = list[i].npcIconPrefab;
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            StroyLineDialogDataBase database = ScriptableObject.CreateInstance<StroyLineDialogDataBase>();

            database._dataTable = new StroyDialogConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new StroyDialogConfigData();
                database._dataTable[i]._DialogID = list[i]._DialogID;
                database._dataTable[i]._Content = list[i]._Content;
				database._dataTable[i]._NpcOrPlayer = list[i]._NpcOrPlayer;
                database._dataTable[i]._DialogType = list[i]._DialogType;
                database._dataTable[i]._NpcName = list[i]._NpcName;
                database._dataTable[i]._NpcIconName = list[i]._NpcIconName;
				database._dataTable[i].npcIconPrefab = list[i].npcIconPrefab;
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }

    private static void CreateActionConfigDataBase(List<StroyActionConfigData> list)
    {
        string fileName = typeof(StroyActionDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_STROY_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            StroyActionDataBase database = (StroyActionDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(StroyActionDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new StroyActionConfigData[list.Count];


            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new StroyActionConfigData();
                database._dataTable[i]._ActionID = list[i]._ActionID;
                database._dataTable[i]._ActionName = list[i]._ActionName;
                database._dataTable[i]._StartPosition = list[i]._StartPosition;
                database._dataTable[i]._ActionType = list[i]._ActionType;
                database._dataTable[i]._StartAngle = list[i]._StartAngle;
                database._dataTable[i]._ModelAngle = list[i]._ModelAngle;
                database._dataTable[i]._Speed = list[i]._Speed;
                database._dataTable[i]._Acceleration = list[i]._Acceleration;
                database._dataTable[i]._Duration = list[i]._Duration;
                database._dataTable[i]._EffectGo = list[i]._EffectGo;
                database._dataTable[i]._EffectStartTime = list[i]._EffectStartTime;
                database._dataTable[i]._EffectPosition = list[i]._EffectPosition;
				database._dataTable[i]._EffectStartAngle = list[i]._EffectStartAngle;
                database._dataTable[i]._EffectLoopTimes = list[i]._EffectLoopTimes;
                database._dataTable[i]._SoundTime = list[i]._SoundTime;
                database._dataTable[i]._SoundName = list[i]._SoundName;
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            StroyActionDataBase database = ScriptableObject.CreateInstance<StroyActionDataBase>();

            database._dataTable = new StroyActionConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new StroyActionConfigData();
                database._dataTable[i]._ActionID = list[i]._ActionID;
                database._dataTable[i]._ActionName = list[i]._ActionName;
                database._dataTable[i]._StartPosition = list[i]._StartPosition;
                database._dataTable[i]._ActionType = list[i]._ActionType;
                database._dataTable[i]._StartAngle = list[i]._StartAngle;
                database._dataTable[i]._ModelAngle = list[i]._ModelAngle;
                database._dataTable[i]._Speed = list[i]._Speed;
                database._dataTable[i]._Acceleration = list[i]._Acceleration;
                database._dataTable[i]._Duration = list[i]._Duration;
                database._dataTable[i]._EffectGo = list[i]._EffectGo;
                database._dataTable[i]._EffectStartTime = list[i]._EffectStartTime;
                database._dataTable[i]._EffectPosition = list[i]._EffectPosition;
				database._dataTable[i]._EffectStartAngle = list[i]._EffectStartAngle;
                database._dataTable[i]._EffectLoopTimes = list[i]._EffectLoopTimes;
                database._dataTable[i]._SoundTime = list[i]._SoundTime;
                database._dataTable[i]._SoundName = list[i]._SoundName;
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }

}
