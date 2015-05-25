using UnityEngine;
using System.Collections;

public class CrusadeMatchingPanel : MonoBehaviour
{
	public LocalButtonCallBack Button_Abandon;
	public LocalButtonCallBack Button_GoKill;
	
	public Transform RandomEctypeItemPoint;
	public UILabel Label_randomEctype;
	public UILabel Label_cutdownTime;
	
	private GameObject m_matchingEctypeItem;
	private float m_matchingConfirmTime = 0;//匹配确认时间

	void Start()
	{
		Button_Abandon.SetCallBackFuntion(OnAbandonClick,null);
		Button_GoKill.SetCallBackFuntion(OnGoKillClick,null);
	}
	void OnAbandonClick(object obj)
	{
		NetServiceManager.Instance.TeamService.SendCrusadeMatchingAnswer(false);
		transform.localPosition = Vector3.back * 1000;
	}

	void OnGoKillClick(object obj)
	{
		NetServiceManager.Instance.TeamService.SendCrusadeMatchingAnswer(true);
		transform.localPosition = Vector3.back * 1000;
	}

	public void CrusadeMatching(SMsgConfirmMatching_SC sMsgConfirmMatching_SC)
	{
		var ectypeConfig = EctypeConfigManager.Instance.EctypeContainerConfigList[sMsgConfirmMatching_SC.dwEctypeContainerID];
		var ectypeSelectCofig = EctypeConfigManager.Instance.EctypeSelectConfigList[EctypeConfigManager.Instance.GetSelectContainerID(sMsgConfirmMatching_SC.dwEctypeContainerID)];
		
		if(m_matchingEctypeItem != null)
		{
			Destroy(m_matchingEctypeItem);
		}
		m_matchingEctypeItem = UI.CreatObjectToNGUI.InstantiateObj(ectypeSelectCofig._EctypeIconPrefab,RandomEctypeItemPoint);
		Label_randomEctype.text = string.Format(LanguageTextManager.GetString("IDS_I19_26"),
		                                        LanguageTextManager.GetString( ectypeConfig.lEctypeName));
		m_matchingConfirmTime = CommonDefineManager.Instance.CommonDefine.Match_Delay;
		Label_cutdownTime.text = m_matchingConfirmTime.ToString();
		transform.localPosition = Vector3.zero;
		InvokeRepeating("MatchingConfirmCutDown",1f,1f);
	}

	void MatchingConfirmCutDown()
	{
		m_matchingConfirmTime--;
		if(m_matchingConfirmTime<=0 && transform.localPosition==Vector3.zero)
		{
			CancelInvoke("MatchingConfirmCutDown");
			transform.localPosition = Vector3.back * 1000;
		}
		Label_cutdownTime.text = m_matchingConfirmTime.ToString();
	}
}

