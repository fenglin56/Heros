
using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class AttributeAssetPostProcessor : AssetPostprocessor
{
	private static readonly string RESOURCE_ATTRIBUTE_FOLDER = "Assets/Data/Attribute/Res";
	private static readonly string ASSET_ATTRIBUTE_FOLDER = "Assets/Data/Attribute/Data";
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
	                                           string[] movedAssets, string[] movedFromPath)
	{
		if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
		{
			RobotConfigPostprocess();
		}
	}
	
	private static void RobotConfigPostprocess()
	{
		string path = System.IO.Path.Combine(RESOURCE_ATTRIBUTE_FOLDER, "PropKey.xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if (text == null)
		{
			Debug.LogError("PropKey file not exist");
			return;
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys = XmlSpreadSheetReader.Keys;
			
			object[] levelIds = sheet[keys[0]];

			List<PropKeyConfigData> tempList = new List<PropKeyConfigData>();
			
			for (int i = 0; i < levelIds.Length; i++)
			{
				if (0 == i || 1 == i) continue;
				PropKeyConfigData data = new PropKeyConfigData();
				data.szDescription = Convert.ToString(sheet["szDescription"][i]);
				data.nPropID = Convert.ToInt32(sheet["nPropID"][i]);
				data.nSettleID = Convert.ToInt32(sheet["nSettleID"][i]);
//				data.PosId = Convert.ToInt32(sheet["PosId"][i]);
//				string[] posStrArray = Convert.ToString(sheet["BornPos"][i]).Split('+');
//				data.BornPos = new Vector3(Convert.ToInt32(posStrArray[0]),0,Convert.ToInt32(posStrArray[1]));
//				data.BornOrientation = Convert.ToSingle(sheet["BornOrientation"][i]);
				
				tempList.Add(data);
			}
			
			CreateRobotConfigDataBase(tempList);
		}
	}
	
	private static bool CheckResModified(string[] files)
	{
		bool fileModified = false;
		foreach (string file in files)
		{
			if (file.Contains(RESOURCE_ATTRIBUTE_FOLDER))
			{
				fileModified = true;
				break;
			}
		}
		return fileModified;
	}
	
	
	private static void CreateRobotConfigDataBase(List<PropKeyConfigData> list)
	{
		string fileName = typeof(PropKeyConfigDataBase).Name;
		string path = System.IO.Path.Combine(ASSET_ATTRIBUTE_FOLDER, fileName + ".asset");
		
		if (File.Exists(path))
		{
			PropKeyConfigDataBase database = (PropKeyConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PropKeyConfigDataBase));
			
			if (null == database)
			{
				return;
			}
			
			database._dataTable = new PropKeyConfigData[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				
				database._dataTable[i] = list[i];
				
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			PropKeyConfigDataBase database = ScriptableObject.CreateInstance<PropKeyConfigDataBase>();
			
			database._dataTable = new PropKeyConfigData[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				database._dataTable[i] = list[i];
				
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
}