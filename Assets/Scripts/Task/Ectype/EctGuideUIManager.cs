using UnityEngine;
using System.Collections;
using UI;
using System.Collections.Generic;
using System.Linq;
using UI.Battle;

public class EctGuideUIManager : View
{
    public GameObject GuidePointTargetEffect;/// 从玩家指向目标的物资，目标可能是绝对位置，也有可能是配置的怪物位置
    public GameObject GuideDialogPanel;
    public GameObject GuidePicPanel;

    private EctGuideStepConfigData m_stepItem;
    private List<GameObject> m_stepEffect = new List<GameObject>();   //引导特效
    private List<EntityModel> m_monsterList = new List<EntityModel>(); //目标类型为怪物时的怪物列表
    private bool m_dynamicPoint;  // 是否玩家身上动态指向
    private Transform m_PlayerAttachArrow;  /// 玩家身上挂载箭头
    private Transform m_hero;  /// 主玩家
    private Vector3 m_fixedPos; /// 固定位置
    private IEnumerable<Transform> m_monsters; /// 指向怪物ID
    private GameObject m_signTips;   //引导文字的Prefab
    private GameObject m_buttonFlashing;
    private GameObject m_dialogPanel;
    private GameObject m_curGuideButton;
    private SingleButtonCallBack m_maskWithCollider;
    private bool m_isSlowMotionStatus;
    //图片引导
    private int m_picIndex;
    private GameObject m_guidePic;
    private List<PlayDataStruct<Animator>> m_SlowMotionAnimator;
    private List<PlayDataStruct<ParticleSystem>> m_SlowMotionParticleSystem;

    private  Transform HeroTrans
    {
        get 
        {
            if (m_hero == null)
            {
                m_hero = PlayerManager.Instance.FindHero().transform;
            }
            return m_hero;
        }
    }
    void Awake()
    {
        m_maskWithCollider = CreatObjectToNGUI.InstantiateObj(GuidePicPanel, this.transform).GetComponent<SingleButtonCallBack>();
        m_maskWithCollider.SetCallBackFuntion((obj) =>
                {
                    if (m_guidePic != null)
                    {
                        Destroy(m_guidePic);
                    }
                    m_picIndex++;
                    StartPicStep(m_stepItem);
                });
        m_maskWithCollider.SetButtonColliderActive(false);
        RegisterEventHandler();
        
    }
    void Start()
    {
        //如果场景尚未初始化，则监听初始化消息，否则直接初始化。取副本ID进行任务启动
        if (GameDataManager.Instance.DataIsNull(DataType.InitializeEctype))
        {
			//服务器消息先来//
            GameDataManager.Instance.dataEvent.RegisterEvent(DataType.InitializeEctype, Init);
        }
        else
        {
			//本地先来//
            Init(null);
        }
    }
    void Init(object obj)
    {
        GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.InitializeEctype, Init);
        SMSGEctypeInitialize_SC sMSGEctypeInitialize_SC = (SMSGEctypeInitialize_SC)GameDataManager.Instance.PeekData(DataType.InitializeEctype);
        CurSelectEctype(sMSGEctypeInitialize_SC.dwEctypeContainerId);
    }
    /// <summary>
    /// 副本初始化是判定进否进行副本新手引导
    /// </summary>
    /// <param name="onSelectEctypeData">当前所选择的副本</param>
    public void CurSelectEctype(int ectypeid)
    {
        if (GuideConfigManager.Instance.EctypeGuideConfigList.ContainsKey(ectypeid))
        {
            EctGuideManager.Instance.IsEctypeGuide = true;
            EctGuideManager.Instance.SetGuideBtnEnable(false);
            //副本需要引导，
            //检查是否引导步骤到达，不到达。禁用所有按钮，已到达。开始步骤处理
            if (EctGuideManager.Instance.CurrGuideStepData != null)
            {
                ExecuteGuideStep();
            }
        }
    }
	
    private void ExecuteGuideStep()
    {
        m_stepItem = EctGuideManager.Instance.CurrGuideStepData.EctGuideStepConfigData;   
        if (m_stepItem.StepSound != "0")
            SoundManager.Instance.PlaySoundEffect(m_stepItem.StepSound); 

             
        EctGuideManager.Instance.CurrGuideStepData.IsExcuting = true;

        RefreshGuideStepUI(false);
        
        switch(m_stepItem.StepType )
        {
            case 0:  //对话向导
                ShowGuideDialog();
                break;
            case 9:  //图片引导
                StartPicStep(m_stepItem);
                break;
            default:
                //判断是否减速处理
                switch (m_stepItem.SlowMonitorType)
                {
                    case 1:  //子弹命中监听
                        //监听子弹打中消息
                        AddEventHandler(EventTypeEnum.HitMonsterForGuide.ToString(), BulletHitSlowMotionCheck);
                        break;
                    case 2:   //怪物破防监听
                        //监听怪物破防消息
                        AddEventHandler(EventTypeEnum.BossBreakProtectForGuide.ToString(), BossBreakProtectSlowMotionCheck);
                        break;
                    default:
                        break;
                }
                break;
        }
    }
    //SlowMotionCheck方法调用
    private IEnumerator SlowMotionHandle()
    {
        yield return new WaitForSeconds(m_stepItem.RetardDelayTime / 1000f);
        StartCoroutine("WaitDoThing", m_stepItem.StepDuration / m_stepItem.DecelerationRate);  //不点击，在等待一定时间后也会执行下一步引导
        Time.timeScale = 1f / m_stepItem.DecelerationRate;
        if (m_buttonFlashing != null)
        {
            m_buttonFlashing.GetComponent<StepFlashing>().FlashingAnim.speed = 1f * m_stepItem.DecelerationRate;
            m_buttonFlashing.GetComponent<StepFlashing>().ArrowingAnim.animation["eff_UI_ArrowN"].speed = m_stepItem.DecelerationRate;
        }
        if (m_signTips != null)
        {
            m_signTips.transform.RecursiveGetComponent<Animator>("Animator",out m_SlowMotionAnimator);
            m_signTips.transform.RecursiveGetComponent<ParticleSystem>("ParticleSystem", out m_SlowMotionParticleSystem);
            if(m_SlowMotionAnimator!=null&&m_SlowMotionAnimator.Count>0)
            {
                m_SlowMotionAnimator.ApplyAllItem(P => { if (P.AnimComponent != null)P.AnimComponent.speed = 1f * m_stepItem.DecelerationRate; });
            }
            if (m_SlowMotionParticleSystem != null && m_SlowMotionParticleSystem.Count > 0)
            {
                //Debug.Log("Before  "+m_SlowMotionAnimator[0].AnimComponent.speed);
                m_SlowMotionParticleSystem.ApplyAllItem(P => { if (P.AnimComponent != null) P.AnimComponent.playbackSpeed = 1f * m_stepItem.DecelerationRate; });
                 //Debug.Log("After  " + m_SlowMotionAnimator[0].AnimComponent.speed);
            }   
        }
    }
    IEnumerator WaitDoThing(object obj)
    {
        yield return new WaitForSeconds((float)obj);
        var btnId = m_curGuideButton.GetComponent<GuideClick>().BtnId;
        if (m_curGuideButton != null)
        {
            m_curGuideButton.GetComponent<BattleSkillButton>().ButtonClickWithRet(null);
        }
        m_curGuideButton.RemoveComponent<GuideClick>("GuideClick");        
    }

    private void SlowMotionFinish(INotifyArgs args)
    {
        SkillFireData skillFireData = (SkillFireData)args;
        var vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
        int skillId=0;
        var slowMonitorFinishTarget=m_stepItem.SlowMonitorFinishTarget.Split('+');
        switch(vocation)
        {
            case 1:  //剑客
                skillId=int.Parse(slowMonitorFinishTarget[0]);
                break;
            case 4:  //刺客
                skillId=int.Parse(slowMonitorFinishTarget[1]);
                break;
        }
        //减速完成条件满足
        if(skillFireData .SkillId==skillId)
        {
            StopSlowMotionAction();
            NetServiceManager.Instance.InteractService.SendEctypeDialogOver(); //减速结束，通知服务器
            if (m_buttonFlashing != null)
                DestroyImmediate(m_buttonFlashing);
        }
    }
    /// <summary>
    /// 中止减速动作
    /// </summary>
    private void StopSlowMotionAction()
    {
        RemoveEventHandler(EventTypeEnum.HitMonsterForGuide.ToString(), BulletHitSlowMotionCheck);
        RemoveEventHandler(EventTypeEnum.FireSkillForGuide.ToString(), SlowMotionFinish);
        StopCoroutine("SlowMotionHandle");
        Time.timeScale = 1f;
        //CancelInvoke("DelayReduction");
        StopCoroutine("WaitDoThing");

        if (m_SlowMotionAnimator != null && m_SlowMotionAnimator.Count > 0)
        {
            m_SlowMotionAnimator.ApplyAllItem(P => { if (P.AnimComponent != null) P.AnimComponent.speed = 1f; });
            m_SlowMotionAnimator.Clear();
        }
        if (m_SlowMotionParticleSystem != null && m_SlowMotionParticleSystem.Count > 0)
        {
            m_SlowMotionParticleSystem.ApplyAllItem(P => { if (P.AnimComponent != null) P.AnimComponent.playbackSpeed = 1f; });
            m_SlowMotionParticleSystem.Clear();
        }       
    }
    /// <summary>
    /// 减速检测
    /// </summary>
    private void BossBreakProtectSlowMotionCheck(INotifyArgs args)
    {
        RemoveEventHandler(EventTypeEnum.BossBreakProtectForGuide.ToString(), BossBreakProtectSlowMotionCheck);
		if (m_stepItem != null && m_stepItem.SlowMonitorType == 2)
        {
            ClearGuideStepUIEffect();
            RefreshGuideStepUI(true);

            StartCoroutine("SlowMotionHandle");

            AddEventHandler(EventTypeEnum.FireSkillForGuide.ToString(), SlowMotionFinish);
        }
    }
    /// <summary>
    /// 减速检测
    /// </summary>
    private void BulletHitSlowMotionCheck(INotifyArgs args)
    {        
        BulletHitData bulletHitData = (BulletHitData)args;
        var vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
        int bulletId=0;
        var slowMonitorCondition=m_stepItem.SlowMonitorCondition.Split('+');
        var monsterId=m_stepItem.SlowMonitorTarget;
        switch(vocation)
        {
            case 1:  //剑客
                bulletId=int.Parse(slowMonitorCondition[0]);
                break;
            case 4:  //刺客
                bulletId=int.Parse(slowMonitorCondition[1]);
                break;
        }
        if (bulletHitData.BulletId == bulletId && monsterId == bulletHitData.BeFightId)
        {
            RemoveEventHandler(EventTypeEnum.HitMonsterForGuide.ToString(), BulletHitSlowMotionCheck);
			if (m_stepItem != null && m_stepItem.SlowMonitorType == 1)
            {
                ClearGuideStepUIEffect();
                RefreshGuideStepUI(true);

                StartCoroutine("SlowMotionHandle");

                AddEventHandler(EventTypeEnum.FireSkillForGuide.ToString(), SlowMotionFinish);
            }
        }
    }
    private void RefreshGuideStepUI(bool flag)
    {
        m_isSlowMotionStatus = flag;
        EctGuideManager.Instance.SetGuideBtnEnable(false);
        //判断步骤的类型，如果
        //非减速地表特效
        ShowStepEffect(flag);
        //非减速引导文字
        StartTipsGuide(flag);
        //点亮非减速按钮
        SetActiveButton(flag);
    }
    /// <summary>
    /// 设置对话面板
    /// </summary>
    /// <param name="item"></param>
    private void ShowGuideDialog()
    {
        if (m_stepItem.GuideDialogInfos == null || m_stepItem.GuideDialogInfos.Length < 1)
        {
            return;
        }

        if (m_dialogPanel == null)
        {
            m_dialogPanel = CreatObjectToNGUI.InstantiateObj(GuideDialogPanel, transform);
            m_dialogPanel.transform.localPosition = new Vector3(0, 0, -500);
        }
        m_dialogPanel.GetComponent<EctypeGuideDialogPanel>().InitDialogPanel(m_stepItem);
    }
    /// <summary>
    /// 设置引导按钮高亮
    /// </summary>
    private void SetActiveButton(bool flag)
    {        
		var vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
        var btns = flag ? m_stepItem.SlowMonitorButtonShielding : m_stepItem.ButtonShielding;
        var tipBtn = flag ? m_stepItem.SlowMonitorGuideButton : m_stepItem.GuideButton;
        var buttonEffectArr = flag ? m_stepItem.SlowMonitorButtonEffect : m_stepItem.ButtonEffect;
		var buttonEffect = buttonEffectArr.Length == 0 ? null : buttonEffectArr.SingleOrDefault(P => P.Vocation == vocation).TipsPrefab;
        foreach (int btnId in btns)
        {
            if (GuideBtnManager.Instance.GetButtonList.ContainsKey(btnId))
            {
                var actButton = GuideBtnManager.Instance.GetButtonList[btnId];
                EctGuideManager.Instance.SetGuideBtnEnable(true, actButton);
                var guideClick=actButton.GuideBtn.GetComponent<GuideClick>();
                if (!guideClick)
                {
                    guideClick = actButton.GuideBtn.AddComponent<GuideClick>();
                    guideClick.BtnId = btnId;
                }
                if (btnId == tipBtn)
                {
                    SetButtonFlashing(actButton.GuideBtn.transform, buttonEffect);  //设置按钮点亮
                    m_curGuideButton = actButton.GuideBtn;
                    guideClick.ClickAct = GuideButtonClickHandle;
                }
            }
        }
    }
    private void GuideButtonClickHandle(GuideClick guideClick)
    {
        guideClick.ClickAct = null;
        if (m_stepItem != null)
        {
            if (m_buttonFlashing != null)
            {
                GameObject.DestroyImmediate(m_buttonFlashing);
            }
            if (m_stepItem.StepType == 7)
            {
                NetServiceManager.Instance.InteractService.SendEctypeDialogOver();
            }
            else
            {
                StartCoroutine("ReActiveGuideBtn");
            }
        }
    }
    private IEnumerator ReActiveGuideBtn()
    {
        if (m_stepItem != null)
        {
            var delayTime=m_isSlowMotionStatus?m_stepItem.SlowMonitorButtonEffectInterval:m_stepItem.ButtonEffectInterval;
            yield return new WaitForSeconds(delayTime/1000f);
            SetActiveButton(m_isSlowMotionStatus);
        }
        else
        {
            yield break;
        }
    }
    /// <summary>
    /// 设置引导按钮的高光亮圈
    /// </summary>
    private void SetButtonFlashing(Transform btn,GameObject btnEffectPrefab)
    {
        if (m_buttonFlashing != null)
        {
            GameObject.DestroyImmediate(m_buttonFlashing);
        }
        if (!m_buttonFlashing && btnEffectPrefab)
        {
            m_buttonFlashing = (GameObject)GameObject.Instantiate(btnEffectPrefab, Vector3.zero, Quaternion.identity);
            m_buttonFlashing.transform.parent = btn;
            m_buttonFlashing.transform.localScale = Vector3.one;
        }
		if (m_buttonFlashing == null) {
			return;		
		}
        m_buttonFlashing.transform.localPosition = btn.position + new Vector3(0, 0, -20);
        m_buttonFlashing.GetComponent<StepFlashing>().FlashingAnim.speed = 1f;

    }
    /// <summary>
    /// 图片引导
    /// </summary>
    /// <param name="newGuideConfigData"></param>
    private void StartPicStep(EctGuideStepConfigData stepItem)
    {
        if (stepItem.GuidePicture == null)
        {
            return;
        }        
        if (m_picIndex < stepItem.GuidePicture.Length)
        {
            m_maskWithCollider.SetButtonColliderActive(true); 

            var picPrefab = stepItem.GuidePicture[m_picIndex];
            if (picPrefab != null)
            {
                m_guidePic = CreatObjectToNGUI.InstantiateObj(picPrefab, this.transform);               
            }
        }
        else
        {
            m_maskWithCollider.SetButtonColliderActive(false); 
            if (m_guidePic != null)
            {
                Destroy(m_guidePic);
            }
            m_picIndex = 0;
            NetServiceManager.Instance.InteractService.SendEctypeDialogOver();
        }
    }

    private void ClearGuideStepUIEffect()
    {
        //取消点亮
        if (m_buttonFlashing != null)
        {
            GameObject.Destroy(m_buttonFlashing);
        }
        //清除引导文字
        if (m_signTips != null)
            DestroyImmediate(m_signTips);
        //隐藏地表特效
        for (int i = 0; i < m_stepEffect.Count; i++)
        {
            if (m_stepEffect[i] != null)
                DestroyImmediate(m_stepEffect[i]);
        }
        m_stepEffect.Clear();
        m_monsterList.Clear();
        //玩家脚底指引箭头
        if (m_PlayerAttachArrow != null)
        {
            GameObject.Destroy(m_PlayerAttachArrow.gameObject);

            //RaiseEvent(EventTypeEnum.HidePlayerEctypeGuideArrow.ToString(), null);
        }
    }
    /// <summary>
    /// 显示当前引导步骤的地形特效 flag 是否减速资源标记
    /// </summary>
    /// <param name="item"></param>
    private void ShowStepEffect(bool flag)
    {
        for (int i = 0; i < m_stepEffect.Count; i++)
        {
            if (m_stepEffect[i] != null)
                DestroyImmediate(m_stepEffect[i]);
        }
        m_stepEffect.Clear();
        m_monsterList.Clear();
        
        var guideEffect=flag?m_stepItem.SlowMonitorGuideEffect:m_stepItem.GuideEffect ;
        var effctPos=flag?m_stepItem._SlowMotionEffectPos:m_stepItem._EffectPos ;
        int effectAngle=flag?m_stepItem.SlowMonitorEffectAngle:m_stepItem.EffectAngle ;

        if (guideEffect!= null)
        {
            var pos = Vector3.zero;
            pos = pos.GetFromServer(effctPos.x, effctPos.z);
            pos.y = m_stepItem._EffectPos.y;
            var stepEffect = Instantiate(guideEffect, pos, Quaternion.Euler(0,effectAngle, 0)) as GameObject;

            m_stepEffect.Add(stepEffect);
        }

         var monsterEffect=flag?m_stepItem.SlowMonitorMonsterEffect:m_stepItem.MonsterEffect ;
         var EffMountmonsterId = flag ? m_stepItem.SlowMonitorMountMonster : m_stepItem.MountMonster;
        int mountType=flag?m_stepItem.SlowMonitorMountType:m_stepItem.MountType ;

        if (monsterEffect != null)
        {           
            var monsterList = MonsterManager.Instance.GetMonstersList();

            for (int i = 0; i < monsterList.Count; i++)
            {
                if (((SMsgPropCreateEntity_SC_Monster)monsterList[i].EntityDataStruct).
                    BaseObjectValues.OBJECT_FIELD_ENTRY_ID == EffMountmonsterId)
                {
                    m_monsterList.Add(monsterList[i]);
                }
            }

            for (int i = 0; i < m_monsterList.Count; i++)
            {
                var stepEffect = Instantiate(monsterEffect) as GameObject;
                stepEffect.transform.parent = m_monsterList[i].GO.transform;
                stepEffect.transform.localPosition = Vector3.zero;
                m_stepEffect.Add(stepEffect);
            }
        }
        TraceUtil.Log(SystemModel.Rocky, "m_stepItem.MountType " + mountType);
        var monsterId = flag ? m_stepItem.SlowMonitorTargetInformation : m_stepItem.TargetInformation;
        //0=不显示箭头，1=地图绝对位置，2=怪物
        if (mountType == 1
            || mountType == 2)
        {
            //RaiseEvent(EventTypeEnum.ShowPlayerEctypeGuideArrow.ToString(), null);
            m_dynamicPoint = true;
            m_PlayerAttachArrow = HeroTrans.FindChild(GuidePointTargetEffect.name);
            if (m_PlayerAttachArrow == null)
            {
                var guidePointTargetEffet = Instantiate(GuidePointTargetEffect) as GameObject;
                guidePointTargetEffet.name = GuidePointTargetEffect.name;
                m_PlayerAttachArrow = guidePointTargetEffet.transform;
                m_PlayerAttachArrow.parent = HeroTrans;
                m_PlayerAttachArrow.localPosition = Vector3.zero;
            }

            if (mountType == 1)
            {
                m_fixedPos = Vector3.zero;
                string[] configPos = monsterId.Split('+');
                m_fixedPos = m_fixedPos.GetFromServer(float.Parse(configPos[0]), float.Parse(configPos[1]));
                m_fixedPos.y = GuidePointTargetEffect.transform.position.y;
            }
            else
            {
                m_monsters = MonsterManager.Instance.GetMonsterListByMonsterId(int.Parse(monsterId));

                TraceUtil.Log("Monsters " + monsterId + "  " + m_monsters.Count());
            }
        }

    }
    private void StartTipsGuide(bool flag)
    {
        var vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;

         var tipsPrefab=flag?m_stepItem.SlowMonitorTipsPrefab:m_stepItem.TipsPrefab ;
        var signTips=flag?m_stepItem.SlowMonitorSignTips:m_stepItem.SignTips ;
        var tipsPrefabOffsetVec=flag?m_stepItem.SlowMotionTipsPrefabOffsetVec:m_stepItem.TipsPrefabOffsetVec ;

        var tips = tipsPrefab.SingleOrDefault(P => P.Vocation == vocation);
        if (tips != null && tips.TipsPrefab != null)
        {
            ShowSignTipsEffect(signTips,tips.TipsPrefab, tipsPrefabOffsetVec);
        }       
    }
    private void ShowSignTipsEffect(string textKey, GameObject tipsPrefab, Vector3 offset)
    {
        if (m_signTips != null)
        {
            m_SlowMotionAnimator.Clear();
            DestroyImmediate(m_signTips);
        }

        m_signTips = CreatObjectToNGUI.InstantiateObj(tipsPrefab, this.transform);
        m_signTips.transform.localPosition = new Vector3(0, -125, 0) + offset;
        m_signTips.GetComponent<SingleButtonCallBack>().SetButtonText(LanguageTextManager.GetString(textKey));
    }
    private void ShowSignTipsEffect(string textKey, GameObject tipsPrefab)
    {
        ShowSignTipsEffect(textKey, tipsPrefab, Vector3.zero);
    }
    void Update()
    {
        if (m_dynamicPoint)
        {
            if (m_stepItem!=null&&HeroTrans != null && m_PlayerAttachArrow != null)
            {
                if (m_stepItem.MountType == 2)
                {
                    float minDistance = 0;
                    foreach (var monster in m_monsters)
                    {
                        var d = Vector3.Distance(HeroTrans.position, monster.position);
                        if (minDistance < d)
                        {
                            minDistance = d;
                            m_fixedPos = monster.position;
                        }
                    }
                }
                var forward = new Vector3(m_fixedPos.x, m_PlayerAttachArrow.position.y, m_fixedPos.z)
                    - new Vector3(HeroTrans.position.x, m_PlayerAttachArrow.position.y, HeroTrans.position.z);
                m_PlayerAttachArrow.rotation = Quaternion.LookRotation(forward);
            }
            else
            {
                m_dynamicPoint = false;
            }
        }
    }
    void OnDestroy()
    {
        //玩家脚底指引箭头
        if (m_PlayerAttachArrow != null)
        {
            GameObject.Destroy(m_PlayerAttachArrow.gameObject);
        }
		EctGuideManager.Instance.SetCurrGuideStepData(null);
        EctGuideManager.Instance.IsEctypeGuide = false;
        RemoveAllEvent();
    }
    private void ReceiveGuideStep(INotifyArgs arg)
    {
        ExecuteGuideStep();
    }
    private void FinishGuideStep(INotifyArgs arg)
    {
        StopSlowMotionAction();
        StopCoroutine("ReActiveGuideBtn");
        m_isSlowMotionStatus = false;
        ClearGuideStepUIEffect();
        EctGuideManager.Instance.SetGuideBtnEnable(true);
        m_stepItem = null;
        m_picIndex = 0;
    }
    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.ReceiveGuideStep.ToString(), ReceiveGuideStep);
        AddEventHandler(EventTypeEnum.FinishGuideStep.ToString(), FinishGuideStep);
    }
}

public struct BulletHitData : INotifyArgs
{
    public int BulletId;
    public int BeFightId;
    public int GetEventArgsType()
    {
        return 0;
    }
}
public struct SkillFireData : INotifyArgs
{
    public int SkillId;
    public int GetEventArgsType()
    {
        return 0;
    }
}