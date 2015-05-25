using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 心跳机制，当Connect到账号服务器，开始发心跳。当准备连接到网关服务器时停止心跳。NewWorld的时候启动心跳
/// LoginService
/// EntityService
/// </summary>
public class HeartFPSManager
{
    public int SamplerAmount = 15;  //采样数
    public bool StartHeart = false;
    private uint m_index = 1;
    private HeartFpsPackage[] m_heartFpsPackages;
    private static HeartFPSManager m_instance;

    public static HeartFPSManager Instance
    {
        get 
        {
            if (m_instance == null)
            {
                m_instance = new HeartFPSManager();
            }
            return m_instance;
        }
    }
    private HeartFPSManager()
    {
        m_heartFpsPackages = new HeartFpsPackage[SamplerAmount];
    }
    class HeartFpsPackage
    {
        public uint Index;
        public float SendTime;
        public float ReceiveTime;
        public float DeltTime
        {
            get { return ReceiveTime - SendTime; }
        }
    }
    public uint MakeHeartFps()
    {
        uint index = 0;
        if (StartHeart)
        {
            var heartFpsPackage = new HeartFpsPackage();
            heartFpsPackage.Index = m_index;
            heartFpsPackage.SendTime = heartFpsPackage.ReceiveTime = Time.realtimeSinceStartup;

            m_heartFpsPackages[(m_index - 1) % SamplerAmount] = heartFpsPackage;
            m_index++;
            index = heartFpsPackage.Index;
        }
       
        return index;
    }
    public void Clear()
    {
        StartHeart = false;
        m_index = 1;
        TraceUtil.Log("StartHeart :" + StartHeart);
        m_heartFpsPackages = new HeartFpsPackage[SamplerAmount];
    }
    public void ReceiveHeartFps(uint index)
    {
        var heartPkg = m_heartFpsPackages[(index - 1) % SamplerAmount];
		if (heartPkg != null) {
			heartPkg.ReceiveTime = Time.realtimeSinceStartup;
			CheatManager.Instance.connectDelayTime = (int)(heartPkg.DeltTime*1000);
		}
    }
    public float GetSamplerDeltTime()
    {
        float totalDelt=0;
        for (int i = 0; i < SamplerAmount; i++)
        {
            if (m_heartFpsPackages[i] != null)
            {
                totalDelt += m_heartFpsPackages[i].DeltTime;
            }
            else
            {
                return totalDelt / ++i;
            }
        }
        return totalDelt / SamplerAmount;
    }
    public ushort GetLastDelay(uint index)
    {
        if (index > 1)
        {
            return (ushort)Math.Ceiling(m_heartFpsPackages[(index - 2) % SamplerAmount].DeltTime * 1000);
        }
        else
        {
            return 0;
        }
    }
    public bool VerifyLoseConnection()
    {
        return m_heartFpsPackages[SamplerAmount - 1] != null && GetSamplerDeltTime() == 0;
    }
}
