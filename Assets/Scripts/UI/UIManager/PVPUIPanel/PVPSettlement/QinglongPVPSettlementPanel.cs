using UnityEngine;
using System.Collections.Generic;


//团队PVP战斗结果界面
public class QinglongPVPSettlementPanel : MonoBehaviour 
{
	public SingleButtonCallBack BackButton;	//返回按钮
	public GameObject LeftResuftSign;		//左边胜负标志
	public GameObject RightResuftSign;		//右边胜负标志

	public QinglongPVPMamberItem[] MamberItem;
	public GameObject[] ResultSignEffPrefab;		//胜负标志出现特效

	public static int TotalDamageSuc{get; set;}		//胜利方造成总伤害
	public static int TotalInjuredSuc{get; set;}	//胜利方受到总伤害
	public static int TotalDamageFai{get; set;}		//失败方造成总伤害
	public static int TotalInjuredFai{get; set;}	//失败方受到总伤害

	private List<QinglongPVPMamberItem> PVPMamberList;
	private SMsgEctypePVP_Result_SC sMsgEctypePVP_Result_SC;

	//第一次初始化，注册事件
	void Awake()
	{
		BackButton.SetCallBackFuntion(BackButtonCallback);
	}

	//接收到服务器消息后调用，传入数据结构体对象
	public void Show(object obj)
	{
		sMsgEctypePVP_Result_SC = (SMsgEctypePVP_Result_SC)obj;

		SoundManager.Instance.StopBGM(0f);
		SoundManager.Instance.PlayBGM("团队PVP战斗胜利音效");

		InitManberList();

		DoForTime.DoFunForTime(0.3f, PlayResultEff, null);
		DoForTime.DoFunForTime(1.3f, ShowMamberList, null);
	}

	/// <summary>
	/// 初始化成员列表
	/// </summary>
	void InitManberList()
	{
		int successNum = sMsgEctypePVP_Result_SC.bySucessNum;
		int failedNum = sMsgEctypePVP_Result_SC.byFailedNum;
		if(sMsgEctypePVP_Result_SC.bySucessFlag == 0)
		{
			for(int i = 0; i < successNum; i++)
			{
				QinglongPVPMamberItem PVPMamberItem = MamberItem[i].GetComponent<QinglongPVPMamberItem>();
				PVPMamberItem.Init(sMsgEctypePVP_Result_SC.sPVPResultSucess[i], Result.Success);
				PVPMamberList.Add(PVPMamberItem);
				TotalDamageSuc += sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwDamage;
				TotalInjuredSuc += sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwInjured;
			}
			for(int i = 0; i < failedNum; i++)
			{
				QinglongPVPMamberItem PVPMamberItem = MamberItem[i+3].GetComponent<QinglongPVPMamberItem>();
				PVPMamberItem.Init(sMsgEctypePVP_Result_SC.sPVPResultFailed[i], Result.Failed);
				PVPMamberList.Add(PVPMamberItem);
				TotalDamageFai += sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwDamage;
				TotalInjuredFai += sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwInjured;
			}
		}
		else
		{
			for(int i = 0; i < failedNum; i++)
			{
				QinglongPVPMamberItem PVPMamberItem = MamberItem[i].GetComponent<QinglongPVPMamberItem>();
				PVPMamberItem.Init(sMsgEctypePVP_Result_SC.sPVPResultFailed[i], Result.Failed);
				PVPMamberList.Add(PVPMamberItem);
				TotalDamageFai += sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwDamage;
				TotalInjuredFai += sMsgEctypePVP_Result_SC.sPVPResultFailed[i].dwInjured;
			}
			for(int i = 0; i < successNum; i++)
			{
				QinglongPVPMamberItem PVPMamberItem = MamberItem[i+3].GetComponent<QinglongPVPMamberItem>();
				PVPMamberItem.Init(sMsgEctypePVP_Result_SC.sPVPResultSucess[i], Result.Success);
				PVPMamberList.Add(PVPMamberItem);
				TotalDamageSuc += sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwDamage;
				TotalInjuredSuc += sMsgEctypePVP_Result_SC.sPVPResultSucess[i].dwInjured;
			}
		}
	}

	//播放战果出现特效
	void PlayResultEff(object obj)
	{
		//实例化特效

		//显示胜负标志
		if(sMsgEctypePVP_Result_SC.bySucessFlag == 0)	//神兵谷胜
		{
			LeftResuftSign.GetComponent<SpriteSwith>().ChangeSprite(1);
			RightResuftSign.GetComponent<SpriteSwith>().ChangeSprite(2);
		}
		else
		{
			LeftResuftSign.GetComponent<SpriteSwith>().ChangeSprite(2);
			RightResuftSign.GetComponent<SpriteSwith>().ChangeSprite(1);
		}
		SoundManager.Instance.PlaySoundEffect("战果出现音效");
	}

	//显示参战成员战斗记录列表
	void ShowMamberList(object obj)
	{
		PVPMamberList.ApplyAllItem(p=>{
			p.Show();
		});
	}

	//返回按钮点击回调
	void BackButtonCallback(object obj)
	{
		//因为一次pvp只显示一次，会切换场景，所以Destroy掉本panel对象
		//发送返回主城场景请求消息给服务端

		SoundManager.Instance.PlaySoundEffect("Sound_Button_CoopQuite");
		long UIDEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
		NetServiceManager.Instance.EctypeService.SendEctypeRequestReturnCity(UIDEntity);
	}
}

