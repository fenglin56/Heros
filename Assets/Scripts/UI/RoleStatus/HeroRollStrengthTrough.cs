using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroRollStrengthTrough : MonoBehaviour 
{
    public SpriteSwith[] m_StrengthPoints;
    public GameObject BarStrengthNoEnoughTip;
    public GameObject GasSlotEffect;
	public GameObject Eff_StrengthBarFlash;

	public UISlider Slider_Strength;
	public UISprite Filled_Strength;

	private int LastStrengthValue = 0;
    private GameObject m_gasSlotEffect;

    int MaxValue = 0;
    int currrenetValue = -1;

	void Awake() 
    {
        UpdateMaxValue();
	}

    public void UpdateMaxValue()
    {
		int max = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAX_ENERGY_NUM;
		MaxValue = max;

//            for (int i = 0; i < m_StrengthPoints.Length; i++)
//            {
//                m_StrengthPoints[i].gameObject.SetActive(i<MaxValue?true:false);
//            }	
		UpdateSlider(0);
    }

	//更新气力值显示
	private void UpdateSlider(int value)
	{
		if(MaxValue<=0)
		{
			MaxValue = 1;
			Debug.LogError("PlayerValues.PLAYER_FIELD_MAX_ENERGY_GRID_NUM is zero");
		}
		float f = value * 1f / MaxValue;
		//Slider_Strength.sliderValue = f;
		Filled_Strength.fillAmount = f;
	}

    public void SetValue(int value)
    {
        if(currrenetValue == value)
            return;
        currrenetValue = value;        

		UpdateSlider(value);
		if(value >= MaxValue)
		{
			Eff_StrengthBarFlash.SetActive(true);
		}
		else
		{
			Eff_StrengthBarFlash.SetActive(false);
		}
//        for (int i = 0; i < MaxValue; i++)
//        {
//            //m_StrengthPoints[i].ChangeSprite(i < value ? 2 : 1);
//			m_StrengthPoints[i].gameObject.SetActive(i < value);
//            if (i == value - 1 && LastStrengthValue < value)
//            {
//                PlayEffect(m_StrengthPoints[i].transform);
//            }
//        }
        LastStrengthValue = value;
    }

    void PlayEffect(Transform slot)
    {
        StopCoroutine("DestroyEffectObjForTime");
        if (m_gasSlotEffect != null)
        {
            Destroy(m_gasSlotEffect);
        }
        m_gasSlotEffect = UI.CreatObjectToNGUI.InstantiateObj(GasSlotEffect, slot.parent);
        m_gasSlotEffect.transform.localPosition = slot.localPosition + Vector3.forward * -10;
        //m_gasSlotEffect.transform.localScale = Vector3.one;
        StartCoroutine(DestroyEffectObjForTime(1));
    }

    IEnumerator DestroyEffectObjForTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (m_gasSlotEffect != null)
        {
            Destroy(m_gasSlotEffect);
        }
    }

    public void ShowNoEnoughStrengthTip()
    {
        //StopAllCoroutines();
        StopCoroutine("Flicker");
        StartCoroutine("Flicker");
    }

    IEnumerator Flicker()
    {
        //TweenAlpha.Begin(BarStrengthNoEnoughTip.gameObject, 0.3f, 1f);
        BarStrengthNoEnoughTip.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        //TweenAlpha.Begin(BarStrengthNoEnoughTip.gameObject, 0.3f, 0);
        BarStrengthNoEnoughTip.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        //TweenAlpha.Begin(BarStrengthNoEnoughTip.gameObject, 0.3f, 1f);
        BarStrengthNoEnoughTip.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        //TweenAlpha.Begin(BarStrengthNoEnoughTip.gameObject, 0.3f, 0);
        BarStrengthNoEnoughTip.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        //TweenAlpha.Begin(BarStrengthNoEnoughTip.gameObject, 0.3f, 1f);
        BarStrengthNoEnoughTip.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        //TweenAlpha.Begin(BarStrengthNoEnoughTip.gameObject, 0.3f, 0);
        BarStrengthNoEnoughTip.SetActive(false);
    }
}
