using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI{
public class Swallow_Ensure : JewelContainerBase {
		public SingleButtonCallBack Button_back;
		public SingleButtonCallBack Button_Beset;
		private List<JewelBset_ContainerItem> SelectItemList;
		private ItemFielInfo _itemfileinfo;
		private long[] IDs;
        public UILabel TipsLable;
		void Awake()
		{
            TipsLable.SetText(LanguageTextManager.GetString("IDS_I9_28"));
			RegisterEventHandler();
			Button_back.SetCallBackFuntion(c=>
			                               {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Click");
				JewelBesetManager.GetInstance().ChangeSubUistate(JewelState.jewelChose_Upgrade);
			});
			Button_Beset.SetCallBackFuntion(c=>
			                                {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Click");
				if(IDs.Length>0)
				{
					byte length= System.Convert.ToByte(IDs.Length);
					NetServiceManager.Instance.EquipStrengthenService.SendRequestGoodsOperateSwallowCommand(_itemfileinfo.sSyncContainerGoods_SC.uidGoods,length,IDs);
					Button_Beset.SetMyButtonActive(false);
				}

			});
            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            Button_back.gameObject.RegisterBtnMappingId(UIType.Gem, BtnMapId_Sub.Gem_SwallowConfirmContainerListPanel_Back);
            Button_Beset.gameObject.RegisterBtnMappingId(UIType.Gem, BtnMapId_Sub.Gem_SwallowConfirmContainerListPanel_Swallow);
        }
		public void Init(ItemFielInfo itemfielInfo,List<JewelBset_ContainerItem> itemFileInfoList )
		{
			Button_Beset.SetMyButtonActive(true);
			_itemfileinfo=itemfielInfo;
		//	SelectItemList.Clear();
			SelectItemList=itemFileInfoList;
			IDs=new long[itemFileInfoList.Count];

			for(int i=0;i<itemFileInfoList.Count;i++)
			{
				IDs[i]=itemFileInfoList[i].ItemFielInfo.sSyncContainerGoods_SC.uidGoods;
			}
			StartCoroutine(RefreshList(JewelState.JewelChose_Upgrade_ensure));
		}
		public override void ItemSelectedEventHandle(JewelBset_ContainerItem selectedEquipItem)
		{
			//确定选择没有点击效果

		}
		public override void InitItemFileinfoList(JewelState tab)
		{

			ItemFileinfoList=new List<ItemFielInfo>();
			SelectItemList.ApplyAllItem(c=>{ItemFileinfoList.Add(c.ItemFielInfo);});
			if(ItemFileinfoList.Count==0)
			{
				ShowNoneItemLable("IDS_I9_21");
			}
			else
			{
				HideNoneItemLable();
			}
		}
		IEnumerator  ShowBesetEff(JewelBset_ContainerItem SelectedStone)
		{
			Vector3 Pos;
			Pos=JewelBesetManager.GetInstance().GetUpdagradeHolePos();
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Stone_Upgrade");
			yield return StartCoroutine(SelectedStone.ShowSwallowEff(Pos));
			
		}
		IEnumerator  ChangTab()
		{
			yield return null;
			JewelBesetManager.GetInstance().ChangeSubUistate(JewelState.JewelUpgrad);
			JewelBesetManager.GetInstance().UPdateContain(JewelState.jewelChose_Upgrade,_itemfileinfo);
            JewelBesetManager.GetInstance().InitBeset_Attribute(null,null);
			//yield return StartCoroutine(SelectedStone.ShowBesetEff(Pos));
			
		}
		public void ReceiveBesetJewelHandel(object arg)
		{
			SMsgGoodsOperateSwallow_SC s= (SMsgGoodsOperateSwallow_SC)arg ;
			if(System.Convert.ToBoolean( s.bySucess))
			{
				foreach(JewelBset_ContainerItem item in ItemList)
				{
			
				StartCoroutine(ShowBesetEff(item));
				StartCoroutine(ChangTab());
			
				}
		
			}
		}
        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReceiveSwallowJewel,ReceiveBesetJewelHandel);
        }
		void RegisterEventHandler()
		{
			UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveSwallowJewel,ReceiveBesetJewelHandel);
			//UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveRemoveJewel,ReceiveRemoveJewelHandel);
		}
}

}