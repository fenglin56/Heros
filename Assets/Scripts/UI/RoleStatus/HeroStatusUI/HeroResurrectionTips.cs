using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.Battle
{

    public class HeroResurrectionTips : View
    {
        public SingleButtonCallBack FullHealthResurgenceBtn;
        public SingleButtonCallBack QuarterHealthResurgenceBtn;
        public SingleButtonCallBack CancelBtn;


        public UILabel MsgLabel;

		private int m_NormalHealthPay = 0;
        private int m_FullHealthPay = 0;

		public SpriteSwith Swith_NormalPay;
		public SpriteSwith Swith_FullPay;

		HeroResurrectionPanel_V2 MyParent;

		public GameObject PassiveResurrect;
		public GameObject InitiativeResurrect;

		public UILabel Label_SurplusResurrect_Tip;
		public UILabel Label_SurplusResurrectTime;

		public UISlider Slider_PassiveResurrect_Time;
		public UILabel Label_Slider_PassiveResurrect_Time;
		public UISlider Slider_InitiativeResurrect_Time;
		public UILabel Label_InitiativeResurrect_Time;

		private Vector3 InitiativeStartPos;
		private Vector3 PassiveStartPos;
		private Vector3 InitiativeEndPos;
		private Vector3 PassiveEndPos;

		private bool m_isCutDowning = false;
		private float m_CurCutDownTime = 0;
		private int m_maxReliveTime = 0;

		private bool m_isNotEnoughtPerfect;
		private bool m_isNotEnoughtNormal;
		private bool m_isNotTimeResurrect;

        void Start()
        {
            FullHealthResurgenceBtn.SetCallBackFuntion(OnFullHealthBtnClick);
            QuarterHealthResurgenceBtn.SetCallBackFuntion(OnQuarterHealthClick);
            CancelBtn.SetCallBackFuntion(OnCancelBtnClick);          
			InitiativeStartPos = Slider_InitiativeResurrect_Time.foreground.position;
			InitiativeEndPos = Label_InitiativeResurrect_Time.transform.position;
			PassiveStartPos = Slider_PassiveResurrect_Time.foreground.position;
			PassiveEndPos = Label_Slider_PassiveResurrect_Time.transform.position;

			RegisterEventHandler();
        }

		void Update()
		{
			//LanguageTextManager.GetString("IDS_H1_66") //秒

			if(m_isCutDowning)
			{
				m_CurCutDownTime -= Time.deltaTime;
				m_CurCutDownTime = Mathf.Clamp(m_CurCutDownTime,0,m_maxReliveTime);
				int second = (int)m_CurCutDownTime;
				float sliderValue = m_CurCutDownTime/m_maxReliveTime;
				if(PassiveResurrect.activeInHierarchy)
				{
					Slider_PassiveResurrect_Time.sliderValue = sliderValue;
					 
					Label_Slider_PassiveResurrect_Time.text = second.ToString()+ LanguageTextManager.GetString("IDS_H1_66");
					Label_Slider_PassiveResurrect_Time.transform.position = 
						Vector3.Lerp(PassiveStartPos, PassiveEndPos, sliderValue);
				}
				if(InitiativeResurrect.activeInHierarchy)
				{
					Slider_InitiativeResurrect_Time.sliderValue = sliderValue;
					Label_InitiativeResurrect_Time.text = second.ToString()+ LanguageTextManager.GetString("IDS_H1_66");
					Label_InitiativeResurrect_Time.transform.position = 
						Vector3.Lerp(InitiativeStartPos, InitiativeEndPos, sliderValue);
				}

				if(m_CurCutDownTime<=0)
				{
					m_isCutDowning = false;
					CloseMyself();
				}
			}
		}

        void OnFullHealthBtnClick(object obj)
        {            				      
			if (m_isNotEnoughtPerfect)
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_44"),1);
                //FullHealthResurgenceBtn.SetButtonText(LanguageTextManager.GetString("IDS_H2_44"));//("元宝不足");
            }
            else
            {
                SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Resurge");
                SendFullHealthToSever();
            }
        }

        void SendFullHealthToSever()
        {
            //long UID = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
            int actorID = PlayerManager.Instance.FindHeroDataModel().ActorID;
            //NetServiceManager.Instance.EctypeService.SendEctypeRequestRevive(UID, UID, EctypeRevive.ER_FULLSTATE);
            NetServiceManager.Instance.EntityService.SendActionRelivePlayer(actorID, actorID, (byte)EctypeRevive.ER_PREFECT);
            TraceUtil.Log("发送满状态复活请求");
            this.CloseMyself();
        }

        void OnQuarterHealthClick(object obj)
        {	
			if (m_isNotEnoughtNormal)
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_35"), 1);

                //GrayButton(QuarterHealthResurgenceBtn);
                //QuarterHealthResurgenceBtn.SetButtonText(LanguageTextManager.GetString("IDS_H2_35"));//("铜币不足");
            }
            else
            {
                SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Resurge");
                SendQuarterHealthToSever();
            }
        }

        void SendQuarterHealthToSever()
        {
            int actorID = PlayerManager.Instance.FindHeroDataModel().ActorID;
            //NetServiceManager.Instance.EctypeService.SendEctypeRequestRevive(UID,UID,EctypeRevive.ER_QUARTERSTATE);
            NetServiceManager.Instance.EntityService.SendActionRelivePlayer(actorID, actorID, (byte)EctypeRevive.ER_NORMAL);
            TraceUtil.Log("发送1/4状态复活请求");
            this.CloseMyself();
        }

        void OnCancelBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            CloseMyself();
			//返回城镇
			long UIDEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
			NetServiceManager.Instance.EctypeService.SendEctypeRequestReturnCity(UIDEntity);
        }

        //void GrayButton(SingleButtonCallBack button)
        //{
        //    button.SetImageButtonComponentActive(false);
        //    button.SetTextColor(Color.gray);
        //    button.SetButtonBackground(2);
        //}

        void RecoverButton(SingleButtonCallBack button)
        {
            button.SetImageButtonComponentActive(true);
            button.SetTextColor(Color.white);
            button.SetButtonBackground(1);
        }

        public void ShowMyself(HeroResurrectionPanel_V2 heroResurrectionPanel_V2)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Voice_Resurge");
            MyParent = heroResurrectionPanel_V2;
            transform.localPosition = new Vector3(0, 0, -200);


			m_maxReliveTime = EctypeManager.Instance.GetCurrentEctypeData().ReviveTime;
			//显示逻辑
			switch(EctypeManager.Instance.GetCurrentEctypeData().ReviveType)
			{
			case 0:
				PassiveResurrect.SetActive(false);
				InitiativeResurrect.SetActive(false);
				return;
			case 1:
				PassiveResurrect.SetActive(false);
				InitiativeResurrect.SetActive(true);
				break;
			case 2:
				PassiveResurrect.SetActive(true);
				InitiativeResurrect.SetActive(false);
				break;
			}

			int vipLevel = PlayerManager.Instance.FindHeroDataModel().GetCommonValue().PLAYER_FIELD_VISIBLE_VIP;
			var reviveNum = EctypeManager.Instance.GetCurrentEctypeData().ReviveNums.SingleOrDefault(p=>p.VipLevel == vipLevel);

			int reliveTime = EctypeManager.Instance.GetEctypeProps().dwReliveTimes + 1;
			int time = reviveNum.Num - reliveTime;

			if(reviveNum.Num == -1)
			{
				Label_SurplusResurrect_Tip.text = LanguageTextManager.GetString("IDS_I11_7");
				Label_SurplusResurrectTime.text = "";
			}
			else
			{
				Label_SurplusResurrect_Tip.text = LanguageTextManager.GetString("IDS_I11_3");
				Label_SurplusResurrectTime.text = time.ToString();

				if(time <= 0)
				{
					m_isNotTimeResurrect = true;
					Label_SurplusResurrect_Tip.text = LanguageTextManager.GetString("IDS_I11_8");
					Label_SurplusResurrectTime.text = "";
					
					QuarterHealthResurgenceBtn.SetButtonColliderActive(false);
					FullHealthResurgenceBtn.SetButtonColliderActive(false);
				}
			}

        	
			//price
			var simplePrice = EctypeManager.Instance.GetCurrentEctypeData().SimpleRevivePrice;
			//计算当前应支付的铜币(元宝) = (向下取整((参数1×〖复活次数〗^2+参数2×复活次数+参数3)/参数4)×参数4)
			m_NormalHealthPay =  (int)((simplePrice.Parma1*reliveTime*reliveTime+simplePrice.Parma2*reliveTime+simplePrice.Parma3)/simplePrice.Parma4)*simplePrice.Parma4;
			if(simplePrice.GoodsID == 3050001)//3050001 铜币
			{
				Swith_NormalPay.ChangeSprite(1);
				m_isNotEnoughtNormal = m_NormalHealthPay > PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
			}
			else
			{
				Swith_NormalPay.ChangeSprite(2);
				m_isNotEnoughtNormal = m_NormalHealthPay > PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
			}

			var pefectPrice = EctypeManager.Instance.GetCurrentEctypeData().PefectRevivePrice;
			//计算当前应支付的铜币(元宝) = (向下取整((参数1×〖复活次数〗^2+参数2×复活次数+参数3)/参数4)×参数4)
			m_FullHealthPay = (int)((pefectPrice.Parma1*reliveTime*reliveTime+pefectPrice.Parma2*reliveTime+pefectPrice.Parma3)/pefectPrice.Parma4)*pefectPrice.Parma4;
			if(pefectPrice.GoodsID == 3050001)//3050001 铜币
			{
				Swith_FullPay.ChangeSprite(1);
				m_isNotEnoughtPerfect = m_FullHealthPay > PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
			}
			else
			{
				Swith_FullPay.ChangeSprite(2);
				m_isNotEnoughtPerfect = m_FullHealthPay > PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;	
			}

			if(m_isNotEnoughtNormal)
			{
				QuarterHealthResurgenceBtn.SetTextColor(Color.red);
			}
			else
			{
				QuarterHealthResurgenceBtn.SetTextColor(Color.white);
			}
			if(m_isNotEnoughtPerfect)
			{
				FullHealthResurgenceBtn.SetTextColor(Color.red);
			}
			else
			{
				FullHealthResurgenceBtn.SetTextColor(Color.white);
			}

			FullHealthResurgenceBtn.SetButtonText("x"+m_FullHealthPay.ToString());
			QuarterHealthResurgenceBtn.SetButtonText("x"+m_NormalHealthPay.ToString());
		}

		public void ResetCutDownTime(float time)
		{
			m_isCutDowning = true;
			m_CurCutDownTime = time;
		}


        public void CloseMyself()
        {
            MyParent.CloseTipsPanel();
//            MyParent.ShowDeathBtn();
        }

		void ClosePanel(object obj)
		{
			CloseMyself();

		}

        protected override void RegisterEventHandler()
        {
			UIEventManager.Instance.RegisterUIEvent(UIEventType.CrusadeSettlement,ClosePanel);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.TrialSettlement,ClosePanel);
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveSettleAccount,ClosePanel);				
        }
		void OnDestroy()
		{
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.CrusadeSettlement,ClosePanel);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.TrialSettlement,ClosePanel);
			UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReceiveSettleAccount,ClosePanel);	
		}
    }
}