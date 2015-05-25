
using UnityEngine;
using System.Collections;
using System.Linq;
using UI;
using Guide;
using System.Collections.Generic;
using UI.Battle;

public class EctypeGuidePanel : View {

    //public GameObject GuideDialogPanel;
    ////public GameObject StepButtonFlashing;
    //public SingleButtonCallBack GuidePicPanel;

   
    //private int m_activeIndex = 1;  //高亮按钮索引
    //private GameObject m_signTips;   //引导文字的Prefab
    //private GameObject m_dialogPanel;  //对话UI面板
    //private EctypeGuideStepConfigData m_stepItem;  //引导步的数据
    //private GameObject m_buttonFlashing;
    //private List<GameObject> m_stepEffect = new List<GameObject>();
    //private GameObject m_curGuideButton;
    //private GameObject m_guidePicGo;

    //#region 玩家身上箭头指向相关变量
    //         /// <summary>
    ///// 从玩家指向目标的物资，目标可能是绝对位置，也有可能是配置的怪物位置
    ///// </summary>
    //public GameObject GuidePointTargetEffet;
    ///// <summary>
    ///// 玩家身上挂载箭头
    ///// </summary>
    //private Transform m_PlayerAttachArrow;
    ///// <summary>
    ///// 主玩家
    ///// </summary>
    //private Transform m_hero;
    ///// <summary>
    ///// 是否玩家身上动态指向
    ///// </summary>
    //private bool m_dynamicPoint;
    ///// <summary>
    ///// 固定位置
    ///// </summary>
    //private Vector3 m_fixedPos;
    ///// <summary>
    ///// 指向怪物ID
    ///// </summary>
    //private IEnumerable<Transform> m_monsters;
    //#endregion
 
	
    //private bool m_isComplate = false;
    //private List<EntityModel> m_monsterList = new List<EntityModel>();

    //void Awake()
    //{
    //    GuidePicPanel.SetCallBackFuntion(PicStepHandle);
    //    GuidePicPanel.SetButtonColliderActive(false);        
    //}

    //void Start()
    //{
    //    UIEventManager.Instance.RegisterUIEvent(UIEventType.ClickTheGuideBtn, NextGuideHandle);
    //}

    //private int m_picIndex = 0;
    //private Animation m_closeAnim;
    //void PicStepHandle(object obj)
    //{
    //    if (m_picIndex >= m_stepItem._GuidePicPrefabs.Length)
    //    {
    //        if(m_guidePicGo != null)
    //        {
    //            m_closeAnim = m_guidePicGo.GetComponentInChildren<Animation>();
    //            StartCoroutine(PicGuideOver());
    //        }           
    //        NetServiceManager.Instance.InteractService.SendEctypeDialogOver();            
    //        GuidePicPanel.SetButtonColliderActive(false);
    //        m_picIndex = 0;
    //        return;
    //    }

    //    if (m_guidePicGo != null)
    //        Destroy(m_guidePicGo);

    //    var picPrefab = m_stepItem._GuidePicPrefabs[m_picIndex];

    //    if (picPrefab != null)
    //    {
    //        m_picIndex++;
    //        m_guidePicGo = CreatObjectToNGUI.InstantiateObj(picPrefab, this.transform);
    //    }
    //}
    //IEnumerator PicGuideOver()
    //{
    //    m_closeAnim.CrossFade("UI_kuangdonghua3");
    //    yield return new WaitForSeconds(m_closeAnim["UI_kuangdonghua3"].length);
    //    Destroy(m_guidePicGo);
    //}
    //public void StartGuideStep(EctypeGuideStepConfigData stepItem)
    //{
    //    m_isComplate = false;
    //    m_stepItem = stepItem;
    //    m_hero = PlayerManager.Instance.FindHero().transform;
    //    var vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
    //    var tips = stepItem._TipsType.SingleOrDefault(P => P.Vocation == vocation);
    //    if(tips != null && tips.TipsPrefab != null)
    //        ShowSignTipsEffect(stepItem._SignTips, tips.TipsPrefab, stepItem._TipsPrefabOffset);
    //    if(stepItem._StepSound != "0")
    //        SoundManager.Instance.PlaySoundEffect(stepItem._StepSound);
    //    ShowGuideDialog(stepItem);
    //    SetActiveButton();
    //    Invoke("ShowStepEffect", 1.5f);
    //    //ShowStepEffect();
    //    if (stepItem._StepType == 7)
    //    {
            
    //        Invoke("DelayReduction", stepItem._ReductionDelayTime);            
    //    }
    //}

    //public void StartPicStep(EctypeGuideStepConfigData stepItem)
    //{
    //    m_isComplate = false;
    //    m_stepItem = stepItem;
    //    m_picIndex = 0;

    //    if (m_guidePicGo != null)
    //        DestroyImmediate(m_guidePicGo);

    //    var picPrefab = m_stepItem._GuidePicPrefabs[m_picIndex];

    //    if (picPrefab != null)
    //    {
    //        m_picIndex = 1;
    //        m_guidePicGo = CreatObjectToNGUI.InstantiateObj(picPrefab, this.transform);
    //    }

    //    GuidePicPanel.SetButtonColliderActive(true);
    //}

    //void DelayReduction()
    //{
    //    StartCoroutine("WaitDoThing", m_stepItem._StepDuration / m_stepItem._ReductionRatio);  //不点击，在等待一定时间后也会执行下一步引导
    //    Time.timeScale = 1f / m_stepItem._ReductionRatio;
    //    if (m_buttonFlashing != null)
    //    {
    //        m_buttonFlashing.GetComponent<StepFlashing>().FlashingAnim.speed = 1f * m_stepItem._ReductionRatio;
    //        m_buttonFlashing.GetComponent<StepFlashing>().ArrowingAnim.animation["eff_UI_ArrowN"].speed = m_stepItem._ReductionRatio;
    //    }
    //}

    //IEnumerator WaitDoThing(object obj)
    //{
    //    yield return new WaitForSeconds((float)obj);
    //    Time.timeScale = 1f;
    //    var btnId = m_curGuideButton.GetComponent<GuideClick>().BtnId;
    //    if (m_curGuideButton != null)
    //    {
    //        m_curGuideButton.GetComponent<BattleSkillButton>().ButtonClick(null);
    //    }

    //    NextGuideHandle(btnId);

    //    //m_curGuideButton.RemoveComponent<GuideClick>("GuideClick");
    //    //DisableButtonList(true);  //打开所有关闭的按钮

    //    //if (m_buttonFlashing != null)
    //    //    DestroyImmediate(m_buttonFlashing);
    //}


    //public void ComplateGuideStep(EctypeGuideStepConfigData stepItem)
    //{
    //    m_isComplate = true;
    //    StepSucess();
    //}
    //private void ShowSignTipsEffect(string textKey, GameObject tipsPrefab,Vector3 offset)
    //{
    //     if (m_signTips != null)
    //        DestroyImmediate(m_signTips);

    //    m_signTips = CreatObjectToNGUI.InstantiateObj(tipsPrefab, this.transform);
    //    m_signTips.transform.localPosition = new Vector3(0, -125, 0)+offset;
    //    m_signTips.GetComponent<SingleButtonCallBack>().SetButtonText(LanguageTextManager.GetString(textKey));
    //}
    //private void ShowSignTipsEffect(string textKey, GameObject tipsPrefab)
    //{
    //   ShowSignTipsEffect(textKey,tipsPrefab,Vector3.zero);
    //}

    ///// <summary>
    ///// 设置对话面板
    ///// </summary>
    ///// <param name="item"></param>
    //private void ShowGuideDialog(EctypeGuideStepConfigData item)
    //{
    //    if (item.StepDialogInfos == null || item.StepDialogInfos.Length < 1)
    //    {
    //        return;
    //    }

    //    if (m_dialogPanel == null)
    //    {
    //        //m_dialogPanel = (GameObject)GameObject.Instantiate(GuideDialogPanel, this.transform.position, Quaternion.identity);
    //        //m_dialogPanel.transform.parent = this.transform;
    //        //m_dialogPanel.transform.localScale = Vector3.one;
    //        m_dialogPanel = CreatObjectToNGUI.InstantiateObj(GuideDialogPanel,transform);
    //        m_dialogPanel.transform.localPosition = new Vector3(0,0,-500);
    //    }
    //    //m_dialogPanel.GetComponent<EctypeGuideDialogPanel>().InitDialogPanel(item);
    //}

    ///// <summary>
    ///// 关闭指定按钮列表
    ///// </summary>
    ///// <param name="flag">false是关闭，true是打开</param>
    //public void DisableButtonList(bool flag)
    //{
    //    var guideButtonList = GuideBtnManager.Instance.GetButtonList;
    //    var disableList = m_stepItem._DisableButtonList;

    //    for (int i = 0; i < disableList.Length; i++)
    //    {
    //        if (guideButtonList.ContainsKey(disableList[i]))
    //        {
    //            var guideBtn = guideButtonList[disableList[i]].GuideBtn;   //处理版本遗留问题
    //            if (guideBtn != null)
    //            {
    //                var skillComponent = guideBtn.GetComponent<BattleSkillButton>();
    //                if (skillComponent != null)
    //                {
    //                    skillComponent.SetButtonStatus(flag);
    //                }
    //                else
    //                {
    //                    var healthComponent = guideBtn.GetComponent<HealthAndMagicButton>();
    //                    if (healthComponent != null)
    //                        healthComponent.SetMyButtonActive(flag);
    //                }
    //            }

    //        }
    //    }

    //    if (flag)
    //    {
    //        SUpdateRollStrengthStruct updateRSStruct = new SUpdateRollStrengthStruct() { strengthValue = PlayerGasSlotManager.Instance.GetAirSlotValue };
    //        RaiseEvent(EventTypeEnum.UpdateRollAirSlot.ToString(), updateRSStruct);
    //    }
        
    //}
	
	
    ///// <summary>
    ///// 显示当前引导步骤的地形特效
    ///// </summary>
    ///// <param name="item"></param>
    //private void ShowStepEffect()
    //{
        
    //    for (int i = 0; i < m_stepEffect.Count; i++)
    //    {
    //        if (m_stepEffect[i] != null)
    //            DestroyImmediate(m_stepEffect[i]);
    //    }
    //    m_stepEffect.Clear();
    //    m_monsterList.Clear();

    //    if (m_stepItem._GuideEffect != null)
    //    {
    //        var pos = Vector3.zero;
    //        pos = pos.GetFromServer(m_stepItem._EffectPos.x, m_stepItem._EffectPos.z);
    //        pos.y = m_stepItem._EffectPos.y;
    //        var stepEffect = Instantiate(m_stepItem._GuideEffect, pos, Quaternion.Euler(0, m_stepItem._EffectAngle, 0)) as GameObject;

    //        m_stepEffect.Add(stepEffect);
    //    }

    //    if (m_stepItem._MonsterEffect != null) //1=玩家，2=场景绝对位置，3=怪物
    //    {
    //        //case 1:
    //        //    var playerGo = PlayerManager.Instance.FindHero();
    //        //    stepEffect = Instantiate(m_stepItem._GuideEffect) as GameObject;
    //        //    stepEffect.transform.parent = playerGo.transform;
    //        //    m_stepEffect.Add(stepEffect);
    //        //    break;

    //            var monsterList = MonsterManager.Instance.GetMonstersList();
			
    //            for (int i = 0; i < monsterList.Count; i++) {
    //                if(((SMsgPropCreateEntity_SC_Monster)monsterList[i].EntityDataStruct).
    //                    BaseObjectValues.OBJECT_FIELD_ENTRY_ID == m_stepItem._MountMonsterID)
    //                {
    //                    m_monsterList.Add(monsterList[i]);
    //                }
    //            }
			
    //            for (int i = 0; i < m_monsterList.Count; i++)
    //            {
    //                var stepEffect = Instantiate(m_stepItem._MonsterEffect) as GameObject;
    //                stepEffect.transform.parent = m_monsterList[i].GO.transform;
    //                stepEffect.transform.localPosition = Vector3.zero;
    //                m_stepEffect.Add(stepEffect);
    //            }
    //    }
    //    TraceUtil.Log("m_stepItem.MountType "+m_stepItem.MountType);
    //    if (m_stepItem.MountType == 1
    //        || m_stepItem.MountType == 2)
    //    {
    //        m_dynamicPoint = true;
    //        m_PlayerAttachArrow = m_hero.FindChild(GuidePointTargetEffet.name);
    //        if (m_PlayerAttachArrow == null)
    //        {
    //            var guidePointTargetEffet = Instantiate(GuidePointTargetEffet) as GameObject;
    //            guidePointTargetEffet.name = GuidePointTargetEffet.name;
    //            m_PlayerAttachArrow = guidePointTargetEffet.transform;
    //            m_PlayerAttachArrow.parent = m_hero;
    //            m_PlayerAttachArrow.localPosition = Vector3.zero;
    //        }

    //        if (m_stepItem.MountType == 1)
    //        {
    //            m_fixedPos = Vector3.zero;
    //            string[] configPos = m_stepItem.TargetInformation.Split('+');
    //            m_fixedPos = m_fixedPos.GetFromServer(float.Parse(configPos[0]), float.Parse(configPos[1]));
    //            m_fixedPos.y = GuidePointTargetEffet.transform.position.y;
    //        }
    //        else
    //        {
    //            m_monsters = MonsterManager.Instance.GetMonsterListByMonsterId(int.Parse(m_stepItem.TargetInformation));

    //            TraceUtil.Log("Monsters " + m_stepItem.TargetInformation + "  " + m_monsters.Count());
    //        }
    //    }
            
    //}
    //void Update()
    //{
    //    if (m_dynamicPoint)
    //    {
    //        if (m_hero != null && m_PlayerAttachArrow != null)
    //        {
    //            if (m_stepItem.MountType == 2)
    //            {
    //                float minDistance = 0;
    //                foreach (var monster in m_monsters)
    //                {
    //                    var d = Vector3.Distance(m_hero.position, monster.position);
    //                    if (minDistance < d)
    //                    {
    //                        minDistance = d;
    //                        m_fixedPos = monster.position;
    //                    }
    //                }

    //            }
    //            var forward=new Vector3(m_fixedPos.x,m_PlayerAttachArrow.position.y,m_fixedPos.z)
    //                -new Vector3(m_hero.position.x,m_PlayerAttachArrow.position.y,m_hero.position.z);
    //            m_PlayerAttachArrow.rotation = Quaternion.LookRotation(forward);
    //        }
    //        else
    //        {
    //            m_dynamicPoint = false;
    //        }
    //    }
    //}
    ///// <summary>
    ///// 设置引导按钮高亮
    ///// </summary>
    //private void SetActiveButton()
    //{
    //    if (m_activeIndex <= m_stepItem._GuideIdList.Length)
    //    {
    //        int guideBtnID = m_stepItem._GuideIdList[m_activeIndex - 1];

    //        if (GuideBtnManager.Instance.GetButtonList.ContainsKey(guideBtnID))
    //        {
    //            m_curGuideButton = GuideBtnManager.Instance.GetButtonList[guideBtnID].GuideBtn;
    //            if (m_curGuideButton != null)
    //            {
    //                if (!m_curGuideButton.GetComponent<GuideClick>())
    //                {
    //                    var guideClick = m_curGuideButton.AddComponent<GuideClick>();
    //                    guideClick.BtnId = guideBtnID;
    //                }
    //                m_curGuideButton.GetComponent<GuideButtonEvent>().IsEnable = false;

    //                SetButtonFlashing(m_curGuideButton.transform);  //设置按钮点亮
    //            }
    //        }
    //    }
    //}

    ///// <summary>
    ///// 设置引导按钮的高光亮圈
    ///// </summary>
    //private void SetButtonFlashing(Transform btn)
    //{
    //    if (!m_buttonFlashing && m_stepItem._ButtonFlshing)
    //    {
    //        m_buttonFlashing = (GameObject)GameObject.Instantiate(m_stepItem._ButtonFlshing, Vector3.zero, Quaternion.identity);
    //        m_buttonFlashing.transform.parent = btn;
    //        m_buttonFlashing.transform.localScale = Vector3.one;


    //    }
    //    m_buttonFlashing.transform.localPosition = btn.position + new Vector3(0,0,-20);
    //    m_buttonFlashing.GetComponent<StepFlashing>().FlashingAnim.speed = 1f;

    //}

    ///// <summary>
    ///// 单击引导按钮后的回调函数
    ///// </summary>
    ///// <param name="notifyArgs"></param>
    //private void NextGuideHandle(object obj)
    //{
    //    int btnId=(int)obj;
    //    if (m_buttonFlashing != null)
    //        DestroyImmediate(m_buttonFlashing);

    //    if (m_stepItem._StepType == 7)
    //    {
    //        //如果不是点击目标按钮，则不处理
    //        if(GuideBtnManager.Instance.GetButtonList[btnId].GuideBtn!=m_curGuideButton)
    //        {
    //            return;
    //        }
    //        Time.timeScale = 1f;
    //        CancelInvoke("DelayReduction");
    //        StopCoroutine("WaitDoThing");
    //        NetServiceManager.Instance.InteractService.SendEctypeDialogOver();
    //        return;
    //    }
    //    m_curGuideButton.RemoveComponent<GuideClick>("GuideClick");
    //    //一个步骤两个Button点击的处理
    //    if (m_activeIndex < m_stepItem._GuideIdList.Length)
    //    {
    //        m_activeIndex++;
    //        Invoke("SetActiveButton", m_stepItem._DelayTime);
    //    }
    //    else if (!m_isComplate) //任务未完成继续高亮
    //    {
    //        m_activeIndex = 1;
    //        Invoke("SetActiveButton", m_stepItem._ButtonEffectInterval*0.001f);
    //    }
    //    //else
    //    //{
    //    //    StepSucess();
    //    //}
    //}

    ///// <summary>
    ///// 步骤完成
    ///// </summary>
    //void StepSucess()
    //{
    //    DisableButtonList(true);  //打开所有关闭的按钮
    //    if (m_signTips != null)
    //        DestroyImmediate(m_signTips);
    //    if (m_buttonFlashing != null)
    //        DestroyImmediate(m_buttonFlashing);
    //    if (m_PlayerAttachArrow != null)
    //    {
    //        DestroyImmediate(m_PlayerAttachArrow.gameObject);
    //    }
		
    //    if(m_dialogPanel!= null)
    //        m_dialogPanel.GetComponent<EctypeGuideDialogPanel>().OnDestroy();
		
    //    for (int i = 0; i < m_stepEffect.Count; i++)
    //    {
    //        if (m_stepEffect[i] != null)
    //            DestroyImmediate(m_stepEffect[i]);
    //    }
    //    m_stepEffect.Clear();
    //    m_activeIndex = 1;
    //}

    //void OnDestroy()
    //{
    //    UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ClickTheGuideBtn, NextGuideHandle);
    //    Destroy(this.gameObject);
    //}

    protected override void RegisterEventHandler()
    {
        return;
    }
}
