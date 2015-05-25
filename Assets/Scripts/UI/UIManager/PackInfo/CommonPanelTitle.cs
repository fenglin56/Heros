using UnityEngine;
using System.Collections;

namespace UI.MainUI{

	public class CommonPanelTitle : View {

		public SingleButtonCallBack GoldMoneyLabel;
		public SingleButtonCallBack CopperLabel;
        public SingleButtonCallBack PracticeLabel;
		
		public Vector3 ShowPos;
		public Vector3 HidePos;
		public GameObject[] IconLightFlash;

		float animTime = 0.3f;

		void Awake()
		{
			GoldMoneyLabel.SetCallBackFuntion(OnGoleMoneyBtnClick);
			CopperLabel.SetCallBackFuntion(OnCopperBtnClick);
			AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
			GetComponent<UIPanel>().alpha = 0;
			IconLightFlash.ApplyAllItem(P=>P.SetActive(false));

            GoldMoneyLabel.gameObject.RegisterBtnMappingId(UIType.Empty, BtnMapId_Sub.Empty_BuyMoney);
            CopperLabel.gameObject.RegisterBtnMappingId(UIType.Empty, BtnMapId_Sub.Empty_BuyIngot);
            //PracticeLabel.gameObject.RegisterBtnMappingId(UIType.Empty, BtnMapId_Sub.Empty_BuyActivity);
		}

		void OnDestroy()
		{
			RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
		}

		protected override void RegisterEventHandler ()
		{
			throw new System.NotImplementedException ();
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

		void Updatelabel()
		{
			var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
            int money=m_HeroDataModel.PlayerValues.PLAYER_FIELD_HOLDMONEY;
            int moneyAbridge=CommonDefineManager.Instance.CommonDefine.GameMoneyAbridge;
            string newCoppercoin = money>moneyAbridge?(money/10000)+"W":money.ToString();
            int newGoldMoney = m_HeroDataModel.PlayerValues.PLAYER_FIELD_BINDPAY;
            int newPractice = m_HeroDataModel.PlayerValues.PLAYER_FIELD_PRACTICE_NUM;
			GoldMoneyLabel.SetButtonText(newGoldMoney.ToString());
			CopperLabel.SetButtonText(newCoppercoin);
            if(null != PracticeLabel)
            {
                PracticeLabel.SetButtonText(newPractice.ToString());
            }
        }
		
		void OnGoleMoneyBtnClick(object obj)
		{
			TraceUtil.Log("打开充值界面");
			SoundManager.Instance.PlaySoundEffect("Sound_Button_BuyIngot");            
			UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI, UIType.TopUp);
		}

		void OnCopperBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_BuyCopperCoin"); 
			PopupObjManager.Instance.NotEnoughMoneyPanel();
		}

	}
}