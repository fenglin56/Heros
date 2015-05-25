using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;

public static class LanguageTextManager  
{
	private static bool _loaded;
	private static Dictionary<string , string> _textTable;
	private static string _currentLang;
	
	public static bool Loaded 
	{
		get 
		{
			return _loaded;
		}
	}
	
	public static string CurrentLang
	{
		get
		{
			return _currentLang;	
		}
	}
	
	public static string CurSystemLang
	{
		get
		{
			string language = "";//LocalizationInfo.GetLanguage();
			
			/*
			if(language.ToUpper().Equals("zh-Hans".ToUpper()))
			{
				language = "zh-Hans";
			}
			else if(language.ToUpper().Equals("zh-Hant".ToUpper()))
			{
				language = "zh-Hant";
			}
			else if(language.ToUpper().Equals("ja".ToUpper()))
			{
				language = "ja";
			}
			else if(language.ToUpper().Equals("ko".ToUpper()))
			{
				language = "ko";
			}
			else if(language.ToUpper().Equals("pt".ToUpper()))
			{
				language = "pt";
			}
			else if(language.ToUpper().Equals("fr".ToUpper()))
			{
				language = "fr";
			}
			else if(language.ToUpper().Equals("de".ToUpper()))
			{
				language = "de";
			}
			else
			{
				language = "en";
			}
			*/
			
			//language = "ko";
			//language = "en";
			language = "zh-Hans";
			//language = "ja";
			//language = "zh-Hant";
			//language = "de";
			//language = "fr";
			//language = "pt";
			return language;
		}
	}
	
	public static bool IsMultiFont()
	{
		if(LanguageTextManager.CurSystemLang.ToUpper().Equals("en".ToUpper()))
		{
			return true;
		}
		
		return false;
	}
	
	public static void LoadDataBase(string lang, LanguageDataBase dataBaseVal)
	{
		_loaded = false;
		LanguageDataBase database = null;
		
//		LanguageContainer lc = (LanguageContainer)GameObject.FindObjectOfType(typeof(LanguageContainer));
//		if(lc != null)
//		{
//			database = lc.GetLanguageDataBase(lang.ToUpper());	
			database = dataBaseVal;
//		}
//#if UNITY_EDITOR
//		else
//		{
//			database = ( LanguageDataBase )UnityEditor.AssetDatabase.LoadAssetAtPath("Asset/Data/TextData/LanguageDataBase" + lang.ToUpper() + ".asset", 
//		                                                                         typeof(LanguageDataBase));
//		}
//		
//#endif
		if(database != null)
		{
			_currentLang = lang;
			LoadDataBase( database );
		}
		else
		{
			TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"multilanguage error: can not find the database!!!!!");	
		}
	                                                                 
	}
	
	private static void LoadDataBase( LanguageDataBase database )
	{
		if(_textTable == null)
		{
			_textTable = new Dictionary<string, string>();
		}
		
		foreach(LanguageTextEntry entry in database.stringTable)
		{
			_textTable[entry.key] = entry.text;	
				
		}
		_loaded = true;
		
	}
	
	public static string GetString(string key)
	{
		string result = "null string, id:" + key;
		if(!string.IsNullOrEmpty(key) && _loaded && _textTable.ContainsKey(key))
		{
			result = _textTable[key];	
		}
		
		////TraceUtil.Log("GetString " + result);
		return result;
	}

}
