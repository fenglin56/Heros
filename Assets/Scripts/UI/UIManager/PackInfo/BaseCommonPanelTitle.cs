using UnityEngine;
using System.Collections;
using UI.MainUI;

public class BaseCommonPanelTitle : View {

	public SingleButtonCallBack LeftAddBtn;
	public SingleButtonCallBack RightAddBtn;
	public SpriteSwith LeftIconSprite;
	public SpriteSwith RightIconSprite;
    public SingleButtonCallBack VigourAddBtn;
    public UILabel VigourUpdateTime;
    public UILabel IDS_VigourUpdateTime;
	public Vector3 ShowPos;
	public Vector3 HidePos;
	public GameObject[] IconLightFlash;
	
	float animTime = 0.3f;
	private CommonTitleType m_leftType;   //左边类型（铜币，元宝，体力）
	private CommonTitleType m_rightType;  //右边类型（铜币，元宝，体力,  钻石）
	private float DiamondNum = 0;

	void Awake()
	{

        IDS_VigourUpdateTime.SetText(LanguageTextManager.GetString("IDS_I5_15"));
		AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
		GetComponent<UIPanel>().alpha = 0;
		IconLightFlash.ApplyAllItem(P=>P.SetActive(false));
       
        RightAddBtn.gameObject.RegisterBtnMappingId(UIType.Empty, BtnMapId_Sub.Empty_BuyIngot);
        if (VigourAddBtn != null)
        {
            VigourAddBtn.gameObject.RegisterBtnMappingId(UIType.Empty, BtnMapId_Sub.Empty_BuyActivity);
        } else
        {
            LeftAddBtn.gameObject.RegisterBtnMappingId(UIType.Empty, BtnMapId_Sub.Empty_BuyMoney);
        }
        UIEventManager.Instance.RegisterUIEvent(UIEventType.UpdateEnegryTimeEvent, ShowEnemgryLeftTimeBar);
	}

	public void Init(CommonTitleType leftType,CommonTitleType rightType)
	{
		m_leftType=leftType;
		m_rightType=rightType;
		LeftAddBtn.SetCallBackFuntion(OnLeftPropertyBtnClick,leftType);
		RightAddBtn.SetCallBackFuntion(OnRightPropertyBtnClick,rightType);
        VigourAddBtn.SetCallBackFuntion(OnVigroupPropertyBtnClick);
		
        if (leftType == CommonTitleType.Power)
        {
            LeftAddBtn.gameObject.SetActive(false);
            VigourAddBtn.gameObject.SetActive(true);
            ShowEnemgryLeftTimeBar(null);
            //LeftIconSprite.ChangeSprite((int)leftType);
        }
		else if(leftType == CommonTitleType.Practice)
		{
			LeftAddBtn.gameObject.SetActive(true);
			if(LeftAddBtn.BackgroundSwithList.Length>0)
			{
				LeftAddBtn.BackgroundSwithList[0].gameObject.SetActive(false);
			}
			VigourAddBtn.gameObject.SetActive(false);
			LeftIconSprite.ChangeSprite((int)leftType);
		}
		else
        {
            LeftAddBtn.gameObject.SetActive(true);
            VigourAddBtn.gameObject.SetActive(false);
            LeftIconSprite.ChangeSprite((int)leftType);
        }
		RightIconSprite.ChangeSprite((int)rightType);
	}
	void OnDestroy()
	{
		RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.UpdateEnegryTimeEvent, ShowEnemgryLeftTimeBar);
	}
	protected override void RegisterEventHandler ()
	{
	}
	
	public void TweenShow()
	{
		Updatelabel();
		TweenAlpha.Begin(gameObject,animTime,0,1,null);
		TweenPosition.Begin(gameObject,animTime,HidePos,ShowPos,null);
		
		StartCoroutine(ShowIconLightFlash(true));
	}
	private IEnumerator ShowIconLightFlash(bool isActive)
	{
		yield return new WaitForSeconds(animTime);
		IconLightFlash.ApplyAllItem(P=>P.SetActive(isActive));
	}
	public void tweenClose()
	{
		TweenAlpha.Begin(gameObject,animTime,0);
		TweenPosition.Begin(gameObject,animTime,HidePos);
		StartCoroutine(ShowIconLightFlash(false));
	}
	
	void UpdateViaNotify(INotifyArgs iNotifyArgs)
	{
		EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)iNotifyArgs;
		if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
		{
			Updatelabel();
		}
	}

	public void UpdateDiamond()
	{
#if (UNITY_ANDROID && !UNITY_EDITOR)  
#if ANDROID_TENCENT
	GetPropertyAmount(m_rightType);
#endif
#endif
	}
	
	void Updatelabel()
	{
		string leftValue=GetPropertyAmount(m_leftType);
        string rightValue = GetPropertyAmount(m_rightType);
        //string VigroupValue = GetPropertyAmount(CommonTitleType.Power);
		LeftAddBtn.SetButtonText(leftValue);
		RightAddBtn.SetButtonText(rightValue);

        VigourAddBtn.SetButtonText(leftValue);
	}
	private string GetPropertyAmount(CommonTitleType commonTitleType)
	{
		var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
		string amount="0";
		switch(commonTitleType)
		{
		case CommonTitleType.GoldIngot:
			amount = m_HeroDataModel.PlayerValues.PLAYER_FIELD_BINDPAY.ToString();
			break;
		case CommonTitleType.Money:
                int money=m_HeroDataModel.PlayerValues.PLAYER_FIELD_HOLDMONEY;
                int moneyAbridge=CommonDefineManager.Instance.CommonDefine.GameMoneyAbridge;
                amount = money>moneyAbridge?(money/10000)+"W":money.ToString();
			break;
		case CommonTitleType.Power:
            amount = string.Format("{0}/{1}", m_HeroDataModel.PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE, m_HeroDataModel.PlayerValues.PLAYER_FIELD_MAX_ACTIVELIFE);
			break;
		case CommonTitleType.Practice:
			amount = m_HeroDataModel.PlayerValues.PLAYER_FIELD_PRACTICE_NUM.ToString();
			break;
		case CommonTitleType.Diamond:
			amount = DiamondNum.ToString();
			JHPlatformConnManager.Instance.TencentBalance((info) => {
				if(info != null && info.ret == 0)
				{
					DiamondNum = info.balance;
				}	

				RightAddBtn.SetButtonText(DiamondNum.ToString());
			});
			break;
		}
		return amount;
	}



	void OnLeftPropertyBtnClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_BuyIngot");            

        switch(m_leftType)
        {
            case CommonTitleType.GoldIngot:
            {
                UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI, UIType.TopUp);
            }
                break;
            case CommonTitleType.Money:
            {
                PopupObjManager.Instance.NotEnoughMoneyPanel();
            }
                break;
       
        }
		
	}
	
	void OnRightPropertyBtnClick(object obj)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_BuyCopperCoin"); 
        switch(m_rightType)
        {
            case CommonTitleType.GoldIngot:
            {
                UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI, UIType.TopUp);
            }
                break;
            case CommonTitleType.Money:
            {
                PopupObjManager.Instance.NotEnoughMoneyPanel();
            }
                break;
            case CommonTitleType.Power:
                {
                    OnVigroupPropertyBtnClick(null);
                }
                break;
			case CommonTitleType.Diamond:
				{
					JHPlatformConnManager.Instance.ChargeMoney((result) => {
						if(result)	UpdateDiamond();
					});
				} 
				break;
        }
	}
	
    void OnVigroupPropertyBtnClick(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_BuyCopperCoin"); 
        PopupObjManager.Instance.ShowAddVigour();
    }

    void ShowEnemgryLeftTimeBar(object obj)
    {
      
        int targetTime = CommonDefineManager.Instance.CommonDefineFile._dataTable.VitRecoverTime*60;
        var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
        int passTime = (int)EnegryColdWorkData.Instance.CurrentPassTime;
        if (PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE >=PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAX_ACTIVELIFE)
        {
           
            VigourUpdateTime.gameObject.SetActive(false);
            IDS_VigourUpdateTime.gameObject.SetActive(false);
        }
        else
        {
            //TraceUtil.Log("自动增加活力时间：" + passTime+",GetTime:"+EnegryColdWorkData.Instance.CurrentPassTime);
            if(!VigourUpdateTime.gameObject.activeSelf)
            {
                VigourUpdateTime.gameObject.SetActive(true);
                IDS_VigourUpdateTime.gameObject.SetActive(true);
            }
            int timeLeft = targetTime - passTime;
            VigourUpdateTime.SetText(timeLeft/60+(timeLeft%60 == 0?0:1));
        }
    }
}
