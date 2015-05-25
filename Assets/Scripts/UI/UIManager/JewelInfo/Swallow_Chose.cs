using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
namespace UI.MainUI{
public class Swallow_Chose : JewelContainerBase {
	public UILabel LevelUPNeedExp;
	public UILabel SelectedJewelTotalExp;
	public SingleButtonCallBack Button_back;
	public SingleButtonCallBack Button_Beset;
	private ItemFielInfo _itemFielInfo;
    private int TotalExp;
		//private List<ItemFielInfo> SelectedItemFileInfoList=new List<ItemFielInfo>();
	void Awake()
		{
			Button_back.SetCallBackFuntion(c=>
			                               {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Click");
				JewelBesetManager.GetInstance().ChangeSubUistate(JewelState.JewelUpgrad);
                SelectedItemList.Clear();
			});
			Button_Beset.SetCallBackFuntion(c=>
			                               {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Click");
				JewelBesetManager.GetInstance().UpdateChodeJewelList_swallow_ensure(_itemFielInfo,SelectedItemList);
				JewelBesetManager.GetInstance().ChangeSubUistate(JewelState.JewelChose_Upgrade_ensure);
				
			});

            TaskGuideBtnRegister();
        }
    /// <summary>
    /// 引导按钮注入代码
    /// </summary>
    private void TaskGuideBtnRegister()
    {
        Button_back.gameObject.RegisterBtnMappingId(UIType.Gem, BtnMapId_Sub.Gem_SwallowContainerListPanel_Back);
        Button_Beset.gameObject.RegisterBtnMappingId(UIType.Gem, BtnMapId_Sub.Gem_SwallowContainerListPanel_Swallow);
    }
    public void Init(ItemFielInfo itemfileInfo)
    {
       
        TotalExp = 0;
            SelectedJewelTotalExp.SetText(TotalExp);
        UpdateSwallowButton();
        this._itemFielInfo = itemfileInfo;
        Jewel jewel = ItemDataManager.Instance.GetItemData(itemfileInfo.LocalItemData._goodID) as Jewel;
        int level = itemfileInfo.materiel.ESTORE_FIELD_LEVEL;

        LevelUPNeedExp.SetText(jewel.StoneExp[level-1] - itemfileInfo.materiel.ESTORE_FIELD_EXP);
        StartCoroutine(RefreshList(JewelState.jewelChose_Upgrade));
        //引导
        if (ItemList != null)
        {
            ItemList.ApplyAllItem(P =>
            {
                if (P != null)
                {
                    P.gameObject.RegisterBtnMappingId(P.ItemFielInfo.LocalItemData._goodID, UIType.Gem, BtnMapId_Sub.Gem_SwallowContainerListPanel_Item
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
			int cexp=0;
			Jewel jewel=ItemDataManager.Instance.GetItemData(selectedEquipItem.ItemFielInfo.LocalItemData._goodID) as Jewel;
			for(int i=0;i<selectedEquipItem.ItemFielInfo.materiel.ESTORE_FIELD_LEVEL;i++)
			{
                int lexp=0;
                for(int j=0;j<selectedEquipItem.ItemFielInfo.materiel.ESTORE_FIELD_LEVEL-1;j++)
                {
                    lexp+=jewel.StoneExp[j];
                }
                cexp+=lexp+selectedEquipItem.ItemFielInfo.materiel.ESTORE_FIELD_EXP;
			}

			float  rate=((float)jewel.StoneExpRate/1000.0f);
			cexp=System.Convert.ToInt32( cexp*rate);
			if(SelectedItemList.Contains(selectedEquipItem))
			{
				selectedEquipItem.OnLoseFocus();
				SelectedItemList.Remove(selectedEquipItem);
				TotalExp-=cexp;
				//SelectedItemFileInfoList.Remove(selectedEquipItem.ItemFielInfo);
			}
			else
			{
		    TotalExp+=cexp;
			selectedEquipItem.OnGetFocus(true);
			SelectedItemList.Add(selectedEquipItem);
			//SelectedItemFileInfoList.Add(selectedEquipItem.ItemFielInfo);
			}
			SelectedJewelTotalExp.SetText(TotalExp);
			UpdateSwallowButton();
			//JewelBesetManager.GetInstance().InitBeset_Attribute(selectedEquipItem.ItemFielInfo);
			//StoreID=selectedEquipItem.ItemFielInfo.LocalItemData._goodID;
		}
	public override void InitItemFileinfoList(JewelState tab)
		{

            SelectedItemList.Clear();
            UpdateSwallowButton();
			List<ItemFielInfo> i=ContainerInfomanager.Instance.itemFielArrayInfo.Where(p=>p.severItemFielType==SeverItemFielInfoType.Jewel).ToList();

			ItemFileinfoList=new List<ItemFielInfo>();
			i.ApplyAllItem(c=>
			               {
				Jewel ljewel= ItemDataManager.Instance.GetItemData(c.LocalItemData._goodID) as Jewel;
				Jewel wjewel=ItemDataManager.Instance.GetItemData(_itemFielInfo.LocalItemData._goodID) as Jewel;
				if(ljewel.StoneType==wjewel.StoneType&&c!=this._itemFielInfo)
				{
					ItemFileinfoList.Add(c);
				}
			});
			if(ItemFileinfoList.Count==0)
			{
                ShowNoneItemLable("IDS_I9_31");
			}
			else
			{
				HideNoneItemLable();
			}

		}
		public void UpdateSwallowButton()
		{
			if(SelectedItemList==null||SelectedItemList.Count==0)
			{
                Button_Beset.SetMyButtonActive(false);
                Button_Beset.BackgroundSwithList.ApplyAllItem(p=>p.ChangeSprite(3));
			}
			else
			{
                Button_Beset.SetMyButtonActive(true);
                Button_Beset.BackgroundSwithList.ApplyAllItem(p=>p.ChangeSprite(1));
			}
		}
}
}