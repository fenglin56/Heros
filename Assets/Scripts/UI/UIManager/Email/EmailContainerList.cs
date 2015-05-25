using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace UI.MainUI
{
public class EmailContainerList : BaseSubUiPanel {
        public UILabel UnReadCountLable;
        private GameObject UnReadLable_go;
		public UIGrid ItemTable;
		public UILabel NoneItemLable;
		public GameObject FriendListItemPrefab;
		protected  List<EmaiListItem> ItemList=new List<EmaiListItem>();
		protected List<SEmailSendUint> EmailList;
        public long SelectedItemID;
		private GameObject Item_go;
		private EmailType _Type;

        private int currentPage;//当前页码
        private int maxPageCount;
        private int PageSize=10;
        private  LoopScrollView view ;

		void Awake()
		{
            NoneItemLable.SetText(LanguageTextManager.GetString("IDS_I22_9"));
            RegUIEvent();
            IsShowing = true;
            UnReadLable_go = UnReadCountLable.transform.parent.gameObject;
            view = ItemTable.GetComponent<LoopScrollView>();
            InitItem();
          
		}

        void RegUIEvent()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ItemEnd, ItemEnd);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.GetEamilList,GetEmailListHandel);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ReadEmail,ReadEmailHandel);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.DeleteEmail,DeleteEmailHandel);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.GetAllAttachment,GetAllAttachmentHandl);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.GetAttachment,GetAttachmentHandel);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.OnChangeItem, OnChangeItem);
        }
        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.GetEamilList,GetEmailListHandel);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReadEmail,ReadEmailHandel);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.DeleteEmail,DeleteEmailHandel);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.GetAllAttachment,GetAllAttachmentHandl);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.GetAttachment,GetAttachmentHandel);
        }
        private void GetAttachmentHandel(object obj)
        {
            SEmailUpdate_SC att=(SEmailUpdate_SC)obj;
            List<long> EmailIDs=new List<long>();
            EmailIDs.Add(att.dwEmailID);
           // List<Attachment> list= EmailDataManager.Instance.GetAttachmentFromLoacl(EmailIDs);
//            foreach(var p in list)
//            {
//                GoodsMessageManager.Instance.Show((int)p.GoodsID,(int)p.count);
//            }
         
            RefreshItem();
         
        }
        private void GetAllAttachmentHandl(object obj)
        {

            LoadingUI.Instance.Close();
            SEmailGetAllGoods_SC Attachment=(SEmailGetAllGoods_SC)obj;
            //List<Attachment> list= EmailDataManager.Instance.GetAttachmentFromLoacl(Attachment.mailIdList);
            //list.ApplyAllItem(p=>GoodsMessageManager.Instance.Show((int)p.GoodsID,(int)p.count));
            StartRefreshList(true);
            EmailInfoPanelManager.GetInstance().UpdateEmaiBottom();
        }
        private void DeleteEmailHandel(object obj)
        {
            LoadingUI.Instance.Close();
            MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I22_20"),1);
            EmailList=EmailDataManager.Instance.GetEmailList(_Type);
            StartRefreshList(false);
           
        }
        private void ReadEmailHandel(object obj)
        {
            SEmailRead_SC Read=(SEmailRead_SC)obj;
            RefreshItem();
            RefreshUnReadCountLable();
        }

        private void GetEmailListHandel(object obj)
        {
            LoadingUI.Instance.Close();
            if(EmailDataManager.Instance.CurrentMainStatus==EmailPageStatus.ShowEmail&&EmailDataManager.Instance.CurrentSubStatue==EmaiSubPageStatus.EmailList)
            {
                EmailInfoPanelManager.GetInstance().UpdateEmaiBottom();
            StartRefreshList(true);
            }
        }

		public override void ShowPanel ()
		{
			base.ShowPanel ();
            if(EmailDataManager.Instance.CurrentMainStatus==EmailPageStatus.ShowEmail&&EmailDataManager.Instance.CurrentSubStatue==EmaiSubPageStatus.EmailList)
            {
			  StartCoroutine(RefreshList());
            }
		}
		public void Init(EmailType type)
		{
			_Type=type;
            StartRefreshList(true);
			
		}

        void RefreshUnReadCountLable()
        {
            EmailInfoPanelManager.GetInstance().SC_EmailTabManager.UpdateUnreaderIcon();
            if (EmailList.Count>0)
            {
                UnReadCountLable.gameObject.SetActive(true);
                UnReadCountLable.text = string.Format(LanguageTextManager.GetString("IDS_I22_11"), NGUIColor.SetTxtColor(EmailDataManager.Instance.GetUnReadCount(_Type).ToString(), TextColor.ChatYellow));
            }
            else
            {
                UnReadCountLable.gameObject.SetActive(false);
            }
        }

		public void StartRefreshList(bool showReadPanel)
		{
            if (!showReadPanel)
            {
                EmailInfoPanelManager.GetInstance().ChangeEmailPageStatus(EmailPageStatus.ShowEmail,EmaiSubPageStatus.EmailList);
            }
			StartCoroutine(RefreshList());
            RefreshUnReadCountLable();
       }


		private  IEnumerator RefreshList()
		{
          
			NoneItemLable.gameObject.SetActive(false);
            UnReadLable_go.SetActive(true);
			//ItemTable.transform.ClearChild();
			//ItemList.Clear();
			InitItemFileinfoList();
            if(EmailList.Count>=CommonDefineManager.Instance.CommonDefine.MailLimit)
            {
                MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I22_10"),1);
            }
           // TraceUtil.Log(SystemModel.wanglei,"s");
            InitFirstPageData();
            ItemTable.Reposition();
            ItemTable.transform.parent.GetComponent<UIDraggablePanel>().ResetPosition();
            yield return null;
			if(EmailList.Count==0)
			{
                UnReadLable_go.SetActive(false);
				NoneItemLable.gameObject.SetActive(true);
			}
            foreach (var item in ItemList)
            {
                //引导按钮
                item.gameObject.RegisterBtnMappingId((int)item._EamilItem.wEmailType, UIType.Mail, BtnMapId_Sub.Mail_MailItem);
            }
			
		}

        void InitItem()
        {
        
            for( int i=0;i<PageSize;i++)
            {
                
                Item_go=NGUITools.AddChild(ItemTable.gameObject,FriendListItemPrefab);
                Item_go.name=FriendListItemPrefab.name+i;
                Item_go.AddComponent<UIDragPanelContents>();
                Item_go.SetActive(false);
                EmaiListItem Sc_item=Item_go.GetComponent<EmaiListItem>();
                //Sc_item.InitItemData(EmailList[i]);
                Sc_item.OnClickCallBack=ItemSelectedEventHandle;
               
                ItemList.Add(Sc_item);
                
            }

         

        }

        void ItemEnd(object obj)
        {
            if (currentPage <= 10)
            {
                //StartCoroutine("AddTestData");
            }
            else
            {
                view.AllPageEnd = true;
            }
        }

		void InitItemFileinfoList()
		{
			EmailList=EmailDataManager.Instance.GetEmailList(_Type);
		}

        #region 循环表
        
        
        void InitFirstPageData()
        {
            SelectedItemID=0;
            EmailList.Sort((x,y)=>{return (x.byState-y.byState);});
            if (ItemList != null && ItemList.Count > 0)
            {
                view.Init(gameObject);
                int count = PageSize;
                if (EmailList.Count < PageSize)
                    count = EmailList.Count;
            
                for (int i=0; i<PageSize; i++)
                {
                    ItemList [i].OnLoseFocus();
                    if (i < count)
                    {
                        ItemList [i].gameObject.SetActive(true);

                        ItemList [i].InitItemData(EmailList [i]);
                    } else
                    {
                        ItemList [i].gameObject.SetActive(false);
                    }
                }
                UpdateListView();
            }
           ItemTable.Reposition();
        }
        private void UpdateListView()
        {
            //// 刷新循环表

            view.UpdateList(EmailList.Count);
            
        }

        // 有子项被修改的通知
        private void OnChangeItem(object go)
        {

            SetItemData((GameObject)go);
        }
        
        private void SetItemData(GameObject go)
        {
            int index = int.Parse(go.name);
            if (index >= EmailList.Count) return;
            EmaiListItem Item =go.GetComponent<EmaiListItem>();
            Item.InitItemData(EmailList[index]);
            if (EmailList [index].llMailID ==SelectedItemID)
            {
                Item.OnGetFocus();
            } else
            {
            
                Item.OnLoseFocus();
            }
            
            
        }
        #endregion


		public void DeleteAllEmail()
		{
			EmailDataManager.Instance.DeleteAllEmail(_Type);
		}


        public void RefreshItem(long EmalID)
        {
            ItemList.ApplyAllItem(p=>{
                if(p._EamilItem.llMailID==EmalID)
                {
                    p.RefreshItem();
                }
            });
            for(int i=0;i<EmailList.Count;i++)
            {
                if(EmailList[i].llMailID==EmalID)
                {
                    EmailList[i]=EmailDataManager.Instance.GetEamilFromLocal(EmalID);
                }
            }
        }

        public void RefreshItem()
        {
          foreach (var item in ItemList)
            {
                if(item._EamilItem.llMailID==SelectedItemID)
                {
                    item.RefreshItemLocal();
                }
            }

            for(int i=0;i<EmailList.Count;i++)
            {
                if(EmailList[i].llMailID==SelectedItemID)
                {
                    EmailList[i]=EmailDataManager.Instance.GetEamilFromLocal(SelectedItemID);
                }
            }
        }

		public  void ItemSelectedEventHandle(EmaiListItem selectedEquipItem)
		{
            SoundManager.Instance.PlaySoundEffect("Sound_Button_MailChoice");
			//所有项LoseFocus
            //SelectedItemID
			ItemList.ApplyAllItem(p=>p.OnLoseFocus());
			selectedEquipItem.OnGetFocus();
            SelectedItemID = selectedEquipItem._EamilItem.llMailID;
            EmailDataManager.Instance.ReadEmail(selectedEquipItem._EamilItem.llMailID,selectedEquipItem._EamilItem.byEmailPage);
			EmailInfoPanelManager.GetInstance().ChangeEmailPageStatus(EmailPageStatus.ShowEmail,EmaiSubPageStatus.EmailRead);
			//EmailInfoPanelManager.GetInstance().ChangeEmailPageStatus(EmailPageStatus.ShowEmail,EmaiSubPageStatus.EmailWrite);
		}
        public override void HidePanel()
        {
            base.HidePanel();
            ItemList.ApplyAllItem(p => p.CancelInvoke());
        }
}

}