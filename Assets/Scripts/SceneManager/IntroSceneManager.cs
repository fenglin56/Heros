using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntroSceneManager : MonoBehaviour {

    public float m_effectLifeTime=74;

	// Use this for initialization
	void Start () {
        StartCoroutine("TimeToChangeScene", m_effectLifeTime);
        IntroBgMusic();
	}
	
    IEnumerator TimeToChangeScene(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        //给服务器发消息，通知开场动画结束
        //Application.LoadLevel("EnterPointScene");
    }
    /// <summary>
    /// 开场动画背景音乐
    /// </summary>
    void IntroBgMusic()
    {
        SoundManager.Instance.StopBGM();
        string musicId = "Music_BFBG_GameStart";
        SoundManager.Instance.PlayBGM(musicId);
    }
}
