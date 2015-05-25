using UnityEngine;
using System.Collections;

public class MiGameSdk
{

#if (UNITY_ANDROID && !UNITY_EDITOR)



	public static void InitSDK (string ReciveMsgGoName, int appId,
	                            string appKey, int screenOrient)
	{
        PlatformLoginBehaviour.message = "ReciveMsName" + ReciveMsgGoName + "   appId:" + appId;
        

		callSdkApi ("InitSDK",
		            ReciveMsgGoName, appId,
		            appKey, screenOrient);
	}

	public static void Login ()
	{
		callSdkApi ("Login");
	}

	public static void Pay (string order,int coinValue, string actorName, string actorId)
	{
		callSdkApi ("Pay", order, coinValue, actorName, actorId);
	}


	private static void callSdkApi (string apiName, params object[] args)
	{

		
		using (AndroidJavaClass cls = new AndroidJavaClass(MIConfigData.SDK_JAVA_CLASS)) {
			cls.CallStatic (apiName, args);
		}
	}


#endif

}
