using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;

public class EquipmentUpgradePanelManager :BaseUIPanel {
    public SingleButtonCallBack Btn_back;
    public SingleButtonCallBack Btn_pack,Btn_Strength,Btn_StarUp,Btn_Upgrade;
    public UpgradeTabPanel TabPanel ;
    public ResultPanel resultPanel;
    public GameObject StrengthSuccessEff;
    public GameObject StrengthFailedEff;
    public GameObject StarUpgradeSuccessEff;
    public GameObject StarUpgradeFailedEff;
    public GameObject UpgradeSuccessEff;
    public Transform Point_Eff;
	public GameObject CommonPanelTitle_Prefab;
    private bool m_isStrengthBack=true;

    private static EquipmentUpgradePanelManager instance;
    public static EquipmentUpgradePanelManager GetInstance ()
    {
        if (!instance) {
            instance = (EquipmentUpgradePanelManager)GameObject.FindObjectOfType (typeof(EquipmentUpgradePanelManager));
            if (!instance)
                Debug.LogError ("没有附加JewelBesetManager脚本的gameobject在场景中");
        }
        return instance;
    }
    #region mono
    void Awake()
    {
        Btn_back.SetCallBackFuntion(Onclick_Back);
        Btn_pack.SetCallBackFuntion(Onclick_pack);
        Btn_Strength.SetCallBackFuntion(Onclick_Strength);
        Btn_StarUp.SetCallBackFuntion(Onclick_StarUp);
        Btn_Upgrade.SetCallBackFuntion(Onclick_Upgrade);
        RegisterEventHandler();
		TaskGuideBtnRegister ();
		var commonPanel = NGUITools.AddChild(gameObject, CommonPanelTitle_Prefab);
		commonPanel.transform.localPosition = CommonPanelTitle_Prefab.transform.localPosition;
		commonPanel.GetComponent<CommonPanelTitle>().TweenShow();
    }
    #endregion

    #region override
	public override void Show(params object[] value)
    {
        base.Show(value);
        m_isStrengthBack=true;
        UpgradeType TargetType=UpgradeType.Strength;
        EquipmentUpgradeDataManger.Instance.CurrentSelectEquip=null;
        ItemFielInfo itemfileInfo=null;
        if(value.Length==1)
        {
         TargetType= (UpgradeType)value[0];
       
        }
        else if(value.Length==2)
        {
            TargetType= (UpgradeType)value[0];
            itemfileInfo=value[1] as ItemFielInfo;
        }

        TabPanel.Init(TargetType,itemfileInfo);

    }

    public override void Close()
    {
        base.Close();
    }

    #endregion

    #region function
    public void UpdateResultPanel()
    {
        UpdateFunctionBtn(EquipmentUpgradeDataManger.Instance.CurrentType);
        resultPanel.UpdateResultPane();
    }
    public void UpdateListPanel()
    {
        TabPanel.UpdateContaierPanel();
    }
    public void RefreshEachListItem()
    {
        TabPanel.containerPanel.RefreshEachListItem();
        TabPanel. CheckFlag();
    }
    public void UpdateFunctionBtn(UpgradeType type)
    {
        switch(type)
        {
            case UpgradeType.Strength:
                Btn_Strength.gameObject.SetActive(true);
                Btn_StarUp.gameObject.SetActive(false);
                Btn_Upgrade.gameObject.SetActive(false);
                break;
            case UpgradeType.StarUp:
                Btn_Strength.gameObject.SetActive(false);
                Btn_StarUp.gameObject.SetActive(true);
                Btn_Upgrade.gameObject.SetActive(false);
                break;
            case UpgradeType.Upgrade:
                Btn_Strength.gameObject.SetActive(false);
                Btn_StarUp.gameObject.SetActive(false);
                Btn_Upgrade.gameObject.SetActive(true);
                break;
        }
    }

    void UpgradeEquipment()
    {
        ItemFielInfo itemfileInfo=EquipmentUpgradeDataManger.Instance.CurrentSelectEquip;
        int itemId;
       if(!ContainerInfomanager.Instance.EnoughUpgradeMaterial(itemfileInfo,out itemId))//材料不足
        {
            MessageBox.Instance.ShowTips(3,string.Format(LanguageTextManager.GetString("IDS_I10_7"),LanguageTextManager.GetString(ItemDataManager.Instance.GetItemData(itemId)._szGoodsName)),1);
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Upgrade_Warning");
        }else if(!ContainerInfomanager.Instance.UpgradeLevelEnough(itemfileInfo.LocalItemData._goodID))//等级不足
        {
            MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I10_8"),1);
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Upgrade_Warning");
        }else
        {
            NetServiceManager.Instance.EquipStrengthenService.SendEquipmentLevelUp(EquipmentUpgradeDataManger.Instance.CurrentSelectEquip.sSyncContainerGoods_SC.uidGoods);
        }
 
    }

    /// <summary>
    ///    强化/升星
    /// </summary>
    void EquipStrength()
    {
        int  ItemId;
        var m_isConsumeEnough=ContainerInfomanager.Instance.HasEnoughMaterial( EquipmentUpgradeDataManger.Instance.CurrentType,EquipmentUpgradeDataManger.Instance.CurrentSelectEquip,out ItemId);
        //SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Click");
        if (m_isStrengthBack)
        {
            if (m_isConsumeEnough)
            {
                ItemFielInfo selectedEquip = EquipmentUpgradeDataManger.Instance.CurrentSelectEquip;
                var playerLv = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
                int wantedStrengthenLv ;
                if(EquipmentUpgradeDataManger.Instance.CurrentType==UpgradeType.Strength)
                {
                    wantedStrengthenLv= PlayerDataManager.Instance.GetEquipmentStrengthLevel((EquiptSlotType)selectedEquip.sSyncContainerGoods_SC.nPlace) + 1;
                }
                else
                {
                    wantedStrengthenLv=PlayerDataManager.Instance.GetEquipmentStarLevel((EquiptSlotType)selectedEquip.sSyncContainerGoods_SC.nPlace) + 1;
                }
                if (playerLv < wantedStrengthenLv)
                {
                    //                  IDS_I3_55   强化等级不能高于自身等级。
                    //                      IDS_I3_56   星阶等级不能高于自身等级。
                    string tipsIDS=EquipmentUpgradeDataManger.Instance.CurrentType==UpgradeType.Strength?"IDS_I3_55":"IDS_I3_56";
                    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString(tipsIDS),1);
                }
                else
                {
                    byte byStrengthType;
                    if(EquipmentUpgradeDataManger.Instance.CurrentType==UpgradeType.Strength)
                    {
                        byStrengthType=(byte)Equipment_Strength_Type.EQUIPMENT_NORMAL_STRENGTH_TYPE;
                    }
                    else
                    {
                        byStrengthType= (byte)Equipment_Strength_Type.EQUIPMENT_START_STRENGTH_TYPE;
                    }
                    SMsgGoodsOperateEquipmentStrength sMsgGoodsOperateEquipmentStrength = new SMsgGoodsOperateEquipmentStrength();
                    sMsgGoodsOperateEquipmentStrength.byStrengthType = byStrengthType;
                    sMsgGoodsOperateEquipmentStrength.dGoodsID = (uint)selectedEquip.LocalItemData._goodID;
                    sMsgGoodsOperateEquipmentStrength.uidGoods = selectedEquip.equipmentEntity.SMsg_Header.uidEntity;
                    NetServiceManager.Instance.EquipStrengthenService.SendGoodsOperateEquipmentStrengthCommand(sMsgGoodsOperateEquipmentStrength);
                    m_isStrengthBack = false;
                    //  m_currStrenghType=packBtnType   ;              
                }
            }
            else
            {
                //Tips系统  提示  升星 "材料不足"  / 强化 到快速购买
                if(EquipmentUpgradeDataManger.Instance.CurrentType==UpgradeType.StarUp)
                {
                    MessageBox.Instance.ShowTips(3, string.Format(LanguageTextManager.GetString("IDS_I10_7"),LanguageTextManager.GetString(ItemDataManager.Instance.GetItemData(ItemId)._szGoodsName)), 1);
                }
                else
                {   
                    MessageBox.Instance.ShowNotEnoughMoneyMsg(()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");MessageBox.Instance.CloseMsgBox();});
                }
            }
        }
    }
    #endregion
    #region ButtonCallBack
    void Onclick_Back(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");
        Close();
    }
    
    void Onclick_pack(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");
        MainUIController.Instance.OpenMainUI(UIType.Package);
    }
    
    void Onclick_Strength(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Click");
		int currentlevel=PlayerDataManager.Instance.GetEquipmentStrengthLevel((EquiptSlotType)EquipmentUpgradeDataManger.Instance.CurrentSelectEquip.sSyncContainerGoods_SC.nPlace);
		if(currentlevel<CommonDefineManager.Instance.CommonDefine.StrengthLimit)
		{
        EquipStrength();
		}
		else
		{
			MessageBox.Instance.ShowTips(1,LanguageTextManager.GetString("IDS_I3_97"),1.5f);
		}
    }
    void Onclick_StarUp(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Click");
		int currentlevel=PlayerDataManager.Instance.GetEquipmentStarLevel((EquiptSlotType)EquipmentUpgradeDataManger.Instance.CurrentSelectEquip.sSyncContainerGoods_SC.nPlace);
		if(currentlevel<CommonDefineManager.Instance.CommonDefine.StartStrengthLimit)
		{
        EquipStrength();
		}
		else
		{
			MessageBox.Instance.ShowTips(1,LanguageTextManager.GetString("IDS_I3_98"),1.5f);
		}
    }
    void Onclick_Upgrade(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Click");
        UpgradeEquipment();
    }

    #endregion
   

    #region event
    void UpdatePanel(object obj)
    {

        //long newEquipmentID = (long)obj;
        SMsgGoodsOperateEquipLevelUp_SC msg = (SMsgGoodsOperateEquipLevelUp_SC)obj;
        if(msg.bySucess ==1)//升级成功
        {
           ContainerInfomanager.Instance.CheckShowEquipmentEff();
            TraceUtil.Log(SystemModel.wanglei,"装备升级成功");
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Equipment_Success");
            EquipmentUpgradeDataManger.Instance.NewItemUID=msg.NewItemUID;
            UpdateListPanel();
            var eff=UpgradeSuccessEff;
            Point_Eff.ClearChild();
            StartCoroutine(GoodsOperateSmeltEff(eff,null));
        }
    }
    /// <summary>
    /// 升星强化成功
    /// </summary>
    /// <param name="inotifyArgs">Inotify arguments.</param>
    private void GoodsOperateSmeltHandle(INotifyArgs inotifyArgs)
    {
        SMsgGoodsOperateEquipmentStrength_SC sMsgGoodsOperateEquipmentStrength_SC = (SMsgGoodsOperateEquipmentStrength_SC)inotifyArgs;
        m_isStrengthBack=true;
        GameObject eff=null;
        if(sMsgGoodsOperateEquipmentStrength_SC.bySucess==0)
        {
            //失败
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Equipment_Fail");
             if(EquipmentUpgradeDataManger.Instance.CurrentType==UpgradeType.Strength)
            {
                eff=StrengthFailedEff;
            }
            else
            {
                eff=StarUpgradeFailedEff;
            }
        }
        else
        {
            ContainerInfomanager.Instance.CheckShowEquipmentEff();
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Equipment_Success");
            if(EquipmentUpgradeDataManger.Instance.CurrentType==UpgradeType.Strength)
            {
                eff=StrengthSuccessEff;
            }
            else
            {
                eff=StarUpgradeSuccessEff;
            }
            RefreshEachListItem();
        }
        eff.SetActive(true);
        var byStrengthType=(Equipment_Strength_Type)sMsgGoodsOperateEquipmentStrength_SC.byStrengthType;
        Point_Eff.ClearChild();
        StartCoroutine(GoodsOperateSmeltEff(eff,null));
        TraceUtil.Log(SystemModel.Rocky,(byStrengthType==Equipment_Strength_Type.EQUIPMENT_NORMAL_STRENGTH_TYPE?"普通强化返回":"星阶升级返回")+sMsgGoodsOperateEquipmentStrength_SC.bySucess.ToString());
        
    }  
    private IEnumerator GoodsOperateSmeltEff(GameObject eff,Vector3? starUpgradeEff)
    {
       // var m_isConsumeEnough = m_equipListBehaviour.CurrrEquipDetails.m_equipStrenUpgradeProperty.EnoughToStren; //重新升星消耗
        m_isStrengthBack = true;
        var effInstance=UI.CreatObjectToNGUI.InstantiateObj(eff,Point_Eff);
//        if(starUpgradeEff!=null)
//            effInstance.transform.position=starUpgradeEff.Value;
        yield return new WaitForSeconds(2);
        GameObject.Destroy(effInstance);
    }
    protected override void RegisterEventHandler()
    {       
        AddEventHandler(EventTypeEnum.GoodsOperateSmelt.ToString(), GoodsOperateSmeltHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.EqipmentLevelUp,UpdatePanel);
//        AddEventHandler(EventTypeEnum.PlayerHoldMoneyUpdate.ToString(), OnPlayerLvOrMoneyUpdate);
       // UIEventManager.Instance.RegisterUIEvent(UIEventType.QuickSmelt,QuickSmeltSuccessHandel);
        //AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
        //MainUIController.Instance.SetPanelActivEvent += SetMySelfStatus;
    }
//    void OnPlayerLvOrMoneyUpdate(INotifyArgs agrs)
//    {
//        IEntityDataStruct data = (IEntityDataStruct)(agrs);
//        if(data.SMsg_Header.IsHero)
//        {
//            UpdateListPanel();
//        }
//    }

    void OnDestroy()
    {
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.EqipmentLevelUp,UpdatePanel);
        RemoveEventHandler(EventTypeEnum.GoodsOperateSmelt.ToString(), GoodsOperateSmeltHandle);
      //  RemoveEventHandler(EventTypeEnum.PlayerHoldMoneyUpdate.ToString(), OnPlayerLvOrMoneyUpdate);
    }
	/// <summary>
	/// 引导按钮注入代码
	/// </summary>
	private void TaskGuideBtnRegister()
	{
		Btn_back.gameObject.RegisterBtnMappingId(UIType.EquipmentUpgrade, BtnMapId_Sub.EquipmentUpgrade_Back);
		Btn_pack.gameObject.RegisterBtnMappingId(UIType.EquipmentUpgrade, BtnMapId_Sub.EquipmentUpgrade_PackBtn);
		Btn_Strength.gameObject.RegisterBtnMappingId(UIType.EquipmentUpgrade, BtnMapId_Sub.EquipmentUpgrade_StrenBtn);
		Btn_StarUp.gameObject.RegisterBtnMappingId(UIType.EquipmentUpgrade, BtnMapId_Sub.EquipmentUpgrade_StarBtn);
		Btn_Upgrade.gameObject.RegisterBtnMappingId(UIType.EquipmentUpgrade, BtnMapId_Sub.EquipmentUpgrade_UpgradeBtn);
	}
    #endregion 
}
