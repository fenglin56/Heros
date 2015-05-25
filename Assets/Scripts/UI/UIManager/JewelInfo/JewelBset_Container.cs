using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace UI.MainUI
{
	public class JewelBset_Container : JewelContainerBase {
	public SingleButtonCallBack BesetButton;
	public SingleButtonCallBack UpgradeButton;
	public ItemFielInfo SelectItemFileInfo;
	void Awake()
	{

            BesetButton.SetCallBackFuntion(OnBesetButtonClick);
            UpgradeButton.SetCallBackFuntion(OnUpgradeButtonClick);
            TaskGuideBtnRegister();
    }
    /// <summary>
    /// 引导按钮注入代码
    /// </summary>
    private void TaskGuideBtnRegister()
    {
        BesetButton.gameObject.RegisterBtnMappingId(UIType.Gem, BtnMapId_Sub.Gem_Inset);
        UpgradeButton.gameObject.RegisterBtnMappingId(UIType.Gem, BtnMapId_Sub.Gem_Upgrade);
    }
	
        public void OnBesetButtonClick(object obj)
        {

            SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Switch");
            ChangeStrenType(true);
            JewelBesetManager.GetInstance().ChangeSubUistate(JewelState.JewelBeset);
            JewelBesetManager.GetInstance().InitRightTipsButton(JewelState.JewelBeset);
            JewelBesetManager.GetInstance().UpdateBesetPanel(null);
            JewelBesetManager.GetInstance().InitBeset_Attribute(null,null);
            ItemFielInfo defultSelectItem=obj as ItemFielInfo;
            //var item= ItemList.Where(c=>c.ItemFielInfo==defultSelectItem).FirstOrDefault();
            if(defultSelectItem!=null )
            {
                UpdateContain(JewelState.JewelBeset,defultSelectItem);
            }
            else
            {
                InitItemList(JewelState.JewelBeset);
            }
            JewelBesetManager.GetInstance().HideEff();

          
        }

        public void OnUpgradeButtonClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Switch");
            ChangeStrenType(false);
            JewelBesetManager.GetInstance().ChangeSubUistate(JewelState.JewelUpgrad);
            JewelBesetManager.GetInstance().InitRightTipsButton(JewelState.JewelUpgrad);
            JewelBesetManager.GetInstance().UpdateUpgradePanel(null);
            //StartCoroutine(RefreshList(JewelState.JewelUpgrad));

            ItemFielInfo defultSelectItem=obj as ItemFielInfo;
            //var item= ItemList.Where(c=>c.ItemFielInfo==defultSelectItem).FirstOrDefault();
            if(defultSelectItem!=null )
             {
                UpdateContain(JewelState.JewelUpgrad,defultSelectItem);
             }
            else
            {
                InitItemList(JewelState.JewelUpgrad);
            }
            JewelBesetManager.GetInstance().HideEff();

        }

		public void UpdateContain(JewelState tab,ItemFielInfo itemfileInfo)
		{
            if(tab==JewelState.JewelBeset)
            {
			ChangeStrenType(true);
            }
            else
            {
                ChangeStrenType(false);
            }
			JewelBesetManager.GetInstance().InitContianerTab(tab);
            JewelBesetManager.GetInstance().UpdateBesetPanel(null);
			//StartCoroutine( RefreshList(tab));
            InitItemList(tab);
			SelectItem(itemfileInfo);
		}
        private void InitItemList(JewelState jewelState)
        {
            StartCoroutine(RefreshList(jewelState, BtnMapId_Sub.Gem_JewelContainerListPanel_Item));           
        }
        public void Init(JewelState tab,object defultSelectItem)
		{
           if(tab==JewelState.JewelBeset)
            {
                OnBesetButtonClick(defultSelectItem);
            }
            else if(tab==JewelState.JewelUpgrad)
            {
                OnUpgradeButtonClick(defultSelectItem);
            }
			//SelectItem(itemfileInfo);
		}
	void ChangeStrenType(bool IsChangeToBeset)
	{
			BesetButton.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(!IsChangeToBeset?1:2));
			UpgradeButton.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(!IsChangeToBeset?2:1));
	}
	
		/// <summary>
		/// 装备项选择处理
		/// </summary>
		/// <param name="selectedEquipItem">Selected equip item.</param>
		public override void  ItemSelectedEventHandle(JewelBset_ContainerItem selectedEquipItem)
		{
			base.ItemSelectedEventHandle(selectedEquipItem);
            SelectItemFileInfo=selectedEquipItem.ItemFielInfo;
			if(selectedEquipItem.ItemFielInfo.severItemFielType==SeverItemFielInfoType.Equid)
			{
				JewelBesetManager.GetInstance().UpdateBesetPanel(selectedEquipItem.ItemFielInfo);
			}
			else
			{
				//JewelBesetManager.GetInstance().SelectedJewel=selectedEquipItem.ItemFielInfo;
				JewelBesetManager.GetInstance().UpdateUpgradePanel(selectedEquipItem.ItemFielInfo);
			}

            if(selectedEquipItem!=null&&selectedEquipItem.ItemFielInfo!=null)
            {
                    JewelBesetManager.GetInstance().InitBeset_Attribute(selectedEquipItem.ItemFielInfo,selectedEquipItem);
            }
			//调用EquipDetails的方法刷新当前选择装备的详细信息
//			CurrrEquipDetails.Init(selectedEquipItem.ItemFielInfo,IsNormalStren);
//			m_selectedItemInfo=selectedEquipItem.ItemFielInfo;
		}

		public override void InitItemFileinfoList(JewelState tab)
		{
		  
			if(tab==JewelState.JewelBeset)
			{
				
				//第一个判断是“是装备”，第二个判断是“有一个孔是激活的”
                ItemFileinfoList=ContainerInfomanager.Instance.GetEquiptItemList().Where(c=>c.LocalItemData._GoodsSubClass!=2).ToList();


				if(ItemFileinfoList.Count==0)
				{
					ShowNoneItemLable("IDS_I9_15");
				}
				else
				{
					HideNoneItemLable();
				}

                ItemFileinfoList.Sort((c1,c2)=>{
                    return c2.LocalItemData._ColorLevel*1000000+c2.LocalItemData._AllowLevel*10000-c1.LocalItemData._ColorLevel*1000000+c1.LocalItemData._AllowLevel*10000;
                });
			}
			else
			{
				ItemFileinfoList=ContainerInfomanager.Instance.GetPackItemList().Where(p=>p.severItemFielType==SeverItemFielInfoType.Jewel).ToList();
			
				if(ItemFileinfoList.Count==0)
				{
					ShowNoneItemLable("IDS_I9_21");
				}
				else
				{
					HideNoneItemLable();
				}
                ItemFileinfoList.Sort((c1,c2)=>{
                    return GetJewelSortValue(c2)-GetJewelSortValue(c1);
                });
			}

           
		}
        int GetJewelSortValue(ItemFielInfo item)
        {
         
            int value=item.LocalItemData._ColorLevel*1000000+item.materiel.ESTORE_FIELD_LEVEL*10000+item.materiel.ESTORE_FIELD_EXP;
            return value;
        }

 }
}
