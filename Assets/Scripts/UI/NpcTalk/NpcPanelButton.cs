using UnityEngine;
using System.Collections;

public class NpcPanelButton : SingleButtonCallBack {

    public UISprite ShopIcon;
    public UISprite ShopTitle;

    private int m_guideBtnID = 0;

    void Start()
    {
        GuideBtnManager.Instance.RegGuideButton(this.gameObject, UI.MainUI.UIType.Empty, SubType.NpcButton, out m_guideBtnID); //TODO 81
    }

    public void OnDestroy()
    {
        GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);//TODO 81
    }

    public void InitButton(string ShopIconRes, string TitleRes)
    {
        ShopIcon.enabled =ShopIconRes == ""? false:true;
        ShopTitle.enabled = TitleRes == "" ? false : true;
        this.ShopIcon.spriteName = ShopIconRes;
        this.ShopTitle.spriteName = TitleRes;
    }
}
