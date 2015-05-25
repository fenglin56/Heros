using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace UI.Friend
{
	public class FriendChildPanel : MonoBehaviour
	{
		public ItemPagerManager ItemPagerManager_Friend;
		public GameObject FriendItemPrefab;
		public UIDraggablePanel DraggablePanel;
		public UIGrid Grid;
		public UILabel Label_PeopleNum;

		public GameObject Tip_NoneFriend;
		public UILabel Label_NoneFriend;

		private List<FriendItem_V3> m_FriendList = new List<FriendItem_V3>();
		private FriendBtnType m_thisType;

		void Awake()
		{
			ItemPagerManager_Friend.OnPageChanged += ItemPageChangedHandle;
		}

		void ItemPageChangedHandle(PageChangedEventArg pageSmg)
		{
			m_FriendList.ApplyAllItem(p=>{
				p.gameObject.SetActive(false);
			});
			int size = ItemPagerManager_Friend.PagerSize;
			var showFriendArray = m_FriendList.Skip((pageSmg.StartPage - 1) * size).Take(size).ToArray();
			showFriendArray.ApplyAllItem(p =>{
				p.gameObject.SetActive(true);
			});
			ItemPagerManager_Friend.UpdateItems(showFriendArray, "friendList");
		}

		public void Show()
		{
			transform.localPosition = Vector3.zero;
		}

		public void Close()
		{
			transform.localPosition = Vector3.back * 800;
		}

		private void CreateItem(PanelElementDataModel data, FriendBtnType type)
		{
			GameObject item = UI.CreatObjectToNGUI.InstantiateObj(FriendItemPrefab,Grid.transform);
			FriendItem_V3 friendItem = item.GetComponent<FriendItem_V3>();
			friendItem.Init(data.sMsgRecvAnswerFriends_SC,type,DeleteFriendItemCallBack );
			m_FriendList.Add(friendItem);
		}
		void DeleteFriendItemCallBack(int friendID)
		{
			for(int i =0 ; i< m_FriendList.Count; i++)
			{
				if(m_FriendList[i].FriendActorID == friendID)
				{
					var friendItem = m_FriendList[i];
					m_FriendList.RemoveAt(i);
					Destroy(friendItem.gameObject);
					break;
				}
			}
			if(gameObject.activeInHierarchy)
			{
				StartCoroutine("LateReposition");
			}
		}
		IEnumerator LateReposition()
		{
			yield return new WaitForEndOfFrame();
			Grid.Reposition();


			int currentPage = ItemPagerManager_Friend.CurrentPage;
			int count = ItemPagerManager_Friend.PagerSize;
			if(currentPage > 1)
			{
				if(currentPage >  Mathf.CeilToInt( m_FriendList.Count * 1f / count))
				{
					currentPage--;
				}
			}
			if (m_FriendList.Count == 0)
			{
				ItemPagerManager_Friend.InitPager(1, 1, 0);
			}
			else
			{
				ItemPagerManager_Friend.InitPager(m_FriendList.Count, currentPage, 0);
			}
//			PageChangedEventArg pageChangedEventArg = new PageChangedEventArg(currentPage, count);
//			ItemPageChangedHandle(pageChangedEventArg);

			if(m_FriendList.Count<=0)
			{
				switch(m_thisType)
				{
				case FriendBtnType.MyFriend:
					Label_NoneFriend.text = LanguageTextManager.GetString("IDS_I23_8");
					break;
				case FriendBtnType.Town:
					Label_NoneFriend.text = LanguageTextManager.GetString("IDS_I23_14");
					break;
				case FriendBtnType.Request:
					Label_NoneFriend.text = LanguageTextManager.GetString("IDS_I23_22");
					UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim,UI.MainUI.UIType.Friend);
					break;
				}

				Tip_NoneFriend.SetActive(true);
			}
			else
			{
				Tip_NoneFriend.SetActive(false);
			}

			UpdateLabelNum(m_thisType);
		}

		/// <summary>
		/// 清除
		/// </summary>
		public void Clear()
		{
			m_FriendList.ApplyAllItem(p=>Destroy(p.gameObject));
			m_FriendList.Clear();

		}

		public void UpdateLabelNum(FriendBtnType type)
		{
			switch(type)
			{
			case FriendBtnType.MyFriend:
				Label_PeopleNum.text = m_FriendList.Count.ToString()+"/"+FriendDataManager.Instance.FriendMaxNum.ToString();
				break;
			case FriendBtnType.Request:
				Label_PeopleNum.text = m_FriendList.Count.ToString()+"/"+CommonDefineManager.Instance.CommonDefine.FriendRequestLimit.ToString();
				break;
			case FriendBtnType.Town:
				Label_PeopleNum.text = m_FriendList.Count.ToString();
				break;
			}
		}

		public void CreateItems(List<PanelElementDataModel> list, FriendBtnType type)
		{
			m_thisType = type;

			Clear();

			list.ApplyAllItem(p=>{
				CreateItem(p,type);
			});

			//初始页数
			if (m_FriendList.Count == 0)
			{
				ItemPagerManager_Friend.InitPager(1, 1, 0);
			}
			else
			{
				ItemPagerManager_Friend.InitPager(m_FriendList.Count, 1, 0);
			}

			if(gameObject.activeInHierarchy)
			{
				StartCoroutine("LateReposition");
			}

		}

	}
}
