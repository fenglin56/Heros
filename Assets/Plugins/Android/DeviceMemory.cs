using UnityEngine;
using System.Collections;

public class DeviceMemory {
	//获取安卓磁盘大小 测试代码
	public static void TestGetAndroidSize()
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
		//获取总磁盘大小
		long allSize = jo.Call<long> ("getTotalInternalAllMemorySize");
		//获取可用总磁盘大小
		long avoidsize = jo.Call<long> ("getTotalInternalAvodiMemorySize");
		bool isGet = jo.Call<bool> ("isExternalStorageAvailable");
		//获取系统磁盘大小
		long sysAllSize = jo.Call<long> ("readSystemAllSize");
		//获取可用系统磁盘大小
		long sysAvaliSize = jo.Call<long> ("readSystemAvailSize");
		//获取SD卡磁盘大小[测试后较准。推荐]
		long SdAllSize = jo.Call<long> ("readSDCardAllSize");
		//获取可用SD卡磁盘大小[测试后较准。推荐]
		long SdAvaliSize = jo.Call<long> ("readSDCardAvailSize");
		//TraceUtil.Log ("==========================isGet==="+isGet+"allSize="+allSize+"avoidsize="+avoidsize+"sysAllSize="+sysAllSize+"sysAvaliSize="+sysAvaliSize+"SdAllSize="+SdAllSize+"SdAvaliSize="+SdAvaliSize);
		#endif
	}
	//获取SD卡磁盘大小[测试后较准。推荐]
	public static long GetAndroidSDCardAllSize()
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
		//获取SD卡磁盘大小[测试后较准。推荐]
		long SdAllSize = jo.Call<long> ("readSDCardAllSize");
		//TraceUtil.Log ("==========================SdAllSize="+SdAllSize);
		return SdAllSize;
		#endif
		return 0;
	}
	//获取可用SD卡磁盘大小[测试后较准。推荐]
	public static long GetAndroidSDCardAvailSize()
	{
		#if !UNITY_EDITOR && UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
		//获取可用SD卡磁盘大小[测试后较准。推荐]
		long SdAvaliSize = jo.Call<long> ("readSDCardAvailSize");
		//TraceUtil.Log ("==========================SdAvaliSize="+SdAvaliSize);
		return SdAvaliSize;
		#endif
		return 0;
	}
}
