using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DefenceSettleInfoBehaviour : MonoBehaviour {

	public GameObject DefenceSlider;
	public GameObject SliderParentNode;
	public GameObject PinFenEff;
	public SpriteSwith PinFenValue;
		
	public Action SettleInfoComplete;
	private Dictionary<DefenceSliderType,DefenceSliderBehaviour> m_sliders;
	private GameObject PinFenValueInstance;
	private float m_doorValue,m_doubleHitVal,m_beHitVal;
	private int m_spriteId;
    private TweenPosition m_posAnim;
	void Awake()
	{
		m_sliders=new Dictionary<DefenceSliderType,DefenceSliderBehaviour>();
        m_posAnim = GetComponent<TweenPosition>();
	}

	public void InitSliders(int ectypeContainerId)
	{
        SMSGECTYPEDEFINE_RESULT_SC defenceEctypeResult = (SMSGECTYPEDEFINE_RESULT_SC)GameDataManager.Instance.GetData(DataType.DefenceEctypeResult);
        m_posAnim.Play(true);
		m_spriteId=1;
        CalcSettleInfoData(ectypeContainerId, defenceEctypeResult);
		m_sliders.Clear();
		float threshold1=CommonDefineManager.Instance.CommonDefine.DefenceLevelJudgeThreshold1;
		float threshold2=CommonDefineManager.Instance.CommonDefine.DefenceLevelJudgeThreshold2;
		GameObject slider;
		DefenceSliderBehaviour sliderBehaviour;
        

		slider=NGUITools.AddChild(SliderParentNode,DefenceSlider);
		sliderBehaviour=slider.GetComponent<DefenceSliderBehaviour>();
        sliderBehaviour.InitThumnb("IDS_I15_5", DefenceSliderType.DoorBlood, threshold1, threshold2, m_doorValue*100);
		sliderBehaviour.transform.localPosition=new Vector3(0,45,0);
		m_sliders.Add(DefenceSliderType.DoorBlood,sliderBehaviour);

		slider=NGUITools.AddChild(SliderParentNode,DefenceSlider);
		sliderBehaviour=slider.GetComponent<DefenceSliderBehaviour>();
        sliderBehaviour.InitThumnb("IDS_I15_6", DefenceSliderType.DoubleHit, threshold1, threshold2, defenceEctypeResult.dwBatterCount);		
		sliderBehaviour.transform.localPosition=new Vector3(0,10,0);
		m_sliders.Add(DefenceSliderType.DoubleHit,sliderBehaviour);

		slider=NGUITools.AddChild(SliderParentNode,DefenceSlider);
		sliderBehaviour=slider.GetComponent<DefenceSliderBehaviour>();
        sliderBehaviour.InitThumnb("IDS_I15_7", DefenceSliderType.BeHit, threshold1, threshold2, defenceEctypeResult.dwHitCount);	
		sliderBehaviour.transform.localPosition=new Vector3(0,-25,0);
		m_sliders.Add(DefenceSliderType.BeHit,sliderBehaviour);

		m_sliders.Values.ApplyAllItem(P=>{P.ReachThresholdCallBack=SliderReachThreshold;P.FinishCallBack=SliderFinish;});

		m_sliders[DefenceSliderType.DoorBlood].StartProcess(m_doorValue);
	}
    private void CalcSettleInfoData(int ectypeContainerId, SMSGECTYPEDEFINE_RESULT_SC defenceEctypeResult)
	{
		float doorBloodPercent=defenceEctypeResult.dwBossPH;
		float doubleHitPercent=defenceEctypeResult.dwBatterCount;
		float beHitPercent=defenceEctypeResult.dwHitCount;

		float initValue,endValue;
		//大门血量
		initValue=CommonDefineManager.Instance.CommonDefine.DefenceLevelJudgeGateHPLeft;
		endValue=EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeContainerId].GateHPRemain;
		m_doorValue=Mathf.Clamp(doorBloodPercent/(endValue-initValue),0,1);

		//连击数
		initValue=CommonDefineManager.Instance.CommonDefine.DefenceLevelJudgeHitPointLeft;
		endValue=EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeContainerId].dwBasicHitPoint;
		m_doubleHitVal=Mathf.Clamp(doubleHitPercent/(endValue-initValue),0,1);
		//受击数
		initValue=CommonDefineManager.Instance.CommonDefine.DefenceLevelJudgeBeHitParam1;
		endValue=CommonDefineManager.Instance.CommonDefine.DefenceLevelJudgeBeHitParam2;
		var basicBeHit=EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeContainerId].BasicBeHit;
		m_beHitVal=Mathf.Clamp(1-beHitPercent/basicBeHit,0,1);

	}

	private void SliderReachThreshold(DefenceSliderType defenceSliderType,bool threshold1,bool threshold2)
	{
		//计算评分
		bool pinfenChanged=false;
		switch(defenceSliderType)
		{
			case DefenceSliderType.DoorBlood:
			if(	threshold1&&threshold2)
			{
				m_spriteId++;
				pinfenChanged=true;
			}
				break;
			case DefenceSliderType.DoubleHit:
			if(threshold1)
			{
				if(threshold2)
				{
					m_spriteId++;
					pinfenChanged=true;
				}
				else
				{
					m_spriteId++;
					pinfenChanged=true;
				}
			}
				break;
			case DefenceSliderType.BeHit:
			if(threshold1)
			{
				if(threshold2)
				{
					m_spriteId++;
					pinfenChanged=true;
				}
				else
				{
					m_spriteId++;
					pinfenChanged=true;
				}
			}
				break;
		}
		if(pinfenChanged)
		{
			PinFenValue.ChangeSprite(m_spriteId);
			//特效
			if(PinFenValueInstance==null)
			{
				PinFenValueInstance=NGUITools.AddChild(PinFenValue.transform.parent.gameObject,PinFenEff);
				PinFenValueInstance.transform.localPosition=PinFenValue.transform.localPosition;
			}
			else
			{
				PinFenValueInstance.SetActive(false);
				PinFenValueInstance.SetActive(true);
			}
			
		}
	}
	private void SliderFinish(DefenceSliderType defenceSliderType)
	{
		//进度条动画完成。播放下一个
		switch(defenceSliderType)
		{
			case DefenceSliderType.DoorBlood:
			m_sliders[DefenceSliderType.DoubleHit].StartProcess(m_doubleHitVal);
				break;
			case DefenceSliderType.DoubleHit:
			m_sliders[DefenceSliderType.BeHit].StartProcess(m_beHitVal);
				break;
			case DefenceSliderType.BeHit:
				//完成，触发委托
				if(SettleInfoComplete!=null)
				{
					SettleInfoComplete();
				}
				break;
		}
	}
}
