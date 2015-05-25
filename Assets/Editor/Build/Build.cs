//#define BREAK_LATE_LINKS_ONLY

using System;
using System.IO;

using UnityEngine;
using UnityEditor;

public static class Build
{
	#region Command Line	
	
	public static void CommandLineMake()
	{
		var buildParams = new Builder.Params();
		
		var args = Environment.GetCommandLineArgs();
		
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i].StartsWith("+")) 
			{
				string param = args[i].Substring(1).ToLower();
				
				switch(param) 
				{
					case "buildtarget":
					{
						string buildTarget = GetParamValue(args, i);
					
						if (buildTarget == null)
							throw new ArgumentException("Missing parameter value on +buildTarget");   
						else if (!string.IsNullOrEmpty(buildTarget))
							buildParams.Target = (BuildTarget)Enum.Parse(typeof(BuildTarget), buildTarget, true);
					
						i++;
						break;
					}
					
					case "buildlocation":
					{
						string buildLocation = GetParamValue(args, i);
					
						if (buildLocation == null)
							throw new ArgumentException("Missing parameter value on +buildLocation");   
						else if (!string.IsNullOrEmpty(buildLocation))
							buildParams.Location = buildLocation;
					
						i++;
						break;
					}

					case "buildoptions":
					{
						string buildOptions = GetParamValue(args, i);
					
						if (buildOptions == null)
							throw new ArgumentException("Missing parameter value on +buildOptions");   
						else if (!string.IsNullOrEmpty(buildOptions))
						{
							 //Release and Test is "none" and debug is"Development"
						
						
                          	if(buildOptions == "Release")
							{
								 buildOptions = "None";
								 buildParams.IsStageBuild = false;
							}else if(buildOptions == "Test")
							{
                                 buildOptions = "None";
								 buildParams.IsStageBuild = true;
							}
							else
							{
								buildOptions = "Development";
								 buildParams.IsStageBuild = true;
							}
					
							buildParams.Options |= (BuildOptions)Enum.Parse(typeof(BuildOptions), buildOptions, true);

						}
						i++;
						break;
					}
					
					case "extrabuildoptions":
					{
						string extraBuildOptions = GetParamValue(args, i);
					
						if (extraBuildOptions == null)
							throw new ArgumentException("Missing parameter value on +extraBuildOptions");   
						else if (!string.IsNullOrEmpty(extraBuildOptions))
							buildParams.ExtraOptions |= (Builder.Params.ExtraBuildOptions)Enum.Parse(typeof(Builder.Params.ExtraBuildOptions), extraBuildOptions, true);
					
						i++;
						break;
					}
					
					case "buildnumber":
					{
						string buildNumber = GetParamValue(args, i);
					
						if (buildNumber == null)
							throw new ArgumentException("Missing parameter value on +buildNumber");   
						else
							buildParams.BuildNumber = buildNumber;
					
						i++;
						break;
					}
					
					case "androidsubtarget":
					{
						string subTarget = GetParamValue(args, i);
					
						if (subTarget == null)
							throw new ArgumentException("Missing parameter value on +androidBuildSubtarget");   
						else if (!string.IsNullOrEmpty(subTarget))
							buildParams.AndroidSubtarget = (AndroidBuildSubtarget)Enum.Parse(typeof(AndroidBuildSubtarget), subTarget, true);;
					
						i++;
						break;
					}
					
					case "isbuildstorebundleonly":
					{
						
						return;
					}
				}
			}
		}
		
		bool continueRequired = false;
		
		try
		{
			Debug.Log("CommandLineMake .......... Make!!!");
			Make(buildParams);
		}
		catch (Builder.ContinueRequiredException)
		{
			continueRequired = true;
		}
		
		if (!continueRequired)
			EditorApplication.Exit(0);
	}
		
	private static string GetParamValue(string[] args, int pos)
	{
		if (pos > args.Length - 1)
			return null;
		
		pos++;
		
		if (args[pos].StartsWith("-") || args[pos].StartsWith("+"))
			return null;
		
		return args[pos];
	}
		
	#endregion

	public static void Make(Builder.Params buildParams)
	{
		using (var builder = Builder.CreateInstance(buildParams))
		{
#if !BREAK_LATE_LINKS_ONLY
			builder.Build();
#endif
		}
	}
	
	public static void MakeIphone()
	{	
		var buildParams = new Builder.Params();
		string path = Path.Combine(Directory.GetCurrentDirectory(),"Build");
		path = Path.Combine(path,"Output");
		path = Path.Combine(path,"iphone");
		
		buildParams.Target = BuildTarget.iPhone;
		buildParams.Location = path;
		buildParams.Options = BuildOptions.None;
		buildParams.ExtraOptions = Builder.Params.ExtraBuildOptions.BundleBasedPlayer|Builder.Params.ExtraBuildOptions.IncludeBundlesCache;
		buildParams.BuildNumber = "1";
		buildParams.IsStageBuild = false;
	
		bool continueRequired = false;
		
		try
		{
			Make(buildParams);
		}
		catch (Builder.ContinueRequiredException)
		{
			continueRequired = true;
		}
		
		if (!continueRequired)
			EditorApplication.Exit(0);
	}
	
	public static void MakeMac()
	{	
		var buildParams = new Builder.Params();
		
		string path = Path.Combine(Directory.GetCurrentDirectory(),"Build");
		path = Path.Combine(path,"Output");
		path = Path.Combine(path,"mac");
		
		buildParams.Target = BuildTarget.StandaloneOSXIntel;
		buildParams.Location = path;
		buildParams.Options = BuildOptions.None;
		buildParams.ExtraOptions = Builder.Params.ExtraBuildOptions.None ;//Builder.Params.ExtraBuildOptions.BundleBasedPlayer|Builder.Params.ExtraBuildOptions.IncludeBundlesCache;
		buildParams.BuildNumber = "";
	
		bool continueRequired = false;
		
		try
		{
			Make(buildParams);
		}
		catch (Builder.ContinueRequiredException)
		{
			continueRequired = true;
		}
		
		if (!continueRequired)
			EditorApplication.Exit(0);
	}
	
	public static void MakeWeb()
	{	
		var buildParams = new Builder.Params();
		
		string path = Path.Combine(Directory.GetCurrentDirectory(),"Build");
		path = Path.Combine(path,"Output");
		path = Path.Combine(path,"web");
		
		buildParams.Target = BuildTarget.WebPlayer;
		buildParams.Location = path;
		buildParams.Options = BuildOptions.None;
		buildParams.ExtraOptions = Builder.Params.ExtraBuildOptions.None ;//Builder.Params.ExtraBuildOptions.BundleBasedPlayer|Builder.Params.ExtraBuildOptions.IncludeBundlesCache;
		buildParams.BuildNumber = "";
	
		bool continueRequired = false;
		
		try
		{
			Make(buildParams);
		}
		catch (Builder.ContinueRequiredException)
		{
			continueRequired = true;
		}
		
		if (!continueRequired)
			EditorApplication.Exit(0);
	}
	
	
}
