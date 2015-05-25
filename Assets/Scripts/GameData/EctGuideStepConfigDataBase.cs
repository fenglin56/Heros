using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class EctGuideStepConfigData
{
    public int StepID;//	步骤ID	唯一的
    /// <summary>
    /// 步骤类型	0=玩家查看对白；1=移动至指定范围；2=所有的怪物死亡；3=施放指定技能；4=增加1格气力；5=将敌人切碎；6=自己成功技能打断；7=点击指定按钮完成引导；8=指定技能击中怪物；9=图片引导；
    /// </summary>
    public int StepType;//	步骤类型	0=玩家查看对白；1=移动至指定范围；2=所有的怪物死亡；3=施放指定技能；4=增加1格气力；5=将敌人切碎；6=自己成功技能打断；7=减速引导；8=指定技能击中怪物；9=图片引导；
    /// <summary>
    /// 完成步骤需求	Type=0填0；+Type=1填X+Y+最小范围；+Type=2填0；+Type=3填男主角技能ID+女主角技能ID；+Type=4填0；+Type=5填0；+Type=6填0；
    /// 减速引导时需要读 技能ID
    /// </summary>
    public string StepNeed;//	完成步骤需求	Type=0填0；+Type=1填X+Y+最小范围；+Type=2填0；+Type=3填男主角技能ID+女主角技能ID；+Type=4填0；+Type=5填0；+Type=6填0；（选择指定按钮ID）
    public int SlowMonitorType;//	减速监听类型	填0不开启监听，填1则开启技能监听，填2则开启破防监听；
    public string SlowMonitorCondition;//	减速监听条件	当SlowMonitorType=1时，填写技能ID，不同职业用“+”隔开；当SlowMonitorType=2时，不读取该字段，填0； 子弹Id打中 SlowMonitorTarget 怪物ID
    public int SlowMonitorTarget;//	减速监听目标	填写监听的目标信息，当SlowMonitorType=1时，填写怪物ID，SlowMonitorType=2时不读取该字段；
    public string SlowMonitorFinishTarget;//减速完成条件 （填写技能ID）不同职业用+分开

    public float RetardDelayTime;//	减速开始的延迟时间	单位毫秒 减速参数1
    public float DecelerationRate;//	减速倍数	type=7时填写，步骤减速的倍数，不能为0  减速参数2
    public float StepDuration;//	步骤持续时间	type=7时填写，步骤的持续时间，单位毫秒；  减速参数3

    [DataSplitToObjectArray(PrefabPath = "Assets/Effects/Prefab", SplitChar = '+', CustomerType = typeof(int))]
    public int[] ButtonShielding;//	步骤开启的按钮	填写按钮ID，没有填写的按钮无法点击，多个用+隔开  （该步骤非减速要启用的按钮）
    [DataSplitToObjectArray(PrefabPath = "Assets/Effects/Prefab", SplitChar = '+', CustomerType = typeof(int))]
    public int[] SlowMonitorButtonShielding;//	减速过程开启的按钮	填写按钮ID，没有填写的按钮无法点击，多个用+隔开 （该步骤减速下要启用的按钮）

    public string SignTips;//	引导Tips文字	填写IDS
    [DataSplitToCustomerObjectArrayAttribute( CustomerType=typeof(TipsType), ParSplitChar='|', Length = 2, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Effects/Prefab"},
            Types = new Type[] { typeof(int), typeof(GameObject)})]
    public TipsType[] TipsPrefab;//	引导Tips文字动画	填写Prefab，0表示无Tips弹出
    public string TipsPrefabOffset;//	引导tips资源偏移	填写偏移量，A+X+Z

    public string SlowMonitorSignTips;//	减速过程的引导Tips文字	填写IDS
    [DataSplitToCustomerObjectArrayAttribute(CustomerType = typeof(TipsType), ParSplitChar = '|', Length = 2, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Effects/Prefab" },
            Types = new Type[] { typeof(int), typeof(GameObject) })]
    public TipsType[] SlowMonitorTipsPrefab;//	减速过程的引导Tips文字动画	填写Prefab，0表示无Tips弹出
    public string SlowMonitorTipsPrefabOffset;//	减速过程的引导tips资源偏移	填写偏移量，A+X+Z

    public int GuideButton;//	按钮提示	按钮ID   
	//public Dictionary<int,GameObject> buttonEffectMap = new Dictionary<int, GameObject>();
	[DataSplitToCustomerObjectArrayAttribute(CustomerType = typeof(TipsType), ParSplitChar = '|', Length = 2, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/GUI/NewbieGuide/Prefab" },
	Types = new Type[] { typeof(int), typeof(GameObject) })]
	public TipsType[] ButtonEffect;
//    [DataToObject(PrefabPath = "Assets/Prefab/GUI/NewbieGuide/Prefab")]
//    public GameObject ButtonEffect;//	按钮提示特效	填写Prefab，0表示无特效
    public int ButtonEffectInterval;//	按钮引导间隔	按钮引导动画的播放间隔，单位毫秒

    public int SlowMonitorGuideButton;//	减速过程的按钮提示	按钮ID
	[DataSplitToCustomerObjectArrayAttribute(CustomerType = typeof(TipsType), ParSplitChar = '|', Length = 2, SplitChar = '+', InitMethodName = "InitData", PrefabPath = new string[] { "Assets/Prefab/GUI/NewbieGuide/Prefab" },
	Types = new Type[] { typeof(int), typeof(GameObject) })]
	public TipsType[] SlowMonitorButtonEffect;
//    [DataToObject(PrefabPath = "Assets/Prefab/GUI/NewbieGuide/Prefab")]
//    public GameObject SlowMonitorButtonEffect;//	减速过程的按钮提示特效	填写Prefab，0表示无特效
    public int SlowMonitorButtonEffectInterval;//	减速过程的按钮引导间隔	按钮引导动画的播放间隔，单位毫秒   

    public string GuideDialog;//	引导界面文字	填写格式：A+【镜头X坐标】+【镜头Y坐标】+【镜头Z坐标】+【镜头移动时间_毫秒】#【对白Id1】+【对白Id2】+【对白Id……】，对话内容填写【头像】&【名字】&【对话内容】&【偏移量X】&【偏移量Y】&【是否主角】，一个镜头参数和多句对话内容为一组，多组参数用“|”隔开；当镜头参数为A+0+0+0时，镜头不移动，当镜头参数为A+1+1+1时，镜头移动到主角的位置；
    public int CamraBackTime;//	对话镜头回来的时间	对话结束后镜头回来的时间，单位毫秒   

    [DataToObject(PrefabPath = "Assets/Effects/Prefab")]
    public GameObject GuideEffect;//	引导地表特效	填写Prefab，0表示不播特效
    public string GuideEffectPos;//	引导地表特效位置	X+Y+Z；相对目标位置的偏移量，Y为高度
    public int EffectAngle;//	引导地表特效偏移角度	单位角度°
    [DataToObject(PrefabPath = "Assets/Effects/Prefab")]
    public GameObject SlowMonitorGuideEffect;//	减速过程的引导地表特效	填写Prefab，0表示不播特效
    public string SlowMonitorGuideEffectPos;//	减速过程的引导地表特效位置	X+Y+Z；相对目标位置的偏移量，Y为高度
    public int SlowMonitorEffectAngle;//	减速过程的引导地表特效偏移角度	单位角度°

    [DataToObject(PrefabPath = "Assets/Effects/Prefab")]
    public GameObject MonsterEffect;//	怪物特效	填写Prefab，0表示不播特效
    public int MountMonster;//	挂载参数	挂载类型为3，填怪物ID；其他填0
    [DataToObject(PrefabPath = "Assets/Effects/Prefab")]
    public GameObject SlowMonitorMonsterEffect;//	减速过程的怪物特效	填写Prefab，0表示不播特效
    public int SlowMonitorMountMonster;//	挂载参数	挂载类型为3，填怪物ID；其他填0
    [DataSplitToObjectArray(PrefabPath = "Assets/Prefab/GUI/IconPrefab/NewbieGuidePic", SplitChar = '+', CustomerType = typeof(GameObject))]
    public GameObject[] GuidePicture;//	引导图片	填写资源ID，多个用+隔开
    public string StepSound;//	步骤音效	填写音效ID

    public int MountType;//	方向引导箭头的目标类型	0=不显示箭头，1=地图绝对位置，2=怪物
    public string TargetInformation;//	目标信息	目标类型=0时，填0；目标类型=1时，填写地图坐标；目标类型=2时，填写怪物ID；

    public int SlowMonitorMountType;//	减速过程的方向引导箭头的目标类型	0=不显示箭头，1=地图绝对位置，2=怪物
    public string SlowMonitorTargetInformation;//	目标信息	目标类型=0时，填0；目标类型=1时，填写地图坐标；目标类型=2时，填写怪物ID；

    [HideInDataReader]
    public GuideDialogInfo[] GuideDialogInfos;
    [HideInDataReader]
    public Vector3 _EffectPos;
    [HideInDataReader]
    public Vector3 _SlowMotionEffectPos;
    [HideInDataReader]
    public Vector3 TipsPrefabOffsetVec;
    [HideInDataReader]
    public Vector3 SlowMotionTipsPrefabOffsetVec;

    public Vector3 StringToVector3(string data)
    {
        Vector3 val = Vector3.zero;
        if (data != "0")
        {
            var posDatas = data.Split('+');
            val= new Vector3(float.Parse(posDatas[0]), float.Parse(posDatas[1]), float.Parse(posDatas[2]));
        }
        return val;
    }
}

public class EctGuideStepConfigDataBase : ConfigBase
{
    public EctGuideStepConfigData[] Datas;

    public override void Init(int length, object dataList)
    {
        Datas = new EctGuideStepConfigData[length];

        var realData = dataList as List<EctGuideStepConfigData>;
        for (int i = 0; i < realData.Count; i++)
        {
            Datas[i] = (EctGuideStepConfigData)realData[i];
            var dialog=Datas[i].GuideDialog;
            if (dialog != "0")
            {
               // Debug.Log(dialog);
                var guideDialogInfos = Datas[i].GuideDialog.Split('|');  //阖掉A+ 然后再分割字符串
                if (guideDialogInfos.Length > 0)
                {
                    Datas[i].GuideDialogInfos = new GuideDialogInfo[guideDialogInfos.Length];
                    for (int j = 0; j < guideDialogInfos.Length; j++)
                    {
                        Datas[i].GuideDialogInfos[j] = new GuideDialogInfo(guideDialogInfos[j].Substring(2));
                    }
                }
            }
            else
            {
                Datas[i].GuideDialogInfos = null;
            }
            Datas[i]._EffectPos = Datas[i].StringToVector3(Datas[i].GuideEffectPos);
            Datas[i]._SlowMotionEffectPos = Datas[i].StringToVector3(Datas[i].SlowMonitorGuideEffectPos);
            if(Datas[i].TipsPrefabOffset!="0")
            {
                string[] tipsPrefabOffset =  Datas[i].TipsPrefabOffset.Split('+');
                Datas[i].TipsPrefabOffsetVec = new Vector3(float.Parse(tipsPrefabOffset[1]), float.Parse(tipsPrefabOffset[2]));
            }
            if (Datas[i].SlowMonitorTipsPrefabOffset != "0")
            {
                string[] tipsPrefabOffset = Datas[i].SlowMonitorTipsPrefabOffset.Split('+');
                Datas[i].SlowMotionTipsPrefabOffsetVec = new Vector3(float.Parse(tipsPrefabOffset[1]), float.Parse(tipsPrefabOffset[2]));
            }
        }
    }
}

[Serializable]
public class GuideDialogInfo
{
    public Vector3 CameraPos;
    public int CameraMoveTime;
    public int[] StepTalkIds;    
    public GuideDialogInfo(string guideDialogInfo)
    {
        var dialog = guideDialogInfo.Split('#');
        var info=dialog[0].Split('+');
        CameraPos = new Vector3(int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[2]));
        CameraMoveTime = int.Parse(info[3]);
        if (dialog.Length > 1)
        {
            info = dialog[1].Split('+');
            StepTalkIds=new int[info.Length];
            for(int i=0;i<info.Length;i++)
            {
                StepTalkIds[i] = int.Parse(info[i]);
            }
        }
    }
}
[Serializable]
public class TipsType
{
    public int Vocation;
    public GameObject TipsPrefab;
    public void InitData(int k, object value)
    {
        switch (k)
        {
            case 0:
                Vocation = int.Parse(value.ToString());
                break;
            case 1:
                TipsPrefab = value as GameObject;
                break;
        }
    }
}