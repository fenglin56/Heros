using UnityEngine;
using System.Collections;
using System.Linq;

public class EctGuideManager  : Model, ISingletonLifeCycle
{
    private EctGuideStepConfigDataBase m_ectGuideStepConfigDataBase;
    
	private static EctGuideManager m_instance;
    public static EctGuideManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new EctGuideManager();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }
    public EctGuideStepData CurrGuideStepData { get; private set; }
    public bool IsEctypeGuide { get; set; }
    public bool EctypeGuideStepReached { get; set; }
	public void SetCurrGuideStepData(EctGuideStepData data)
	{
		CurrGuideStepData = data;
	}
    private EctGuideManager()
    {
        m_ectGuideStepConfigDataBase = GuideConfigManager.Instance.EctGuideStepConfigData;
        CurrGuideStepData = null;
    }
    public void ReceiveEctypeGuideStep(SC_GuideStepInfo sC_GuideStepInfo)
    {
        TraceUtil.Log(SystemModel.Rocky, "副本步骤:" + sC_GuideStepInfo.dwStepID + "  " + sC_GuideStepInfo.byStepStute);
        switch (sC_GuideStepInfo.byStepStute)
        {
            case 1: //当前步骤在执行状态
                CurrGuideStepData = new EctGuideStepData();
                CurrGuideStepData .StepData= sC_GuideStepInfo;
                CurrGuideStepData.IsExcuting = false;
                CurrGuideStepData.EctGuideStepConfigData = m_ectGuideStepConfigDataBase.Datas.SingleOrDefault(P => P.StepID == sC_GuideStepInfo.dwStepID);
                RaiseEvent(EventTypeEnum.ReceiveGuideStep.ToString(), null);
                break;
            case 2: //步骤完成通知
                CurrGuideStepData = null;
                RaiseEvent(EventTypeEnum.FinishGuideStep.ToString(), null);
                break;
            default:
                break;
        }
       
    }
    public void SetGuideBtnEnable(bool flag)
    {
        var guideButtonList = GuideBtnManager.Instance.GetButtonList;
        foreach (var btn in guideButtonList.Values)
        {
            SetGuideBtnEnable(flag,btn);
        }
    }
    public void SetGuideBtnEnable(bool flag, GuideBtnParam btn)
    {

        if (!btn.GuideBtn)
        {
            TraceUtil.Log(SystemModel.Common, TraceLevel.Error, btn.UIType + "  " + btn.SubUIType + "  ");
            return;
        }
        var skillComponent = btn.GuideBtn.GetComponent<UI.Battle.BattleSkillButton>();
        if (skillComponent != null)
        {
			skillComponent.SetStepDisable(flag);
			if(flag)
			{
				switch (skillComponent.SpecialType)
				{
				case UI.Battle.SpecialSkillType.Roll://翻滚
				{
					flag = PlayerGasSlotManager.Instance.GetAirSlotValue < skillComponent.skillEnergyComsume ? false : true;
				}
					break;
				case UI.Battle.SpecialSkillType.Explode://爆气
				{
					flag = PlayerGasSlotManager.Instance.GetAirSlotValue < skillComponent.skillEnergyComsume ? false : true;
				}
					break;
				case UI.Battle.SpecialSkillType.Meaning://奥义
				{
					flag = PlayerGasSlotManager.Instance.GetAirSlotValue < skillComponent.skillEnergyComsume ? false : true;
					if(EctypeManager.Instance.GetSirenSkillSurplusValue() <= 0)
					{
						flag = false;
					}
				}
					break;
				default:
					break;
				}
			}
            skillComponent.SetButtonStatus(flag);
        }
        else
        {
            var healthComponent = btn.GuideBtn.GetComponent<UI.Battle.HealthAndMagicButton>();
            if (healthComponent != null)
                healthComponent.SetMyButtonActive(flag);
        }
    }
    public void Instantiate()
    {
    }

    public void LifeOver()
    {
        m_instance = null;
    }

    protected override void RegisterEventHandler()
    {
        throw new System.NotImplementedException();
    }
}
public class EctGuideStepData
{
    public SC_GuideStepInfo StepData;
    public bool IsExcuting;
    public EctGuideStepConfigData EctGuideStepConfigData;
}
