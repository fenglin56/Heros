using UnityEngine;
using System.Collections;
using System;

public class DefenceSliderBehaviour : MonoBehaviour {

	public UILabel Title;
	public GameObject ThresholdThumb1;
	public GameObject ThresholdThumb2;
	public GameObject MarkEff;
	public Action<DefenceSliderType> FinishCallBack;  //播放后触发
	public Action<DefenceSliderType,bool,bool> ReachThresholdCallBack;  //到达阀值后触发
	public UISlider ProcessBar;
    public UILabel PercentValue;
    public UILabel PercentSymbal;

	private const float WidthFactor=251f;
	private float m_thumbValue1;
	private float m_thumbValue2;
	private DefenceSliderType m_defenceSliderType;
    private float m_originValue;
	private bool m_reachThreshold1,m_reachThreshold2;
	void Awake()
	{
		ProcessBar.sliderValue=0;
		m_reachThreshold1=m_reachThreshold2=false;
	}
	/// <summary>
	/// 初始化进度条，这里的thumb值是归一后的值
	/// </summary>
	/// <param name="title_IDS">Title_ identifier.</param>
	/// <param name="thumbValue1">Thumb value1.</param>
	/// <param name="thumbValue2">Thumb value2.</param>
	public void InitThumnb(string title_IDS,DefenceSliderType defenceSliderType,float thumbValue1,float thumbValue2,float originValue)
	{
        m_originValue=originValue;
		m_defenceSliderType=defenceSliderType;
		Title.text=LanguageTextManager.GetString(title_IDS);
		m_thumbValue1=thumbValue1*0.01f;
		m_thumbValue2=thumbValue2*0.01f;
		ThresholdThumb1.transform.localPosition=new Vector3(m_thumbValue1*WidthFactor,0,0);
		ThresholdThumb2.transform.localPosition=new Vector3(m_thumbValue2*WidthFactor,0,0);
        if (defenceSliderType != DefenceSliderType.DoorBlood)
        {
            PercentSymbal.gameObject.SetActive(false);
        }
	}
	/// <summary>
	/// 开始播放进度条动画
	/// </summary>
	/// <param name="targetValue">Target value.</param>
	public void StartProcess(float targetValue)
	{
        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_DefenceResult_1");
        TweenFloat.Begin(targetValue, 0, m_originValue, (val) =>
            {
                PercentValue.text =Mathf.CeilToInt(val).ToString();
            });
		TweenFloat.Begin(targetValue,0,targetValue,(val)=>
		                 {//Begin Delegate
			ProcessBar.sliderValue=val;            
			if(!m_reachThreshold1&&m_thumbValue1-val<=0.01f)
			{
				//到达。播特效
                SoundManager.Instance.PlaySoundEffect("Sound_UIEff_DefenceResult_2");
				NGUITools.AddChild(ThresholdThumb1,MarkEff);
				m_reachThreshold1=true;
				if(ReachThresholdCallBack!=null)
				{
					ReachThresholdCallBack(m_defenceSliderType,m_reachThreshold1,false);
				}
			}
			else if(!m_reachThreshold2&&m_thumbValue2-val<=0.01f)
			{
                SoundManager.Instance.PlaySoundEffect("Sound_UIEff_DefenceResult_2");
				NGUITools.AddChild(ThresholdThumb2,MarkEff);
				m_reachThreshold2=true;
				if(ReachThresholdCallBack!=null)
				{
					ReachThresholdCallBack(m_defenceSliderType,m_reachThreshold1,m_reachThreshold2);
				}
			}
		},(obj)=>
		{//Finish Delegate
			if(FinishCallBack!=null)
			{
				FinishCallBack(m_defenceSliderType);
			}
		});
	}
}