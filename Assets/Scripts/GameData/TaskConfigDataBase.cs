using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class TaskConfigData
{
    public int _TaskID;
    public int _TaskSeries;   //任务种类
    public int _TaskSeriesPic;   //任务种类图标
    public string _TaskTitle;
    public int _TaskType;
    public string _TaskNeed;  // 任务目标  与TaskType相关
    public string _TaskDesc;
    public string _TaskGoals; //任务目标（IDS）
    public int _AwardType; //奖励类型
    public AwardItem[] _AwardItemList; //物品奖励
    public int _AwardMoney;  //金钱奖励
    public int _AwardExp;   //经验奖励
    public int _AwardActive; //奖励活力
    public int _AwardXiuWei; //奖励修为   
    public int[] _TownGuideList;
    public int[] _CompleteTownGuideList;
    public string _TaskGetSound;  //获得任务播放的音效
    public string _TaskCompleteSound;  //完成任务播放的音效
    public int ButtonProcess;  //按钮开启进度
    public UI.MainUI.UIType _EnableFunc;
    public string GuideText;//引导按钮文字(IDS)
    /// <summary>
    /// 1,2
    /// </summary>
    public int _CloseUI;
    /// <summary>
    /// 0 - 弱引导， 1 - 强引导
    /// </summary>
    public int _GuideType;
    public int _GuideStar;  // 自动触发箭头引导 0=手动触发箭头，1=自动触发箭头
    public float _DelayTIme;  //引导开始延迟时间(毫秒/1000)
	public bool _IsEnableLvTips;
	public int _EnableLevel;
	public string _FunctionIconName;
	public string _FunctionName;
    public int _NewFunDelayTime;  //新功能开启延时时间
   

}

[Serializable]
public class AwardItem
{
    public int _Vocation;
    public int _PropID;
    public int _PropNum;
}

public class TaskConfigDataBase : ScriptableObject
{
    public TaskConfigData[] _dataTable;
}
