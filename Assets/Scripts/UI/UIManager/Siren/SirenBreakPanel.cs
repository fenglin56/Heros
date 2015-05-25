using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;

namespace UI.Siren
{
	public class SirenBreakPanel : MonoBehaviour
	{
		public Transform FirstAttributePoint;
		public GameObject AttributeBreakPrefab;
		public LocalButtonCallBack Button_Break;
		public LocalButtonCallBack Button_Cancel;
		public GameObject[] Materials = new GameObject[2];
		public UISprite[] UI_Materials = new UISprite[2];
		public SpriteSwith[] Swith_MaterialColors = new SpriteSwith[2];
		public UILabel[] Label_MaterialProcesses = new UILabel[2];

		public UILabel Label_LevelCeiling;
		public UILabel Label_Skill;

		private List<SirenAttributeItem> m_attributeItemList = new  List<SirenAttributeItem>();
		private int mAttributeItemNum = 8;

		private Vector3 m_OneMaterialPos = new Vector3(0,47,0);
		private Vector3[] m_TwoMaterialPos = new Vector3[2]{new Vector3(-52,47,0),new Vector3(52,47,0)};

		private bool[] m_isMaterialsSatisfy = new bool[2];

		void Awake()
		{
			for(int i = 0; i< mAttributeItemNum;i++)
			{
				GameObject item = (GameObject)Instantiate(AttributeBreakPrefab);
				item.transform.parent = FirstAttributePoint;
				item.transform.localPosition = Vector3.down * 30 * i;
				item.transform.localScale = Vector3.one;
				SirenAttributeItem  sirenAttributeItem = item.GetComponent<SirenAttributeItem>();
				m_attributeItemList.Add(sirenAttributeItem);
			}
			TaskGuideBtnRegister ();
		}

		void Start()
		{
			Button_Cancel.SetCallBackFuntion(OnCloseClock,null);
		}

		public void Show(ButtonCallBack callBack, SirenConfigData data, int nextMaxLevel, List<SirenGrowthEffect> curGrowthEffList,List<SirenGrowthEffect> nextGrowthEffList)
		{
			Button_Break.SetCallBackFuntion(callBack);

			//材料
			Materials.ApplyAllItem(p=>p.SetActive(false));

			var breakCondition = data.BreakCondition;
			switch(breakCondition.Length)
			{
			case 1:
				Materials[0].transform.localPosition = m_OneMaterialPos;
				m_isMaterialsSatisfy[0] = false;
				m_isMaterialsSatisfy[1] = true;
				break;
			case 2:
				Materials[0].transform.localPosition = m_TwoMaterialPos[0];
				Materials[1].transform.localPosition = m_TwoMaterialPos[1];
				m_isMaterialsSatisfy[0] = false;
				m_isMaterialsSatisfy[1] = false;
				break;
			default:
				m_isMaterialsSatisfy[0] = true;
				m_isMaterialsSatisfy[1] = true;
				break;
			}
			for(int i=0;i<breakCondition.Length;i++)
			{
				Materials[i].SetActive(true);
				var itemData = ItemDataManager.Instance.GetItemData(breakCondition[i].ItemID);
				if(itemData!=null)
				{
					UI_Materials[i].spriteName = itemData.smallDisplay;
					Swith_MaterialColors[i].ChangeSprite(itemData._ColorLevel+1);
				}
				else
				{
					Materials[i].SetActive(false);
				}			
				Label_MaterialProcesses[i].text = GetMaterialProcess(breakCondition[i].ItemID,breakCondition[i].ItemNum, out m_isMaterialsSatisfy[i]);			
			}

			if(m_isMaterialsSatisfy[0] && m_isMaterialsSatisfy[1])
			{
				StartCoroutine(ChangeButtonEnable(Button_Break,true));
			}
			else
			{
				StartCoroutine(ChangeButtonEnable(Button_Break,false));
			}

			//等级上限
			Label_LevelCeiling.text = LanguageTextManager.GetString("IDS_I2_14")+ NGUIColor.SetTxtColor(data.BreakStageMaxLevel.ToString(), TextColor.white)
				+ " → "+NGUIColor.SetTxtColor(nextMaxLevel,TextColor.green);
			//团队技能
			Label_Skill.text = NGUIColor.SetTxtColor(LanguageTextManager.GetString("IDS_I2_15"), TextColor.DescriptionYellow)+
				NGUIColor.SetTxtColor(LanguageTextManager.GetString(data.BreakDesc), TextColor.white);

			//属性增长
			int listLenget = curGrowthEffList.Count;
			for(int i=0;i<m_attributeItemList.Count;i++)
			{
				if(i>=listLenget)
				{
					m_attributeItemList[i].gameObject.SetActive(false);
				}
				else
				{
					m_attributeItemList[i].gameObject.SetActive(true);
					m_attributeItemList[i].InitBreakLabel(curGrowthEffList[i].EffectData.IDS,curGrowthEffList[i].EffectData.EffectRes,
					                            curGrowthEffList[i].GrowthEffectValue,nextGrowthEffList[i].GrowthEffectValue);
				}
				
			}

			transform.localPosition = Vector3.zero;
		}

		IEnumerator ChangeButtonEnable(LocalButtonCallBack button, bool isFlag)
		{
			yield return new WaitForEndOfFrame();
			button.SetEnabled(isFlag);
		}

		/// <summary>
		/// 是否突破中
		/// </summary>
		/// <returns><c>true</c> if this instance is break; otherwise, <c>false</c>.</returns>
		public bool IsBreak()
		{
			return transform.localPosition == Vector3.zero;
		}

		private string GetMaterialProcess(int itemID, int itemNum, out bool isSatisfy)
		{
			TextColorType color = TextColorType.Red;
			string str = "";
			int hadNum = ContainerInfomanager.Instance.GetItemNumber(itemID);
			if(hadNum>=itemNum)
			{
				color = TextColorType.ItemQuality0;
				isSatisfy = true;
			}
			else
			{
				isSatisfy = false;
			}
			str = hadNum.ToString()+"/"+itemNum.ToString();
			return str.SetColor(color);
		}
		private void SetMaterialSpriteName(GameObject obj, UISprite sprite,SpriteSwith swith, ItemData itemData)
		{
			if(itemData!=null)
			{
				sprite.spriteName = itemData.smallDisplay;
				swith.ChangeSprite(itemData._ColorLevel);
				obj.SetActive(true);
			}
			else
			{
				obj.SetActive(false);
			}
		}

		void OnCloseClock(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Siren_BreakCancel");
			Close();
		}

		public void Close()
		{
			transform.localPosition = Vector3.back * 1000;
		}
		/// <summary>
		/// 引导按钮注入代码
		/// </summary>
		private void TaskGuideBtnRegister()
		{
			Button_Break.gameObject.RegisterBtnMappingId(UIType.Siren, BtnMapId_Sub.Siren_Break_Btn);
			Button_Cancel.gameObject.RegisterBtnMappingId(UIType.Siren, BtnMapId_Sub.Siren_Break_Back);
		}
	}
}
