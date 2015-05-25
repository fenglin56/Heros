using UnityEngine;
using System.Collections;

public class EndLessPassWaveEff : MonoBehaviour {
	public UILabel label;
	public GameObject firstEff;
	public float firstTime = 0.8f;//跑进来的时间
	public GameObject secondEff;
	public float secondTime = 0.8f;
	private int loopCount;
	// Use this for initialization
	public void Show (string str,int loopCount) {
		label.text = str;
		this.loopCount = loopCount;
		//跑进来时间，加停留时间
		firstTime = firstTime + this.loopCount * CommonDefineManager.Instance.CommonDefine.EndlessSingleMassageShowTime;
		PlayerEff ();
	}
	void PlayerEff()
	{
		secondEff.SetActive (false);
		Invoke ("PlayerSecond",firstTime);
	}
	void PlayerSecond()
	{
		firstEff.SetActive (false);
		secondEff.SetActive (true);
		label.enabled = false;
		Invoke ("FinishEff",secondTime);
	}
	void FinishEff()
	{
		Destroy (gameObject);
	}
}
