using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Globalization;

public static class NativeUtils
{
	public enum NetworkType
	{
		NetNone,
		NetWiFi,
		Net3G
	};
	
	public static bool IsNetworkAvailable()
	{
		int netStatus = NetworkStatus();
		//TraceUtil.Log("NetworkStatus = " + netStatus);
		return (netStatus != 0);
	}

	
#if UNITY_IPHONE 
	[DllImport ("__Internal")]
	private static extern int NetworkStatus();
#else
	private static int NetworkStatus()
	{
		return 0;
	}
#endif	
	
    public static void ReportMemory(string eventName)
	{
#if UNITY_IPHONE 
		if (Debug.isDebugBuild)
		{
			//TraceUtil.Log(eventName + " " + MT_GetCurrentMemoryBytes());
		}
#endif
	}
	
	public static int GetCurrentMemoryBytes()
	{
		return MT_GetCurrentMemoryBytes();
	}

#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport (m_ExternalFileName,CharSet=CharSet.Ansi)]
    private static extern int MT_GetCurrentMemoryBytes();
    private const string m_ExternalFileName = "__Internal";
#else
	private static int MT_GetCurrentMemoryBytes()
	{
		return -1;
	}
#endif
}
