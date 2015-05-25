using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.MainUI;


public class ResultPanel : MonoBehaviour {
    public StrengthPanel strengthPanel;
    public StarUppanel starUpPanel;
    public UpgradePanel upgradePanel;
    public AttPanel attPanel;
    public MaterialItem LF_MaterialItem,MD_MaterialItem,RG_MaterialItem;
	public GameObject Title;
	public UILabel lable_None;

	void setMaterialPanelNone ()
	{
		MD_MaterialItem.gameObject.SetActive (false);
		LF_MaterialItem.gameObject.SetActive (false);
		RG_MaterialItem.gameObject.SetActive (false);
		Title.SetActive (false);
		lable_None.gameObject.SetActive (true);
	}

	public void UpdateResultPane()
    {
		lable_None.gameObject.SetActive(false);
		Title.gameObject.SetActive(true);
        if(EquipmentUpgradeDataManger.Instance.CurrentSelectEquip!=null)
        {
            List<UpgradeRequire> UpgradeRequires=new List<UpgradeRequire>();
            attPanel.gameObject.SetActive(true);
			EquiptType type=(EquiptType)EquipmentUpgradeDataManger.Instance.CurrentSelectEquip.LocalItemData._GoodsSubClass;
            switch(EquipmentUpgradeDataManger.Instance.CurrentType)
            {
			case UpgradeType.Strength:	                    
						strengthPanel.gameObject.SetActive(true);
						strengthPanel.Init(EquipmentUpgradeDataManger.Instance.CurrentSelectEquip);
						starUpPanel.gameObject.SetActive(false);
						upgradePanel.gameObject.SetActive(false);

						int currentlevel=PlayerDataManager.Instance.GetEquipmentStrengthLevel((EquiptSlotType)EquipmentUpgradeDataManger.Instance.CurrentSelectEquip.sSyncContainerGoods_SC.nPlace);
						if(currentlevel<CommonDefineManager.Instance.CommonDefine.StrengthLimit)
						{

						   
						UpgradeRequires=PlayerDataManager.Instance.GetStrengCost(type,currentlevel);
						}
						else
						{
							lable_None.SetText(LanguageTextManager.GetString("IDS_I3_97"));
							setMaterialPanelNone ();
						}
                    break;
                case UpgradeType.StarUp:
				
					strengthPanel.gameObject.SetActive(false);
					starUpPanel.gameObject.SetActive(true);
					starUpPanel.Init(EquipmentUpgradeDataManger.Instance.CurrentSelectEquip);
					upgradePanel.gameObject.SetActive(false);
					int currentstarlevel=PlayerDataManager.Instance.GetEquipmentStarLevel((EquiptSlotType)EquipmentUpgradeDataManger.Instance.CurrentSelectEquip.sSyncContainerGoods_SC.nPlace);
					if(currentstarlevel<CommonDefineManager.Instance.CommonDefine.StartStrengthLimit)
					{	
						UpgradeRequires=PlayerDataManager.Instance.GetStarUpCost(type,currentstarlevel);
					}
					else
					{
						lable_None.SetText(LanguageTextManager.GetString("IDS_I3_98"));
						setMaterialPanelNone ();
					}
                    break;
                case UpgradeType.Upgrade:
                    strengthPanel.gameObject.SetActive(false);
                    starUpPanel.gameObject.SetActive(false);
                    upgradePanel.gameObject.SetActive(true);
                    upgradePanel.Init(EquipmentUpgradeDataManger.Instance.CurrentSelectEquip);
                    UpgradeRequires=ContainerInfomanager.Instance.GetUpgradeRequire(EquipmentUpgradeDataManger.Instance.CurrentSelectEquip.LocalItemData as EquipmentData);
                    break;

            }
			 if(UpgradeRequires.Count==1)
            {
                MD_MaterialItem.gameObject.SetActive(true);
                LF_MaterialItem.gameObject.SetActive(false);
                RG_MaterialItem.gameObject.SetActive(false);
                MD_MaterialItem.Init(UpgradeRequires[0]);
            }
			else if(UpgradeRequires.Count==2)
            {
                MD_MaterialItem.gameObject.SetActive(false);
                LF_MaterialItem.gameObject.SetActive(true);
                RG_MaterialItem.gameObject.SetActive(true);
                LF_MaterialItem.Init(UpgradeRequires[0]);
                RG_MaterialItem.Init(UpgradeRequires[1]);
            }else
			{
				MD_MaterialItem.gameObject.SetActive(false);
				LF_MaterialItem.gameObject.SetActive(false);
				RG_MaterialItem.gameObject.SetActive(false);
			}

            attPanel.Init(EquipmentUpgradeDataManger.Instance.CurrentSelectEquip,EquipmentUpgradeDataManger.Instance.CurrentType);

        }
        else{
            switch(EquipmentUpgradeDataManger.Instance.CurrentType)
            {
                case UpgradeType.Strength:
                    UI.MessageBox.Instance.ShowTips(1,LanguageTextManager.GetString("IDS_I3_93"),1f);
                    break;
                case UpgradeType.StarUp:
                    UI.MessageBox.Instance.ShowTips(1,LanguageTextManager.GetString("IDS_I3_94"),1f);
                    break;
                case UpgradeType.Upgrade:
                    UI.MessageBox.Instance.ShowTips(1,LanguageTextManager.GetString("IDS_I3_95"),1f);
                    break;
            }
            strengthPanel.gameObject.SetActive(false);
            starUpPanel.gameObject.SetActive(false);
            upgradePanel.gameObject.SetActive(false);
            MD_MaterialItem.gameObject.SetActive(false);
            LF_MaterialItem.gameObject.SetActive(false);
            RG_MaterialItem.gameObject.SetActive(false);
            attPanel.gameObject.SetActive(false);
        }
    }
}
