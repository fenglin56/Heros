using UnityEngine;
using System.Collections;




public class GameConfigData
{
    public bool SoundActive;
    public bool MusicActive;
    public bool ShowOtherPlayerEffect;
    public bool ShowScenceEffect;
    public bool ShowEnemyName;
    public GameConfigData()
    {
        this.SoundActive = true;
        this.MusicActive = true;
        this.ShowScenceEffect = true;
        this.ShowOtherPlayerEffect = true;
        this.ShowEnemyName = true;
    }
}

public class LoadSceneData
{
    public enum LoadSceneType {Login,Battle,Town,StoryLine,OpenAnimation}
    public object LoadSceneInfo;
    public LoadSceneType loadSceneType;
    public float Progress;

    public LoadSceneData(LoadSceneType loadSceneType,float Progress)
    {
        this.loadSceneType = loadSceneType;
        this.Progress = Progress;
    }


    public LoadSceneData(LoadSceneType loadSceneType, float Progress,object loadSceneInfo)
    {
        this.loadSceneType = loadSceneType;
        this.Progress = Progress;
        this.LoadSceneInfo = loadSceneInfo;
    }
}

#region containerInfo
#endregion