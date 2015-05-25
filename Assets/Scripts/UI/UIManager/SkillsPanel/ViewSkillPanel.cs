using UnityEngine;
using System.Collections;
using System;
using UI.MainUI;

public class ViewSkillPanel : View {
    
	public UILabel SkillName;
	public UILabel BreakLev;
	public UILabel SkillLev;
	public UILabel SkillDesc;
	public UILabel SkillPower;
	public UILabel SkillHitNum;
	public UILabel SkillCDTime;
	public UILabel SkillHurt;
	public UILabel UpgradeConsume;

    #region IDS
    public UILabel SkillLevIDS;
    #endregion

    public GameObject EffPoint;
	public GameObject FullLevEff;
	public GameObject UpgradeBtnEff;
	public GameObject Prefab_SkillBreakDesc;

	public UILabel NotReachUpLev;
	public SingleButtonCallBack SkillUpgradeBtn;
	public SingleButtonCallBack SkillBreakDescBtn;
	public Action<SingleSkillInfo> SkillUpgradeAct;

	private SingleSkillInfo m_itemFielInfo;
	private bool m_enough,m_reachUpLev;
	private SkillBreakLevInfoBehaviour m_skillBreakDesc;
	private TweenPosition m_tweenPosComponent;
	//private bool m_skillUpgrading=false;
	// Use this for initialization
	void Awake()
	{
        SkillLevIDS.text = LanguageTextManager.GetString("IDS_I7_9");
		m_tweenPosComponent=GetComponent<TweenPosition>();
        SkillUpgradeBtn.SetCallBackFuntion(UpgradeSkill);
        SkillBreakDescBtn.SetPressCallBack(SkillBreakDesc);
        RegisterEventHandler();
        //TaskGuideBtnRegister();
    }
    private void SkillBreakDesc(bool isPressed)
    {
        if (isPressed)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_SkillInterruptLevel");
            if (m_skillBreakDesc == null)
            {
                var skillBreakDesc = NGUITools.AddChild(transform.parent.gameObject, Prefab_SkillBreakDesc);
                skillBreakDesc.transform.localPosition = Prefab_SkillBreakDesc.transform.localPosition;
                m_skillBreakDesc = skillBreakDesc.GetComponent<SkillBreakLevInfoBehaviour>();
            }
            m_skillBreakDesc.Show(true);
        }
        else
        {
            m_skillBreakDesc.Show(false);
        }
    }
    private void UpgradeSkill(object obj)
    {
        int upNeedLev;
        int skillManaValue = m_itemFielInfo.UpgradeConsume(out m_enough, out m_reachUpLev, out upNeedLev);
        if (!m_enough)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_SkillNotEnoughMoney");
            UI.MessageBox.Instance.ShowNotEnoughMoneyMsg(null);
        }
        else
        {
            if (SkillUpgradeAct != null)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_SkillUpgrade");
                SkillUpgradeAct(m_itemFielInfo);
            }
        }
    }
    /// <summary>
    /// 引导按钮注入代码
    /// </summary>
    private void TaskGuideBtnRegister()
    {
//        SkillBreakDescBtn.gameObject.RegisterBtnMappingId(UIType.Skill, BtnMapId_Sub.Skill_ViewBreakLevDesc);
//        SkillUpgradeBtn.gameObject.RegisterBtnMappingId(UIType.Skill, BtnMapId_Sub.Skill_Upgrade);
    }

	/// <summary>
	///  打开界面，飞入动画
	/// </summary>
	public void ShowAnim()
	{
		m_tweenPosComponent.Play(true);
	}
	public void CloseAnim()
	{
		m_tweenPosComponent.Play(false);
	}
    public void InitViewInfo(SingleSkillInfo itemFielInfo)
    {
		m_itemFielInfo=itemFielInfo;
		bool isNull=itemFielInfo==null;
		string skillName=isNull?"":LanguageTextManager.GetString(itemFielInfo.localSkillData.m_name);
		string breakLev=isNull?"":ConvertBreakLev(itemFielInfo.localSkillData.m_breakLevel);
		string skillLev=isNull?"":itemFielInfo.SkillLevel.ToString();
		string skillDesc=isNull?"":LanguageTextManager.GetString(itemFielInfo.localSkillData.m_descSimple);
		string skillPower=isNull?"":SkillValue.GetSkillValue(itemFielInfo.SkillLevel, itemFielInfo.localSkillData.m_mightParams).ToString();
		string skillHitNum=isNull?"":itemFielInfo.localSkillData.m_skillAttacktimes.ToString();
		string skillCDTime=isNull?"":itemFielInfo.localSkillData.m_coolDown.ToString();
		//float[] skillHurts=itemFielInfo.localSkillData.m_skillDamage;
        if(!isNull)
        {
            //向下取整（(向下取整((参数1×（技能等级）^2+参数2×技能等级+参数3)/参数4)×参数4) /1000）*100%
            //int skillHurt = Mathf.FloorToInt((Mathf.FloorToInt((skillHurts[0] * Mathf.Pow(itemFielInfo.SkillLevel, 2) + skillHurts[1] * itemFielInfo.SkillLevel + skillHurts[2]) / skillHurts[3]) * skillHurts[3]) / 1000) * 100;
            //2014-8-27策划修改
            //向下取整（(向下取整((参数1×（技能等级）^2+参数2×技能等级+参数3)/参数4)×参数4) /10）
//            int skillHurt = Mathf.FloorToInt((Mathf.FloorToInt((skillHurts[0] * Mathf.Pow(itemFielInfo.SkillLevel, 2) + skillHurts[1] * itemFielInfo.SkillLevel + skillHurts[2]) / skillHurts[3]) * skillHurts[3]) / 10);
//            SkillHurt.text=skillHurt.ToString();
		}
        else
        {
            SkillHurt.text=string.Empty;
        }
		SkillName.text=skillName;
		BreakLev.text=breakLev;
		SkillLev.text=skillLev;
		SkillDesc.text=skillDesc;
		SkillPower.text=skillPower;
		SkillHitNum.text=skillHitNum;
		SkillCDTime.text=skillCDTime;
		

		SkillUpgradeBtn.gameObject.SetActive(!isNull);
		EffPoint.transform.ClearChild();
		//是否满级处理
		if(itemFielInfo.IsFullLev())
		{
			SkillUpgradeBtn.gameObject.SetActive(false);
			NotReachUpLev.gameObject.SetActive(false);
			NGUITools.AddChild(EffPoint,FullLevEff);

			UpgradeConsume.text = "0";
		}
		else
		{
			int upNeedLev;
			int skillManaValue = itemFielInfo.UpgradeConsume(out m_enough,out m_reachUpLev,out upNeedLev);
			SkillUpgradeBtn.gameObject.SetActive(true);
			
			//铜币是否足够
			if(m_enough)
			{
				SkillUpgradeBtn.Enable=true;
                UpgradeConsume.text = "[fffa6f]" + skillManaValue.ToString() + "[-]";
				if(m_reachUpLev)
				{
					NGUITools.AddChild(EffPoint,UpgradeBtnEff);
				}
			}
			else
			{
				//SkillUpgradeBtn.Enable=false;
                UpgradeConsume.text = "[fe768b]" + skillManaValue.ToString() + "[-]";
			}
			NotReachUpLev.text=m_reachUpLev?"":string.Format(LanguageTextManager.GetString("IDS_I7_14"),upNeedLev);
			NotReachUpLev.gameObject.SetActive(!m_reachUpLev);
			SkillUpgradeBtn.gameObject.SetActive(m_reachUpLev);
		}
		//选中特效
	}

	private string ConvertBreakLev(int breakLev)
	{
		string Ids="";
		switch(breakLev)
		{
			case 2:
			Ids="IDS_I7_19";
				break;
			case 3:
			Ids="IDS_I7_20";
				break;
			case 4:
			Ids="IDS_I7_21";
				break;
			case 5:
			Ids="IDS_I7_22";
				break;
		}
		return LanguageTextManager.GetString(Ids);
	}
    void DestroySelf()
    {
        DestroyImmediate(this.gameObject);
        RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetStrengthConsume);
    }
    private void ResetStrengthConsume(INotifyArgs args)
    {
        if (!m_itemFielInfo.IsFullLev())
        {
            int upNeedLev;
            int skillManaValue = m_itemFielInfo.UpgradeConsume(out m_enough, out m_reachUpLev, out upNeedLev);
            //铜币是否足够
            if (m_enough)
            {
                UpgradeConsume.text = "[fffa6f]" + skillManaValue.ToString() + "[-]";
            }
            else
            {
                UpgradeConsume.text = "[fe768b]" + skillManaValue.ToString() + "[-]";
            }
        }
    }
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), ResetStrengthConsume);
    }
}
