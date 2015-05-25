using UnityEngine;
using System.Collections;
using System.Linq;
/// <summary>
/// 管理技能装备及升级，挂在Prefab_SkillManager预设件上
/// </summary>
using UI.MainUI;


public class SkillManagerBehaviour : BaseUIPanel {

	public GameObject Prefab_SkillList;
	public GameObject Prefab_AssemblySkill;
	public GameObject Prefab_ViewSkill; 
	public SingleButtonCallBack BackBtn;
	public GameObject CommonPanelTitle_Prefab;

	public SingleSkillInfoList SingleSkillInfoList;
	private SkillListBehaviour SkillList;
	private AssemblySkillPanel AssemblySkill;
	private ViewSkillPanel ViewSkill;
	private GameObject m_skillListInstance,m_skillAssemblyInstance,m_viewSkillInstance;

	private BaseCommonPanelTitle m_commonPanelTitle;
	//是否创建实例
	private bool m_createInstance,m_isUpgradeBack;
	// Use this for initialization
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
		m_isUpgradeBack=m_createInstance=true;
		var commonPanel=NGUITools.AddChild(gameObject,CommonPanelTitle_Prefab);
		commonPanel.transform.localPosition=CommonPanelTitle_Prefab.transform.localPosition;
		m_commonPanelTitle=commonPanel.GetComponent<BaseCommonPanelTitle>();
		m_commonPanelTitle.Init(CommonTitleType.Money,CommonTitleType.GoldIngot);
		RegisterEventHandler();

        //TaskGuideBtnRegister();
    }
    /// <summary>
    /// 引导按钮注入代码
    /// </summary>
    private void TaskGuideBtnRegister()
    {
//        BackBtn.gameObject.RegisterBtnMappingId(UIType.Skill, BtnMapId_Sub.Skill_Back);
//        m_commonPanelTitle.LeftAddBtn.gameObject.RegisterBtnMappingId(UIType.Skill, BtnMapId_Sub.Skill_BuyMoney);
//        m_commonPanelTitle.RightAddBtn.gameObject.RegisterBtnMappingId(UIType.Skill, BtnMapId_Sub.Skill_BuyIngot);
    }

	public IEnumerator Init()
	{
		SingleSkillInfoList=new SingleSkillInfoList();
		if(m_createInstance)
		{
			m_createInstance=false;
			m_skillListInstance=NGUITools.AddChild(gameObject,Prefab_SkillList);
			m_skillListInstance.transform.localPosition=Prefab_SkillList.transform.localPosition;
			SkillList=m_skillListInstance.GetComponent<SkillListBehaviour>();
			
			m_skillAssemblyInstance=NGUITools.AddChild(gameObject,Prefab_AssemblySkill);
			m_skillAssemblyInstance.transform.localPosition=Prefab_AssemblySkill.transform.localPosition;
			AssemblySkill=m_skillAssemblyInstance.GetComponent<AssemblySkillPanel>();
			
			m_viewSkillInstance=NGUITools.AddChild(gameObject,Prefab_ViewSkill);
			m_viewSkillInstance.transform.localPosition=Prefab_ViewSkill.transform.localPosition;
			ViewSkill=m_viewSkillInstance.GetComponent<ViewSkillPanel>();
			
			SkillList.OnItemClick=(skillItem)=>
			                       {
				//通知ViewSkillPanel更新
				//通知AssemblySkillPanel待装备处理
				ViewSkill.InitViewInfo(skillItem.ItemFielInfo);
				AssemblySkill.SkillInListBeSelected(skillItem);
			};
			AssemblySkill.EquipItemClickAct=(skillsItem,index)=>
			{
				if(SkillList.CurrentSelectedItem!=null)  //技能列表栏有选中技能的处理
				{
					if(SkillList.CurrentSelectedItem.ItemFielInfo!=skillsItem.ItemFielInfo)
					{
						//提交装备请求
						SetSkillEquipInfoToSever(skillsItem.ItemFielInfo,SkillList.CurrentSelectedItem.ItemFielInfo,index);
					}
					else
					{
						SkillList.CurrentSelectedItem=null;
						SkillList.FocusSkillItem(skillsItem.ItemFielInfo);
					}
				}
				else   //技能列表栏没有选中技能的处理
				{
					if(skillsItem.ItemFielInfo!=null)
					{
						ViewSkill.InitViewInfo(skillsItem.ItemFielInfo);
						SkillList.FocusSkillItem(skillsItem.ItemFielInfo);
						AssemblySkill.SkillInListBeSelected(skillsItem);
					}
				}
			};
			ViewSkill.SkillUpgradeAct=(skillInfo)=>
			{
				NetServiceManager.Instance.EntityService.SendSkillUpgrade((byte)skillInfo.localSkillData.m_skillId);
			};
		}
		yield return null;
		AssemblySkill.Init(SingleSkillInfoList.EquipSkillsList);
		SkillList.Init(SingleSkillInfoList);
	}
	void SetSkillEquipInfoToSever(SingleSkillInfo RemovesingleSkillInfo, SingleSkillInfo AddsingleSkillInfo,int index)
	{
		SkillEquipEntity skillEquipEntity = new SkillEquipEntity();
		skillEquipEntity.Skills = new System.Collections.Generic.Dictionary<byte, ushort>();

		SingleSkillInfo[] equipSkillsList = SingleSkillInfoList.EquipSkillsList;

		int addPreIndex=-1;
		for (int i = 0; i < equipSkillsList.Length; i++)
		{
			if (AddsingleSkillInfo == equipSkillsList[i])
			{
				addPreIndex=i;
			}
		}
		//替换与被替换都不为空。由两个技能位置互换位置
		if(addPreIndex!=-1)
		{
			equipSkillsList[addPreIndex]=RemovesingleSkillInfo;
		}
		equipSkillsList[index]=AddsingleSkillInfo;

		for (int i = 0; i < equipSkillsList.Length; i++)
		{
			if (equipSkillsList[i] != null)
			{
				skillEquipEntity.Skills.Add((byte)i, (byte)equipSkillsList[i].localSkillData.m_skillId);
			}
			else
			{
				skillEquipEntity.Skills.Add((byte)i, 0);
			}
		}
		skillEquipEntity.Skills.ApplyAllItem(P=>
		                                     {
			TraceUtil.Log(SystemModel.Rocky,"Send:"+P.Key+":"+P.Value);
		});
		NetServiceManager.Instance.EntityService.SendSkillEquip(skillEquipEntity);
	}
	protected override void RegisterEventHandler()
	{
        AddEventHandler(EventTypeEnum.EquipSkill.ToString(), EquipSkill);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.UpgrateSkillInfo, SkillUpgrade);

	}
    void OnDestroy()
    {
        RemoveEventHandler(EventTypeEnum.EquipSkill.ToString(), EquipSkill);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.UpgrateSkillInfo, SkillUpgrade);
    }
    private void EquipSkill(INotifyArgs obj)
    {
        //技能装备完成。
        SingleSkillInfoList.SetEquipSkills();
        //刷新装备栏
        AssemblySkill.SkillEquipSuucess(SingleSkillInfoList.EquipSkillsList, SkillList.CurrentSelectedItem);
        SkillList.FocusSkillItem(null);
        SkillList.CurrentSelectedItem = null;
    }
    private void SkillUpgrade(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_SkillUpgrade");
        SSkillInfo sSkillInfo = (SSkillInfo)obj;
        var upgradeSkillInfo = SingleSkillInfoList.singleSkillInfoList.Single(P => P.localSkillData.m_skillId == sSkillInfo.wSkillID);
        upgradeSkillInfo.SkillLevel = sSkillInfo.wSkillLV;
        AssemblySkill.UpgradeSkillItem(upgradeSkillInfo);
        SkillList.UpgradeSkillItem(upgradeSkillInfo);
        ViewSkill.InitViewInfo(upgradeSkillInfo);        
    }
	public override void Show(params object[] value)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_SkillAppear");
		StartCoroutine(Init());
		base.Show(value);		
		SkillList.ShowAnim();
		AssemblySkill.ShowAnim();
		ViewSkill.ShowAnim();

		m_commonPanelTitle.TweenShow();
	}
	
	public override void Close()
	{
		if (!IsShow)
			return;
		//		UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
		//		base.Close();
        CleanUpUIStatus();
        SkillList.CurrentSelectedItem = null;
		StartCoroutine(AnimToClose());
		m_commonPanelTitle.tweenClose();
	}
	/// <summary>
	/// 播放关闭动画
	/// </summary>
	/// <returns>The to close.</returns>
	private IEnumerator AnimToClose()
	{
		SkillList.CloseAnim();
		AssemblySkill.CloseAnim();
		ViewSkill.CloseAnim();
		yield return new WaitForSeconds(0.16f);   //动画时长
		UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
		base.Close();
	}
}
