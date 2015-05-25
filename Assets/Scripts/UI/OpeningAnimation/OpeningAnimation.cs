using UnityEngine;
using System.Collections;

public class OpeningAnimation : MonoBehaviour {
	public GameObject skipBtn;
	// Use this for initialization
	void Start () {
		//Invoke ("OpeningAnim",CommonDefineManager.Instance.CommonDefine.gameStarTime);
		StartCoroutine (OpeningAnim());
		SoundManager.Instance.PlayBGM("Music_BFBG_GameStart");
		UIEventListener.Get (skipBtn).onClick = OnSkipBtnEvent;
	}
	void OnSkipBtnEvent(GameObject btn)
	{
		GameManager.Instance.SavePlayOpeningAnimation ();
		GameManager.Instance.GotoState(GameManager.GameState.GAME_STATE_LOGIN);
	}
	IEnumerator OpeningAnim()
	{
		yield return new WaitForSeconds (CommonDefineManager.Instance.CommonDefine.gameStarTime);
		GameManager.Instance.SavePlayOpeningAnimation ();
		GameManager.Instance.GotoState(GameManager.GameState.GAME_STATE_LOGIN);
	}
}
