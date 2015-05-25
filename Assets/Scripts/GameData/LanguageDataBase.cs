using UnityEngine;
using System;
using System.Collections;
[Serializable]
public class LanguageTextEntry
{
	public string key;  
	public string text;
}

public class LanguageDataBase : ScriptableObject
{
	public LanguageTextEntry[] stringTable;
	
}
