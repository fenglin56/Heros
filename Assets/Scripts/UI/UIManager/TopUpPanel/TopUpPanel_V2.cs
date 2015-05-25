using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace UI.MainUI
{
    public class TopUpPanel_V2 : BaseUIPanel
    {
		public VipPrevillegeResDataBase vipPreResDataBase ;
		public GoldRechargeDataManager GoldRechargeDataManager;
		public UISprite spriteTitle;
		public BaseCommonPanelTitle comPanelTitle;
		//vip显示界面
		public VipShowPanel vipPanel;
		//购买界面
		public GameObject payPanel;
		public GameObject goodListPanel ;
		public GameObject goodsPerfab;
		private string goodStr = "GoodsCard";
		//6个商品//
		private List<SingleTopUpCard> SingleTopUpCardList = new List<SingleTopUpCard>();
		//当前选择标记
		public Transform curSelectCardMark ;
        private SingleTopUpCard curSelectCard;
		//特权界面
		public GameObject powersParent;
		public GameObject powersPanel;
		private VipPrevillegePanelController powerPanelScript;
		//boundView
		//public GameObject boundView;
		//public GameObject CommonToolPrefab;

        private int CurrentPageNumber = 1;
        private List<GoldRechargeData> GoldList;
        //public GameObject UIBottomBtnPrefab;
        //public Transform CreatBottomBtnPoint;
        //private CommonUIBottomButtonTool commonUIBottomButtonTool;
        public SingleButtonCallBack BuyButton;
        //public SingleButtonCallBack LastPageBtn;
        //public SingleButtonCallBack NextPageBtn;
        //public UILabel PageLabel;

        public GameObject NPCPrefab;
        public Camera NPCCamera;
		private GameObject NpcObj;
		private string DefaultAnim;

        public Vector3 NpcPos;
		public SingleButtonCallBack btnBack;
		//当前界面为支付界面
		[HideInInspector]
		public bool isCurPayPanel = true;
		private Vector3 originalPos ;
		private Vector3 movePos = new Vector3(51,0,0);
		private float moveTime = 0.167f;
        void Awake()
        {
			for (int i = 1; i <= 6; i++) {
				Transform tran = goodListPanel.transform.Find (goodStr+i);
				GameObject goods = NGUITools.AddChild(tran.gameObject,goodsPerfab);
				SingleTopUpCardList.Add(goods.gameObject.GetComponent<SingleTopUpCard>());
				SingleTopUpCardList[i-1].Init(true,this,null,i);
				SingleTopUpCardList[i-1].PositionX = (i-1)/3+1;
				SingleTopUpCardList[i-1].PositionY = (i-1)%3+1;
			}
			goodsPerfab.SetActive (false);
            BuyButton.SetCallBackFuntion(OnBuyButtonClick);
			btnBack.SetCallBackFuntion(OnBackButtonTapped);
			//npc展示
            NpcObj = GameObject.Instantiate(NPCPrefab) as GameObject;
			NpcObj.transform.parent = NPCCamera.transform.Find ("ModelParent");
            NpcObj.transform.localPosition = NpcPos;
			NpcObj.transform.localRotation = Quaternion.Euler(0,197f,0);
			NpcObj.transform.localScale = Vector3.one;
            DefaultAnim = NpcObj.animation.clip.name;
			UIEventManager.Instance.RegisterUIEvent(UIEventType.VipGradeUpdate, OnVipGradeUpdate);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.VipPaySuccess, OnVipPaySuccess);
			vipPanel.Init (this);
			originalPos = payPanel.transform.localPosition;
			GameObject go = UI.CreatObjectToNGUI.InstantiateObj (powersPanel,powersParent.transform);
			powerPanelScript = go.GetComponent<VipPrevillegePanelController>();
			//Invoke ("Test",8);
        }
		//设置当前选中项
		void SetCurPaySelectMark(int shopID)
		{
			curSelectCardMark.parent = SingleTopUpCardList[shopID].transform;
			curSelectCardMark.localPosition = Vector3.zero;
		}
		//vip升级
		void OnVipGradeUpdate(object obj)
		{
			if (!gameObject.activeInHierarchy)
				return;
			int vipGrade = (int)obj;
			//更新界面数据
			vipPanel .VipUpgradeSuccess();
		}
		//充值成功
		void OnVipPaySuccess(object obj)
		{
			if (!gameObject.activeInHierarchy)
				return;
			vipPanel.StartProgressEff ();
		}
        public override void Show(params object[] value)
        {
            NPCCamera.gameObject.SetActive(true);
            int MPlatForm = 0;
            if (Application.platform != RuntimePlatform.WindowsEditor && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
            {
                MPlatForm = 2;
            }
            else
            {
                MPlatForm = 1;
            }
            GoldList = GoldRechargeDataManager.goldRechargeConfigDataBase.GoledRechargeDataList.Where(P => P.RechargeType == MPlatForm).ToList();
            ResetpageInfo();
			SetCurPaySelectMark (0);
			isCurPayPanel = true;
			OnVipShowButtonClick (isCurPayPanel);
			vipPanel .Show(true);
#if (UNITY_ANDROID && !UNITY_EDITOR)  
#if ANDROID_TENCENT
			comPanelTitle.Init(CommonTitleType.GoldIngot, CommonTitleType.Diamond);
#endif
#else
			comPanelTitle.Init(CommonTitleType.Money, CommonTitleType.GoldIngot);
#endif
			comPanelTitle.GetComponent<BaseCommonPanelTitle>().TweenShow ();
            base.Show(value);
        }

        public override void Close()
        {
            if (!IsShow)
                return;
            NPCCamera.gameObject.SetActive(false);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
			comPanelTitle.GetComponent<BaseCommonPanelTitle>().tweenClose ();
			TweenClose ();
            base.Close();
        }

        public void NpcPlayAnim(string animationClip)
        {
            this.NpcObj.animation.CrossFade(animationClip);
            StopAllCoroutines();
            StartCoroutine(PlayIdleForTime(NpcObj.animation[animationClip].clip.length));
        }

        IEnumerator PlayIdleForTime(float time)
        {
            yield return new WaitForSeconds(time);
            NpcObj.animation.CrossFade(DefaultAnim,0.1f);
        }

        void ShowBottomBtn()
        {
            //CommonBtnInfo btnInfo = new CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnBackButtonTapped);
            //commonUIBottomButtonTool.Show(new List<CommonBtnInfo>() { btnInfo });
        }

        /*void OnNextPageBtnClick(object obj)
        {
            if (GoldList.Count > (CurrentPageNumber) * SingleTopUpCardList.Count)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
                CurrentPageNumber++;
                ResetpageInfo();
            }
        }

        void OnLastPageBtnClick(object obj)
        {
            if (CurrentPageNumber > 1)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
                CurrentPageNumber--;
                ResetpageInfo();
            }
        }*/

        void ResetpageInfo()
        {
            SingleTopUpCardList.ApplyAllItem(P=>P.Close());
			//SingleTopUpCardList [0].OnTopUpCardSelect ();//SingleTopUpCardList[0]);
            foreach(var child in GoldList)
            {
                string[] m_Position = child.RechargePosition.Split('+');
                if (m_Position[0] == CurrentPageNumber.ToString())
                {
					SingleTopUpCardList.First(P => P.PositionX == int.Parse(m_Position[1]) && P.PositionY == int.Parse(m_Position[2])).ShowVip(child);
                }
            }
            OnTopUpCardSelect(SingleTopUpCardList[0]);
            //Color enabelColor = new Color(1, 1, 1, 1);
            //Color disabelColor = new Color(1, 1, 1, 0.3f);
//            LastPageBtn.BackgroundSprite.color = CurrentPageNumber > 1 ? enabelColor : disabelColor;
//            NextPageBtn.BackgroundSprite.color = GoldList.Count < CurrentPageNumber * SingleTopUpCardList.Count?disabelColor:enabelColor;
            TraceUtil.Log(GoldList.Count/6+ "," + (float)GoldList.Count % 6f);
//            PageLabel.SetText(string.Format("{0}/{1}",CurrentPageNumber,GoldList.Count/6+((float)GoldList.Count%6f>0?1:0)));
        }

        public void OnTopUpCardSelect(SingleTopUpCard selectTopUpCard)
        {
            curSelectCard = selectTopUpCard;
            NpcPlayAnim(selectTopUpCard.goldRechargeData.TouchAnimation);
			SetCurPaySelectMark (selectTopUpCard.shopIndex-1);
			selectTopUpCard.OnTopUpCardSelect ();
			//SingleTopUpCardList.ApplyAllItem(P=>P.OnTopUpCardSelect(selectTopUpCard));//jamfing
        }

        void OnBuyButtonClick(object obj)
        {
            if(curSelectCard!=null)
            {

//                #if (UNITY_ANDROID && !UNITY_EDITOR)  
//                #if ANDROID_OPPO   
//                JHPlatformConnManager.Instance.Payment(null,curSelectCard.goldRechargeData.CurrencyNumber,curSelectCard.goldRechargeData.GoldNumber,curSelectCard.goldRechargeData.GoldNumber,1);
//                #endif
//                #endif
				#if (UNITY_ANDROID && !UNITY_EDITOR)  
				#if ANDROID_TENCENT
				string msg = string.Format(LanguageTextManager.GetString("IDS_I4_120"), curSelectCard.goldRechargeData.CurrencyNumber,	curSelectCard.goldRechargeData.GoldNumber);
				MessageBox.Instance.Show(4,"", msg,  LanguageTextManager.GetString("IDS_I4_123"),  LanguageTextManager.GetString("IDS_I4_124"), CancelBuy, BuyComfirm);

				#endif 
				#endif

   				//MessageBox.Instance.ShowTips(1,LanguageTextManager.GetString("IDS_I4_68"),1.5f);
            }
            TraceUtil.Log("购买物品");
			SoundManager.Instance.PlaySoundEffect("Sound_Button_VIPPrivilegeReturn");
        }

		private void CancelBuy()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_VIPRechargeClick");
		}

		private void BuyComfirm()
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_VIPRechargeClick");
			JHPlatformConnManager.Instance.TencentPurchase((P) => {
				if(P == null)
				{
					MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_I4_122"), LanguageTextManager.GetString("IDS_H2_55"));
					return;
				}

				switch((PurchaseFlag)P.ret)
				{
//				case PurchaseFlag.Success:
//					MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_524"), LanguageTextManager.GetString("IDS_H2_55"));
//					break;
//				case PurchaseFlag.DeliveryFail:
//					MessageBox.Instance.Show(3, "", "发货失败，请联系客服，流水账号" + P.billno, LanguageTextManager.GetString("IDS_H2_55"));
//					break;
				case PurchaseFlag.BalanceNotEnough:
					MessageBox.Instance.Show(3,"", LanguageTextManager.GetString("IDS_I4_121"), LanguageTextManager.GetString("IDS_H2_28"),  LanguageTextManager.GetString("IDS_H2_55"), null, ()=> {
						JHPlatformConnManager.Instance.ChargeMoney((result) => {
							if(result) comPanelTitle.UpdateDiamond(); 
						});
					});
					break;
				case PurchaseFlag.PurchaseFail:
				case PurchaseFlag.NetworkError:
					MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_I4_122"), LanguageTextManager.GetString("IDS_H2_55"));
					break;
				default:
					break;
			}
			}, curSelectCard.goldRechargeData.CurrencyNumber,curSelectCard.goldRechargeData.GoldNumber);

		}

        private void OnBackButtonTapped(object obj)
        {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_VIPBack");
            Close();
            CleanUpUIStatus();
        }
		//切换界面
		public void OnVipShowButtonClick(bool isPayPanel)
		{
			SetPayPanelAct(isPayPanel);
			SetPowerPanelAct(!isPayPanel);
			spriteTitle.spriteName = isPayPanel?"JH_UI_Typeface_1311":"JH_UI_Typeface_1315";
		}
		private void SetPayPanelAct(bool isShow)
		{
			//这里不需要刷新界面
			payPanel.SetActive(isShow);
			if (isShow) {
				TweenShow ();	
			} else {
				TweenClose();	
			}
		}
		private void SetPowerPanelAct(bool isShow)
		{
			//这里需要刷新界面
			powerPanelScript.gameObject.SetActive(isShow);
			if (isShow) {
				//再调用特权的show
				powerPanelScript.OnShow();
			}
		}
		public void TweenShow()
		{
			TweenPosition.Begin(payPanel,moveTime,originalPos-movePos,originalPos);
		}
		
		public void TweenClose()
		{
			TweenPosition.Begin(payPanel,moveTime,originalPos,originalPos-movePos);
		}
		//
		void OnDestroy()
		{
			UIEventManager.Instance.RegisterUIEvent(UIEventType.VipPaySuccess, OnVipPaySuccess);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.VipGradeUpdate, OnVipGradeUpdate);
		}
    }
}