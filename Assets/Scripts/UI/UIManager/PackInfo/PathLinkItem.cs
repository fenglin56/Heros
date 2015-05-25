using UnityEngine;
using System.Collections;
using UI;
using System.Linq;
using UI.MainUI;


public enum LinkType
{
    /// <summary>
    ///普通副本 
    /// </summary>
    Battle=1,
    /// <summary>
    ///该类型不可链接
    /// </summary>
    NoneLink,
    /// <summary>
    /// 系统功能
    /// </summary>
    SystemFun,
    /// <summary>
    /// 挑战副本
    /// </summary>
    CrusadeBattle,
    /// <summary>
    /// 防守副本
    /// </summary>
    DefenseBattle,

}

public class PathLinkItem : MonoBehaviour {
    public SpriteSwith Sps_Background;
    public GameObject icon_View;
    public GameObject icon_lock;
    public Transform Point_icon;
    public UILabel Lable_Name;
    public UILabel Lable_Tips;
    public SingleButtonCallBack btn_Itembtn;
    public GameObject icon_SelectIcon;
   
    private LinkConfigItemData CurrentItem=null;
    void Awake()
    {
        btn_Itembtn.SetCallBackFuntion(ItemClick);
    }
	public void Show(string linkId)
    {
        LinkConfigItemData item=PathLinkConfigManager.Instance.GetLinkConfigItem(linkId);
        CurrentItem=item;
        if(item==null)
        {
            gameObject.SetActive(false);
            TraceUtil.Log(SystemModel.Common,"没有找到linkID:"+linkId);
            return;
        }
        gameObject.SetActive(true);
        Lable_Name.SetText(LanguageTextManager.GetString( item.LinkName));
       
        ShowTips();   

    }

    void UpdateUI_View()
    {
        Lable_Tips.SetText(NGUIColor.SetTxtColor( LanguageTextManager.GetString("IDS_I1_42"),TextColor.green));
        Sps_Background.ChangeSprite(1);
        icon_View.SetActive(true);
        icon_lock.SetActive(false);
        Point_icon.ClearChild();
        CreatObjectToNGUI.InstantiateObj(CurrentItem.LinkIcon[0], Point_icon);
    }

    void UpdateUI_Lock()
    {
        Lable_Tips.SetText(NGUIColor.SetTxtColor( LanguageTextManager.GetString("IDS_H1_233"),TextColor.red));
        Sps_Background.ChangeSprite(2);
        icon_View.SetActive(false);
        icon_lock.SetActive(true);
        Point_icon.ClearChild();
        CreatObjectToNGUI.InstantiateObj(CurrentItem.LinkIcon[1], Point_icon);
    }

    void UpdateUI_NoneLink()
    {
        Lable_Tips.SetText(NGUIColor.SetTxtColor( LanguageTextManager.GetString("IDS_I1_43"),TextColor.green));
        Sps_Background.ChangeSprite(1);
        icon_View.SetActive(false);
        icon_lock.SetActive(false);
        Point_icon.ClearChild();
        CreatObjectToNGUI.InstantiateObj(CurrentItem.LinkIcon[0], Point_icon);
    }
    public void ShowTips()
    {
        icon_SelectIcon.SetActive(false);
        switch(CurrentItem.LinkType)
        {
            case LinkType.Battle:
                if(EctypeModel.Instance.IsOpenEctype(int.Parse( CurrentItem.LinkPara)))
                {
                  
                    UpdateUI_View();
                }
                else
                {
                 
                    UpdateUI_Lock();
                }
                break;
            case LinkType.CrusadeBattle:
                if(EctypeManager.Instance.IsCrusadeEctypeUnlock(int.Parse( CurrentItem.LinkPara)))
                {
                    UpdateUI_View();
                }
                  else
                {
                    UpdateUI_Lock();
                }
                break;
            case LinkType.DefenseBattle:
                if(DefenceEntryManager.DefenceEctypeEnabled(int.Parse( CurrentItem.LinkPara)))
                {
                    UpdateUI_View();
                }
                else
                {
                    UpdateUI_Lock();
                }
                break;
            case LinkType.NoneLink:
                UpdateUI_NoneLink();
                break;
            case LinkType.SystemFun:
                if(IsOpenSysFun((UIType)System.Convert.ToInt32( CurrentItem.LinkPara)))
                {
                    UpdateUI_View();
                }
                else
                {
                    UpdateUI_Lock();
                }
                break;
        }
    }

    private bool IsOpenSysFun(UIType btnType)
    {
        bool Contain=false;

        UIType[] uiType =NewUIDataManager.Instance.InitMainButtonList.SingleOrDefault(P => P.ButtonProgress == GameManager.Instance.MainButtonIndex).MainButtonList;
       
        if ( uiType.LocalContains(btnType))
        {
            Contain=true;
        }
        return Contain;
    }
   void  ItemClick(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default_Open");
        switch(CurrentItem.LinkType)
        {
            case LinkType.Battle:
                if(EctypeModel.Instance.IsOpenEctype(int.Parse( CurrentItem.LinkPara)))
                {
                    ItemInfoTipsManager.Instance.Close();
                    EctypeModel.Instance.OpenPointToEctypePanel(int.Parse( CurrentItem.LinkPara));
                }
                else
                {
                    return;
                }
                break;
		case LinkType.NoneLink:
			return;
		case LinkType.SystemFun:
			var type=(UIType)System.Convert.ToInt32( CurrentItem.LinkPara);
			if(IsOpenSysFun(type))
			{
				ItemInfoTipsManager.Instance.Close();
				//MainUIController.Instance.OpenMainUI(type);
				TaskModel.Instance.JumpTownView(type,CurrentItem.LinkChildren);
			}
			else
			{
				return;
			}
			break;
	    case LinkType.CrusadeBattle:
	        if(EctypeManager.Instance.IsCrusadeEctypeUnlock(int.Parse( CurrentItem.LinkPara)))
	        {
	            ItemInfoTipsManager.Instance.Close();
	            UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Crusade,int.Parse(CurrentItem.LinkPara));
	        }
	        else
	        {
	            return;
	        }
	            break;
	    case LinkType.DefenseBattle:
	        if(DefenceEntryManager.DefenceEctypeEnabled(int.Parse( CurrentItem.LinkPara)))
	        {
	            ItemInfoTipsManager.Instance.Close();
				UI.MainUI.MainUIController.Instance.OpenMainUI(UI.MainUI.UIType.Defence,int.Parse(CurrentItem.LinkPara));
	        }
	        else
	        {
	            return;
	        }
	        break;
        }
        icon_SelectIcon.SetActive(true);
    }
}
