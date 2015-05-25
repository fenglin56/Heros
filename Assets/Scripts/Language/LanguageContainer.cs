using UnityEngine;
using System.Collections;
using System;

 
[Serializable]
public class LanguageGroup
{
	public string lang;
	public LanguageDataBase m_dataBase;
}

/// <summary>
/// EnterPoint Scene GameManager  多语言管理
/// </summary>
///
public class LanguageContainer : MonoBehaviour
{
	public LanguageGroup[] m_languageDataGroups;
	
	
	private static LanguageContainer _instance;
    public static LanguageContainer Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType(typeof(LanguageContainer)) as LanguageContainer;
				if(_instance != null)
				{
					_instance.Load();
				}
			}
			return _instance;
		}
	}
	
	
	
	public void Awake()
	{
		Load();
	}
	
	public void Load()
	{
		string lang = "chs";
		foreach( LanguageGroup lg in m_languageDataGroups)
		{
			if(lg.lang.ToUpper().Equals(lang.ToUpper()))
			{
				LanguageTextManager.LoadDataBase(lang, lg.m_dataBase);
				break;	
			}
		}
		
		
	}
	
//	public LanguageDataBase GetLanguageDataBase(string lang)
//	{
//		LanguageDataBase langData = null;
//		if(_languages.Length > 0)
//		{
//			foreach(Language language in _languages)
//			{
//				if(language.Lang.ToUpper().Equals(lang.ToUpper()))
//				{
//					langData = language.LangDataBase;
//					break;
//				}
//			}
//		}
//		return langData;
//	}
}
