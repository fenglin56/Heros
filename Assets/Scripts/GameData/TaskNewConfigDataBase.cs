using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class TaskNewConfigData
{
    public int TaskID;//	编号	必填，不能超过4000
    public int TaskSeries;//	任务种类	1=新手任务；2=主线任务；3=支线任务；4=日常任务；5=循环任务；
    public string AcceptGrade;//	接受等级限制	必填，min+max
    public int TaskType;//	任务类型	2=强化任意一件装备、3=购买任意一件道具、4=升级任意一个技能、5=装备任意一个技能、8=装备任意一件装备、11=进行一次果实收取、12=提升到指定等级、19=出售任意道具、20=通关指定副本N次、21=通关指定副本区域内任意副本、22=通关指定类型防守副本、23=升级任意妖女、24=升级任意装备、25=升级任意器魂、26=提升任意装备星级、27=达到指定战力、28=与NPC对话、29=天梯通关到指定层数、30=参与任意世界Boss战斗、31=完成抽奖、32=与剧情人物对话
    public string TaskNeed;//	任务目标	TaskType参数决定目标填写规则；(2)强化任意一件装备=填0、(3)购买任意一件道具=填0、(4)升级任意一个技能=填0、(5)装备任意一个技能=填0、(8)装备任意一件装备=填0、(11)进行一次果实收取=填0、(12)提升到指定等级=填写需求等级；还要填写字段Accept_Lua和ITriggerCallBack_UpgradeLevel、(19)出售任意道具=填0、(20)通关指定副本N次=副本ID、(21)通关指定副本区域内任意副本=副本区域ID、(22)通关指定类型防守副本=防守副本类型、(23)升级任意妖女=填0、(24)升级任意装备=填0、(25)升级任意器魂=填0、(26)提升任意装备星级=填0、(27)达到指定战力=填写需求战力值、(28)与NPC对话=对话NPC的ID、(29)天梯通关到指定层数=到达层数、(30)参与任意世界Boss战斗=填0、(31)完成抽奖=填0、(32)与剧情人物对话=填写剧情人物编号（1~8）
    public int NeedNum;//	任务目标次数	TaskType参数决定目标填写规则；(2)强化任意一件装备=填1、(3)购买任意一件道具=填1、(4)升级任意一个技能=填1、(5)装备任意一个技能=填1、(8)装备任意一件装备=填1、(11)进行一次果实收取=填1、(12)提升到指定等级=填1、(19)出售任意道具=填1、(20)通关指定副本N次=通关次数、(21)通关指定副本区域内任意副本=通关次数、(22)通关指定类型防守副本=通关次数、(23)升级任意妖女=升级次数、(24)升级任意装备=升级个数、(25)升级任意器魂=升级个数、(26)提升任意装备星级=升级个数、(27)达到指定战力=填1、(28)与NPC对话=填1、(29)天梯通关到指定层数=填1、(30)参与任意世界Boss战斗=战斗次数、(31)完成抽奖=抽奖次数、(32)与剧情人物对话=填1
    public int CompleteNum;//	可完成次数限制	填写次数
    public int TaskReset;//	完成次数重置周期	0=不可重置、1=日重置、2=周重置
    public string TaskPre;//	前置任务编号（可以有多个）	预留扩展，暂时填0；支持任务1+任务2
    public int TaskNext;//	后序任务编号	填写后续任务ID，没有填0
    public int SkipCondition;//	跳过任务条件	0=无，1=角色等级，2=没有指定ID道具，3=铜币需求，4=指定妖女未收服，5=没有指定类型道具；6=背包没有任何装备；
    public string SkipParameters;//	跳过任务参数	（0）跳过条件=0，（1）角色等级=角色等级；（2）需要指定ID道具=道具ID+数量；（3）铜币需求=铜币数量；（4）指定妖女未收服=妖女ID；（5）没有指定类型道具=物品种类+物品子类型；（6）背包没有任何装备=0；
    public int SkipGetAward;//	跳过任务后能否获得奖励	0=不能获得；1=能获得；
    public string Accept_Lua;//	接受任务扩展函数名	默认填DefaultEx，任务是提升等级填ITriggerCallBack_UpgradeLevel，跳过条件为等级填ITriggerCallBack_UpgradeLevelSkip，跳过条件为无道具填ITriggerCallBack_NoGoodsSkip
    public string Finish_Lua;//	完成任务执行动作扩展函数名	预留扩展用，暂时填0
    public string Login_Lua;//	登陆扩展	默认填DefaultEx，任务是提升等级填ITriggerCallBack_UpgradeLevel，跳过条件为等级填ITriggerCallBack_UpgradeLevelSkip，跳过条件为无道具填ITriggerCallBack_NoGoodsSkip
    public string AwardItem;//	物品奖励	职业+道具ID+道具数量|职业+道具ID+道具数量
    public int AwardMoney;//	铜币奖励	直接填数值，无填0（前端显示奖励为道具ID3050001）
    public int AwardGold;//	元宝奖励	直接填数值，无填0（前端显示奖励为道具ID3050002）
    public int AwardExp;//	经验奖励	直接填数值，无填0（前端显示奖励为道具ID3050004）
    public int AwardActive;//	活力奖励	直接填数值，无填0（前端显示奖励为道具ID3050003）
    public int AwardXiuwei;//	修为奖励	直接填数值，无填0（前端显示奖励为道具ID3050006）
    [DataToObject(PrefabPath = "Assets/Prefab/GUI/IconPrefab/TaskIcon")]
    public GameObject TaskSeriesPic;//	任务类型图标	填写Prefab文件名，资源在Assets\Prefab\GUI\IconPrefab\TaskIcon文件夹
    public string TaskTitle;//	标题	前端
    public string TaskGoals;//	任务目标（前端显示）	填写IDS
    public string TaskDescription;//	任务描述	填写IDS
    public string GuideText;//	引导按钮文字	填写IDS
    public int CloseUI;//	当前任务开始前关闭所有界面	0表示不关闭，1表示关闭，2表示保留城镇主界面按钮
    [EnumMap]
    public TaskGuideType GuideType;//	引导类型	0=弱引导，1=强引导，
    [EnumMap]
    public TaskStartType GuideStar;//	自动触发箭头引导	0=手动触发箭头，1=自动触发箭头
    public int DelayTime;//	引导开始延迟时间	单位毫秒
    public string GuideGroup;//	获得任务引导箭头组	引导箭头组（NewbieGuideConfig的GuideID），多个用“+”号隔开
    public string CompleteGuideGroup;//	完成任务引导箭头组	引导箭头组（NewbieGuideConfig的GuideID），多个用“+”号隔开
    public string TaskGetSound;//	获得任务播放的音效	音效ID+延迟播放时间（单位毫秒）；0表示不播；
    public string TaskCompleteSound;//	完成任务播放音效	音效ID+延迟播放时间（单位毫秒）；0表示不播；
    public int ButtonProgress;//	按钮开启进度	不能为1
    public int AppearButton;//	新功能开启动画	填写按钮ID，播放动画时显示对应图标
/*   [DataToObject(PrefabPath = "Assets/Prefab/GUI/IconPrefab/TaskIcon")]
    public GameObject FunctionIcon;//	下一个功能图标	填写Prefab文件名，资源在Assets\Prefab\GUI\IconPrefab\TaskIcon文件夹
    */
	public string FunctionName;
    public string TownStroyMusic;//	城镇剧情背景音乐	填写背景音乐名称
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
            Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    //X轴+Z轴+Y轴+模型Prefab（资源在Assets\Prefab\NPC\Prefab）+对白框类型(1=头像左、2=头像右、3=无头像左、4=无头像右、5=旁白)+头像Prefab（资源在Assets\Prefab\GUI\IconPrefab\StroyPersonHead）+名称IDS+对话IDS+人物音效调用资源ID；没有剧情人物填0；
    public StoryPersonInfo StroyPerson01;//	剧情人物1	
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
            Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson02;//	剧情人物2	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
            Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson03;//	剧情人物3	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
            Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson04;//	剧情人物4	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
            Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson05;//	剧情人物5	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
            Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson06;//	剧情人物6	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
            Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson07;//	剧情人物7	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
            Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson08;//	剧情人物8	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
           Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson09;//	剧情人物9	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
           Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson10;//	剧情人物10	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
           Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson11;//	剧情人物11	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
           Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson12;//	剧情人物12	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
           Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson13;//	剧情人物13	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
           Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson14;//	剧情人物14	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
           Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson15;//	剧情人物15	同StroyPerson01
    [CustomerParseAttribute(Length = 10, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/NPC/Prefab", "Assets/Prefab/GUI/IconPrefab/StroyPersonHead" },
           Types = new Type[] { typeof(string), typeof(string), typeof(string), typeof(float), typeof(GameObject), typeof(int), typeof(GameObject), typeof(string), typeof(string), typeof(string) })]
    public StoryPersonInfo StroyPerson16;//	剧情人物16	同StroyPerson01
    public List<AwardItemInfo> AwardItemInfos { get; set; }
	public int ShowLimit;
	public int Link;//跳转到相应界面，对应：linkConfig表//
    /// <summary>
    /// 分解任务开始引导箭头组，对应NewGuideConfigData,严格按顺序
    /// </summary>
    /// <returns></returns>
    public int[] GetStartGuideGroupIds()
    {
        if (GuideGroup != "0")
        {
            var groups = GuideGroup.Split('+');
            int[] guideGroupIDs = new int[groups.Length];
            for (int i = 0; i < groups.Length; i++)
            {
                guideGroupIDs[i] = int.Parse(groups[i]);
            }
            return guideGroupIDs;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 分解任务完成引导箭头组，对应NewGuideConfigData
    /// </summary>
    /// <returns></returns>
    public int[] GetCompleteGuideGroupIds()
    {
        if (CompleteGuideGroup != "0")
        {
            var groups = CompleteGuideGroup.Split('+');
            int[] guideGroupIDs = new int[groups.Length];
            for (int i = 0; i < groups.Length; i++)
            {
                guideGroupIDs[i] = int.Parse(groups[i]);
            }
            return guideGroupIDs;
        }
        else
        {
            return null;
        }
    }
    public StoryPersonInfo GetStoryPersonInfo(int index)
    {
        StoryPersonInfo storyPersonInfo = null;
        switch (index)
        {
            case 0:
                storyPersonInfo = StroyPerson01;
                break;
            case 1:
                storyPersonInfo = StroyPerson02;
                break;
            case 2:
                storyPersonInfo = StroyPerson03;
                break;
            case 3:
                storyPersonInfo = StroyPerson04;
                break;
            case 4:
                storyPersonInfo = StroyPerson05;
                break;
            case 5:
                storyPersonInfo = StroyPerson06;
                break;
            case 6:
                storyPersonInfo = StroyPerson07;
                break;
            case 7:
                storyPersonInfo = StroyPerson08;
                break;
            case 8:
                storyPersonInfo = StroyPerson09;
                break;
            case 9:
                storyPersonInfo = StroyPerson10;
                break;
            case 10:
                storyPersonInfo = StroyPerson11;
                break;
            case 11:
                storyPersonInfo = StroyPerson12;
                break;
            case 12:
                storyPersonInfo = StroyPerson13;
                break;
            case 13:
                storyPersonInfo = StroyPerson14;
                break;
            case 14:
                storyPersonInfo = StroyPerson15;
                break;
            case 15:
                storyPersonInfo = StroyPerson16;
                break;
        }
        return storyPersonInfo;
    }
    public TaskAwardInfo[] GetTaskAwardInfo()
    {
        List<TaskAwardInfo> taskAwardInfos = new List<TaskAwardInfo>();
        if (AwardItem != "0")
        {
            var vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            string[] vocas = AwardItem.Split('|');
            for (int i = 0; i < vocas.Length; i++)
            {
                string[] details = vocas[i].Split('+');
                if (details[0] == vocation.ToString())
                {
                    taskAwardInfos.Add(new TaskAwardInfo() { GoodsId = int.Parse(details[1]), AwardAmount = int.Parse(details[2]) });
                }
            }
        }
        if (AwardMoney != 0)
        {
            taskAwardInfos.Add(new TaskAwardInfo() { GoodsId = 3050001, AwardAmount = AwardMoney });
        }
        if (AwardGold != 0)
        {
            taskAwardInfos.Add(new TaskAwardInfo() { GoodsId = 3050002, AwardAmount = AwardGold });
        }
        if (AwardExp != 0)
        {
            taskAwardInfos.Add(new TaskAwardInfo() { GoodsId = 3050004, AwardAmount = AwardExp });
        }
        if (AwardActive != 0)
        {
            taskAwardInfos.Add(new TaskAwardInfo() { GoodsId = 3050003, AwardAmount = AwardActive });
        }
        if (AwardXiuwei != 0)
        {
            taskAwardInfos.Add(new TaskAwardInfo() { GoodsId = 3050006, AwardAmount = AwardXiuwei });
        }

        return taskAwardInfos.ToArray();
    }
}
public class TaskNewConfigDataBase : ConfigBase
{
    public TaskNewConfigData[] Datas;

    public override void Init(int length, object dataList)
    {
        Datas = new TaskNewConfigData[length];

        var realData = dataList as List<TaskNewConfigData>;
        for (int i = 0; i < realData.Count; i++)
        {
            Datas[i] = (TaskNewConfigData)realData[i];
            Datas[i].AwardItemInfos = new List<AwardItemInfo>();
            if(Datas[i].AwardItem!="0")
            {
                var awardItems = Datas[i].AwardItem.Split('|');
                awardItems.ApplyAllItem(P => Datas[i].AwardItemInfos.Add(new AwardItemInfo(P)));
            }
        }
    }
    public TaskNewConfigData GetTaskNewConfigData(int taskId)
    {
        return Datas.SingleOrDefault(P => P.TaskID == taskId);
    }
}
public enum TaskGuideType
{
    Weak = 0,   //弱引导
    Enforce = 1, //强引导
    None=2,  //无引导
}
public enum TaskStartType
{
    /// <summary>
    /// 手动引导
    /// </summary>
    Manual,
    /// <summary>
    /// 自动引导
    /// </summary>
    Auto,
}
public class TaskAwardInfo
{
    public int GoodsId;
    public int AwardAmount;
}