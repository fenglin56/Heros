using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class NewbieGuideConfig : AssetPostprocessor 
{
	private static readonly string RESOURCE_GUIDE_CONFIG_FOLDER = "Assets/Data/NewbieGuideConfig/Res";
    private static readonly string ASSET_GUIDE_CONFIG_FOLDER = "Assets/Data/NewbieGuideConfig/Data";
    private static readonly string ASSET_GUIDE_RES_CONFIG_FOLDER = "Assets/Prefab/GUI/NewbieGuide/Prefab/";
    //private static readonly string ASSET_GUIDE_ATALS_CONFIG_FOLDER = "Assets/UI/NewTextures/NewbieGuide/Compress/Atlas/";
    private static readonly string ASSET_GUIDE_EFFECT_CONFIG_FOLDER = "Assets/Effects/Prefab/";
    private static readonly string ASSET_GUIDE_PIC_CONFIG_FOLDER = "Assets/Prefab/GUI/IconPrefab/NewbieGuidePic/";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
        //return;
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
            NewbieGuideConfigPostprocess();
            EctypeGuideConfigPostprocess();
            EctypeGuideStepConfigPostprocess();
		}
	}

    private static void EctypeGuideStepConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_GUIDE_CONFIG_FOLDER, "GuideStepConfig.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("newbie guide config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<EctypeGuideStepConfigData> tempList = new List<EctypeGuideStepConfigData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                EctypeGuideStepConfigData data = new EctypeGuideStepConfigData();
                data._GuideStep = Convert.ToInt32(sheet["StepID"][i]);
                data._StepType = Convert.ToInt32(sheet["StepType"][i]);
                data._ReductionDelayTime = Convert.ToSingle(sheet["RetardDelayTime"][i]) * 0.001f;
                data._ReductionRatio = Convert.ToInt32(sheet["DecelerationRate"][i]);
                data._StepDuration = Convert.ToSingle(sheet["StepDuration"][i]);
                string[] disButtonStr = Convert.ToString(sheet["ButtonShielding"][i]).Split('+');
                data._DisableButtonList = new int[disButtonStr.Length];
                for (int k = 0; k < disButtonStr.Length; k++)
                {
                    data._DisableButtonList[k] = Convert.ToInt32(disButtonStr[k]);
                }
                data._SignTips = Convert.ToString(sheet["SignTips"][i]);
                string[] tipsListStr = Convert.ToString(sheet["TipsPrefab"][i]).Split('|');
                data._TipsType = new TipsType[tipsListStr.Length];
                for (int j = 0; j < tipsListStr.Length; j++)
                {
                    string[] tipsItemStr = Convert.ToString(tipsListStr[j]).Split('+');
                    if (tipsItemStr.Length == 2)
                    {
                        data._TipsType[j] = new TipsType();
                        data._TipsType[j].Vocation = Convert.ToInt32(tipsItemStr[0]);
                        string tipPrefabName = Convert.ToString(tipsItemStr[1]);
                        string tipsPrefabRes = System.IO.Path.Combine(ASSET_GUIDE_EFFECT_CONFIG_FOLDER, tipPrefabName + ".prefab");
                        data._TipsType[j].TipsPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(tipsPrefabRes, typeof(GameObject));
                    }
                }
                string[] tipsPrefabOffset = Convert.ToString(sheet["TipsPrefabOffset"][i]).Split('+');
                data._TipsPrefabOffset = new Vector3(float.Parse(tipsPrefabOffset[1]), float.Parse(tipsPrefabOffset[2]));

                string[] guideIdStr = Convert.ToString(sheet["GuideButton"][i]).Split('+');
                if (guideIdStr.Length <= 2)
                {
                    data._GuideIdList = new int[guideIdStr.Length];
                    for (int j = 0; j < guideIdStr.Length; j++)
                    {
                        data._GuideIdList[j] = Convert.ToInt32(guideIdStr[j]);
                    }
                }
                data._DelayTime = Convert.ToInt32(sheet["DelayTime"][i]);
                data._ButtonEffectInterval = Convert.ToInt32(sheet["ButtonEffectInterval"][i]);
                //data._NpcIcon = Convert.ToString(sheet["NpcIcon"][i]);
                //data._NpcName = Convert.ToString(sheet["NpcName"][i]);
                //data._DialogList = Convert.ToString(sheet["GuideDialog"][i]).Split('+');
                //data._DialogTitle = Convert.ToString(sheet["DialogTitle"][i]);
                var stepDialogInfoStr = Convert.ToString(sheet["GuideDialog"][i]);
                if (stepDialogInfoStr != "0")
                {
                    string[] stepDialogInfoStrs = stepDialogInfoStr.Split('|');
                    StepDialogInfo[] stepDialogInfos = new StepDialogInfo[stepDialogInfoStrs.Length];
                    for (int k = 0; k < stepDialogInfoStrs.Length;k++ )
                    {
                        string[] info = stepDialogInfoStrs[k].Split('+');
                        stepDialogInfos[k] = new StepDialogInfo()
                        {
                            NpcIcon = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/" + @info[0] + ".prefab", typeof(GameObject))
                            , NpcName = info[1], DialogContent = info[2],
                            Offset =new Vector3(float.Parse(info[3]),float.Parse(info[4])),
                            IsHeroFlag = info[5] == "0" ? false : true
                        };
                    }
                    data.StepDialogInfos = stepDialogInfos;
                }

                string effectName = Convert.ToString(sheet["GuideEffect"][i]);
                string effectPrefabRes = System.IO.Path.Combine(ASSET_GUIDE_EFFECT_CONFIG_FOLDER, effectName + ".prefab");
                data._GuideEffect = (GameObject)AssetDatabase.LoadAssetAtPath(effectPrefabRes, typeof(GameObject));
                string flashName = Convert.ToString(sheet["ButtonEffect"][i]);
                string flashPrefabRes = System.IO.Path.Combine(ASSET_GUIDE_RES_CONFIG_FOLDER, flashName + ".prefab");
                data._ButtonFlshing = (GameObject)AssetDatabase.LoadAssetAtPath(flashPrefabRes, typeof(GameObject));

                string monsterEffectName = Convert.ToString(sheet["MonsterEffect"][i]);
                string monsterEffectPrefabRes = System.IO.Path.Combine(ASSET_GUIDE_EFFECT_CONFIG_FOLDER, monsterEffectName + ".prefab");
                data._MonsterEffect = (GameObject)AssetDatabase.LoadAssetAtPath(monsterEffectPrefabRes, typeof(GameObject));
                data._MountMonsterID = Convert.ToInt32(sheet["MountMonster"][i]);
                string[] effectPosStr = Convert.ToString(sheet["GuideEffectPos"][i]).Split('+');
                if (effectPosStr.Length == 3)
                {
                    data._EffectPos.x = Convert.ToSingle(effectPosStr[0]);
                    data._EffectPos.y = Convert.ToSingle(effectPosStr[1]);
                    data._EffectPos.z = Convert.ToSingle(effectPosStr[2]);
                }
                data._EffectAngle = Convert.ToSingle(sheet["EffectAngle"][i]);
                string[] picPrefabName = Convert.ToString(sheet["GuidePicture"][i]).Split('+');
                data._GuidePicPrefabs = new GameObject[picPrefabName.Length];
                for (int j = 0; j < picPrefabName.Length; j++)
                {
                    string picPrefabPath = System.IO.Path.Combine(ASSET_GUIDE_PIC_CONFIG_FOLDER, picPrefabName[j] + ".prefab");
                    data._GuidePicPrefabs[j] = (GameObject)AssetDatabase.LoadAssetAtPath(picPrefabPath, typeof(GameObject));
                }
                data._StepSound = Convert.ToString(sheet["StepSound"][i]);
                data.MountType = Convert.ToInt32(sheet["MountType"][i]);
                data.TargetInformation = Convert.ToString(sheet["TargetInformation"][i]);
                

                tempList.Add(data);
            }

            CreateEctypeGuideStepConfigDataBase(tempList);
        }
    }

    private static void EctypeGuideConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_GUIDE_CONFIG_FOLDER, "EctypeGuideConfig.xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("newbie guide config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<EctypeGuideConfigData> tempList = new List<EctypeGuideConfigData>();

            for (int i = 0; i < levelIds.Length; i++)
            {
                if (0 == i || 1 == i) continue;
                EctypeGuideConfigData data = new EctypeGuideConfigData();
                data._EctypeID = Convert.ToInt32(sheet["lEctypeContainerID"][i]);
                string[] stepStrList = Convert.ToString(sheet["lEctypeGuide"][i]).Split('+');
                data._StepList = new int[stepStrList.Length];
                for (int j = 0; j < stepStrList.Length; j++)
                {
                    data._StepList[j] = Convert.ToInt32(stepStrList[j]);
                }
                string[] joyStickStepStrList = Convert.ToString(sheet["RockerOperationGuide"][i]).Split('+');
                data._JoyStickStepList = new int[joyStickStepStrList.Length];
                for (int j = 0; j < joyStickStepStrList.Length; j++)
                {
                    data._JoyStickStepList[j] = Convert.ToInt32(joyStickStepStrList[j]);
                }
                tempList.Add(data);
            }

            CreateEctypeGuideConfigDataBase(tempList);
        }
    }

    private static void NewbieGuideConfigPostprocess()
    {
        string path = System.IO.Path.Combine(RESOURCE_GUIDE_CONFIG_FOLDER, "NewbieGuideConfig.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
			
		if(text == null)
		{
			Debug.LogError("newbie guide config file not exist");
			return;	
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys =  XmlSpreadSheetReader.Keys;
				
			object[] levelIds = sheet[keys[0]];

            List<GuideConfigData> tempList = new List<GuideConfigData>();

			for(int i = 0; i< levelIds.Length; i++ )
			{
                if (0 == i || 1 == i) continue;
                GuideConfigData data = new GuideConfigData();
                data._GuideID = Convert.ToInt32(sheet["GuideID"][i]);
                data._GuideType = Convert.ToInt32(sheet["GuideType"][i]);
                data._NpcName = Convert.ToString(sheet["NpcName"][i]);
                data._NpcIcon = Convert.ToString(sheet["NpcIcon"][i]);
                data._PreDialogList = Convert.ToString(sheet["PreDialog"][i]).Split('+');
                data._DialogTitle = Convert.ToString(sheet["DialogTitle"][i]);
                data._BtnSignText = Convert.ToString(sheet["BtnSignText"][i]);
                string signPrefab = Convert.ToString(sheet["BtnSignPrefab"][i]);
                string prefabPath = System.IO.Path.Combine(ASSET_GUIDE_RES_CONFIG_FOLDER, signPrefab + ".prefab");
                data._ArrowPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                data._ArrowOffsetX = Convert.ToSingle(sheet["BtnSignOffsetX"][i]);
                data._ArrowOffsetY = Convert.ToSingle(sheet["BtnSignOffsetY"][i]);

                string[] guideBtnIDStr = Convert.ToString(sheet["GuideBtnID"][i]).Split('+');
                data._GuideBtnID = new int[guideBtnIDStr.Length];
                for (int k = 0; k < guideBtnIDStr.Length; k++ )
                {
                    data._GuideBtnID[k] = Convert.ToInt32(guideBtnIDStr[k]);
                }
                string[] guideBtnPosOffset = Convert.ToString(sheet["BtnPositionOffset"][i]).Split('+');
                data._BtnPosOffset = new Vector3(float.Parse(guideBtnPosOffset[1]), float.Parse(guideBtnPosOffset[2]));

                data._FrameScale = Convert.ToInt32(sheet["HighlightScale"][i]);

                string[] prefabpath = Convert.ToString(sheet["HighlightRes"][i]).Split('+');
                string pathRes = System.IO.Path.Combine(ASSET_GUIDE_RES_CONFIG_FOLDER, prefabpath[0] + ".prefab");
                data._SourceFrame = (GameObject)AssetDatabase.LoadAssetAtPath(pathRes, typeof(GameObject));
                if (prefabpath.Length > 1)
                {
                    string path2Res = System.IO.Path.Combine(ASSET_GUIDE_RES_CONFIG_FOLDER, prefabpath[1] + ".prefab");
                    data._TargetFrame = (GameObject)AssetDatabase.LoadAssetAtPath(path2Res, typeof(GameObject));
                }
                
				data._SkipRole = Convert.ToInt32(sheet["SkipRole"][i]);
                data._OverRole = Convert.ToInt32(sheet["OverRole"][i]);

				tempList.Add(data);
			}

            CreateGuideConfigDataBase(tempList);
		}
    }


	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach(string file in files)
		{
            if (file.Contains(RESOURCE_GUIDE_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}


    private static void CreateGuideConfigDataBase(List<GuideConfigData> list)
	{
        string fileName = typeof(NewbieGuideConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_GUIDE_CONFIG_FOLDER, fileName + ".asset");
		
		if(File.Exists(path))
		{
            NewbieGuideConfigDataBase database = (NewbieGuideConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(NewbieGuideConfigDataBase));
			
			if(null == database)
			{
				return;
			}

            database._dataTable = new GuideConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = new GuideConfigData();
                database._dataTable[i]._GuideID = list[i]._GuideID;
                database._dataTable[i]._GuideType = list[i]._GuideType;
                database._dataTable[i]._NpcName = list[i]._NpcName;
                database._dataTable[i]._NpcIcon = list[i]._NpcIcon;
                database._dataTable[i]._PreDialogList = list[i]._PreDialogList;
                database._dataTable[i]._DialogTitle = list[i]._DialogTitle;
                database._dataTable[i]._BtnSignText = list[i]._BtnSignText;
                database._dataTable[i]._ArrowPrefab = list[i]._ArrowPrefab;
                database._dataTable[i]._ArrowOffsetX = list[i]._ArrowOffsetX;
                database._dataTable[i]._ArrowOffsetY = list[i]._ArrowOffsetY;
                database._dataTable[i]._GuideBtnID = list[i]._GuideBtnID;
                database._dataTable[i]._BtnPosOffset = list[i]._BtnPosOffset;
                database._dataTable[i]._FrameScale = list[i]._FrameScale;
                database._dataTable[i]._SourceFrame = list[i]._SourceFrame;
                database._dataTable[i]._TargetFrame = list[i]._TargetFrame;
                database._dataTable[i]._SkipRole = list[i]._SkipRole;
                database._dataTable[i]._OverRole = list[i]._OverRole;
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
            NewbieGuideConfigDataBase database = ScriptableObject.CreateInstance<NewbieGuideConfigDataBase>();

            database._dataTable = new GuideConfigData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
                database._dataTable[i] = new GuideConfigData();
                database._dataTable[i]._GuideID = list[i]._GuideID;
                database._dataTable[i]._GuideType = list[i]._GuideType;
                database._dataTable[i]._NpcName = list[i]._NpcName;
                database._dataTable[i]._NpcIcon = list[i]._NpcIcon;
                database._dataTable[i]._PreDialogList = list[i]._PreDialogList;
                database._dataTable[i]._DialogTitle = list[i]._DialogTitle;
                database._dataTable[i]._BtnSignText = list[i]._BtnSignText;
                database._dataTable[i]._ArrowPrefab = list[i]._ArrowPrefab;
                database._dataTable[i]._ArrowOffsetX = list[i]._ArrowOffsetX;
                database._dataTable[i]._ArrowOffsetY = list[i]._ArrowOffsetY;
                database._dataTable[i]._GuideBtnID = list[i]._GuideBtnID;
                database._dataTable[i]._BtnPosOffset = list[i]._BtnPosOffset;
                database._dataTable[i]._FrameScale = list[i]._FrameScale;
                database._dataTable[i]._SourceFrame = list[i]._SourceFrame;
                database._dataTable[i]._TargetFrame = list[i]._TargetFrame;
                database._dataTable[i]._SkipRole = list[i]._SkipRole;
                database._dataTable[i]._OverRole = list[i]._OverRole;
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}

    private static void CreateEctypeGuideStepConfigDataBase(List<EctypeGuideStepConfigData> list)
    {
        string fileName = typeof(EctypeGuideStepConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_GUIDE_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            EctypeGuideStepConfigDataBase database = (EctypeGuideStepConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(EctypeGuideStepConfigDataBase));

            if (null == database)
            {
                return;
            }

            //database._dataTable = new EctypeGuideStepConfigData[list.Count];

            database._dataTable = list.ToArray();

            //for (int i = 0; i < list.Count; i++)
            //{
            //    database._dataTable[i] = new EctypeGuideStepConfigData();
            //    database._dataTable[i]._GuideStep = list[i]._GuideStep;
            //    database._dataTable[i]._SignTips = list[i]._SignTips;
            //    database._dataTable[i]._TipsPrefab = list[i]._TipsPrefab;
            //    database._dataTable[i]._GuideBtnA = list[i]._GuideBtnA;
            //}
            EditorUtility.SetDirty(database);
        }
        else
        {
            EctypeGuideStepConfigDataBase database = ScriptableObject.CreateInstance<EctypeGuideStepConfigDataBase>();

            database._dataTable = list.ToArray();

            //database._dataTable = new EctypeGuideStepConfigData[list.Count];

            //for (int i = 0; i < list.Count; i++)
            //{
            //    database._dataTable[i] = new EctypeGuideStepConfigData();
            //    database._dataTable[i]._GuideStep = list[i]._GuideStep;
            //}
            AssetDatabase.CreateAsset(database, path);
        }
    }

    private static void CreateEctypeGuideConfigDataBase(List<EctypeGuideConfigData> list)
    {
        string fileName = typeof(EctypeGuideConfigDataBase).Name;
        string path = System.IO.Path.Combine(ASSET_GUIDE_CONFIG_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            EctypeGuideConfigDataBase database = (EctypeGuideConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(EctypeGuideConfigDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new EctypeGuideConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new EctypeGuideConfigData();
                database._dataTable[i]._EctypeID = list[i]._EctypeID;
                database._dataTable[i]._StepList = list[i]._StepList;
                database._dataTable[i]._JoyStickStepList = list[i]._JoyStickStepList;
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            EctypeGuideConfigDataBase database = ScriptableObject.CreateInstance<EctypeGuideConfigDataBase>();

            database._dataTable = new EctypeGuideConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = new EctypeGuideConfigData();
                database._dataTable[i]._EctypeID = list[i]._EctypeID;
                database._dataTable[i]._StepList = list[i]._StepList;
                database._dataTable[i]._JoyStickStepList = list[i]._JoyStickStepList;
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }

}
