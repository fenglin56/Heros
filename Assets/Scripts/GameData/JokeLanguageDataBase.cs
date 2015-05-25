using UnityEngine;
using System;
using System.Collections;
[Serializable]
public class JokeLanguageData
{
	public string key;  
	public string text;
}

public class JokeLanguageDataBase : ScriptableObject
{
	public JokeLanguageData[] stringTable;
	
}