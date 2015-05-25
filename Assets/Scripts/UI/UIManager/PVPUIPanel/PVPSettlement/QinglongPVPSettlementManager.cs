using UnityEngine;
using System.Collections;

/// <summary>
/// 青龙会PVP结算界面管理器
/// </summary>
public class QinglongPVPSettlementManager : View 
{
	public GameObject QinglongSettlementPrefab;

	private readonly float ScaleTimeSpeed = 0.3f;
	private readonly float ScaleTimeLength = 3.0f;

	private SMsgEctypePVP_Result_SC sMsgEctypePVP_Result_SC;

	//PVP副本结算处理
	void EctypeFinishHandle(INotifyArgs arg)
	{
		//1缓存消息体
		//2场景摄像机移动主建筑上方
		//3播放摄像机慢镜头倒塌动画
		//4弹出结算界面
		sMsgEctypePVP_Result_SC = (SMsgEctypePVP_Result_SC)arg;

		TraceUtil.Log(SystemModel.Lee,"开始慢镜头:" + Time.time);
		StartCoroutine("StartTimeScale");
	}

	IEnumerator StartTimeScale()
	{
		Time.timeScale = ScaleTimeSpeed;
		yield return new WaitForSeconds(ScaleTimeLength);
		Time.timeScale = 1f;
		yield return new WaitForSeconds(EctypeManager.Instance.GetCurrentEctypeData().ResultAppearDelay);
		GameObject panelObj = UI.CreatObjectToNGUI.InstantiateObj(QinglongSettlementPrefab, transform);
		QinglongPVPSettlementPanel crusadeSettlementPanel = panelObj.GetComponent<QinglongPVPSettlementPanel>();
		crusadeSettlementPanel.Show(sMsgEctypePVP_Result_SC);
	}

	void OnDestroy()
	{

		RemoveEventHandler(EventTypeEnum.EctypeFinish.ToString(),EctypeFinishHandle);
	}
	
	protected override void RegisterEventHandler()
	{

		AddEventHandler(EventTypeEnum.EctypeFinish.ToString(),EctypeFinishHandle);
	}
}

