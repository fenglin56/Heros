using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLocalDataManager : MonoBehaviour {

    private string GameDataKey = "JiangHuGameConfig";

    void Awake()
    {
        LoadGamedata();
    }

    public void LoadGamedata()
    {
        string strData = "";
        strData = PlayerPrefs.GetString(GameDataKey);
        GameConfigData gameConfigData = StringToData(strData);
        GameDataManager.Instance.ResetData(DataType.GameConfigData,gameConfigData);
    }

    public void SaveGameData()
    {
        GameConfigData gameConfigData = GameDataManager.Instance.PeekData(DataType.GameConfigData) as GameConfigData;
        string strData = DataToString(gameConfigData);
        PlayerPrefs.SetString(GameDataKey,strData);
    }

    GameConfigData StringToData(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return new GameConfigData();
        }
        else
        {
            GameConfigData gameConfigData = new GameConfigData();
            string[] StrChild = str.Split(';');
            Dictionary<string,bool> dataConfig = new Dictionary<string,bool>();
            foreach (var child in StrChild)
            {
                string[] childData = child.Split(':');
                dataConfig.Add(childData[0], bool.Parse(childData[1]));
            }
            gameConfigData.SoundActive = dataConfig["SoundActive"];
            gameConfigData.MusicActive = dataConfig["MusicActive"];
            gameConfigData.ShowOtherPlayerEffect = dataConfig["ShowOtherPlayerEffect"];
            gameConfigData.ShowScenceEffect = dataConfig["ShowScenceEffect"];
            gameConfigData.ShowEnemyName = dataConfig["ShowEnemyName"];
            return gameConfigData;
        }
    }

    string DataToString(GameConfigData gameConfigData)
    {
        string strData = "";
        strData += "SoundActive:" + gameConfigData.SoundActive.ToString() + ";";
        strData += "MusicActive:" + gameConfigData.MusicActive.ToString() + ";";
        strData += "ShowOtherPlayerEffect:" + gameConfigData.ShowOtherPlayerEffect.ToString() + ";";
        strData += "ShowScenceEffect:" + gameConfigData.ShowScenceEffect.ToString() + ";";
        strData += "ShowEnemyName:" + gameConfigData.ShowEnemyName.ToString() + ";";
        return strData;
    }

}
