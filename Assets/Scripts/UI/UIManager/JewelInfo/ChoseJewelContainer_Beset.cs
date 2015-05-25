using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;


namespace UI.MainUI
{
public class ChoseJewelContainer_Beset : JewelContainerBase {
		public SingleButtonCallBack Button_back;
		public SingleButtonCallBack Button_Beset;
		private long EquipID;
		private long StoneID;
		private int place;
		private ItemFielInfo CurrentItemFileInfo;
		private JewelBset_ContainerItem SelectedStone;
		void Awake()
		{
			RegisterEventHandler();
			Button_back.SetCallBackFuntion(c=>{
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Cancel");
				JewelBesetManager.GetInstance().ChangeSubUistate(JewelState.JewelBeset);
                JewelBesetManager.GetInstance().UPdateContain(JewelState.JewelBeset,CurrentItemFileInfo);
                JewelBesetManager.GetInstance().InitBeset_Attribute(null,null);
			});
			Button_Beset.SetCallBackFuntion(c=>{
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Confirm");
                Button_Beset.SetMyButtonActive(false);
				byte p=System.Convert.ToByte(place);
				NetServiceManager.Instance.EquipStrengthenService.SendRequestGoodsOperateBesetCommmand(EquipID,StoneID,p);
				//JewelBesetManager.GetInstance().besetQueue.Enqueue(place);
			});
            TaskGuideBtnRegister();
        }
        /// <summary>
        /// ������ťע�����?
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            Button_back.gameObject.RegisterBtnMappingId(UIType.Gem, BtnMapId_Sub.Gem_ChoseJewelContainerListPanel_Back);
            Button_Beset.gameObject.RegisterBtnMappingId(UIType.Gem, BtnMapId_Sub.Gem_ChoseJewelContainerListPanel_Inset);
        }
		public  void Init(ItemFielInfo SelectEuqItemfielInfo,int Place)
		{
            Button_Beset.SetMyButtonActive(true);
			CurrentItemFileInfo=SelectEuqItemfielInfo;
			EquipID=SelectEuqItemfielInfo.sSyncContainerGoods_SC.uidGoods;
			this.place=Place ;
			StartCoroutine(RefreshList(JewelState.JewelBeset));
            //����
            if (ItemList != null)
            {
                ItemList.ApplyAllItem(P =>
                    {
                        if (P != null)
                        {
                            P.gameObject.RegisterBtnMappingId(P.ItemFielInfo.LocalItemData._goodID, UIType.Gem, BtnMapId_Sub.Gem_ChoseJewelContainerListPanel_Item
                                , NoticeToDragSlerp, P.DragAmount);
                    }
                });
            }
            if (m_noticeToDragAmount != 0)
            {
                StartCoroutine(DragAmountSlerp(m_noticeToDragAmount));
                m_noticeToDragAmount = 0;
            }
		}


		public override void ItemSelectedEventHandle(JewelBset_ContainerItem selectedEquipItem)
		{
			base.ItemSelectedEventHandle(selectedEquipItem);
			StoneID=selectedEquipItem.ItemFielInfo.sSyncContainerGoods_SC.uidGoods;
			SelectedStone=selectedEquipItem;
            if(selectedEquipItem!=null&&selectedEquipItem.ItemFielInfo!=null)
            {

                    JewelBesetManager.GetInstance().InitBeset_Attribute(selectedEquipItem.ItemFielInfo,selectedEquipItem);

            }
		}
		public override void InitItemFileinfoList(JewelState tab)
		{
            EquipmentData eqitem=ItemDataManager.Instance.GetItemData(CurrentItemFileInfo.LocalItemData._goodID)as EquipmentData;
            string pos=eqitem._GoodsSubClass.ToString();
            //&&((Jewel)ItemDataManager.Instance.GetItemData(c.LocalItemData._goodID)).StonePosition.Contains(eqitem._GoodsSubClass.ToString())
            ItemFileinfoList=new List<ItemFielInfo>();
            List<JewelInfo> jewelInfos=PlayerDataManager.Instance.GetJewelInfo((EquiptSlotType)CurrentItemFileInfo.sSyncContainerGoods_SC.nPlace);
            foreach(var item in ContainerInfomanager.Instance.itemFielArrayInfo)
            {
                if(item.severItemFielType==SeverItemFielInfoType.Jewel)
                {
                    Jewel jewel=(Jewel)ItemDataManager.Instance.GetItemData(item.LocalItemData._goodID);
                    bool inpos=false;
                    foreach(var s in jewel.StonePosition)
                    {
                        if(s==pos&&(jewel._goodID!=jewelInfos[0].JewelID&&jewel._goodID!=jewelInfos[1].JewelID))
                        {
                            inpos=true;
                            break;
                        }
                    }
                    if(inpos)
                    {
                        ItemFileinfoList.Add(item);
                    }
                }
            }
            ItemFileinfoList.Sort((c1,c2)=>{
                return GetJewelSortValue(c2)-GetJewelSortValue(c1);
            });
			if(ItemFileinfoList.Count==0)
			{
				ShowNoneItemLable("IDS_I9_21");
			}
			else
			{
				HideNoneItemLable();
			}
		}

        int GetJewelSortValue(ItemFielInfo item)
        {
            
            int value=item.LocalItemData._ColorLevel*1000000+item.materiel.ESTORE_FIELD_LEVEL*10000+item.materiel.ESTORE_FIELD_EXP;
            return value;
        }
		IEnumerator  ShowBesetEff()
		{

		
			Vector3 Pos;
			if(place==1)
			{
				Pos=JewelBesetManager.GetInstance().GetFirsHolePos();
			}
			else
			{
				Pos=JewelBesetManager.GetInstance().GetSecondHolePos();
			}
			yield return StartCoroutine(SelectedStone.ShowBesetEff(Pos));

		}
		IEnumerator HideChosePanel()
		{
			yield return new WaitForSeconds(1);
			JewelBesetManager.GetInstance().ChangeSubUistate(JewelState.JewelBeset);
			yield return new WaitForSeconds(1f);
			JewelBesetManager.GetInstance().UpdateBesetPanel(CurrentItemFileInfo);
		}
		public void ReceiveBesetJewelHandel(object arg)
		{
			SMsgGoodsOperateBeset_SC s= (SMsgGoodsOperateBeset_SC)arg ;
			if(System.Convert.ToBoolean( s.bySucess))
			{
                ContainerInfomanager.Instance.ShowJewelEff();
                SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Stone_Inlay");
				StartCoroutine(ShowBesetEff());
				StartCoroutine(HideChosePanel());
			}
		}

        void OnDestroy()
        {
            DeregisterEventHandler();
        }
        void DeregisterEventHandler()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReceiveBesetJewel,ReceiveBesetJewelHandel);
        }
		protected override void RegisterEventHandler()
		{
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveBesetJewel,ReceiveBesetJewelHandel);
			//UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveRemoveJewel,ReceiveRemoveJewelHandel);
		}
}
}