using UnityEngine;
using System.Collections;

public class ConnectDelayTip : MonoBehaviour {

	public UILabel infoLabel;
	public UILabel timeLabel;
	private string colorStr = "[ffffff]";
	void Start()
	{
		infoLabel.text = LanguageTextManager.GetString ("IDS_I1_44");
		InvokeRepeating ("DelayUpdateFun",0.01f,CommonDefineManager.Instance.CommonDefine.PingDelayTime);
	}
	void DelayUpdateFun()
	{
		if (CheatManager.Instance.connectDelayTime <= 100) {
			colorStr = "[008000]";
		} else if (CheatManager.Instance.connectDelayTime > 100 && CheatManager.Instance.connectDelayTime <= 500) {
			colorStr = "[FFFF00]";
		} else {
			colorStr = "[FF0000]";
		}
		timeLabel.text = colorStr + CheatManager.Instance.connectDelayTime;
	}
	void OnDestroy()
	{
		if (IsInvoking ("DelayUpdateFun")) {
			CancelInvoke("DelayUpdateFun");	
		}
	}
}
