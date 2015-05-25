using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.Friend;
namespace UI.MainUI
{
public class EmaiFriendList : BaseSubUiPanel {
		// List<PanelElementDataModel> FriendList=new List<PanelElementDataModel>();
		public UITable ItemTable;
		public UILabel NoneItemLable;
		public GameObject FriendListItemPrefab;
		public SingleButtonCallBack BackButton;
		public SingleButtonCallBack ChoseButton;
		protected  List<EmaiFriendListItem> ItemList=new List<EmaiFriendListItem>();
		protected List<PanelElementDataModel> FriendList;
		public List<EmaiFriendListItem> SelectedItemList=new List<EmaiFriendListItem>();
		private GameObject Item_go;
        public GameObject ButEff;

		void Awake()
		{
            NoneItemLable.SetText(LanguageTextManager.GetString("IDS_I22_37"));
			BackButton.SetCallBackFuntion(OnBackButtonClick);
			ChoseButton.SetCallBackFuntion(OnChoseButtonClick);
		}

		void OnBackButtonClick(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailSelectfriendCancel");
			EmailInfoPanelManager.GetInstance().ChangeEmailPageStatus(EmailPageStatus.ShowEmail,EmaiSubPageStatus.EmailWrite);
		}


		void OnChoseButtonClick(object obj)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailSelectfriendConfirmation");
			EmailInfoPanelManager.GetInstance().ChangeEmailPageStatus(EmailPageStatus.ShowEmail,EmaiSubPageStatus.EmailWrite);
			EmailInfoPanelManager.GetInstance().SC_EmailTabManager.Sc_SendEmailContent.SetFriendInfo(SelectedItemList[0].Friend);
		}

        void SetChoseBtnDesable()
        {
            ChoseButton.SetButtonColliderActive(false);
            ChoseButton.BackgroundSwithList.ApplyAllItem(c=>c.ChangeSprite(3));
        }
        void SetChosebtnEnable()
        {
            ChoseButton.SetButtonColliderActive(true);
            ChoseButton.BackgroundSwithList.ApplyAllItem(c=>c.ChangeSprite(1));
        }

		public override void ShowPanel ()
		{
			base.ShowPanel ();
			StartCoroutine(RefreshList());
		}
		public virtual IEnumerator RefreshList()
		{
            SetChosebtnEnable();
			NoneItemLable.gameObject.SetActive(false);
			ItemTable.transform.ClearChild();
			ItemList.Clear();
			InitItemFileinfoList();
            FriendListSort();
			if(FriendList.Count>0)
			{
                ButEff.SetActive(true);
				for( int i=0;i<FriendList.Count;i++)
				{
					
					Item_go=NGUITools.AddChild(ItemTable.gameObject,FriendListItemPrefab);
					Item_go.name=FriendListItemPrefab.name+i;
                    Item_go.AddComponent<UIDragPanelContents>();
					EmaiFriendListItem Sc_item=Item_go.GetComponent<EmaiFriendListItem>();
					Sc_item.InitItemData(FriendList[i]);
					Sc_item.OnClickCallBack=ItemSelectedEventHandle;
					ItemList.Add(Sc_item);

				}
				ItemList[0].BeSelected();
             
				yield return null;
				ItemTable.Reposition();
                ItemTable.transform.parent.GetComponent<UIDraggablePanel>().ResetPosition();
			}
			else
			{
                SetChoseBtnDesable();
				NoneItemLable.gameObject.SetActive(true);
                ButEff.SetActive(false);
			}
			
		}
        void FriendListSort()
        {
            FriendList.Sort((c1,c2)=>{
                if(c1.sMsgRecvAnswerFriends_SC.sActorLevel>c2.sMsgRecvAnswerFriends_SC.sActorLevel)
                {
                    return -1;
                }
                else if(c1.sMsgRecvAnswerFriends_SC.sActorLevel<c2.sMsgRecvAnswerFriends_SC.sActorLevel)
                {
                    return 1;
                }
                else
                {
                    if(c1.sMsgRecvAnswerFriends_SC.dwFriendID<c2.sMsgRecvAnswerFriends_SC.dwFriendID)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }

            });
        }
		void InitItemFileinfoList()
		{
			FriendList=FriendDataManager.Instance.GetFriendListData;
		}
        public override void HidePanel()
        {
            base.HidePanel();
            ButEff.SetActive(false);
        }
		public  void ItemSelectedEventHandle(EmaiFriendListItem selectedEquipItem)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailSelectfriend");
			//所有项LoseFocus
			SelectedItemList.Clear();
			ItemList.ApplyAllItem(p=>p.OnLoseFocus());
			selectedEquipItem.OnGetFocus();
			SelectedItemList.Add(selectedEquipItem);
			//EmailInfoPanelManager.GetInstance().ChangeEmailPageStatus(EmailPageStatus.ShowEmail,EmaiSubPageStatus.EmailWrite);
		}
}
}