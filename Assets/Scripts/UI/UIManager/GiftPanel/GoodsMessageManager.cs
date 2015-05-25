using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.Gift;

namespace UI
{
	public class GoodsMessageManager : MonoBehaviour 
	{
		private static GoodsMessageManager m_instance = null;
		public static GoodsMessageManager Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = FindObjectOfType(typeof(GoodsMessageManager)) as GoodsMessageManager;
				}
				return m_instance;
			}
		}

		public GameObject MessageItemPrefab;
		public GameObject NoticeMessageItemPrefab;
		private List<GameObject> m_MessageItemList = new List<GameObject>();
		private List<ShowGoodsInfo> m_GoodsInfoList = new List<ShowGoodsInfo>();
		private Vector3 m_MessageItemAppearPos = new Vector3(0, 200, -200);

		private bool m_IsShowLater = false;

		private List<string> m_MessageContentList = new List<string>();

		private List<SMsgChat_SC> m_noticeMessageList = new List<SMsgChat_SC>();
		private GameObject m_CurNoticeMessage = null;

		/* 旧方法,先保留
		public void Show(int goodsID, int num)
		{
			if(m_IsShowLater)
			{
				m_GoodsInfoList.Add (new ShowGoodsInfo(){dwGoodsID = goodsID,dwGoodsNum = num});
			}
			else
			{
				m_IsShowLater = true;
				ShowImmediately(goodsID, num);
				StartCoroutine("LateRestore");
			}
		}
		*/

		public void Show(int goodsID, int num)
		{
			if(m_IsShowLater)
			{
				m_MessageContentList.Add (PraseToString(goodsID,num));
			}
			else
			{
				m_IsShowLater = true;
				ShowImmediately(PraseToString(goodsID,num));
				StartCoroutine("LateRestore");
			}
		}

		public void Show(string smg)
		{
			if(m_IsShowLater)
			{
				m_MessageContentList.Add (smg);
			}
			else
			{
				m_IsShowLater = true;
				ShowImmediately(smg);
				StartCoroutine("LateRestore");
			}
		}

		#region
		public void AddNoticeMessage(SMsgChat_SC sChat)
		{
			m_noticeMessageList.Add(sChat);
			ShowNoticeMessage();
		}
		public void ClearCurrentNoticeMessage()
		{
			m_CurNoticeMessage = null;
			ShowNoticeMessage();
		}
		private void ShowNoticeMessage()
		{
			if(m_CurNoticeMessage == null)
			{
				if(m_noticeMessageList.Count > 0)
				{
					m_CurNoticeMessage = UI.CreatObjectToNGUI.InstantiateObj(NoticeMessageItemPrefab,transform);
					NoticeMessageItem noticeMessageItem = m_CurNoticeMessage.GetComponent<NoticeMessageItem>();
					noticeMessageItem.Show(m_noticeMessageList[0]);
					m_noticeMessageList.RemoveAt(0);
				}
			}
		}
		#endregion

		IEnumerator LateRestore()
		{
			yield return new WaitForSeconds(CommonDefineManager.Instance.CommonDefine.ItemMsgTimeGap);
			if(m_MessageContentList.Count > 0)
			{
				ShowImmediately(m_MessageContentList[0]);
				m_MessageContentList.RemoveAt(0);
				StartCoroutine("LateRestore");
			}
			else
			{
				m_IsShowLater = false;
			}
		}

		private void ShowCacheInfo()
		{

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
		/* 旧方法,先保留
		private void ShowImmediately(int goodsID, int num)
		{
			int listLength = m_MessageItemList.Count;
			if(listLength >= CommonDefineManager.Instance.CommonDefine.ItemMsgLimit)
			{
				GiftMessageItem gm = m_MessageItemList[listLength-1].GetComponent<GiftMessageItem>();
				gm.Disappear();
				m_MessageItemList.RemoveAt(listLength-1);
			}
			
			m_MessageItemList.ApplyAllItem(p=>
			                               {
				GiftMessageItem pItem = p.GetComponent<GiftMessageItem>();
				pItem.Push();
			});
			
			GameObject message = (GameObject)Instantiate(MessageItemPrefab);
			message.transform.parent = PopupObjManager.Instance.UICamera.transform;
			message.transform.localScale = Vector3.one;
			
			m_MessageItemList.Add(message);
			GiftMessageItem gmItem = message.GetComponent<GiftMessageItem>();
			gmItem.Show(goodsID, num ,m_MessageItemAppearPos);
			
			StartCoroutine(LateDestroy(message));
		}
		*/
		private void ShowImmediately(string smg)
		{
			int listLength = m_MessageItemList.Count;
			if(listLength >= CommonDefineManager.Instance.CommonDefine.ItemMsgLimit)
			{
				GiftMessageItem gm = m_MessageItemList[listLength-1].GetComponent<GiftMessageItem>();
				gm.Disappear();
				m_MessageItemList.RemoveAt(listLength-1);
			}
			
			m_MessageItemList.ApplyAllItem(p=>
			                               {
				GiftMessageItem pItem = p.GetComponent<GiftMessageItem>();
				pItem.Push();
			});
			
			GameObject message = (GameObject)Instantiate(MessageItemPrefab);
			message.transform.parent = PopupObjManager.Instance.UICamera.transform;
			message.transform.localScale = Vector3.one;
			
			m_MessageItemList.Add(message);
			GiftMessageItem gmItem = message.GetComponent<GiftMessageItem>();
			gmItem.Show(smg,m_MessageItemAppearPos);
			
			StartCoroutine(LateDestroy(message));
		}




		IEnumerator LateDestroy(GameObject gobj)
		{
			yield return new  WaitForSeconds(CommonDefineManager.Instance.CommonDefine.ItemMsgTimeDisappear);
			if(gobj != null)
			{
				m_MessageItemList.Remove(gobj);
				GiftMessageItem item = gobj.GetComponent<GiftMessageItem>();
				item.Disappear();
			}
		}
	}
}
