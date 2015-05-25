using UnityEngine;
using System.Collections;
using UI.MainUI;
public class EquipIconItem : MonoBehaviour {
    public GameObject Icon_IsEquip;
    public SpriteSwith Sps_Background;
    public Transform Point_Icon;
    public SingleButtonCallBack ClickItem;
    private int m_Goodsid=-1;
    void Awake()
    {
		if(ClickItem!=null)
		{
       	 ClickItem.SetCallBackFuntion(OnItemClick);
		}
    }
    void OnItemClick(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Choice");
        if(m_Goodsid!=-1)
        {
        ItemInfoTipsManager.Instance.Show(m_Goodsid);

        }
        else
        {
            Debug.LogError("id=null");
        }
    }
    public void Init(ItemFielInfo itemfileInfo)
    {
        Point_Icon.transform.ClearChild();
        if(Icon_IsEquip!=null)
        {
        Icon_IsEquip.SetActive(isShowEquipIcon(itemfileInfo));
        }
        Sps_Background.ChangeSprite(itemfileInfo.LocalItemData._ColorLevel+2);
        UI.CreatObjectToNGUI.InstantiateObj(itemfileInfo.LocalItemData._picPrefab,Point_Icon);
        m_Goodsid=itemfileInfo.LocalItemData._goodID;
    }
    public bool isShowEquipIcon(ItemFielInfo itemfileIfon)
    {
        switch(EquipmentUpgradeDataManger.Instance.CurrentType)
        {
            case UpgradeType.Strength:
                return true;
            case UpgradeType.StarUp:
                return true;
            case UpgradeType.Upgrade: 
                return   ContainerInfomanager.Instance.IsItemEquipped(itemfileIfon);
            default:
                return false;
        }
    }
}
