using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class VersionConfigData
{
	public string Date;
	public int Version;
}

public class VersionConfigDatabase : ScriptableObject
{

	public VersionConfigData versionData;
}
