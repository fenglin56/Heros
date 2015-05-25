using UnityEngine;
using System.Collections;
using UI.Forging;
using System;

public class SelectButtonPanel : MonoBehaviour {
//    public GameObject SelectedStyleIcon;
    public SpriteSwith SpriteSwith_ForgTypeIcon,SpriteSwith_ForgTypeName;
    public ForgingSelectButton Btn_Equipment,Btn_GiftBox,Btn_Material;
    public ForgingSelectButton btn_ChoseForgingType;
    //public SelectButtonPanel selectButtonPanel;
    public GameObject SelectPanel;
    public TweenParameter PanelTweenParameter ;
    private TweenPosition tweenPosition;
    private TweenScale tweenScale;
    private bool PanelIsShowing;
    void Awake()
    {
        btn_ChoseForgingType.SetCallBackFuntion(SelectChoseForgingBtn,DeSelectChoseForgingBtn);
        Close();
    }

    public void UpdateSelectButton()
    {
        SpriteSwith_ForgTypeIcon.ChangeSprite((int)ForgingPanelManager.GetInstance().CurrentForingType);
        SpriteSwith_ForgTypeName.ChangeSprite((int)ForgingPanelManager.GetInstance().CurrentForingType);
    }
    public void SelectChoseForgingBtn(object obj)
    {
       
        ShowPanel();

    }

    public void DeSelectChoseForgingBtn(object obj)
    {
        TweenClose();
    }

    public void ShowPanel()
    {
       // SelectPanel.transform.localPosition=ShowPoint;
        bool EquipmentCanForg=false;
       foreach(var item in ForgingRecipeConfigDataManager.Instance.ForgeRecipeDataList(ForgingType.ForgEquipment))
        {
            if(ForgingRecipeConfigDataManager.Instance.IsCanForging(item))
            {
                EquipmentCanForg=true;
                break;
            }
        }
        bool GiftBoxCanForg=false;
        foreach(var item in ForgingRecipeConfigDataManager.Instance.ForgeRecipeDataList(ForgingType.ForgGiftBox))
        {
            if(ForgingRecipeConfigDataManager.Instance.IsCanForging(item))
            {
                GiftBoxCanForg=true;
                break;
            }
        }
        bool MaterialCanForg=false;
        foreach(var item in ForgingRecipeConfigDataManager.Instance.ForgeRecipeDataList(ForgingType.ForgMaterial))
        {
            if(ForgingRecipeConfigDataManager.Instance.IsCanForging(item))
            {
                MaterialCanForg=true;
                break;
            }
        }
        Btn_Equipment.SetCallBackFuntion(OnEquipmentClick,EquipmentCanForg);
        Btn_GiftBox.SetCallBackFuntion(OnGiftBoxtClick,GiftBoxCanForg);
        Btn_Material.SetCallBackFuntion(OnMaterialClick,MaterialCanForg);

        switch(ForgingPanelManager.GetInstance().CurrentForingType)
        {
        case ForgingType.ForgEquipment:
                Btn_Equipment.Select();
                break;
            case ForgingType.ForgGiftBox:
                Btn_GiftBox.Select();

                break;
            case ForgingType.ForgMaterial:
                Btn_Material.Select();
                break;
        }
        TweenShow();
        PanelIsShowing=true;
		TaskGuideBtnRegister ();
    }
	private void TaskGuideBtnRegister()
	{
		Btn_Equipment.gameObject.RegisterBtnMappingId(UI.MainUI.UIType.Forging, BtnMapId_Sub.Forging_EquipmentBtn);
		Btn_GiftBox.gameObject.RegisterBtnMappingId(UI.MainUI.UIType.Forging, BtnMapId_Sub.Forging_GiftBoxBtn);
		Btn_Material.gameObject.RegisterBtnMappingId(UI.MainUI.UIType.Forging, BtnMapId_Sub.Forging_MaterialBtn);
	}
    public void TweenShow()
    {
        if(!PanelIsShowing)
        {
            PanelIsShowing=true;
        tweenPosition= TweenPosition.Begin(SelectPanel,0.17f,PanelTweenParameter.From,PanelTweenParameter.To);
        tweenScale=TweenScale.Begin(SelectPanel,0.17f,Vector3.zero,Vector3.one,null);
        }
    }
    void Close()
    {
        SelectPanel.transform.localPosition=new Vector3(0,0,-1100);
    }
    public void TweenClose()
    {
        if(PanelIsShowing)
        {
        PanelIsShowing=false;
        tweenPosition= TweenPosition.Begin(SelectPanel,0.17f,PanelTweenParameter.To,PanelTweenParameter.From);
        tweenScale=TweenScale.Begin(SelectPanel,0.17f,Vector3.one,Vector3.zero,null);
        }
    }

    void OnEquipmentClick(object obj)
    {
     
        var perType=ForgingPanelManager.GetInstance().CurrentForingType;
        ForgingPanelManager.GetInstance().SetCurrentForingType(ForgingType.ForgEquipment);
        //Btn_Equipment.CancelSelect();
        Btn_GiftBox.CancelSelect();
        Btn_Material.CancelSelect();
        UpdateSelectButton();
        if(perType!=ForgingType.ForgEquipment)
        {
        ForgingPanelManager.GetInstance().UpdateList();
        }
        if(PanelIsShowing)
        {
            btn_ChoseForgingType.CancelSelect();
        }
    }

    void OnGiftBoxtClick(object obj)
    {
  
        var perType=ForgingPanelManager.GetInstance().CurrentForingType;
        ForgingPanelManager.GetInstance().SetCurrentForingType(ForgingType.ForgGiftBox);
        Btn_Equipment.CancelSelect();
        //Btn_GiftBox.CancelSelect();
        Btn_Material.CancelSelect();
        UpdateSelectButton();
        if(perType!=ForgingType.ForgGiftBox)
        {
            ForgingPanelManager.GetInstance().UpdateList();
        }
        if(PanelIsShowing)
        {
            btn_ChoseForgingType.CancelSelect();
        }
    }

    void OnMaterialClick(object obj)
    {
        var perType=ForgingPanelManager.GetInstance().CurrentForingType;
        ForgingPanelManager.GetInstance().SetCurrentForingType(ForgingType.ForgMaterial);
        Btn_Equipment.CancelSelect();
        Btn_GiftBox.CancelSelect();
        UpdateSelectButton();
        if(perType!=ForgingType.ForgMaterial)
        {
            ForgingPanelManager.GetInstance().UpdateList();
        }
        if(PanelIsShowing)
        {
            btn_ChoseForgingType.CancelSelect();
        }
        //Btn_Material.CancelSelect();
    }
}
[Serializable]
public class TweenParameter
{
   public Vector3 From;
   public Vector3 To;
}