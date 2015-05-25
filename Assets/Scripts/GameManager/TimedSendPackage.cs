using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 定时向服务器发包,需要挂在一个场景中的物件
/// </summary>
public class TimedSendPackage : MonoBehaviour {

    private List<ISendInfoToServer> m_senderContainer = new List< ISendInfoToServer>();
    private float m_sendRate;
    public float TimesPerSecond;
    
	void Start () {
        if (TimesPerSecond == 0) TimesPerSecond = 4;  //默认每秒发送4次

        this.m_sendRate=1 / TimesPerSecond;
        ////TraceUtil.Log("this.m_sendRate:"+this.m_sendRate);
        InvokeRepeating("TimedSend", Random.Range(0, this.m_sendRate), this.m_sendRate);
	}
    void TimedSend()
    {
        lock (this.m_senderContainer)
        {
            for(int i=0;i<this.m_senderContainer.Count;i++)
            {
                NetServiceManager.Instance.CommonService.SendTimedPackage(this.m_senderContainer[i].GetSendInfoPkg());
            }
        }
        
    }
    /// <summary>
    /// 添加需要定时发送数据包给服务器的接口方法
    /// </summary>
    /// <param name="sendInfoObj"></param>
    public void AddSendInfoObj(ISendInfoToServer sendInfoObj)
    {
        lock (this.m_senderContainer)
        {
            this.m_senderContainer.Add(sendInfoObj);
        }
    }
    public void RemoveSendInfoObj(ISendInfoToServer sendInfoObj)
    {
        lock (this.m_senderContainer)
        {
            this.m_senderContainer.Remove(sendInfoObj);
        }
    }    
}
public interface ISendInfoToServer
{
    Package GetSendInfoPkg();
}
