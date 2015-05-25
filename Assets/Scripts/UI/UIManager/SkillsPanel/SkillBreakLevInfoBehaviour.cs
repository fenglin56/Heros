using UnityEngine;
using System.Collections;

public class SkillBreakLevInfoBehaviour : MonoBehaviour {
	public UILabel Title;
	public UILabel Desc1;
	public UILabel Desc2;
	public UILabel Desc3;

	private TweenScale m_tweenScale;

	void Awake()
	{
        Title.text = LanguageTextManager.GetString("IDS_I7_15").Replace(@"\n", "\n");
        Desc1.text = LanguageTextManager.GetString("IDS_I7_16").Replace(@"\n", "\n");
        Desc2.text = LanguageTextManager.GetString("IDS_I7_17").Replace(@"\n", "\n");
        Desc3.text = LanguageTextManager.GetString("IDS_I7_18").Replace(@"\n", "\n");

		m_tweenScale=GetComponent<TweenScale>();
	}

	public void Show(bool flag)
	{
		m_tweenScale.Play(flag);
	}
}
