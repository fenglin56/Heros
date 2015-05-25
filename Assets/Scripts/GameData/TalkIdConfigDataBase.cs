using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class TalkIdConfigData
{
    public int TalkID;//	对话的ID	
    [EnumMap]
    public DialogBoxType DialogPrefab;//	对话框类型		对白框类型：1=头像左、2=头像右、3=无头像左、4=无头像右、5=旁白；
    public string TextPos;//	对话框位置		对话框坐标与屏幕中心点的偏移值，填写格式：X+Y；
    [EnumMap]
    public StoryTallType TalkType;//	说话者类型		1=玩家，2=NPC；
    public string NPCName;//	对话NPC名称		当TalkType=2，读取这个字段作为对话NPC名称；填写文本IDS；
    public string TalkText;//	NPC对白		填写文本IDS；多页对话用“+”间隔；
    [DataToObjectAttribute(PrefabPath = "Assets/Prefab/GUI/IconPrefab/StroyPersonHead")]
    public GameObject TalkHead; //说话者头像  填写Prefab名称（资源在Assets\Prefab\GUI\IconPrefab\StroyPersonHead），0表示无头像
	[HideInDataReaderAttribute]
	public bool isTaskTalkMark;
}
public class TalkIdConfigDataBase : ConfigBase
{
    public TalkIdConfigData[] Datas;

    public override void Init(int length, object dataList)
    {
        Datas = new TalkIdConfigData[length];

        var realData = dataList as List<TalkIdConfigData>;
        for (int i = 0; i < realData.Count; i++)
        {
            Datas[i] = (TalkIdConfigData)realData[i];
        }
    }
    /// <summary>
    /// 根据NewGuideConfig的TalkIds，找到TalkIdConfigData列表
    /// </summary>
    /// <param name="groupIds"></param>
    /// <returns></returns>
    public TalkIdConfigData[] GetTalkIdConfigDataByGroup(int[] talkIds)
    {
        if (talkIds == null) return null;
        TalkIdConfigData[] talkIdConfigData = new TalkIdConfigData[talkIds.Length];

        for (int i = 0; i < talkIds.Length; i++)
        {
            talkIdConfigData[i] = Datas.SingleOrDefault(P => P.TalkID == talkIds[i]);
        }
        return talkIdConfigData;
    }
}
