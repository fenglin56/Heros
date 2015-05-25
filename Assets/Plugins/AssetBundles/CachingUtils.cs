using System;
using System.IO;

using UnityEngine;

public static class CachingUtils
{
	public static string CacheDirectory
	{
		get
		{
			switch (Application.platform)
			{
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.OSXDashboardPlayer:
				case RuntimePlatform.OSXPlayer:
				case RuntimePlatform.OSXWebPlayer:
					return Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library/Caches/Unity");

				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsWebPlayer:
					return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unity\\WebPlayer\\Cache");
				
				case RuntimePlatform.IPhonePlayer:
					return Path.Combine(Application.temporaryCachePath.Substring(0, Application.temporaryCachePath.Length - 6), "UnityCache");
			}
			
			return Path.Combine(Application.persistentDataPath, "UnityCache");
		}
	}

	public static string GetAppIdentifierForCache(string companyName, string productName)
	{
		#if UNITY_WEBPLAYER || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
		return ConvertToLegalPathNameWithUnityBug(companyName) + "_" + ConvertToLegalPathNameWithUnityBug(productName);
		#else
		return "Shared";
		#endif
	}
	
	public static string GetCacheDirectory(string companyName, string productName)
	{
		return Path.Combine(CacheDirectory, GetAppIdentifierForCache(companyName, productName));
	}
	
	// Unity has a bug in the way if canonize the path
	private static string ConvertToLegalPathNameWithUnityBug(string name)
	{
		System.Text.StringBuilder builder = new System.Text.StringBuilder(name);
		
		for (int i = builder.Length; i > 0; i--)
		{
			char c = builder[i - 1];
			if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
				continue;
			
			builder[i] = '_';
		}
		
		return builder.ToString();
	}
		
	private static string ConvertToLegalPathName(string name)
	{
		string invalid = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());
		
		System.Text.StringBuilder builder = new System.Text.StringBuilder(name);
		
		foreach (char c in invalid)
		{
			builder.Replace(c, '_');
		}		
		
		return builder.ToString();
	}
	
	public static int GetVersionFromHash(byte[] hash)
	{
		int val = BitConverter.ToInt32(hash, 0);
		if (val < 0)
			val = -val;
		
		return val;
	}
	
	public static int GetVersionFromId(string id)
	{
		int val = Convert.ToInt32(id, 16);
		if (val < 0)
			val = -val;	
		
		return val;
	}
}
