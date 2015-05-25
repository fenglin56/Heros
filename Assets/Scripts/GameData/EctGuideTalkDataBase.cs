using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class StepDialogData
{
    public int TalkID;
    [DataToObject(PrefabPath="Assets/Prefab/GUI/SirenHead")]
    public GameObject TalkHead;
    public string NPCName;
    public string TalkText;
    public string TextPos;
    public int TalkType;  //1=玩家，2=怪物；
    [HideInDataReader]
    public Vector3 OffsetVect;
}

public class EctGuideTalkDataBase : ConfigBase
{
    public StepDialogData[] Datas;

    public override void Init(int length, object dataList)
    {
        Datas = new StepDialogData[length];

        var realData = dataList as List<StepDialogData>;
        for (int i = 0; i < realData.Count; i++)
        {
            Datas[i] = (StepDialogData)realData[i];
            var textPos = Datas[i].TextPos.Split('+');  //阖掉A+ 然后再分割字符串
            if (textPos.Length > 0)
            {
                Datas[i].OffsetVect = new Vector3(int.Parse(textPos[1]), int.Parse(textPos[2]));
            }
        }
    }
}
