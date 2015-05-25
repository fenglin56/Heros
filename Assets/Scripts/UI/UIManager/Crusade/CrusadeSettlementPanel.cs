using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.Crusade
{
	public class CrusadeSettlementPanel : MonoBehaviour 
	{
		public UIPanel Panel_Background;

		public GameObject HeroInfoItemPrefab;
		public Transform[] HeroInfoPoint = new Transform[3];

		public LocalButtonCallBack Button_ReturnTown;

		public Transform RatingIconParent;
		public UISprite UI_RatingIcon;
		public SpriteSwith Swith_RatingIcon;
		public UISprite UI_Time;
		public UISprite UI_TimeBg;
		public UILabel Label_Time;

		public LocalButtonCallBack Button_View;
		public GameObject Eff_BoxOpen;
		public UIPanel Panel_Box;
		public UIPanel Panel_Rewards;
		public SpriteSwith Swith_Reward;
		public GameObject[] Rewards = new GameObject[2];
		public Transform ItemPoint1;
		public Transform ItemPoint2;
		public UISprite[] UI_Items = new UISprite[2];
		public UILabel[] Label_ItemNums = new UILabel[2];
		public UILabel[] Label_ItemNames = new UILabel[2];

		private SMSGECTYPE_CRUSADERESULT_SC m_result;
		private bool m_isBoxOpen = false;
		//private Vector3 m_ReturnTownPos;
		private List<IEntityDataStruct> m_dataStructList = new List<IEntityDataStruct>();

		const float DELAYTIME_RATINGINFO = 2.4f;



		void Awake()
		{
			Button_ReturnTown.SetCallBackFuntion(OnReturnTownClick, null);
			Button_View.SetCallBackFuntion(OnViewClick, null);

			Panel_Background.alpha = 0;
			UI_RatingIcon.alpha = 0;
			UI_Time.alpha = 0;
			UI_TimeBg.alpha = 0;
			Label_Time.alpha = 0;

			Panel_Box.alpha = 0;
			Panel_Rewards.alpha = 0;

			Button_ReturnTown.GetComponent<UIPanel>().alpha = 0;
		}
			

		public void Show(List<IEntityDataStruct> dataStructList, SMSGECTYPE_CRUSADERESULT_SC result)
		{
			this.m_result = result;
			this.m_dataStructList = dataStructList;

			int[] bonusTime = EctypeManager.Instance.GetCurrentEctypeData().Coop_BonusTime;
			int grade = bonusTime.Length;
			for(int i=0;i<bonusTime.Length;i++)
			{
				if(m_result.dwTime<bonusTime[i])
				{
					grade = i;
					break;
				}
			}
			Swith_RatingIcon.ChangeSprite(grade+1);
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_CoopJudgeIntro");
			TweenAlpha.Begin(Panel_Background.gameObject,0.1f,0,1f,ShowBackgroundOverHandle);
		}

		void ShowBackgroundOverHandle(object obj)
		{
			int allHit = 0;
			m_result.sHitNumInfo.ApplyAllItem(p=>allHit += p.dwHitNum);
			if(allHit <= 0)
			{
				allHit = 1;
			}

			for(int i = 0 ; i < m_result.byPlayerNum ; i++)
			{
				if(m_dataStructList.Any(p=>p.SMsg_Header.uidEntity == m_result.sHitNumInfo[i].uid))
				{
					IEntityDataStruct dataStruct = m_dataStructList.SingleOrDefault(p=>p.SMsg_Header.uidEntity == m_result.sHitNumInfo[i].uid);
					GameObject heroItem = UI.CreatObjectToNGUI.InstantiateObj(HeroInfoItemPrefab,transform);
					heroItem.transform.position = HeroInfoPoint[i].position;
					CrusadeSettlementHeroInfoItem item = heroItem.GetComponent<CrusadeSettlementHeroInfoItem>();
					item.Show(m_result.sHitNumInfo[i],dataStruct, allHit, i==0, i*0.1f);
				}
			}
			
			StartCoroutine(AppearRatingInfo(DELAYTIME_RATINGINFO+m_result.byPlayerNum*0.1f));
		}


		IEnumerator AppearRatingInfo(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_CoopJudgeBlink");
			//时间闪烁三次
			float flashTime = 0;
			float onceTime = 0.2f;
			float multiple = 1 / onceTime ;
			while(flashTime < onceTime * 5)
			{
				flashTime += Time.deltaTime;
				float value = flashTime * multiple;
				float a = value % 2;
				if(a < 1)
				{
					UI_Time.alpha = a;
					UI_TimeBg.alpha = a;
				}
				else
				{
					UI_Time.alpha = 2 - a;
					UI_TimeBg.alpha = 2 - a;
				}
				yield return null;
			}
			Label_Time.alpha = 1f;
			//计算出时间的格式
			int hour = m_result.dwTime / 60;
			string hourTenStr = (hour/10).ToString();
			string hourBitStr = (hour%10).ToString();
			string minTenStr = ((m_result.dwTime%60)/10).ToString();
			string minBitStr = ((m_result.dwTime%60)%10).ToString();

			//数字渐渐定格
			float numberShowTime = 0;
			while(numberShowTime < 0.5f)
			{
				numberShowTime+=Time.deltaTime;
				string randomStr = Random.Range(0,10).ToString();
				Label_Time.text = randomStr + randomStr + ":" +randomStr+randomStr;
				yield return null;
			}
			while(numberShowTime < 1f)
			{
				numberShowTime+=Time.deltaTime;
				string randomStr = Random.Range(0,10).ToString();
				Label_Time.text = hourTenStr + randomStr + ":" +randomStr+randomStr;
				yield return null;
			}
			while(numberShowTime < 1.5f)
			{
				numberShowTime+=Time.deltaTime;
				string randomStr = Random.Range(0,10).ToString();
				Label_Time.text = hourTenStr + hourBitStr + ":" +randomStr+randomStr;
				yield return null;
			}
			while(numberShowTime < 2f)
			{
				numberShowTime+=Time.deltaTime;
				string randomStr = Random.Range(0,10).ToString();
				Label_Time.text = hourTenStr + hourBitStr + ":" +minTenStr+randomStr;
				yield return null;
			}
			Label_Time.text = hourTenStr + hourBitStr + ":" +minTenStr+minBitStr;
			//成绩甴大变小渐显出现
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_CoopJudgeTime");
			while(numberShowTime < 2.5f)
			{
				numberShowTime+=Time.deltaTime;

				RatingIconParent.localScale = Vector3.one * (1+(2.5f-numberShowTime)*4);
				UI_RatingIcon.alpha = 1 - (2.5f-numberShowTime)*2 ;

				yield return null;
			}
			RatingIconParent.localScale = Vector3.one;
			UI_RatingIcon.alpha = 1f;

			//宝箱
			Vector3 boxPos = Panel_Box.transform.localPosition;
			TweenPosition.Begin(Panel_Box.gameObject,0.2f,boxPos+Vector3.up * 50, boxPos);
			TweenAlpha.Begin(Panel_Box.gameObject,0.2f,1f);

			//返回按钮
			Vector3 returnTownPos = Button_ReturnTown.transform.localPosition;
			TweenPosition.Begin(Button_ReturnTown.gameObject,0.2f,returnTownPos+Vector3.left * 50, returnTownPos);
			TweenAlpha.Begin(Button_ReturnTown.gameObject,0.2f,1f);
		}

		void OnReturnTownClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_CoopQuite");
			long UIDEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
			NetServiceManager.Instance.EctypeService.SendEctypeRequestReturnCity(UIDEntity);
		}
		void OnViewClick(object obj)
		{
			if(!m_isBoxOpen)
			{
				SoundManager.Instance.PlaySoundEffect("Sound_UIEff_CoopBoxOpen");
				for(int i =0 ; i< m_result.sEquip.Length;i++)
				{
					var itemData = ItemDataManager.Instance.GetItemData((int)m_result.sEquip[i].dwEquipId);
					if(itemData!= null)
					{
						UI_Items[i].spriteName = itemData.smallDisplay;
						TextColor textColor = TextColor.white;
						switch( itemData._ColorLevel)
						{
						case 0:
							textColor = TextColor.EquipmentGreen;
							break;
						case 1:
							textColor = TextColor.EquipmentBlue;
							break;
						case 2:
							textColor = TextColor.EquipmentMagenta;
							break;
						case 3:
							textColor= TextColor.EquipmentYellow;
							break;
						}
						Label_ItemNames[i].text = NGUIColor.SetTxtColor(LanguageTextManager.GetString( itemData._szGoodsName), textColor);
						Label_ItemNums[i].text = m_result.sEquip[i].dwEquipNum.ToString();
					}
					else
					{
						Rewards[i].SetActive(false);
					}
				}

				GameObject eff = UI.CreatObjectToNGUI.InstantiateObj(Eff_BoxOpen, Button_View.transform);

				Swith_Reward.ChangeSprite(2);
				Vector3 rewardPos = Panel_Rewards.transform.localPosition;
				TweenPosition.Begin(Panel_Rewards.gameObject,0.2f,rewardPos+Vector3.left * 50, rewardPos);
				TweenAlpha.Begin(Panel_Rewards.gameObject,0.2f,0,1f,null);

				m_isBoxOpen = true;
			}
		}
	}
}
