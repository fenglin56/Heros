using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class DailyTaskRewardConfigData
{
    public int _boxSequence;//宝箱序号
    public string _boxDisplayId;//宝箱资源
    public int _requirementActiveValue;//开启需求活跃度
    
    //public int[] _awardItem_profession;//奖励对应职业
    //public int[] _awardItem_prop;// 奖励物品id
    //public int[] _awardItem_num;// 奖励物品数量

    public int[] _awardType;//奖励类型
    
    public AwardItem[] _awardItem;

    public int _awardMoney;//金钱奖励
    public int _awardExp;//经验奖励
    public int _awardActive;//活力奖励
    public int _awardXiuwei;//修为奖励
    public int _awardIngot;//元宝奖励


    [Serializable]
    public class AwardItem
    {
        public int Profession;
        public int PropID;
        public int Num;
    }
}




public class DailyTaskRewardConfigDataBase : ScriptableObject
{
    public DailyTaskRewardConfigData[] _dataTable;
}
