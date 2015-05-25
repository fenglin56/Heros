using UnityEngine;
using System.Collections;

public class EndLessRewardTipRun : MonoBehaviour {
	public UILabel waveLabel;
	public UILabel rewardLabel;
	private int tweenTime = 2;
	private float pos = 20;
	private EndLessScenePanel uiParent;
	private bool isRewardMark = false;
	//显示[奖励]
	public void Show (EndLessScenePanel uiParent,int waveCount,int goodID,int goodCount) {
		this.uiParent = uiParent;
		isRewardMark = true;
		waveLabel.text = waveCount.ToString ();
		ItemData getItem = ItemDataManager.Instance.GetItemData(goodID);
		string goodsName = UI.NGUIColor.SetTxtColor (LanguageTextManager.GetString(getItem._szGoodsName),(UI.TextColor)getItem._ColorLevel);
		rewardLabel.text = string.Format ("{0} x {1}",goodsName,goodCount);
		FirstRun ();
	}
	//波数开始
	public void Show(EndLessScenePanel uiParent,int startWaveCount)
	{
		this.uiParent = uiParent;
		isRewardMark = false;
		waveLabel.text = startWaveCount.ToString ();
		FirstRun ();
	}
	void FirstRun()
	{
		TweenPosition.Begin(gameObject, 0.33f,Vector3.zero,new Vector3 (0,51,0));
		TweenAlpha.Begin (gameObject,0.33f,0,1,(obj)=>{
			SecondRun();
		});
	}
	void SecondRun()
	{
		Invoke ("ThirdRun",0.66f);
	}
	void ThirdRun()
	{
		TweenPosition.Begin(gameObject, tweenTime,transform.localPosition,new Vector3 (0,71,0),(obj)=>{
			TweenBack();
		});
		TweenAlpha.Begin (gameObject,tweenTime,1,0,(obj)=>{
			TweenBack();
		});
	}
	bool isAgain = false;
	void TweenBack()
	{
		if (isAgain) {
			if(isRewardMark)
			{
				uiParent.RewardTipFinish();
			}
			Destroy(gameObject);
		}
		isAgain = true;
	}
	/*IEnumerable DestroyTween()
	{
		yield return 
		Destroy(gameObject);
	}*/

}
