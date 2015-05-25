using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;

public class MultiJokeLanguageAssetPostProcessor : AssetPostprocessor 
{
	public static readonly string EW_RESOURCE_MULTILANGUAGE_FOLDER = "Assets/Data/JokeData/Res";
	public static readonly string EW_ASSETS_MULTILANGUAGE_FOLDER = "Assets/Data/JokeData/Data";
	
	
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
	                                           string[] movedAssets, string[] movedFromPath)
	{
		if( CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets) )
		{
			string path = System.IO.Path.Combine( EW_RESOURCE_MULTILANGUAGE_FOLDER, "JokeLanguageText.xml" );
			
			TextReader tr = new StreamReader(path);
			
			string text = tr.ReadToEnd();
			
			if(text == null)
			{
				Debug.LogError("language file not exist");
				return;	
			}
			else
			{
				XmlSpreadSheetReader.ReadSheet(text);
				
				XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
				
				int langCount =  XmlSpreadSheetReader.Keys.Length - 1;
				
				string[] keys =  XmlSpreadSheetReader.Keys;
				
				object[] textIds = sheet[keys[0]];
				
				for(int i = 0; i < langCount; i++)
				{
					string lang = keys[i + 1];
					
					List<JokeLanguageData> tempList = new List<JokeLanguageData>();
					for(int j = 0; j < textIds.Length; j++)
					{
						if(null == textIds[j])
						{
							Debug.LogError("invalid ids in line: " + j.ToString());	
						}
						if(null == sheet[lang][j])
						{
							Debug.LogError("invalid text for language of " + lang + " in line: " + j.ToString());	
						}
						
						JokeLanguageData textEntry = new JokeLanguageData();
						textEntry.key = textIds[j].ToString();
						textEntry.text = sheet[lang][j].ToString();
						tempList.Add(textEntry);
					}
					
					if(lang != null)
					{
						CreateLanguageDataBase(lang.ToUpper(), tempList);	
					}	
				}
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
	
	
	private static void CreateLanguageDataBase(string lang, List<JokeLanguageData> list)
	{
		string className = typeof(JokeLanguageDataBase).Name;
		string path = System.IO.Path.Combine(EW_ASSETS_MULTILANGUAGE_FOLDER, className + lang + ".asset");
		
		if(File.Exists(path))
		{
			JokeLanguageDataBase database = (JokeLanguageDataBase)AssetDatabase.LoadAssetAtPath(path,typeof(JokeLanguageDataBase));
			
			if(null == database)
			{
				return;
			}
			database.stringTable = new JokeLanguageData[list.Count];
			
			for(int i = 0; i < list.Count; i++)
			{
				database.stringTable[i] = new JokeLanguageData();
				database.stringTable[i].key = list[i].key;
				database.stringTable[i].text = list[i].text;
			}
			EditorUtility.SetDirty(database);
		}
		
		else
		{
			JokeLanguageDataBase database = ScriptableObject.CreateInstance<JokeLanguageDataBase>();
			
			database.stringTable = new JokeLanguageData[list.Count];
			for(int i = 0; i < list.Count; i++)
			{
				database.stringTable[i] = new JokeLanguageData();
				database.stringTable[i].key = list[i].key;
				database.stringTable[i].text = list[i].text;
			}
			AssetDatabase.CreateAsset(database, path);
		}
		
	}
	
	
	
	
	
	
	
	
}

