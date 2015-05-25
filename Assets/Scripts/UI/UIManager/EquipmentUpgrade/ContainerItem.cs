using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;
using System;


public class ContainerItem : MonoBehaviour {
    public EquipIconItem equipItem;
    public UILabel Lable_Name;
    public UILabel Lable_position;
    public UILabel Lable_Force;
    public UILabel lable_Level;
    public GameObject Icon_CanUp;
    public  ItemFielInfo m_itemFileInfo{get;private set;}
    public GameObject Icon_selcet;
    private Action<ContainerItem>  selectedCallBack;
    private readonly string[] StarLeveNameDic=new string[]{"IDS_I3_78","IDS_I3_79","IDS_I3_80","IDS_I3_81","IDS_I3_82","IDS_I3_83","IDS_I3_84"};
    void Awake()
    {
        GetComponent<UIEventListener>().onClick=OnItemClick;
        
    }
    public void Init(ItemFielInfo itemfileInfo)
    {
        m_itemFileInfo=itemfileInfo;
        RefreshItem();
    }

    public void RefreshItem()
    {
        equipItem.Init(m_itemFileInfo);
        Lable_Name.SetText(NGUIColor.SetTxtColor( LanguageTextManager.GetString( m_itemFileInfo.LocalItemData._szGoodsName),(TextColor)m_itemFileInfo.LocalItemData._ColorLevel));
        Lable_position.SetText(NGUIColor.SetTxtColor(EquipmentUpgradeDataManger.Instance.PositionDic[m_itemFileInfo.LocalItemData._GoodsSubClass],TextColor.green));
        Lable_Force.SetText((int)EquipItem.GetEquipForce(m_itemFileInfo));
        Icon_CanUp.SetActive(ContainerInfomanager.Instance.EquipmentCanUp(EquipmentUpgradeDataManger.Instance.CurrentType,m_itemFileInfo));
        SetItemLevel(EquipmentUpgradeDataManger.Instance.CurrentType,m_itemFileInfo);
    }
    public void  SetItemLevel( UpgradeType type,ItemFielInfo itemfileInfo)
    {
        string text="";
        EquiptSlotType place=(EquiptSlotType)itemfileInfo.sSyncContainerGoods_SC.nPlace;
        switch(type)
        {
            case UpgradeType.Strength:
                text=string.Format(LanguageTextManager.GetString("IDS_I3_85"),PlayerDataManager.Instance.GetEquipmentStrengthLevel(place));
                text=UI.NGUIColor.SetTxtColor(text,TextColor.yellow);
                break;
            case UpgradeType.StarUp:
                  int level=PlayerDataManager.Instance.GetEquipmentStarLevel(place);
                  int temp=level-1;
                int par=temp/10;
                int star=(temp%10)+1;
                if(level>0)
                {
                    text=string.Format(LanguageTextManager.GetString(StarLeveNameDic[par]),star);
                }


                break;
            case UpgradeType.Upgrade:
                text=string.Format(LanguageTextManager.GetString("IDS_I3_86"),itemfileInfo.LocalItemData._AllowLevel);
                text=UI.NGUIColor.SetTxtColor(text,TextColor.yellow);
                break;
        }
        lable_Level.SetText(text);
    }
    void OnItemClick(GameObject go)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Choice");
        SelectItem();
    }
    public void SelectItem()
    {
        if(selectedCallBack!=null)
        {
            selectedCallBack(this);
        }
    }

    public void OnFouce()
    {
        Icon_selcet.gameObject.SetActive(true);
    }

    public void LoseFouce()
    {
        Icon_selcet.gameObject.SetActive(false);
    }


    public void SetSelcetCallBack(Action<ContainerItem> callback)
    {
        selectedCallBack=callback;
    }
	
}
