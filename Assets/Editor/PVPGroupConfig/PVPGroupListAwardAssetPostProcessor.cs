using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class PVPGroupListAwardAssetPostProcessor : AssetPostprocessor
{

	private static readonly string RESOURCE_PVP_CONFIG_FOLDER = "Assets/Data/PVPGroupConfig/Res";
	private static readonly string ASSET_PVP_CONFIG_FOLDER = "Assets/Data/PVPGroupConfig/Data";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
		
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
			OnPostprocessPVPListAward();
		}
	}
	
	
	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach(string file in files)
		{
			if (file.Contains(RESOURCE_PVP_CONFIG_FOLDER))
			{
				fileModified = true;	
				break;
			}
		}
		return fileModified;
	}
	
	private static  void OnPostprocessPVPListAward()
	{
		string path = System.IO.Path.Combine(RESOURCE_PVP_CONFIG_FOLDER, "PVPGroupListAward.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if (text == null)
		{
			Debug.LogError("PVPGroupListAward config file not exist");
			return;
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys = XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];
			
			List<PVPGroupListAward> tempList = new List<PVPGroupListAward>();
			
			for (int i = 2; i < levelIds.Length; i++)
			{
				PVPGroupListAward data = new PVPGroupListAward();
				data.ListAward = Convert.ToInt32(sheet["ListAward"][i]);
				data.ListAwardType = Convert.ToInt32(sheet["ListAwardType"][i]);
				data.ListAwardGroup = Convert.ToInt32(sheet["ListAwardGroup"][i]);
				data.ListAwardPlace = Convert.ToString(sheet["ListAwardPlace"][i]);
				data.ListAwardParam1 = Convert.ToString(sheet["ListAwardParam1"][i]);
				data.ListAwardParam2 = Convert.ToString(sheet["ListAwardParam2"][i]);
				data.ListAwardParam3 = Convert.ToString(sheet["ListAwardParam3"][i]);
				data.ListAwardMail = Convert.ToInt32(sheet["ListAwardMail"][i]);
				data.ListAwardIcon = Convert.ToString(sheet["ListAwardIcon"][i]);
				data.ListAwardName = Convert.ToString(sheet["ListAwardName"][i]);
				data.ListAward01Icon = Convert.ToString(sheet["ListAward01Icon"][i]);
				data.ListAward01Des = Convert.ToString(sheet["ListAward01Des"][i]);
				data.ListAward02Icon = Convert.ToString(sheet["ListAward02Icon"][i]);
				data.ListAward02Des = Convert.ToString(sheet["ListAward02Des"][i]);

				tempList.Add(data);
			}
			
			CreateConfigDataBase(tempList);
		}
	}

	private static void CreateConfigDataBase(List<PVPGroupListAward> list)
	{
		String fileName = typeof(PVPGroupListAward).Name + "DataBase";
		string path = System.IO.Path.Combine(ASSET_PVP_CONFIG_FOLDER, fileName + ".asset");
		
		if (File.Exists(path))
		{
			PVPGroupListAwardDataBase database = (PVPGroupListAwardDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PVPGroupListAwardDataBase));
			
			if (null == database)
			{
				return;
			}
			
			database._dataTable = new PVPGroupListAward[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			PVPGroupListAwardDataBase database = ScriptableObject.CreateInstance<PVPGroupListAwardDataBase>();
			database._dataTable = new PVPGroupListAward[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
	}
}