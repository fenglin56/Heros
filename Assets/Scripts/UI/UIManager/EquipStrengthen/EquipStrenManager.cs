using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;
using System.Linq;
using UI;

/// <summary>
/// 装备强化
/// 1、初始化装备列表及默认装备选择，排序
/// 2、强化及升星切换
/// 3、出现动画
/// 4、强化逻辑
/// 5、升星逻辑
/// </summary>
public class EquipStrenManager : BaseUIPanel {

    public GameObject Prefab_EquipList;
	public GameObject Prefab_EquipDetails;
	public GameObject Prefab_PackRightBtnList;  
	public SingleButtonCallBack BackBtn;
	public GameObject CommonPanelTitle_Prefab;

	public GameObject StrengthSuccessEff;
	public GameObject StrengthFailedEff;
	public GameObject StarUpgradeSuccessEff;
	public GameObject StarUpgradeFailedEff;
	public GameObject StrengthUpgradeEff;

	public PassiveSkillDataBase PassiveSkillDataBase;

	private EquipListBehaviour m_equipListBehaviour;  //当前装备列表
	private GameObject m_euiqpListInstance,m_equipDetails,m_packRightBtnList;


    private List<int> m_itemGuideID = new List<int>();
	//是否创建实例
	private bool m_createInstance,m_isStrengthBack;
	private PackBtnType m_currStrenghType;
    private BaseCommonPanelTitle m_baseCommonPanelTitle;
    private readonly string[] StrenTips=new string[5]
    {
        "IDS_I3_65",//强化成功
        "IDS_I3_66",//强化失败
        "IDS_I3_73",//铜币不足
        "IDS_I3_74",//升星石不足
        "IDS_I3_55",//强化等级不能高于自身等级
    };
    private readonly string[] StarUpgradeTips=new string[5]
    {
        "IDS_I3_67",//升星成功 
        "IDS_I3_68",//升星失败
        "IDS_I3_73",//铜币不足
        "IDS_I3_74",//升星石不足
        "IDS_I3_56",//星阶等级不能高于自身等级。
    };
    void Awake()
	{
		//返回按钮点击
		BackBtn.SetCallBackFuntion((obj)=>
		                           {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");
			this.Close();
		});
		//返回按钮按下/松开效果
		BackBtn.SetPressCallBack((isPressed)=>
		                         {
			BackBtn.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(isPressed?2:1));
		});
		m_isStrengthBack=m_createInstance=true;
		var commonPanel=NGUITools.AddChild(gameObject,CommonPanelTitle_Prefab);
		commonPanel.transform.localPosition=CommonPanelTitle_Prefab.transform.localPosition;
        m_baseCommonPanelTitle = commonPanel.GetComponent<BaseCommonPanelTitle>();
        m_baseCommonPanelTitle.Init(CommonTitleType.Money, CommonTitleType.GoldIngot);
        m_baseCommonPanelTitle.TweenShow();
		RegisterEventHandler();

        TaskGuideBtnRegister();
    }
    /// <summary>
    /// 引导按钮注入代码
    /// </summary>
    private void TaskGuideBtnRegister()
    {
        BackBtn.gameObject.RegisterBtnMappingId(UIType.EquipStrengthen, BtnMapId_Sub.EquipStrengthen_Back);
        m_baseCommonPanelTitle.LeftAddBtn.gameObject.RegisterBtnMappingId(UIType.EquipStrengthen, BtnMapId_Sub.EquipStrengthen_BuyMoney);
        m_baseCommonPanelTitle.LeftAddBtn.gameObject.RegisterBtnMappingId(UIType.EquipStrengthen, BtnMapId_Sub.EquipStrengthen_BuyIngot);
    }
	
	public override void Show(params object[] value)
	{
        PackBtnType packBtnType = PackBtnType.Strength;
        ItemFielInfo selectedItemFielInfo = null;
		if (value != null && value.Length > 0)
        {
            packBtnType = (PackBtnType)value[0];
            selectedItemFielInfo = value[1] as ItemFielInfo;
        }
        Init(selectedItemFielInfo);
        base.Show(value);
        m_equipListBehaviour.ShowAnim(packBtnType == PackBtnType.Strength);
	}
	
	public override void Close()
	{
		if (!IsShow)
			return;
		StartCoroutine(AnimToClose());
	}
	/// <summary>
	/// 播放关闭动画
	/// </summary>
	/// <returns>The to close.</returns>
	private IEnumerator AnimToClose()
	{
		m_equipListBehaviour.CloseAnim();
		yield return new WaitForSeconds(0.16f);   //动画时长
		UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
		base.Close();
	}
    /// <summary>
    /// 初始化处理，实例化（如果没有）。
	/// 初始化数据
	/// 动画
    /// </summary>
    public void Init(ItemFielInfo selectedItem)
	{
		if(m_createInstance)
		{
			m_createInstance=false;
			m_euiqpListInstance=NGUITools.AddChild(gameObject,Prefab_EquipList);
			m_euiqpListInstance.transform.localPosition=Prefab_EquipList.transform.localPosition;
			m_equipListBehaviour=m_euiqpListInstance.GetComponent<EquipListBehaviour>();

			m_equipDetails=NGUITools.AddChild(gameObject,Prefab_EquipDetails);
			m_equipDetails.transform.localPosition=Prefab_EquipDetails.transform.localPosition;
			m_equipListBehaviour.CurrrEquipDetails=m_equipDetails.GetComponent<EquipDetails>();

			m_packRightBtnList=NGUITools.AddChild(gameObject,Prefab_PackRightBtnList);
			m_packRightBtnList.transform.localPosition=Prefab_PackRightBtnList.transform.localPosition;
			m_equipListBehaviour.PackRightBtnManager=m_packRightBtnList.GetComponent<PackRightBtnManager>();            

			m_equipListBehaviour.PackRightBtnManager.PackBtnOnClick=(packBtnType)=>
			{
				//点击处理（TODO）向服务器发送强化或升星请求
				TraceUtil.Log(SystemModel.Rocky,packBtnType.ToString());
				switch(packBtnType)
				{
				case PackBtnType.Package:
                        SoundManager.Instance.PlaySoundEffect("Sound_Button_Stone_Click");
                        MainUIController.Instance.OpenMainUI(UIType.Package);
					break;
				case PackBtnType.Strength:
                        EquipStrength(packBtnType);
                        break;
				case PackBtnType.StarUpgrade:
					
                        EquipStrength(packBtnType);
                        break;
                case PackBtnType.QuickStrengthen:
                        SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Click");
                        MessageBox.Instance.Show(1,"",LanguageTextManager.GetString("IDS_I3_69"),LanguageTextManager.GetString("IDS_I3_71"),LanguageTextManager.GetString("IDS_I3_72"),()=>{ SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Confirm"); QuickEquipStrength(packBtnType);},()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");MessageBox.Instance.CloseMsgBox();});
                      
                        break;
                case PackBtnType.QuickUpgradeStar:
                        SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Click");
                        MessageBox.Instance.Show(1,"",LanguageTextManager.GetString("IDS_I3_70"),LanguageTextManager.GetString("IDS_I3_71"),LanguageTextManager.GetString("IDS_I3_72"),()=>{ SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Confirm"); QuickEquipStrength(packBtnType);},()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");MessageBox.Instance.CloseMsgBox();});
                    
                     break;

				}
			};
		}
        m_equipListBehaviour.Init(selectedItem);
		m_equipListBehaviour.ChangeStrenType(true);

        //引导代码
        StartCoroutine(RegisterRightBtn());
	}
    private IEnumerator RegisterRightBtn()
    {
        //因为右边按钮的创建，在下一帧进行位置调整，所以这里两帧后再注入引导，保证引导信息会加在正确的按钮上
        yield return null;
        yield return null;
        m_equipListBehaviour.PackRightBtnManager.RegisterGuideBtn(PackBtnType.Strength, UIType.EquipStrengthen, BtnMapId_Sub.EquipStrengthen_Strength);
        m_equipListBehaviour.PackRightBtnManager.RegisterGuideBtn(PackBtnType.StarUpgrade, UIType.EquipStrengthen, BtnMapId_Sub.EquipStrengthen_StarUp);
        m_equipListBehaviour.PackRightBtnManager.RegisterGuideBtn(PackBtnType.Package, UIType.EquipStrengthen, BtnMapId_Sub.EquipStrengthen_Package);
        m_equipListBehaviour.PackRightBtnManager.RegisterGuideBtn(PackBtnType.QuickStrengthen, UIType.EquipStrengthen, BtnMapId_Sub.EquipStrengthen_QuickStrengthen);
        m_equipListBehaviour.PackRightBtnManager.RegisterGuideBtn(PackBtnType.QuickUpgradeStar, UIType.EquipStrengthen, BtnMapId_Sub.EquipStrengthen_QuickUpgradeStar);
    }
	/// <summary>
	///    强化
	/// </summary>
	void EquipStrength(PackBtnType packBtnType)
	{
		var m_isConsumeEnough=m_equipListBehaviour.CurrrEquipDetails.m_equipStrenUpgradeProperty.EnoughToStren;
		SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Click");
        if (m_isStrengthBack)
		{
			if (m_isConsumeEnough)
			{
				ItemFielInfo selectedEquip = m_equipListBehaviour.CurrrEquipDetails.CurrItemFielInfo;
                var playerLv = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
				var wantedStrengthenLv =packBtnType==PackBtnType.Strength?
					selectedEquip.equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL + 1
						:selectedEquip.equipmentEntity.EQUIP_FIELD_START_LEVEL + 1;
                if (playerLv < wantedStrengthenLv)
                {
//					IDS_I3_55	强化等级不能高于自身等级。
//						IDS_I3_56	星阶等级不能高于自身等级。
					string tipsIDS=packBtnType==PackBtnType.Strength?"IDS_I3_55":"IDS_I3_56";
					MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString(tipsIDS),1);
                }
                else
                {
					var byStrengthType=(byte)(packBtnType==PackBtnType.Strength?Equipment_Strength_Type.EQUIPMENT_NORMAL_STRENGTH_TYPE:Equipment_Strength_Type.EQUIPMENT_START_STRENGTH_TYPE);

                    SMsgGoodsOperateEquipmentStrength sMsgGoodsOperateEquipmentStrength = new SMsgGoodsOperateEquipmentStrength();
					sMsgGoodsOperateEquipmentStrength.byStrengthType = byStrengthType;
                    sMsgGoodsOperateEquipmentStrength.dGoodsID = (uint)selectedEquip.LocalItemData._goodID;
                    sMsgGoodsOperateEquipmentStrength.uidGoods = selectedEquip.equipmentEntity.SMsg_Header.uidEntity;
                    NetServiceManager.Instance.EquipStrengthenService.SendGoodsOperateEquipmentStrengthCommand(sMsgGoodsOperateEquipmentStrength);
                    m_isStrengthBack = false;
					m_currStrenghType=packBtnType   ;              
                }
			}
			else
			{
				//Tips系统  提示  升星 "材料不足"  / 强化 到快速购买
				if(packBtnType==PackBtnType.StarUpgrade)
				{
					MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I3_54"), 1);
				}
				else
				{
//					MessageBox.Instance.Show(1,"", LanguageTextManager.GetString("IDS_I3_50"),LanguageTextManager.GetString("IDS_I3_44"),LanguageTextManager.GetString("IDS_I3_61")
//					                         ,()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");MessageBox.Instance.CloseMsgBox();}
//					,()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Confirm");
//					//TODO  打开快速购买界面
//					});
                    MessageBox.Instance.ShowNotEnoughMoneyMsg(()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");MessageBox.Instance.CloseMsgBox();});
				}
			}
		}
	}
    void QuickEquipStrength(PackBtnType packBtnType)
    {
        var m_isConsumeEnough=m_equipListBehaviour.CurrrEquipDetails.m_equipStrenUpgradeProperty.EnoughToStren;

        if (true)
        {
            if (m_isConsumeEnough)
            {
                ItemFielInfo selectedEquip = m_equipListBehaviour.CurrrEquipDetails.CurrItemFielInfo;
                var playerLv = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
                var wantedStrengthenLv =packBtnType==PackBtnType.QuickStrengthen?
                    selectedEquip.equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL + 1
                        :selectedEquip.equipmentEntity.EQUIP_FIELD_START_LEVEL + 1;
                if (playerLv < wantedStrengthenLv)
                {
                    //                  IDS_I3_55   强化等级不能高于自身等级。
                    //                      IDS_I3_56   星阶等级不能高于自身等级。
                    string tipsIDS=packBtnType==PackBtnType.QuickStrengthen?"IDS_I3_55":"IDS_I3_56";
                    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString(tipsIDS),1);
                }
                else
                {
                    var byStrengthType=(byte)((packBtnType == PackBtnType.QuickStrengthen)?Equipment_Strength_Type.EQUIPMENT_NORMAL_STRENGTH_TYPE:Equipment_Strength_Type.EQUIPMENT_START_STRENGTH_TYPE);
                    
                    SGoodsOperateQuickSmelt_CS sGoodsOperateQuickSmelt_CS = new SGoodsOperateQuickSmelt_CS();
                    sGoodsOperateQuickSmelt_CS.byStrengthType = byStrengthType;
                    sGoodsOperateQuickSmelt_CS.dGoodsID = (uint)selectedEquip.LocalItemData._goodID;
                    sGoodsOperateQuickSmelt_CS.uidGoods = selectedEquip.equipmentEntity.SMsg_Header.uidEntity;
                    NetServiceManager.Instance.EquipStrengthenService.SendGoodsOperateQuickSmeltCommand(sGoodsOperateQuickSmelt_CS);
                    m_isStrengthBack = false;
                    m_currStrenghType=packBtnType;              
                }
            }
            else
            {
                //Tips系统  提示  升星 "材料不足"  / 强化 到快速购买
                if(packBtnType==PackBtnType.QuickUpgradeStar)
                {
                    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I3_54"), 1);
                }
                else
                {
//                    MessageBox.Instance.Show(1,"", LanguageTextManager.GetString("IDS_I3_50"),LanguageTextManager.GetString("IDS_I3_44"),LanguageTextManager.GetString("IDS_I3_61")
//                                             ,()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");MessageBox.Instance.CloseMsgBox();}
//                    ,()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Confirm");
//                        //TODO  打开快速购买界面
//                    });
                    MessageBox.Instance.ShowNotEnoughMoneyMsg(()=>{SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");MessageBox.Instance.CloseMsgBox();});
                }
            }
        }
    }
	protected override void RegisterEventHandler()
	{       
		AddEventHandler(EventTypeEnum.GoodsOperateSmelt.ToString(), GoodsOperateSmeltHandle);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.QuickSmelt,QuickSmeltSuccessHandel);
		//AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
		//MainUIController.Instance.SetPanelActivEvent += SetMySelfStatus;
	}
    private void QuickSmeltSuccessHandel(object obj)
    {
        SGoodsOperateQuickSmelt_SC res=(SGoodsOperateQuickSmelt_SC)obj;
        if(res.byStrengthType==(byte)Equipment_Strength_Type.EQUIPMENT_NORMAL_STRENGTH_TYPE)
        {
        res.TipsList.ApplyAllItem(c=>GoodsMessageManager.Instance.Show(LanguageTextManager.GetString( StrenTips[(int)c-1])));
        }
        else
        {
            res.TipsList.ApplyAllItem(c=>GoodsMessageManager.Instance.Show(LanguageTextManager.GetString(StarUpgradeTips[(int)c-1])));
        }
        m_equipListBehaviour.StrenAndStarUpgradeSuccess();
        var m_isConsumeEnough = m_equipListBehaviour.CurrrEquipDetails.m_equipStrenUpgradeProperty.EnoughToStren; //重新强化消耗
        m_isStrengthBack = true;
    }
	private void GoodsOperateSmeltHandle(INotifyArgs inotifyArgs)
	{
		SMsgGoodsOperateEquipmentStrength_SC sMsgGoodsOperateEquipmentStrength_SC = (SMsgGoodsOperateEquipmentStrength_SC)inotifyArgs;

		GameObject eff=null;
		if(sMsgGoodsOperateEquipmentStrength_SC.bySucess==0)
		{
			//失败
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Equipment_Fail");
			eff=m_currStrenghType==PackBtnType.Strength?StrengthFailedEff:StarUpgradeFailedEff;
		}
		else
		{
			//成功
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Equipment_Success");
			m_equipListBehaviour.StrenAndStarUpgradeSuccess();
			eff=m_currStrenghType==PackBtnType.Strength?StrengthSuccessEff:StarUpgradeSuccessEff;
			if(m_currStrenghType==PackBtnType.StarUpgrade)
			{
				var starUpgradePos=m_equipListBehaviour.CurrrEquipDetails.m_equipItemMainProperty.StarLev.transform.position;
				StartCoroutine(GoodsOperateSmeltEff(StrengthUpgradeEff,starUpgradePos));
			}
		}
		StartCoroutine(GoodsOperateSmeltEff(eff,null));

		var byStrengthType=(Equipment_Strength_Type)sMsgGoodsOperateEquipmentStrength_SC.byStrengthType;
		TraceUtil.Log(SystemModel.Rocky,(byStrengthType==Equipment_Strength_Type.EQUIPMENT_NORMAL_STRENGTH_TYPE?"普通强化返回":"星阶升级返回")+sMsgGoodsOperateEquipmentStrength_SC.bySucess.ToString());

	}  
	private IEnumerator GoodsOperateSmeltEff(GameObject eff,Vector3? starUpgradeEff)
	{
        var m_isConsumeEnough = m_equipListBehaviour.CurrrEquipDetails.m_equipStrenUpgradeProperty.EnoughToStren; //重新升星消耗
		m_isStrengthBack = true;
		var effInstance=NGUITools.AddChild(gameObject,eff);
		if(starUpgradeEff!=null)
		   effInstance.transform.position=starUpgradeEff.Value;
		yield return new WaitForSeconds(2);
		GameObject.Destroy(effInstance);
	}

    void OnDestroy()
    {
//		RemoveEventHandler(EventTypeEnum.GoodsOperateSmelt.ToString(), GoodsOperateSmeltHandle);
//        for (int i = 0; i < m_guideBtnID.Length; i++)
//        {
//            GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
//        }
//
//        for (int i = 0; i < m_itemGuideID.Count; i++)
//        {
//            GuideBtnManager.Instance.DelGuideButton(m_itemGuideID[i]);
//        }
//        m_itemGuideID.Clear();
    }

    private void UpdateViaNotify(INotifyArgs inotifyArgs)//设置各种属性
    {       
    //    EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
    //    if (entityDataUpdateNotify.IsHero)
    //    {
    //        if (entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_ITEM)
    //        {
    //            InitEquipList();
    //        }
    //        else if (entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
    //        {
    //            this.MoneyValue.text = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY.ToString();
    //        }
    //    }
    }    

    private string GetStrengthProcBarColor(int currentLevel, int thresholdValue, out string procValue, out float rate)
    {
        string spriteName = "JH_UI_BG_1113_01";
        procValue = currentLevel.ToString();
        if (currentLevel >= thresholdValue)
        {
            //this.StrenDesc.SetActive(true);
            rate = 1;
            switch (currentLevel - thresholdValue)
            {
                case 0:
                    spriteName = "JH_UI_BG_1113_01";
                    break;
                case 1:
                    spriteName = "JH_UI_BG_1113_02";
                    break;
                case 2:
                    spriteName = "JH_UI_BG_1113_03";
                    break;
                case 3:
                    spriteName = "JH_UI_BG_1113_04";
                    break;
                case 4:
                    spriteName = "JH_UI_BG_1113_05";
                    break;
                case 5:
                    spriteName = "JH_UI_BG_1113_06";
                    break;
                default:
                    spriteName = "JH_UI_BG_1113_07";
                    break;
            }
        }
        else
        {
            //this.StrenDesc.SetActive(false);
            rate = currentLevel*1f / thresholdValue;
            procValue += "/" + thresholdValue.ToString();
        }
        return spriteName;
    }
}
