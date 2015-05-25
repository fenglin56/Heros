using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class DailyTaskConfigData
{
    public int _taskID;//任务ID 
    public int _taskType;//类型 1=通关任意普通副本；2=通关任意封魔普通副本；3=通关任意封妖普通副本；4=进入任意试练副本，至少通关1层；5=在武馆领取一次修为；6=强化任意装备；7=炼化任意装备；8=洗炼任意装备；9=升级任意妖女；10=升级任意技能；11=升级任意经脉；12=收获果实；13=在武馆强行突破1次；14=使用仙露；
    public int _triggerCondition;//激活条件 引导步骤
    public string _taskDescription;//任务描述ids
    public int _conditionParameter;//条件参数 完成次数
    public int _activeValue;//可获得的活跃值
    public int _quickTripTo;//引导模块 0=不跳转；1=武馆；2=试练；3=副本（直接打开最新解锁那一页）；4=炼妖；5=锻造；6=技能；7=经脉；8=宝树；

}

public class DailyTaskConfigDataBase : ScriptableObject
{
    public DailyTaskConfigData[] _dataTable;
}
