using UnityEngine;
using System.Collections;
using System.Linq;
using UI;

public class DefenceSettleBoxBehaviour : MonoBehaviour {

	public GameObject BoxOpenEff;
	public SingleButtonCallBack OpenBtn;
	public GameObject OpenEffPoint;
	public SpriteSwith BoxSwitch;
    public GameObject BoxOpenTipsEff;
    public GameObject BoxOpenEffPoint;  //可点击开启特效挂载点
    public GameObject OPenResume;  //开启消耗
    public GameObject BoxReward;//  开宝箱奖励

	private bool m_isLeftNormalAward;  //奖励或花元宝开启
	private EctypeContainerData m_ectypeContainerData;
	private Vector3 ICON_SCALE=new Vector3(35,35,1);

    private SpriteSwith m_resumeIconSwitch;
    private UILabel m_resumeAwount;
    private UILabel m_rewardName;
    private UILabel m_rewardAwount;
    private GameObject m_openEff;
	void Awake()
	{
        m_resumeIconSwitch = OPenResume.GetComponentInChildren<SpriteSwith>();
        m_resumeAwount = OPenResume.GetComponentInChildren<UILabel>();
        Transform rewardName,rewardAmount;
         BoxReward.transform.RecursiveFindObject("Name", out rewardName);
         m_rewardName = rewardName.GetComponent<UILabel>();
         BoxReward.transform.RecursiveFindObject("Value", out rewardAmount);
         m_rewardAwount = rewardAmount.GetComponent<UILabel>();

		OpenBtn.SetCallBackFuntion((obj)=>
		                           {
			if(m_isLeftNormalAward)
			{
				NetServiceManager.Instance.EctypeService.SendSMSGEctypeClickTreasure_CS(0);
				OpenBtn.Enable=false;
			}
			else
			{
				var playerIngot=m_ectypeContainerData.ByCostType==0?
					PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY
						:PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
				if(playerIngot<m_ectypeContainerData.ByCost)
				{
					//不足,弹提示
                    if(m_ectypeContainerData.ByCostType==0)
                    {
                        UI.MessageBox.Instance.ShowTips(3, string.Format(LanguageTextManager.GetString("IDS_I15_4"), "元宝"), 1);
                        PopupObjManager.Instance.NotEnoughMoneyPanel();
                    }
                    else
                    {
                        UI.MessageBox.Instance.ShowTips(3, string.Format(LanguageTextManager.GetString("IDS_I15_4"),"铜币"), 1);
                        PopupObjManager.Instance.NotEnoughMoneyPanel();
                    }
				}
				else
				{
					NetServiceManager.Instance.EctypeService.SendSMSGEctypeClickTreasure_CS(1);
					OpenBtn.Enable=false;
				}
			}
		});

		GameDataManager.Instance.dataEvent.RegisterEvent(DataType.EctypeTreasureReward, UpdateTreasureChests); 
	}
	public void Init(bool isLeftNormalAward,int ectypeContainerId)
	{
        BoxReward.SetActive(false);
		m_ectypeContainerData=EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeContainerId];
		m_isLeftNormalAward=isLeftNormalAward;
		int spriteId=0;
		string byCostValue="";
		if(!isLeftNormalAward)
		{
			spriteId=m_ectypeContainerData.ByCostType==0?2:1;  //付费翻牌,0元宝   1铜币
			byCostValue=m_ectypeContainerData.ByCost.ToString();

            m_resumeIconSwitch.ChangeSprite(spriteId);
            m_resumeAwount.text = byCostValue;
		}
        OPenResume.SetActive(!isLeftNormalAward);
        m_openEff=NGUITools.AddChild(BoxOpenEffPoint, BoxOpenTipsEff);
	}
	/// <summary>
	/// 刷新宝箱状态
	/// </summary>
	void UpdateTreasureChests(object obj)
	{
		EctypeTreasureRewardList ectypeTreasureRewardList = GameDataManager.Instance.PeekData(DataType.EctypeTreasureReward) as EctypeTreasureRewardList;
		if (ectypeTreasureRewardList != null)
		{
			var myTreasureChestsData = ectypeTreasureRewardList.TreasureList.FirstOrDefault(P => P.dwUID == PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity);
			//TraceUtil.Log( SystemModel.Rocky, "myTreasureChestsData.UID:" + myTreasureChestsData.dwUID);
			if (myTreasureChestsData.dwUID != 0)
			{
				if(myTreasureChestsData.byClickType==(m_isLeftNormalAward?0:1))
				{
					TweenOpenTreasureChests(myTreasureChestsData);

					ectypeTreasureRewardList.TreasureList.Remove(myTreasureChestsData);
				}
			}
		}
	}
	/// <summary>
	/// 打开宝箱
	/// </summary>
	void TweenOpenTreasureChests(SMSGEctypeTreasureReward_SC data)
	{
        GameObject.Destroy(m_openEff);
        OPenResume.SetActive(false);

        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_DefenceResult_5");
		GameObject effectOBj =NGUITools.AddChild(OpenEffPoint,BoxOpenEff);
		BoxSwitch.ChangeSprite(2);
		Transform itemTran;
		if(effectOBj.transform.RecursiveFindObject("ngui",out itemTran))
		{
			var awardItemData=ItemDataManager.Instance.GetItemData(data.dwEquipId);
			itemTran.ClearChild();
			var skillIcon=CreatObjectToNGUI.InstantiateObj(awardItemData._picPrefab, itemTran);
		}

		DoForTime.DoFunForTime(1.3f, SwithTreasurePanel, data);
	}
	void SwithTreasurePanel(object obj)
	{
		SMSGEctypeTreasureReward_SC data = (SMSGEctypeTreasureReward_SC)obj;
		var awardItemData=ItemDataManager.Instance.GetItemData(data.dwEquipId);
        BoxReward.SetActive(true);
        m_rewardName.text = LanguageTextManager.GetString(awardItemData._szGoodsName);
        m_rewardAwount.text = data.dwEquipNum.ToString();
	}
	void OnDestroy()
	{
		GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.EctypeTreasureReward, UpdateTreasureChests); 
	}
}
