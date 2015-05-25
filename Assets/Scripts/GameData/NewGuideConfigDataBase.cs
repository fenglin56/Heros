using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UI.MainUI;

[Serializable]
public class NewGuideConfigData
{
    public int	GuideID	;//	引导ID	Task触发的引导ID
    [EnumMap]
    public GuideConfigType	GuideType	;//	引导方式	0=普通箭头，1=副本智能选定，3=寻路引导，4=智能寻找背包物品和技能；5=打开指定界面；6=自动寻路至剧情人物并触发城镇剧情对话；
    public string	GuideBtnID	;//	按钮ID	（0）普通箭头=按钮ID；（1）副本智能选定=副本按钮ID；（3）寻路引导=寻路NPC的ID；（4）智能寻找背包物品=物品ID,用职业ID+物品ID|职业ID+技能ID的方式来填写，如果是智能寻找技能就用技能ID，用职业ID+技能ID|职业ID+技能ID；（5）打开指定界面=界面按钮ID；（6）自动寻路至剧情人物并触发城镇剧情对话=填0；
	public string	BtnSignText	;//	触摸文字ID	箭头引导文本框文字IDS
    [DataToObject(PrefabPath="Assets/Prefab/GUI/NewbieGuide/Prefab")]
    public GameObject	BtnSignPrefab	;//	提示框	提示框和箭头资源Prefab文件名上右下左，引导方式为3时为拖动的手的资源（资源在Assets\Prefab\GUI\NewbieGuide\Prefab文件夹内）
    public string BtnSignOffset;//	偏移量	提示框资源Prefab中心点与按钮中心点坐标偏移量，填写格式：A+X轴+Y轴+Z轴；
    public string BtnPositionOffset;//	按钮位置偏移	按钮特效显示位置偏移，填写格式：A+X轴+Y轴+Z轴；
    public float	HighlightScale	;//	按钮显示半径	按钮提示亮光大小比例
    [DataToObject(PrefabPath = "Assets/Prefab/GUI/NewbieGuide/Prefab")]
    public GameObject	HighlightRes	;//	按钮高亮资源	引导按钮高亮显示的美术资源，当引导类型为2时，格式为：起始位置资源名+终点位置资源名（资源在Assets\Prefab\GUI\NewbieGuide\Prefab文件夹内）
    public int	SkipRole	;//	是否可以跳过	0=不可终止；1-6为终止条件：1=当前身上所有的铜币小于背包中第一个装备强化需求的铜币；或者背包中第一个不是武器装备；2=身上所有的铜币小于技能栏第一个技能升级需要的铜币；3=当天的剩余体力小于8； 4=身上的铜币小于100；5=身上的元宝数小于10；6=当天的PVP次数为0；7=当前选中的果实已经成熟；8=当前选择的按钮不存在；9=第一个妖女已经解锁；10=系统菜单已经展开；11=没有显示NPC对话框；12=没有显示背包界面；13=没有显示技能界面；14=没有显示炼妖界面；15=没有显示强化界面；16=没有显示宝树界面；17=没有显示武馆界面；18=没有显示经脉界面；19=没有显示时装界面；
    public int	OverRole	;//	是否要终节引导	0=不可终止；1-6为终止条件：1=当前身上所有的铜币小于背包中第一个装备强化需求的铜币；或者背包中第一个不是武器装备；2=身上所有的铜币小于技能栏第一个技能升级需要的铜币；3=当天的剩余体力小于8； 4=身上的铜币小于100；5=身上的元宝数小于10；6=当天的PVP次数为0；7=当前选中的果实已经成熟；
    public string	StroyPos	;//	剧情对话时玩家所在坐标	填写角色与NPC的偏移值：X+Z
    public string StroyCamera;//	剧情对话时镜头坐标偏移	填写X轴+Z轴+Y轴
    public string	StroyCameraTarget	;//	剧情对话时镜头目标点坐标偏移	填写A+X轴+Y轴+Z轴
    public string TalkID;//	对话的ID	从对话表中把ID加到一起去即可 Id+Id+Id


    /// <summary>
    /// 引导所属的任务配置信息，动态赋值
    /// </summary>
    public TaskNewConfigData TaskNewConfigData { get; set; }
    
    /// <summary>
    /// 分解任务开始引导箭头组，对应NewGuideConfigData,严格按顺序
    /// </summary>
    /// <returns></returns>
    public int[] GetTalkIDIds()
    {
        if (TalkID != "0")
        {
            var groups = TalkID.Split('+');
            int[] talkIdsIDs = new int[groups.Length];
            for (int i = 0; i < groups.Length; i++)
            {
                talkIdsIDs[i] = int.Parse(groups[i]);
            }
            return talkIdsIDs;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 如果是剧情对话引导，这里保存的是该引导的对话信息
    /// </summary>
    public TalkIdConfigData[] TalkIdConfigDatas { get; set; }        
}
public class NewGuideConfigDataBase : ConfigBase
{
    public NewGuideConfigData[] Datas;

    public override void Init(int length, object dataList)
    {
        Datas = new NewGuideConfigData[length];

        var realData = dataList as List<NewGuideConfigData>;
        for (int i = 0; i < realData.Count; i++)
        {
            Datas[i] = (NewGuideConfigData)realData[i];
        }
    }
    /// <summary>
    /// 根据TaskConfig的GroupIds，找到NewGuideConfigData列表
    /// </summary>
    /// <param name="groupIds"></param>
    /// <returns></returns>
    public TaskGuideExtendData[] GetNewGuideConfigDataByGroup(int[] groupIds)
    {
        if (groupIds == null) return null;

        TaskGuideExtendData[] taskGuideExtendData = new TaskGuideExtendData[groupIds.Length];

        for (int i = 0; i < groupIds.Length; i++)
        {
            taskGuideExtendData[i] =new TaskGuideExtendData(Datas.Single(P => P.GuideID == groupIds[i]));
        }
        return taskGuideExtendData;
    }    
}
public enum GuideConfigType
{
    /// <summary>
    /// 普通箭头
    /// </summary>
    NormalArrow=0,
    FindEctype=1,
    Drag=2,
    NavPath=3,
    FindMaterial=4,
    /// <summary>
    /// 打开指定界面
    /// </summary>
    OpenSpecUI=5,
    /// <summary>
    /// 触发城镇剧情
    /// </summary>
    TownStory=6,
    FindTask=7,
	FindEquipPosition = 8,
	FindIntellSiren = 9,//智能查找妖女
}
public class TaskGuideExtendData
{
    public NewGuideConfigData NewGuideConfigDatas { get; private set; }
    public TaskGuideExtendData(NewGuideConfigData newGuideConfigData)
    {
        NewGuideConfigDatas = newGuideConfigData;
        switch (NewGuideConfigDatas.GuideType)
            {
                case GuideConfigType.FindTask:
                case GuideConfigType.FindEctype:
                case GuideConfigType.NormalArrow:
				case GuideConfigType.FindEquipPosition:
				case GuideConfigType.FindIntellSiren:
                    MappingId = int.Parse(NewGuideConfigDatas.GuideBtnID);
                    break;
                case GuideConfigType.FindMaterial:
                    var playerDataStruct = (IPlayerDataStruct)PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct;
                    int vocationId = playerDataStruct.GetCommonValue().PLAYER_FIELD_VISIBLE_VOCATION;
                    string[] vocas = NewGuideConfigDatas.GuideBtnID.Split('|');
                    MappingCategory = TaskBtnManager.Instance.GetUITypeByBtnId(vocas[0]); 
                    for (int i = 1; i < vocas.Length; i++)
                    {
                        string[] details = vocas[i].Split('+');
                        if (details[0] == vocationId.ToString())
                        {
                            MappingId = int.Parse(details[1]);
                            break;
                        }
                    }
                    
                    break;
            }        
    }
    /// <summary>
    /// 引导完成标记
    /// </summary>
    public bool FinishFlag { get; set; }
    /// <summary>
    /// 映射Id,在UI的按钮注册到TaskBtnManager时候，会根据这个MappingId找到该TaskGuide引导数据，并置IsGuideUIReady为true,并触发GuideBtnUIReadyAct委托
    /// 在该引导结束或中断的时候重置回来
    /// </summary>
    public int MappingId { get; private set; }
    public UIType MappingCategory { get; private set; }
    /// <summary>
    /// 该引导步骤UI装备完毕的Action
    /// </summary>
    public Action<bool> GuideBtnUIReadyAct;
    private bool m_isGuideUIReady = false;
    public bool IsGuideUIReady
    {
        get
        {
			if (NewGuideConfigDatas.GuideType == GuideConfigType.TownStory || NewGuideConfigDatas.GuideType == GuideConfigType.FindIntellSiren
			    || NewGuideConfigDatas.GuideType == GuideConfigType.OpenSpecUI)
            {
                m_isGuideUIReady = true;
            }
            if (!m_isGuideUIReady)
            {
                if (NewGuideConfigDatas.GuideType == GuideConfigType.FindMaterial)
                {
                    m_isGuideUIReady = TaskBtnManager.Instance.FindGuideBtnParamViaMappingId(MappingCategory, MappingId) != null;
                }
                else
                {
                    m_isGuideUIReady = TaskBtnManager.Instance.FindGuideBtnParamViaMappingId(MappingId) != null;
                }
            }
            return m_isGuideUIReady;
        }
        set
        {
            m_isGuideUIReady = value;            
            if (GuideBtnUIReadyAct != null)
            {
                GuideBtnUIReadyAct(value);
            }
        }
    }    
}