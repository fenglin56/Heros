using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Linq;
public class UpgradeTabPanel : MonoBehaviour {
    public SingleButtonCallBack Tab_Strength,Tab_StarUP,Tab_Upgrade;
    public ContainerPanel containerPanel;
    public UILabel Tips;
    public GameObject StrenthFlag,StarUpFlag,UpgradeFlag;
    void Awake()
    {
        Tab_Strength.SetCallBackFuntion(OnStrengthTabClick);
        Tab_StarUP.SetCallBackFuntion(OnStarUptabClick);
        Tab_Upgrade.SetCallBackFuntion(OnUpgradeTabClick);

		TaskGuideBtnRegister ();
    }

   
    public void UpdateContaierPanel()
    {
        containerPanel.UpdateListPanel();
        CheckFlag();
    }
    public void Init(UpgradeType type,ItemFielInfo itemfileInfo)
    {
        if(ContainerInfomanager.Instance.CheckPackBtnIsEnable(PackBtnType.Strength))
        {
            Tab_Strength.gameObject.SetActive(true);
        }
        else
        {
            Tab_Strength.gameObject.SetActive(false);
        }
        if(ContainerInfomanager.Instance.CheckPackBtnIsEnable(PackBtnType.StarUpgrade))
        {
            Tab_StarUP.gameObject.SetActive(true);
        }
        else
        {
            Tab_StarUP.gameObject.SetActive(false);
        }
        if(ContainerInfomanager.Instance.CheckPackBtnIsEnable(PackBtnType.Upgrade))
        {
            Tab_Upgrade.gameObject.SetActive(true);
        }
        else
        {
            Tab_Upgrade.gameObject.SetActive(false);
        }
        CheckFlag();
        switch(type)
        {
            case UpgradeType.Strength:
                ChoseStengthTab(itemfileInfo);
                break;
            case UpgradeType.StarUp:
                ChoseStarUpTab(itemfileInfo);
                break;
            case UpgradeType.Upgrade:
                ChoseUpgradeTab(itemfileInfo);
                break;
        }
     
    }
    public void CheckFlag()
    {
        StrenthFlag.SetActive(false);
        StarUpFlag.SetActive(false);
        UpgradeFlag.SetActive(false);
        foreach(var item in  ContainerInfomanager.Instance.GetEquiptItemList())
        {
            if(ContainerInfomanager.Instance.EquipmentCanUp(UpgradeType.Strength,item))
            {
                StrenthFlag.SetActive(true);
                break;
            }
          
       
        }
        foreach(var item in  ContainerInfomanager.Instance.GetEquiptItemList())
        {
            if(ContainerInfomanager.Instance.EquipmentCanUp(UpgradeType.StarUp,item))
            {
                StarUpFlag.SetActive(true);
                break;
            }
            
        }
        foreach(var item in  ContainerInfomanager.Instance.GetAllEquipment())
        {
            if(ContainerInfomanager.Instance.EquipmentCanUp(UpgradeType.Upgrade,item))
            {
                UpgradeFlag.SetActive(true);
                break;
            }
        }
    }
    void OnStrengthTabClick(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Switch");
        ChoseStengthTab(null);
    }
    void ChoseStengthTab(ItemFielInfo itemfileInfo)
    {
        if(itemfileInfo!=null)
        {
        EquipmentUpgradeDataManger.Instance.CurrentSelectEquip=ContainerInfomanager.Instance.GetEquiptItemList().SingleOrDefault(c=>c.sSyncContainerGoods_SC.nPlace==itemfileInfo.sSyncContainerGoods_SC.nPlace);
        
        }
        else
        {
            EquipmentUpgradeDataManger.Instance.CurrentSelectEquip=null;
        }
        EquipmentUpgradeDataManger.Instance.CurrentType=UpgradeType.Strength;
        //EquipmentUpgradeDataManger.Instance.CurrentSelectEquip=null;
        Tab_Strength.spriteSwithList.ApplyAllItem(c=>c.ChangeSprite(2));
        Tab_StarUP.spriteSwithList.ApplyAllItem(c=>c.ChangeSprite(1));
        Tab_Upgrade.spriteSwithList.ApplyAllItem(c=>c.ChangeSprite(1));
        containerPanel.UpdateListPanel();
        Tips.SetText(LanguageTextManager.GetString("IDS_I3_90"));
    }
    void OnStarUptabClick(object obj)
    {

        SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Switch");
        ChoseStarUpTab(null);
      
    }
    void ChoseStarUpTab(ItemFielInfo itemfileInfo)
    {
        if(itemfileInfo!=null)
        {
        EquipmentUpgradeDataManger.Instance.CurrentSelectEquip=ContainerInfomanager.Instance.GetEquiptItemList().SingleOrDefault(c=>c.sSyncContainerGoods_SC.nPlace==itemfileInfo.sSyncContainerGoods_SC.nPlace);
        
        }
        else
        {
            EquipmentUpgradeDataManger.Instance.CurrentSelectEquip=null;
        }
        EquipmentUpgradeDataManger.Instance.CurrentType=UpgradeType.StarUp;
        //EquipmentUpgradeDataManger.Instance.CurrentSelectEquip=null;
        Tab_Strength.spriteSwithList.ApplyAllItem(c=>c.ChangeSprite(1));
        Tab_StarUP.spriteSwithList.ApplyAllItem(c=>c.ChangeSprite(2));
        Tab_Upgrade.spriteSwithList.ApplyAllItem(c=>c.ChangeSprite(1));
        containerPanel.UpdateListPanel();
        Tips.SetText(LanguageTextManager.GetString("IDS_I3_91"));

    }
    void OnUpgradeTabClick(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Switch");

        ChoseUpgradeTab(null);
    }
    void ChoseUpgradeTab(ItemFielInfo itemFileInfo)
    {
        if(itemFileInfo!=null)
        {
        EquipmentUpgradeDataManger.Instance.CurrentSelectEquip=ContainerInfomanager.Instance.GetItemFileInfoBuyUID(itemFileInfo.sSyncContainerGoods_SC.uidGoods);
        }
        else
        {
            EquipmentUpgradeDataManger.Instance.CurrentSelectEquip=null;
        }
        EquipmentUpgradeDataManger.Instance.CurrentType=UpgradeType.Upgrade;
        //EquipmentUpgradeDataManger.Instance.CurrentSelectEquip=null;
        Tab_Strength.spriteSwithList.ApplyAllItem(c=>c.ChangeSprite(1));
        Tab_StarUP.spriteSwithList.ApplyAllItem(c=>c.ChangeSprite(1));
        Tab_Upgrade.spriteSwithList.ApplyAllItem(c=>c.ChangeSprite(2));
        containerPanel.UpdateListPanel();
        Tips.SetText(LanguageTextManager.GetString("IDS_I3_92"));
    }
	/// <summary>
	/// 引导按钮注入代码
	/// </summary>
	private void TaskGuideBtnRegister()
	{
		Tab_Strength.gameObject.RegisterBtnMappingId(UIType.EquipmentUpgrade, BtnMapId_Sub.EquipmentUpgrade_TabStren);
		Tab_StarUP.gameObject.RegisterBtnMappingId(UIType.EquipmentUpgrade, BtnMapId_Sub.EquipmentUpgrade_TabStarUpgrade);
		Tab_Upgrade.gameObject.RegisterBtnMappingId(UIType.EquipmentUpgrade, BtnMapId_Sub.EquipmentUpgrade_TabUpgrade);
	}
}
