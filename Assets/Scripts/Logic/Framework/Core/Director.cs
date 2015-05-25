using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Director : ViewNotifier {

    List<string> m_sceneList;
    int m_currentSceneIndex;
    
	// Use this for initialization
	void Start () {
        m_currentSceneIndex = Application.levelCount;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    /// <summary>
    /// 接受SendMessage消息，启动场景。在游戏内，只能在这里启动场景
    /// </summary>
    public void LoadLevel(string levelName)
    {
        Application.LoadLevelAsync(levelName);
    }

    /// <summary>
    /// 从当前场景跳转到前一个场景
    /// </summary>
    public void LoadPreviousScene()
    {
        m_currentSceneIndex--;
        Application.LoadLevel(m_currentSceneIndex);
    }

    /// <summary>
    /// 从当前场景跳转到下一个场景
    /// </summary>
    public void LoadNextScene()
    {
        m_currentSceneIndex++;
        Application.LoadLevel(m_currentSceneIndex);
    }
}
