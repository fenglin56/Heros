using UnityEngine;
using System.Collections;

public class PopupFinish : MonoBehaviour {

    private bool m_activePopup;
    public float Height;
    public int CritScale;
    private Vector3? m_hostPosition;
    private float alphaDuration;
    private TweenAlpha m_tweenAlpha ;
    private TweenPosition m_tweenPosition ;
    private TweenScale m_tweenScale ;
    
	// Use this for initialization
	void Start () {
        SetTweenComponent();
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    private void SetTweenComponent()
    {
        if (m_tweenAlpha == null)
        {
            m_tweenAlpha = transform.GetComponent<TweenAlpha>();
            alphaDuration = m_tweenAlpha.duration;
        }
        if (m_tweenPosition == null) 
            m_tweenPosition = transform.GetComponent<TweenPosition>();
        if (m_tweenScale == null)
            m_tweenScale = transform.GetComponent<TweenScale>();
    }
    private void ShowPopUp()
    {
        if (HostPosition != null)
        {
            Vector3 uiPos = PopupTextController.GetPopupPos(HostPosition.Value, PopupObjManager.Instance.UICamera);
            transform.position = uiPos;
            switch (this.FightEffectType)
            {
                case global::FightEffectType.BATTLE_EFFECT_CRIT: 
                    m_tweenScale.from = transform.localScale;
                    m_tweenScale.to = transform.localScale * CritScale;
                    m_tweenPosition.enabled = false;
                    m_tweenScale.enabled = true;                  
                    break;
                case FightEffectType.BATTLE_ADDHP:
                case FightEffectType.BATTLE_ADDMONEY:                   
                case FightEffectType.BATTLE_ADDMP:
                case FightEffectType.BATTLE_EFFECT_DODGE:
                case FightEffectType.BATTLE_EFFECT_HIT:
                case FightEffectType.BATTLE_EFFECT_HP:
                case FightEffectType.BATTLE_EFFECT_EXPSHOW:
                    AnimShow();
                    break;
                case FightEffectType.TOWN_EFFECT_ZHANLI:
                    AnimShow();
                    break;
            }
            m_tweenAlpha.enabled = true;
        }
    }

    void AnimShow()
    {
        var objPos = transform.localPosition+new Vector3(0,0,500);
        m_tweenPosition.from = objPos;
        m_tweenPosition.to = objPos + Vector3.up * Height;//
        m_tweenScale.enabled = false;
        m_tweenPosition.enabled = true;
    }

    public Vector3? HostPosition {
        get { return m_hostPosition; }
        set 
        { 
            this.m_hostPosition = value;
            SetTweenComponent();
            ShowPopUp(); 
            StartCoroutine(CallWhenFinished()); 
        }
    }
    public FightEffectType FightEffectType { get; set; }
    IEnumerator CallWhenFinished()
    {
        yield return new WaitForSeconds(alphaDuration);
        m_hostPosition = null;
        GameObjectPool.Instance.Release(gameObject);
        m_tweenAlpha.Reset();
        m_tweenPosition.Reset();
        m_tweenScale.Reset();
    }
}
