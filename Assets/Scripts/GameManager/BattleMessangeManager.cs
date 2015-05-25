using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.Battle
{
	public class BattleMessangeManager : MonoBehaviour 
	{
		private static BattleMessangeManager m_instance = null;
		public static BattleMessangeManager Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = FindObjectOfType(typeof(BattleMessangeManager)) as BattleMessangeManager;
				}
				return m_instance;
			}
		}

		public GameObject BattleMessageItemPrefab;
		private List<GameObject> m_MessageItemList = new List<GameObject>();

		private Vector3 m_MessageItemAppearPos = new Vector3(0,/*-320 // 旧值*/ -200, -10);
		private bool m_IsShowLater = false;

		private List<CacheData> m_CacheMessageList = new List<CacheData>();	

		private float m_lifeTime = 3f;//持续时间
		private float m_gapTime = 1f;//间隔

		public void Show(string numStr, int itemID)
		{
			var goodData = ItemDataManager.Instance.GetItemData(itemID);
			if(goodData == null)
				return;
			TextColor textColor = TextColor.white;
			switch(goodData._ColorLevel)
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
				textColor = TextColor.EquipmentYellow;
				break;
			default:
				break;
			}
			string nameStr = NGUIColor.SetTxtColor(LanguageTextManager.GetString(goodData._szGoodsName),textColor);			                               

			if(m_IsShowLater)
			{
				m_CacheMessageList.Add(new CacheData(numStr,nameStr));
			}
			else
			{
				m_IsShowLater = true;
				ShowImmediately(numStr,nameStr);
				StartCoroutine("LateRestore");
			}
		}

		public void Show(string numStr, string nameStr)
		{
			nameStr = NGUIColor.SetTxtColor(nameStr,TextColor.EquipmentGreen);	
			if(m_IsShowLater)
			{
				m_CacheMessageList.Add(new CacheData(numStr,nameStr));
			}
			else
			{
				m_IsShowLater = true;
				ShowImmediately(numStr,nameStr);
				StartCoroutine("LateRestore");
			}
		}

		IEnumerator LateRestore()
		{
			yield return new WaitForSeconds(m_gapTime);
			if(m_CacheMessageList.Count > 0)
			{
				ShowImmediately(m_CacheMessageList[0].NumStr,m_CacheMessageList[0].NameStr);
				m_CacheMessageList.RemoveAt(0);
				StartCoroutine("LateRestore");
			}
			else
			{
				m_IsShowLater = false;
			}
		}
		
		private string PraseToString(int goodsID, int num)
		{
			var goodData = ItemDataManager.Instance.GetItemData(goodsID);
			if(goodData == null)
				return "ItemID is Wrong";
			TextColor textColor = TextColor.white;
			switch(goodData._ColorLevel)
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
				textColor = TextColor.EquipmentYellow;
				break;
			default:
				break;
			}
			string content = string.Format(LanguageTextManager.GetString("IDS_H1_568"),
			                               NGUIColor.SetTxtColor(LanguageTextManager.GetString(goodData._szGoodsName),textColor), num.ToString());
			return content;
		}
	

		private void ShowImmediately(string num, string name)
		{
			int listLength = m_MessageItemList.Count;
			if(listLength >= CommonDefineManager.Instance.CommonDefine.ItemMsgLimit)
			{
				BattleMessageItem gm = m_MessageItemList[listLength-1].GetComponent<BattleMessageItem>();
				gm.Disappear();
				m_MessageItemList.RemoveAt(listLength-1);
			}
			
			GameObject message = (GameObject)Instantiate(BattleMessageItemPrefab);
			message.transform.parent = PopupObjManager.Instance.UICamera.transform;
			message.transform.localScale = Vector3.one;
			message.transform.localPosition = m_MessageItemAppearPos;

			m_MessageItemList.Add(message);
			BattleMessageItem gmItem = message.GetComponent<BattleMessageItem>();
			gmItem.Show(num,name);
			
			StartCoroutine(LateDestroy(message));
		}
							
		IEnumerator LateDestroy(GameObject gobj)
		{
			yield return new  WaitForSeconds(CommonDefineManager.Instance.CommonDefine.ItemMsgTimeDisappear);
			if(gobj != null)
			{
				m_MessageItemList.Remove(gobj);
				BattleMessageItem item = gobj.GetComponent<BattleMessageItem>();
				item.Disappear();
			}
		}

		public class CacheData
		{
			public string NumStr;
			public string NameStr;

			public CacheData(string numStr,string nameStr)
			{
				NumStr = numStr;
				NameStr = nameStr;
			}
		}

	}
}