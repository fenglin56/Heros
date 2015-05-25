using UnityEngine;
using System.Collections;
using System.Linq;
using UI.MainUI;

/// <summary>
/// 防守副本入口脚本，挂在Prefab_DefenceEctypeList预设件上
/// 负责读表，装备数据，呈现副本按钮，“开始挑战”指令。
/// </summary>
using System;


public class DefenceEctypeListBehaviour : MonoBehaviour {

	public GameObject DefenceEctypeEnableItemPrefab;
	public GameObject DefenceEctypeDisableItemPrefab;

	public GameObject[] EnableItemPoints;
    public GameObject[] DisableItemPoints;
	public SingleButtonCallBack ChallengeBtn;

	private DefenceEctypeEnableItemBehaviour m_selectedDefenceItem;

	private DefenceEctypeEnableItemBehaviour[] m_enableItems=new DefenceEctypeEnableItemBehaviour[3];
	private SpriteSwith m_challengeBtnSpriteSwitch;
    private TweenPosition m_posAnim;
	void Awake()
	{
        m_posAnim = GetComponent<TweenPosition>();
		m_challengeBtnSpriteSwitch=ChallengeBtn.GetComponentInChildren<SpriteSwith>();
		//按下/抬起 效果切换
		ChallengeBtn.SetPressCallBack(flag=>{if(ChallengeBtn.Enable)m_challengeBtnSpriteSwitch.ChangeSprite(flag?2:1);});
        
		ChallengeBtn.SetCallBackFuntion(obj=>
		                                {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_DefenceLevelStart");
            //var selectEct = EctypeConfigManager.Instance.EctypeSelectConfigList.Values.SingleOrDefault(P => P._vectContainer.Contains(m_selectedDefenceItem.EctypeContainerData.lEctypeContainerID));
            int remainNum = 0;
            int.TryParse(m_selectedDefenceItem.RemainNum.text, out remainNum);
			//挑战防守副本，判断体力和次数
            if (remainNum <= 0)
                {
                    //次数用完
                    UI.MessageBox.Instance.ShowTips(1, LanguageTextManager.GetString("IDS_I15_3"), 1);
                }
                else if (NotEnoughtActivity(int.Parse(m_selectedDefenceItem.ConsumeNum.text)))
                {
                    //体力不足,接入副本中的体力不足
                    PopupObjManager.Instance.ShowAddVigour();
                }
                else
                {
					Action action = ()=>{
						JudgeAndExitTeam(); //add by lee
						SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS = new SMSGEctypeRequestCreate_CS()
						{
							dwEctypeContainerID = m_selectedDefenceItem.EctypeContainerData.lEctypeContainerID
						};
						TraceUtil.Log(SystemModel.Jiang, "发送加入副本请求：" + sMSGEctypeRequestCreate_CS.dwEctypeContainerID);
						NetServiceManager.Instance.EctypeService.SendEctypeGuideCreate(sMSGEctypeRequestCreate_CS);
					};

					if(TeamManager.Instance.IsTeamExist())
					{
						TeamManager.Instance.ShowLeaveTeamTip(action);
					}
					else
					{
						action();
					}
                }
		});
        TaskGuideBtnRegister();
    }

	private void JudgeAndExitTeam()
	{
		if(TeamManager.Instance.IsTeamExist())
		{
			var playerData = PlayerManager.Instance.FindHeroDataModel();
			var teamSmg = TeamManager.Instance.MyTeamProp;
			if(playerData.ActorID == teamSmg.TeamContext.dwCaptainId)
			{
				NetServiceManager.Instance.TeamService.SendTeamDisbandMsg(new SMsgTeamDisband_CS{
					dwActorID = (uint)playerData.ActorID,
					dwTeamID = teamSmg.TeamContext.dwId
				});
			}
			else
			{
				NetServiceManager.Instance.TeamService.SendTeamMemberLeaveMsg(new SMsgTeamMemberLeave_SC(){
					dwActorID = (uint)playerData.ActorID,
					dwTeamID = teamSmg.TeamContext.dwId
				});
			}
		}
	}

    private bool NotEnoughtActivity(int consumeAct)
    {
        var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
        return m_HeroDataModel.PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE < consumeAct;
    }
    /// <summary>
    /// 引导按钮注入代码
    /// </summary>
    private void TaskGuideBtnRegister()
    {
        ChallengeBtn.gameObject.RegisterBtnMappingId(UIType.Defence, BtnMapId_Sub.Defence_GotoFight);
    }
    public void Init()
    {
        Init(null);
    }
	public void Init(int? selectedEctype)
	{
        m_selectedDefenceItem = null;
        m_posAnim.Reset();
        m_posAnim.Play(true);
		EnableItemPoints.ApplyAllItem(P => P.transform.ClearChildImmediate());
		DisableItemPoints.ApplyAllItem(P => P.transform.ClearChildImmediate());

        var heroPlayerDataModel=PlayerManager.Instance.FindHeroDataModel();
		//副本类型为8  指防守副本
		var defenceEctypes=EctypeConfigManager.Instance.EctypeContainerConfigFile.ectypeContainerDataList.Where(P=>P.lEctypeType==8);
        var playerLev = heroPlayerDataModel.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
		//获得能挑战的防守副本
		var sortedList=defenceEctypes.ToList();

		sortedList.Sort((x,y)=>{
			if(x.lMinActorLevel==y.lMinActorLevel)
			{
				return 0;
			}
			else if(x.lMinActorLevel<y.lMinActorLevel)
			{
				return 1;
			}
			else
			{
				return -1;
			}
			//return x.lMinActorLevel<y.lMinActorLevel?1:-1;
		});
//		for(int i=0;i<sortedList.Count;i++)
//		{
//			Debug.Log(sortedList[i].lMinActorLevel);
//		}
		EctypeContainerData[] canChallengeEctypes=new EctypeContainerData[3];

		//EctypeConfigManager.Instance.EctypeSelectConfigList.Values.ApplyAllItem(P=>P.DefenceChallengeRemainNum=2);

		canChallengeEctypes[0]=sortedList.FirstOrDefault(P=>P.lMinActorLevel<=playerLev&&P.lEctypePos[2]=="1");
		canChallengeEctypes[1]=sortedList.FirstOrDefault(P=>P.lMinActorLevel<=playerLev&&P.lEctypePos[2]=="2");
		canChallengeEctypes[2]=sortedList.FirstOrDefault(P=>P.lMinActorLevel<=playerLev&&P.lEctypePos[2]=="3");
        
		for(int i=0;i<3;i++)
		{
			if(canChallengeEctypes[i]==null) 
				continue;
			var enableItem=NGUITools.AddChild(EnableItemPoints[i], DefenceEctypeEnableItemPrefab);
			var itemBehaviour=enableItem.GetComponent<DefenceEctypeEnableItemBehaviour>();
			itemBehaviour.Init(canChallengeEctypes[i]);

            #region 引导代码
            BtnMapId_Sub defence_Stage = BtnMapId_Sub.Defence_Stage1;
            int remainNum = 0;
            switch (i)
            {
                case 0:
                    defence_Stage = BtnMapId_Sub.Defence_Stage1;
                    remainNum = heroPlayerDataModel.PlayerValues.PLAYER_FIELD_EXPDEFIEND_VALUE;
                    break;
                case 1:
                    defence_Stage = BtnMapId_Sub.Defence_Stage2;
                    remainNum = heroPlayerDataModel.PlayerValues.PLAYER_FIELD_COINDEFINED_VALUE;
                    break;
                case 2:
                    defence_Stage = BtnMapId_Sub.Defence_Stage3;
                    remainNum = heroPlayerDataModel.PlayerValues.PLAYER_FIELD_GOLDDEFINED_VALUE;
                    break;
            }
            itemBehaviour.CallBackBtn.gameObject.RegisterBtnMappingId(UIType.Defence, defence_Stage);
            #endregion


            //var selectEct=EctypeConfigManager.Instance.EctypeSelectConfigList.Values.SingleOrDefault(P=>P._vectContainer.Contains(canChallengeEctypes[i].lEctypeContainerID));
            if (m_selectedDefenceItem == null && remainNum > 0)
			{
				itemBehaviour.SetFocus(true);
				m_selectedDefenceItem=itemBehaviour;
			}
			else
			{
				itemBehaviour.SetFocus(false);
			}
			m_enableItems[i]=itemBehaviour;
			//监听点击事件处理
			itemBehaviour.CallBackAct=DefenceItemClick;
		}
        if (selectedEctype != null)
        {
            for (int m = 0; m < m_enableItems.Length; m++)
            {
                if (m_enableItems[m] != null && m_enableItems[m].EctypeContainerData.lEctypeContainerID == selectedEctype.Value)
                {
                    if (m_selectedDefenceItem != null)
                    {
                        m_selectedDefenceItem.SetFocus(false);
                    }
                    m_selectedDefenceItem = m_enableItems[m];
                    m_selectedDefenceItem.SetFocus(true);
                    break;
                }
            }
        }
		//没有一个副本可以挑战，按钮不可用
        if (m_selectedDefenceItem == null)
        {
            ChallengeBtn.Enable = false;
            //置灰
            m_challengeBtnSpriteSwitch.ChangeSprite(3);
        }
        else
        {
            ChallengeBtn.Enable = true;
            m_challengeBtnSpriteSwitch.ChangeSprite(1);
        }


		//下一级挑战副本
		EctypeContainerData[] CntnChallengeEctypes=new EctypeContainerData[3];

		CntnChallengeEctypes[0]=sortedList.LastOrDefault(P=>P.lMinActorLevel>playerLev&&P.lEctypePos[2]=="1");
		CntnChallengeEctypes[1]=sortedList.LastOrDefault(P=>P.lMinActorLevel>playerLev&&P.lEctypePos[2]=="2");
		CntnChallengeEctypes[2]=sortedList.LastOrDefault(P=>P.lMinActorLevel>playerLev&&P.lEctypePos[2]=="3");

		for(int i=0;i<3;i++)
		{
			if(CntnChallengeEctypes[i]==null) 
				continue;
			var enableItem=NGUITools.AddChild(DisableItemPoints[i], DefenceEctypeDisableItemPrefab);
			var itemBehaviour=enableItem.GetComponent<defenceEctypedisableItemBehaviour>();
			itemBehaviour.Init(CntnChallengeEctypes[i]);
		}

	}
	
	private void DefenceItemClick(DefenceEctypeEnableItemBehaviour clickItem)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_Button_DefenceLevelSelect");
        m_enableItems.ApplyAllItem(P => { if (P != null)P.SetFocus(false); });
		clickItem.SetFocus(true);
		m_selectedDefenceItem=clickItem;
		int remainNum = 0;
		int.TryParse(m_selectedDefenceItem.RemainNum.text, out remainNum);
		//挑战防守副本，判断体力和次数
		if (remainNum <= 0) {
			ChallengeBtn.Enable = false;
			m_challengeBtnSpriteSwitch.ChangeSprite (3);	
		} else {
			ChallengeBtn.Enable = true;
			m_challengeBtnSpriteSwitch.ChangeSprite(1);		
		}
	}
}
