using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UI.MainUI
{
    public class MainUIController:View
    {
        /// <summary>
        /// 主UI控制器
        /// </summary>
        private static MainUIController m_instance = null;
        public static MainUIController Instance{get{return m_instance;}}
        public PanelPrefabData[] PanelPrefabDataList;
        public Dictionary<UIType, BaseUIPanel> m_UIPanelList { get; private set; }

        [HideInInspector]
        public UIType CurrentUIStatus = UIType.Empty;
		[HideInInspector]
		public UIType NextUIStatus = UIType.Empty;

        [HideInInspector]
        public bool IsShowCommonTool = true;//显示顶部通用工具栏


        void Awake()
        {
            m_instance = this;
            RegisterEventHandler();
            m_UIPanelList = new Dictionary<UIType, BaseUIPanel>();
        }

		protected override void RegisterEventHandler()
		{
            //AddEventHandler(EventTypeEnum.OpenItemOperateUI.ToString(), OnOpenItemOperateUI);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.OpentMainUI,OpentMainUIEvent);
            
		}

        void OpentMainUIEvent(object obj)
        {
            UIType uiType = (UIType)obj;
            OpenMainUI(uiType);
			//SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        }
		
        void OnDestroy()
        {
            m_instance = null;
            //RemoveEventHandler(EventTypeEnum.OpenItemOperateUI.ToString(), OnOpenItemOperateUI);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.OpentMainUI, OpentMainUIEvent);
        }

        public void OpenMainUI(UIType type, params object[] value)//打开第一层主UI
        {
          // if(CurrentUIStatus==type)
            //    return;

			//Edit by lee
			Action func = ()=>{
				CurrentUIStatus = type;
				if (type != UIType.Empty)
				{
					NextUIStatus = type;
					BaseUIPanel showPanel = null;
					if (!m_UIPanelList.TryGetValue(type, out showPanel))
					{
						showPanel = GetPanelInstance(type);
						m_UIPanelList.Add(type, showPanel);
					}
					if (!showPanel.IsShow)
					{
						m_UIPanelList.ApplyAllItem(P => P.Value.Close());
						showPanel.Show(value);
					}
				}
				else
				{
					m_UIPanelList.ApplyAllItem(P => P.Value.Close());
				}
			}; 

			if(CrusadeManager.Instance.IsMatchingEctype && (type == UIType.Trial || type== UIType.Defence||type==UIType.Battle))
			{
				MessageBox.Instance.Show(4,"",LanguageTextManager.GetString("IDS_I19_25"),LanguageTextManager.GetString("IDS_I19_22"),LanguageTextManager.GetString("IDS_I19_12"),null,()=> {
					UIEventManager.Instance.TriggerUIEvent(UIEventType.CancelRandomRewardMatching,null);
					func();
				});
			}
			else
			{
				func();
			}						          
        }

        public BaseUIPanel GetPanel(UIType type)
        {
            BaseUIPanel getPanel;
            m_UIPanelList.TryGetValue(type, out getPanel);
            return getPanel;
        }

        BaseUIPanel GetPanelInstance(UIType uitype)
        {
            GameObject getPrefab = PanelPrefabDataList.FirstOrDefault(P=>P.PrefabType == uitype).PanelPrefab;
            if (getPrefab == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"对应Prefab未关联到UIController,UIType:"+uitype); }
            return CreatObjectToNGUI.InstantiateObj(getPrefab,transform).GetComponent<BaseUIPanel>();
        }

        public void CloseAllPanel()
        {
            OpenMainUI(UIType.Empty);
        }


    }

    public enum UIType:byte
    {
        #region NewUIType
        #region Town UI main button
        [EnumDesc(Description="")]
        Empty = 0,
        [EnumDesc(Description = "任务")]
        Task = 1, //任务1
        [EnumDesc(Description = "快速任务按钮")]
        NewbieGuide = 2, //任务按钮
        [EnumDesc(Description = "武功")]
        Skill = 3,//技能1
        [EnumDesc(Description = "行囊")]
        Package = 4, //背包1
        [EnumDesc(Description = "排行")]
        Ranking = 5,    //排行 
        [EnumDesc(Description = "器魂")]
        Gem = 6,  //器魂1
        [EnumDesc(Description = "书信")]
        Mail = 7,  //
         [EnumDesc(Description = "宝树")]
        Treasure = 8,  //
         [EnumDesc(Description = "亲友")]
        Friend = 9,  //
         [EnumDesc(Description = "私聊")]
        Chat = 10,   //
         [EnumDesc(Description = "江湖")]
        Battle = 11,  //
         [EnumDesc(Description = "御敌")]
        Defence = 12,  //
         [EnumDesc(Description = "首领讨伐")]
        Crusade = 13,  //
         [EnumDesc(Description = "无尽试炼")]
        Trial = 14,  //
         [EnumDesc(Description = "聚宝盆")]
        PlayerLuckDraw = 15,   //
         [EnumDesc(Description = "公共商店")]
        Shop = 16,   //
         [EnumDesc(Description = "黑市")]
        Auction = 17,    //
         [EnumDesc(Description = "签到")]
        SignIn = 18,  //
         [EnumDesc(Description = "三财贺喜")]
        Activity = 19,  //
         [EnumDesc(Description = "妖姬")]
        Siren=20,   //
        [EnumDesc(Description = "通用按钮")]
        CommonButton = 21,   //
        [EnumDesc(Description = "系统设置按钮")]
        SystemSetting = 22,   //1
        [EnumDesc(Description = "铸造")]
        Forging = 23,   //
		[EnumDesc(Description = "商行")]
		CarryShop = 24,   //1
        [EnumDesc(Description = "装备")]
        EquipmentUpgrade = 25,   //1
		[EnumDesc(Description = "PVP")]
		PvpUiPanel = 26,
        #endregion

        #region 其他功能从41开始 预留40个号给主界面功能按钮
         [EnumDesc(Description = "队伍")]
        TeamInfo = 41,//队伍
         [EnumDesc(Description = "NPC对话界面")]
        NPCTalkPanel = 42,//NPC对话框
         [EnumDesc(Description = "VIP")]
        TopUp = 43,//充值面板  
         [EnumDesc(Description = "PVP界面")]
        PVPBattle = 44,//pvp战斗
		[EnumDesc(Description = "战斗失败界面")]
		BattleFail = 45,//战斗失败界面
		[EnumDesc(Description = "装备界面(停用)")]
		EquipStrengthen = 46,//装备强化界面1
        //HeroInfo = 41,//人物
        //SocialInfo=42,//社交
        //WorldMap = 43,//世界地图
        //NPCTalkPanel = 45,//NPC对话框
        //UpgradeSkill = 46,//技能升级界面
        //EquipStrengthen = 47,//装备强化界面
        //TeamInfo = 48,//队伍
        //Fashion = 49,//时装
        //SkillMain = 50,  //新版本的技能界面（包括技能升级和技能装配）
        //TreasureChests = 51,//宝箱抽奖UI
        //Meridians = 52,//经脉
        //PVPBattle = 53,//pvp战斗
        //BattleInvite = 54,//封魔副本邀请
        //TopUp = 55,//充值面板        
        //MartialArtsRoom = 56,//练功房
        //TrialsEctypePanel = 57,//试炼副本
        //DailyTaskPanel = 58,//女官任务
        #endregion

        #endregion
    }

    [System.Serializable]
    public class PanelPrefabData
    {
        public UIType PrefabType;
        public GameObject PanelPrefab;
    }

}
