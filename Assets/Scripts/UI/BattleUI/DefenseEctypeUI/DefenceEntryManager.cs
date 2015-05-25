using UnityEngine;
using System.Collections;
using System.Linq;
using UI.MainUI;

public class DefenceEntryManager : BaseUIPanel {

	public SingleButtonCallBack BackBtn;
	public GameObject CommonPanelTitle_Prefab;
	public GameObject DefenceEctypeList_Prefab;
    public GameObject CommonTitlePoint;

	private BaseCommonPanelTitle m_commonPanelTitle;
	private DefenceEctypeListBehaviour m_defenceEctypeListBehaviour;
	// Use this for initialization
	void Awake () {
		//返回按钮点击
		BackBtn.SetCallBackFuntion((obj)=>
		                           {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_DefenceLevelBack");
			this.Close();
		});
		//返回按钮按下/松开效果
		BackBtn.SetPressCallBack((isPressed)=>
		                         {
			BackBtn.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(isPressed?2:1));
		});
        var commonPanel = NGUITools.AddChild(CommonTitlePoint, CommonPanelTitle_Prefab);
		m_commonPanelTitle=commonPanel.GetComponent<BaseCommonPanelTitle>();
        m_commonPanelTitle.HidePos = new Vector3(100, 0, 0);
        m_commonPanelTitle.ShowPos = Vector3.zero;
        m_commonPanelTitle.Init(CommonTitleType.Power, CommonTitleType.GoldIngot);
		//RegisterEventHandler();
        TaskGuideBtnRegister();
    }
    /// <summary>
    /// 引导按钮注入代码
    /// </summary>
    private void TaskGuideBtnRegister()
    {
        BackBtn.gameObject.RegisterBtnMappingId(UIType.Defence, BtnMapId_Sub.Defence_Back);
    }
	public override void Show(params object[] value)
	{
		SoundManager.Instance.PlaySoundEffect("Sound_UIEff_DefenceLevelAppear");
        int? ectypeId = null;
        if (value != null && value.Length > 0)
        {
            ectypeId = (int)value[0];
        }

        StartCoroutine(Init(ectypeId));
		base.Show(value);       
		
		m_commonPanelTitle.TweenShow();
	}
    IEnumerator Init(int? selectedEctype)
	{
		if(m_defenceEctypeListBehaviour==null)
		{
			var ectypeList=NGUITools.AddChild(gameObject,DefenceEctypeList_Prefab);
			m_defenceEctypeListBehaviour=ectypeList.GetComponent<DefenceEctypeListBehaviour>();
		}
        m_defenceEctypeListBehaviour.Init(selectedEctype);
		yield return null;
	}
	//1,2,3对应三个防守副本//
    public static bool DefenceEctypeEnabled(int ectypeMark)
    {
        bool flag = false;
        var heroPlayerDataModel = PlayerManager.Instance.FindHeroDataModel();
        var defenceEctypes = EctypeConfigManager.Instance.EctypeContainerConfigFile.ectypeContainerDataList.Where(P => P.lEctypeType == 8);
        var playerLev = heroPlayerDataModel.GetUnitValue().sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
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
        EctypeContainerData[] canChallengeEctypes=new EctypeContainerData[3];
        canChallengeEctypes[0]=sortedList.FirstOrDefault(P=>P.lMinActorLevel<=playerLev&&P.lEctypePos[2]=="1");
		canChallengeEctypes[1]=sortedList.FirstOrDefault(P=>P.lMinActorLevel<=playerLev&&P.lEctypePos[2]=="2");
		canChallengeEctypes[2]=sortedList.FirstOrDefault(P=>P.lMinActorLevel<=playerLev&&P.lEctypePos[2]=="3");
        int remainNum = 0;
        for (int i = 0; i < 3; i++)
        {
			if (canChallengeEctypes[i] == null || i+1 != ectypeMark)
                continue;
            
            switch (i)
            {
                case 0:
                    remainNum = heroPlayerDataModel.PlayerValues.PLAYER_FIELD_EXPDEFIEND_VALUE;
                    break;
                case 1:
                    remainNum = heroPlayerDataModel.PlayerValues.PLAYER_FIELD_COINDEFINED_VALUE;
                    break;
                case 2:
                    remainNum = heroPlayerDataModel.PlayerValues.PLAYER_FIELD_GOLDDEFINED_VALUE;
                    break;
            }
        }
        flag = remainNum > 0;
        return flag;
    }
	public override void Close()
	{
		if (!IsShow)
			return;
		StartCoroutine(AnimToClose());
		//SkillList.tweenClose();
		m_commonPanelTitle.tweenClose();
	}
	/// <summary>
	/// 播放关闭动画
	/// </summary>
	/// <returns>The to close.</returns>
	private IEnumerator AnimToClose()
	{
		yield return new WaitForSeconds(0.16f);   //动画时长
		UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
		base.Close();
	}
}
