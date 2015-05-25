using UnityEngine;
using System.Collections;
using System;
using UI.Battle;

public class SkillBtnRemember : View {
    /// <summary>
    /// 被缓存的按钮被点击，参数表示是否执行生效
    /// </summary>
    public Action<bool> OnSkillBtnClicked;
    [HideInInspector]
    public RememberBtnType BtnRememberType;
    [HideInInspector]
    public float BtnMemTime;

    private GameObject m_rememberEff;
    public void ResetBtnMemTime()
    {
        BtnMemTime = CommonDefineManager.Instance.CommonDefine.ButtonMemTime/1000f;
    }
    public void ShowRememberEff(bool flag)
    {
        if (flag)
        {
			GameObject effGo=null;
			if(BtnRememberType==SkillBtnRemember.RememberBtnType.NormalSkillBtn)
			{
				effGo=BattleUIManager.Instance.RememberBtnEffNormal;
			}
			else
			{
				effGo=BattleUIManager.Instance.RememberBtnEff;
				//隐藏多段技能特效
				var battleSkillBtn=GetComponent<BattleSkillButton>();
				if(battleSkillBtn!=null&&battleSkillBtn.MultiSegmentsSkillEff!=null)
				{
					battleSkillBtn.MultiSegmentsSkillEff.SetActive(false);
				}
			}
			m_rememberEff = NGUITools.AddChild(gameObject, effGo);
			m_rememberEff.transform.localPosition=new Vector3(0,0,-10);
        }
        else
        {
            if (m_rememberEff != null)
            {
                Destroy(m_rememberEff);
            }
			if(BtnRememberType!=SkillBtnRemember.RememberBtnType.NormalSkillBtn)
			{
				//隐藏多段技能特效
				var battleSkillBtn=GetComponent<BattleSkillButton>();
				if(battleSkillBtn!=null&&battleSkillBtn.MultiSegmentsSkillEff!=null)
				{
					battleSkillBtn.MultiSegmentsSkillEff.SetActive(true);
				}
			}
        }
    }
    protected override void RegisterEventHandler()
    {
    }

    public enum RememberBtnType
    {
        ScrollBtn,   //翻滚
        ExplosiveBtn,//爆气
        LoreBtn,  //奥义
        NormalSkillBtn,  //普通攻击
        SkillBtn,  //技能按钮
    }
}

