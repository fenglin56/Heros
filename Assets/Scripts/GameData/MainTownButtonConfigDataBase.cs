using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class MainTownButtonConfigData
{
    [EnumMap]
    public UI.MainUI.UIType ButtonFunc;
    public string ButtonName;
    /// <summary>
    /// 按钮区域
    /// </summary>
    [EnumMap]
    public MainTownButtonArea ButtonArea;
    public int Button_Row;
    public int  Button_RowIndex;
    public string ButtonIndex;
    public int  Button_ListSequence;
    public float ButtonRadius;
    [DataToObject(PrefabPath = "Assets/Prefab/GUI/MainButton")]
    public GameObject ButtonPrefab;
    public int DefaultEnable;
    [HideInDataReaderAttribute]
    public Vector2 ButtonPositionOffset;
    public string ButtonSound;
}

public enum MainTownButtonArea//按钮区域
{
    LeftUp = 1,
    RightUp=2,
    RightDown=3,
}
public class MainTownButtonConfigDataBase : ConfigBase
{
    public MainTownButtonConfigData[] Datas;

    public override void Init(int length, object dataList)
    {
        Datas = new MainTownButtonConfigData[length];

        var realData = dataList as List<MainTownButtonConfigData>;
        for (int i = 0; i < realData.Count; i++)
        {
            Datas[i] = (MainTownButtonConfigData)realData[i];
            var buttonIndex=Datas[i].ButtonIndex;
            if (buttonIndex != "0")
            {
                var pos = buttonIndex.Remove(0,1).Split('+');
                var x = float.Parse(pos[0]);
                var y = float.Parse(pos[1]);
                Datas[i].ButtonPositionOffset = new Vector2(x, y);
            }
        }
    }
}
