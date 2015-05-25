using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UI.Siren
{ 
    /// <summary>
    /// 炼妖面板
    /// </summary>
    public class SirenPanelUIManager : BaseUIPanel
    {

        public LocalButtonCallBack Button_Exit;
        public LocalButtonCallBack Button_Refinery;
        public LocalButtonCallBack Button_Touch;
        public LocalButtonCallBack Button_NedanCollection;

        //public ItemPagerManager ItemPageManager_Siren;
        //public SirenItemControl SirenItem;

        public SirenRefineryEffectControl EffControl_Refinery;
        public GameObject Mark_Refinery;
        public SirenModelViewControl ViewControl_Siren;
        public SirenDialogManager DialogManager_Siren;
        public CollectNeiDanAnimation CollectLianDanAnimationPrefab;
        

        public UISlicedSprite[] Sprite_AddAttribute = new UISlicedSprite[3];
        public UILabel[] Label_AddAttributeName = new UILabel[3];
        public UILabel[] Label_AddAttributeValue = new UILabel[3];
        public Transform[] ProcessPos = new Transform[4];
        private GameObject[] AddAttributeParent = new GameObject[3];

        //public UISlicedSprite Sprite_Item;
        public UILabel Label_NeedItemNum;
        public UILabel Label_Meditation;
        public UILabel Label_Upanishads;
        public UILabel Label_Process;

        public UILabel Label_RefineryCost;
        

        //interface
        public GameObject Interface_Lock;   //未解锁
        public GameObject Interface_Unlock; //解锁
        public GameObject Button_Lock;
        public LocalButtonCallBack Button_Unlock;
        public SpriteSwith Swith_EffUnlock;
        public UILabel Label_Explanation;

        public GameObject RefineryComplete;
        public GameObject RefineryCost;
        public UILabel Label_SitExplanation;

        //neidan
        public UILabel Label_NeiDanNum;
        public UILabel Label_NeiDanTime;
        private float m_NeiDanColdTime = 0;
        public Transform IconParent;
        private UISprite Icon_RefiningItem;

        //help
        public SingleButtonCallBack Button_Help;
        public SingleButtonCallBack Button_HelpFarme;
        public GameObject HelpObj;

        //page
        public UISlicedSprite Sprite_SirenName;
        public SingleButtonCallBack Button_PageUp;
        public SingleButtonCallBack Button_PageDown;
        public UILabel Label_Pagination;
        private int m_curSirenNo = 1;

        private const float EffUnlockFlashTime = 0.3f;

        //public GameObject BeSelectedSirenItem;
        //private GameObject m_BeSelectedSirenItem;

		private Dictionary<int, SirenItemControl_V3> m_SirenItemDict = new Dictionary<int, SirenItemControl_V3>();

        private int m_CurSelectedSirenItemID = 0;
        //private List<SirenInfo> m_SirenInfoList = new List<SirenInfo>();

        private const float RefineryUnderWayTime = 3f;
        private GameObject gEffRefineryUnderWay;
        private GameObject gEffRefineryResult;

        private SMsgActionLianHua_SC? m_LianHuaResult = null;

        //循环默认对白
        private float m_defaultDialogColdTime = 0;
        private float m_defaultWordCd = 10000;
        private SirenDialogConfigData m_defaultDialogConfigData;

        private int[] m_guideBtnID = new int[7];

        void Awake()
        {
            Button_Exit.SetCallBackFuntion(OnExitClick, null);
            Button_Refinery.SetCallBackFuntion(OnRefineryClick, null);
            Button_Touch.SetCallBackFuntion(OnTouchClick, null);
            Button_Unlock.SetCallBackFuntion(OnRefineryClick, null);
            Button_NedanCollection.SetCallBackFuntion(OnNedanCollectionClick, null);

            Button_Help.SetCallBackFuntion(OnHelpClick, null);
            Button_HelpFarme.SetCallBackFuntion(OnHelpClick, null);

            Button_PageUp.SetCallBackFuntion(OnPageUpClick, null);
            Button_PageDown.SetCallBackFuntion(OnPageDownClick, null);

            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Exit.gameObject, UIType.Siren, SubType.SirenMainButton, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Refinery.gameObject, UIType.Siren, SubType.SirenMainButton, out m_guideBtnID[1]);
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Touch.gameObject, UIType.Siren, SubType.SirenMainButton, out m_guideBtnID[2]);
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Unlock.gameObject, UIType.Siren, SubType.SirenMainButton, out m_guideBtnID[3]);
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_NedanCollection.gameObject, UIType.Siren, SubType.SirenMainButton, out m_guideBtnID[4]);
			//TODO GuideBtnManager.Instance.RegGuideButton(Button_PageUp.gameObject, UIType.Siren, SubType.SirenMainButton, out m_guideBtnID[5]);
			//TODO GuideBtnManager.Instance.RegGuideButton(Button_PageDown.gameObject, UIType.Siren, SubType.SirenMainButton, out m_guideBtnID[6]);
            ////TODO GuideBtnManager.Instance.RegGuideButton(Button_Unlock.gameObject, UIType.Siren, SubType.TopCommon, out m_guideBtnID[4]);

            this.RegisterEventHandler();

            InitSirenList();

            for (int i = 0; i < AddAttributeParent.Length; i++)
            {
                AddAttributeParent[i] = Sprite_AddAttribute[i].transform.parent.gameObject;
            }

            //移出摄像机
            ViewControl_Siren.transform.parent = null;
            ViewControl_Siren.transform.transform.localScale = Vector3.one;

            //内丹图标
            GameObject refiningItem = UI.CreatObjectToNGUI.InstantiateObj(IconPrefabManager.Instance.getIcon("RefiningItem"), IconParent);
            Icon_RefiningItem = refiningItem.GetComponent<UISprite>();
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.AddColdWork, AddColdWork);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.RemoveColdWork, RemoveColdWork);
            RemoveEventHandler(EventTypeEnum.LianHuaResult.ToString(), LianHuaResultHandle);            
        }

        public override void Show(params object[] value)
        {
            SoundManager.Instance.StopBGM(0.0f);
            SoundManager.Instance.PlayBGM("Music_UIBG_Siren", 0.0f);
            ViewControl_Siren.gameObject.SetActive(true);
            ViewControl_Siren.SetSirenSceneActive(true);
            //默认第一个解锁妖女
            if (SirenManager.Instance.GetYaoNvList().Count > 0)
            {
                m_CurSelectedSirenItemID = SirenManager.Instance.GetYaoNvList().First().byYaoNvID;
            }
            else//没有解锁 默认第一个妖女
            {
                m_CurSelectedSirenItemID = m_SirenItemDict.Keys.First();
            }
            this.SirenBeSelectedHandle(m_CurSelectedSirenItemID);

            //更新页码
            var sirenList = SirenDataManager.Instance.GetPlayerSirenList();

            for (int i = 0; i < sirenList.Count; i++)
            {
                if (sirenList[i]._sirenID == m_CurSelectedSirenItemID)
                {
                    m_curSirenNo = i + 1;
                    PageUpdate();
                    break;
                }
            }
            //播放按钮动画
            PlayButtonAnimation();
            base.Show(value);
        }

        public override void Close()
        {
            if (!IsShow)
                return;
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, null);
            GameManager.Instance.PlaySceneMusic();
            //transform.localPosition = new Vector3(0, 0, -800);
            HelpObj.SetActive(false);
            ViewControl_Siren.SetSirenSceneActive(false);
            ViewControl_Siren.gameObject.SetActive(false);
            base.Close();
        }

        void Update()
        {
            if (transform.localPosition != Vector3.zero)
                return;

            m_defaultDialogColdTime += Time.deltaTime;
            //TraceUtil.Log("" + m_defaultDialogColdTime + " ? " + m_defaultWordCd);
            if (m_defaultDialogColdTime >= m_defaultWordCd)
            {
                ShowSirenDialog(m_defaultDialogConfigData);
            }

            if (m_NeiDanColdTime > 0)
            {
                m_NeiDanColdTime -= Time.deltaTime;
                //TraceUtil.Log("[m_NeiDanColdTime]" + m_NeiDanColdTime);
                uint hour = (uint)(m_NeiDanColdTime / 3600);
                uint min = (uint)((m_NeiDanColdTime % 3600) / 60);                
                uint sec = (uint)(m_NeiDanColdTime % 60);
                string timeStr = string.Format(LanguageTextManager.GetString("IDS_H1_513"), PraseClock(hour), PraseClock(min), PraseClock(sec));
                //TraceUtil.Log("[timeStr]" + timeStr);
                Label_NeiDanTime.text = timeStr;
            }            

           // TraceUtil.Log("[NedanCollection boxcollider enabled] = " + Button_NedanCollection.GetComponent<BoxCollider>().enabled);
        }

        void OnHelpClick(object obj)
        {
            HelpObj.SetActive(!HelpObj.activeInHierarchy);
        }

        void OnPageUpClick(object obj)
        {
            if (m_curSirenNo <= 1)
                return;
            m_curSirenNo--;
            int dictLength = m_SirenItemDict.Count;
            //var list = m_SirenItemDict.ToArray();
            m_SirenItemDict[m_curSirenNo].OnButtonClick(null);
            Button_PageUp.BackgroundSprite.alpha = m_curSirenNo <= 1 ? 0.5f : 1f;
            Button_PageDown.BackgroundSprite.alpha = m_curSirenNo >= dictLength ? 0.5f : 1f;
            Label_Pagination.text = m_curSirenNo.ToString() + "/" + dictLength.ToString();
        }
        void OnPageDownClick(object obj)
        {
            int dictLength = m_SirenItemDict.Count;
            if (m_curSirenNo >= dictLength)
                return;
            m_curSirenNo++;
            //var list = m_SirenItemDict.ToArray();
            m_SirenItemDict[m_curSirenNo].OnButtonClick(null);
            Button_PageUp.BackgroundSprite.alpha = m_curSirenNo <= 1 ? 0.5f : 1f;
            Button_PageDown.BackgroundSprite.alpha = m_curSirenNo >= dictLength ? 0.5f : 1f;
            Label_Pagination.text = m_curSirenNo.ToString() + "/" + dictLength.ToString();
        }
        void PageUpdate()
        {
            int dictLength = m_SirenItemDict.Count;
            m_SirenItemDict[m_curSirenNo].OnButtonClick(null);
            Button_PageUp.BackgroundSprite.alpha = m_curSirenNo <= 1 ? 0.5f : 1f;
            Button_PageDown.BackgroundSprite.alpha = m_curSirenNo >= dictLength ? 0.5f : 1f;
            Label_Pagination.text = m_curSirenNo.ToString() + "/" + dictLength.ToString();
        }

        void OnNedanCollectionClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            NetServiceManager.Instance.EntityService.SendGetYaoNvNeiDan(m_CurSelectedSirenItemID);
            var sirenData = m_SirenItemDict[m_CurSelectedSirenItemID].GetSirenConfigData();
            for (int i = 0; i < sirenData._refiningItem_itemNum; i++)
            {
				CollectNeiDanAnimation collectLianDanAnimation = CreatObjectToNGUI.InstantiateObj(CollectLianDanAnimationPrefab.gameObject, transform).GetComponent<CollectNeiDanAnimation>();
                //
                GameObject icon = UI.CreatObjectToNGUI.InstantiateObj(IconPrefabManager.Instance.getIcon("RefiningItem"), IconParent);
                Vector3 scale = icon.transform.localScale;
                icon.transform.parent = collectLianDanAnimation.transform;
                icon.transform.localPosition = Vector3.zero;
                icon.transform.localScale = scale;

                Vector3 endPos = Button_Exit.transform.localPosition + Button_Exit.transform.parent.localPosition;
                Vector3 startPos = Button_NedanCollection.transform.localPosition;
                collectLianDanAnimation.Show(startPos, 1, endPos);
            }
            
        }
        void OnExitClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            Close();
            CleanUpUIStatus();
            //this.CloseSirenPanel();
        }
        void OnTouchClick(object obj)
        {            
            var sirenData = m_SirenItemDict[m_CurSelectedSirenItemID].GetSirenConfigData();
            //TraceUtil.Log("[触摸妖女]" + sirenData._touchAnim);
            if (ViewControl_Siren.PlayAnimation(sirenData._touchAnim))
            {
                SoundManager.Instance.PlaySoundEffect(sirenData._touchSound);//播放触摸语音    
            }
            ShowSirenDialog(sirenData._touchWord);                  
        }
        void OnRefineryClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Pay");
            var sirenControl = m_SirenItemDict[m_CurSelectedSirenItemID];
            if (sirenControl.SendLianHuaMsg())
            {
                if (sirenControl.IsUnlock())//如果已经解锁
                {
                    gEffRefineryUnderWay = EffControl_Refinery.PlayEff(SirenRefineryEffectControl.Refinery.Underway);
                    //开启遮罩
                    Mark_Refinery.SetActive(true);
                    //开启计时
                    StartCoroutine(RefineryUnderWay());
                    //妖女表现
                    //var sirenData = sirenControl.GetPlayerSirenConfigData();
                    //ViewControl_Siren.PlayAnimation(sirenData._fearAnim);
                    //DialogManager_Siren.CloseDialogImmediately();
                    
                    //振动效果
                    //var sirenData = sirenControl.GetSirenConfigData();
                    //ViewControl_Siren.FollowCamera.ShakeCamera(sirenData._refiningShakeTime, sirenData._refiningShakeAttenuation, sirenData._refiningShakeInitSpeed, sirenData._refiningShakeElasticity);
                    ViewControl_Siren.ShakeCamera();
                }
                else
                {
                    //解锁副本
                    StartCoroutine(UnlockSiren());
                }                
            }            
        }
        IEnumerator RefineryUnderWay()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_SirenLoading");

            yield return new WaitForSeconds(RefineryUnderWayTime);
            
            Mark_Refinery.SetActive(false);

            if (gEffRefineryUnderWay != null)
            {
                Destroy(gEffRefineryUnderWay);
            }
            
            if (m_LianHuaResult != null)
            {
                if (m_LianHuaResult.Value.bySucess == 0)//炼化失败
                {
                    SoundManager.Instance.PlaySoundEffect("Sound_UIEff_SirenFail");
                    gEffRefineryResult = EffControl_Refinery.PlayEff(SirenRefineryEffectControl.Refinery.Fail);
                    //妖女表现
                    var sirenData = m_SirenItemDict[m_CurSelectedSirenItemID].GetSirenConfigData();
                    ViewControl_Siren.PlayAnimation(sirenData._defaultAnim);                    
                    ShowSirenDialog(sirenData._failWord);
                }
                else//炼化成功
                {
                   
                    //更新界面
                    SirenItemControl_V3 sirenItemControl;
                    m_SirenItemDict.TryGetValue(m_LianHuaResult.Value.byYaoNvID, out sirenItemControl);

                    if (sirenItemControl == null)
                    {
                        yield return null;
                    }

                    List<SirenGrowthEffect> lastEffect = sirenItemControl.GetSirenGrowthEffect();
                    sirenItemControl.UpdateView(m_LianHuaResult.Value.byLianHuaLevel);//更新等级 和 界面

                    //特效
                    if (sirenItemControl.IsMaxLevel())
                    {
                        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_SirenComplete");
                        gEffRefineryResult = EffControl_Refinery.PlayEff(SirenRefineryEffectControl.Refinery.Complete);
                        //满级 置灰按钮
                        Button_Refinery.SetEnabled(false);                        
                    }
                    else
                    {
                        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_SirenSuccess");
                        gEffRefineryResult = EffControl_Refinery.PlayEff(SirenRefineryEffectControl.Refinery.Success);
                    }
                    //属性改变动画
                    List<SirenGrowthEffect> curEffect = sirenItemControl.GetSirenGrowthEffect();
                    for (int i = 0; i < curEffect.Count; i++)
                    {
                        GameObject clone = (GameObject)Instantiate(AddAttributeParent[i]);
                        Vector3 oriPos = AddAttributeParent[i].transform.localPosition;
                        clone.transform.parent = AddAttributeParent[i].transform.parent;
                        clone.transform.localScale = Vector3.one;
                        clone.AddComponent<UIPanel>();
                        var tweenAlpha = clone.AddComponent<TweenAlpha>();
                        tweenAlpha.duration = 0.5f;
                        tweenAlpha.from = 1;
                        tweenAlpha.to = 0;
                        var tweenPos = clone.AddComponent<TweenPosition>();
                        tweenPos.duration = 0.5f;
                        tweenPos.from = oriPos + new Vector3(0, 0, -2);
                        tweenPos.to = oriPos + new Vector3(0, 20, -2);
                        clone.AddComponent<DestroySelf>();
                        var textLabel = clone.GetComponentInChildren<UILabel>();
                        textLabel.text = ((curEffect[i].GrowthEffectValue - lastEffect[i].GrowthEffectValue) * CommonDefineManager.Instance.CommonDefine.Display_MaxMP).ToString();
                    }

                    //妖女表现                    
                    var sirenData = m_SirenItemDict[m_CurSelectedSirenItemID].GetSirenConfigData();
                    ViewControl_Siren.UpdateSiren(m_CurSelectedSirenItemID, sirenData);
                    ViewControl_Siren.PlayAnimation(sirenData._defaultAnim);
                    ShowSirenDialog(sirenData._successWord);
                    //振动效果
                    ViewControl_Siren.ShakeCamera();
                    //ViewControl_Siren.FollowCamera.ShakeCamera(sirenData._refiningShakeTime, sirenData._refiningShakeAttenuation, sirenData._refiningShakeInitSpeed, sirenData._refiningShakeElasticity);

                }
                m_LianHuaResult = null;
            }                        
        }

        IEnumerator UnlockSiren()
        {
            yield return new WaitForEndOfFrame();
            if (m_LianHuaResult == null)
            {
                StartCoroutine(UnlockSiren());
            }
            else
            {
                //更新界面
                SirenItemControl_V3 sirenItemControl;
                m_SirenItemDict.TryGetValue(m_LianHuaResult.Value.byYaoNvID, out sirenItemControl);                
                if (sirenItemControl != null)
                {
                    sirenItemControl.UpdateView(m_LianHuaResult.Value.byLianHuaLevel);
                }
                //妖女表现                
                var sirenData = m_SirenItemDict[m_CurSelectedSirenItemID].GetSirenConfigData();
                ViewControl_Siren.UpdateSiren(m_CurSelectedSirenItemID, sirenData);
				OnTouchClick(null);
//				ViewControl_Siren.PlayAnimation(sirenData._touchAnim);
//                ShowSirenDialog(sirenData._successWord);
                m_LianHuaResult = null;								                
            }
        }
        private void ShowSirenDialog(SirenDialogConfigData data)
        {
            m_defaultDialogColdTime = 0;
            //TraceUtil.Log("[SirenDialogConfigData.IDS]" + data.IDS);
            if (data.IDS != "0")
            {
                DialogManager_Siren.ShowDialog(data);
            }            
        }       
        private void ResetDefaultDialogConfigData(float cdTime, SirenDialogConfigData data)
        {
            m_defaultWordCd = cdTime / 1000;    //毫秒转换为秒
            m_defaultDialogConfigData = data;
        }
        //初始化女妖列表
        private void InitSirenList()
        {
            //var sirenList = PlayerDataManager.Instance.GetPlayerSirenList();
            var sirenList = SirenDataManager.Instance.GetPlayerSirenList();
            sirenList.ApplyAllItem(p =>
                {
                    //GameObject sirenItem = (GameObject)Instantiate(SirenItem.gameObject);
                    //sirenItem.transform.parent = ItemPageManager_Siren.transform;
                    //sirenItem.transform.localScale = Vector3.one;
                    //SirenItemControl itemCtrl = sirenItem.GetComponent<SirenItemControl>();
                    SirenItemControl_V3 itemCtrl = new SirenItemControl_V3();
                    itemCtrl.Init(p, SirenBeSelectedHandle);
                    m_SirenItemDict.Add(p._sirenID, itemCtrl);
                });
            //ItemPageManager_Siren.InitPager(sirenList.Count, 1, 0);
            Button_PageUp.BackgroundSprite.alpha = m_curSirenNo <= 1 ? 0.5f : 1f;
            Label_Pagination.text = "1/" + m_SirenItemDict.Count.ToString();
        }
        //翻页
        void OnPageChanged(PageChangedEventArg arg)
        {
            //m_SirenItemDict.Values.ApplyAllItem(p =>
            //    {
            //        p.transform.position = new Vector3(-2000, 0, 0);
            //    });
            //int size = ItemPageManager_Siren.ItemBgs.Length;
            //var sirenArray = m_SirenItemDict.Values.Skip((arg.StartPage - 1) * size).Take(size).ToArray();
            //ItemPageManager_Siren.UpdateItems(sirenArray, "SirenList");
        }
        //选择女妖调用
        void SirenBeSelectedHandle(int sirenId)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            //安全判断
            if (m_SirenItemDict.ContainsKey(sirenId) == false)
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"sirenId is null");
                return;
            }
            //更新当前选择的妖女id
            m_CurSelectedSirenItemID = sirenId;
            
            //选择框
            //if (m_BeSelectedSirenItem == null)
            //{
            //    m_BeSelectedSirenItem = (GameObject)Instantiate(BeSelectedSirenItem);
            //}
            //m_BeSelectedSirenItem.transform.parent = m_SirenItemDict[sirenId].transform;
            //m_BeSelectedSirenItem.transform.localPosition = Vector3.zero;
            //m_BeSelectedSirenItem.transform.localScale = Vector3.one;
            
            //获得妖女配置信息            
            var playerSirenConfigData = m_SirenItemDict[sirenId].GetPlayerSirenConfigData();
            var sirenData = m_SirenItemDict[sirenId].GetSirenConfigData();
			SirenDataManager.Instance.CurSelectSiren = m_SirenItemDict [sirenId];

            //重置默认对白信息
            ResetDefaultDialogConfigData(playerSirenConfigData._defaultWordCd, sirenData._defaultWord);
            
            if (m_SirenItemDict[sirenId].IsUnlock())//解锁
            {
                //界面
                Interface_Lock.SetActive(false);
                Interface_Unlock.SetActive(true);
                Button_Lock.SetActive(false);

                //显示进度
                Label_Process.text = m_SirenItemDict[sirenId].GetProcessValue();
                //显示花费
                Label_RefineryCost.text = sirenData._growthCost.ToString();
                //显示打坐
                Label_Meditation.transform.parent.gameObject.SetActive(true);
                Label_Meditation.text = sirenData._sitEffect + "%";

                //TraceUtil.Log("[选择妖女]is maxLevel = " + m_SirenItemDict[sirenId].IsMaxLevel());
                //满级 置灰按钮
                if (m_SirenItemDict[sirenId].IsMaxLevel())
                {
                    Button_Refinery.SetEnabled(false);
                    RefineryComplete.SetActive(true);
                    RefineryCost.SetActive(false);
                    //出现打坐加成提示
                    Label_SitExplanation.text = LanguageTextManager.GetString(sirenData._sitEffectTips);
                }
                else
                {
                    Button_Refinery.SetEnabled(true);
                    RefineryComplete.SetActive(false);
                    RefineryCost.SetActive(true);
                }

                //内丹
                Label_NeiDanNum.text = sirenData._refiningItem_itemNum.ToString();
                SetCollectionNedanButtonCDTime();

            }
            else//未解锁
            {
                //界面
                Interface_Lock.SetActive(true);
                Interface_Unlock.SetActive(false);

                //判断道具是否足够                
                int itemNum = UI.MainUI.ContainerInfomanager.Instance.GetItemNumber(playerSirenConfigData._composeCost_itemID);
                if (itemNum < playerSirenConfigData._composeCost_itemNum)
                {
                    ///道具不足
                    Button_Lock.SetActive(true);
                    Button_Unlock.gameObject.SetActive(false);
                    Swith_EffUnlock.gameObject.SetActive(false);
                    Label_NeedItemNum.color = Color.red;
                }
                else
                {
                    Button_Lock.SetActive(false);
                    Button_Unlock.gameObject.SetActive(true);
                    Swith_EffUnlock.gameObject.SetActive(true);
                    InvokeRepeating("EffUnlockFlashing", 0, EffUnlockFlashTime);
                    Label_NeedItemNum.color = Color.green;
                }

                //不显示打坐
                Label_Meditation.transform.parent.gameObject.SetActive(false);

                //显示需要道具
                //var itemData = ItemDataManager.Instance.GetItemData(playerSirenConfigData._composeCost_itemID);
                //Sprite_Item.spriteName = itemData.smallDisplay;
                Label_NeedItemNum.text = itemNum.ToString() + "/" + playerSirenConfigData._composeCost_itemNum.ToString();
                //道具来源说明
                Label_Explanation.text = LanguageTextManager.GetString(playerSirenConfigData._unlockTips);

            }
            //显示模型
            ViewControl_Siren.ShowSiren(sirenId, sirenData);

            //显示属性加成信息            
            var sirenGrowthEffectList = m_SirenItemDict[sirenId].GetSirenGrowthEffect();
            int growthItemLength = sirenGrowthEffectList.Count;
            for (int i = 0; i < 3; i++)
            {
                if (i >= growthItemLength)
                {
                    AddAttributeParent[i].SetActive(false);
                }
                else
                {
                    AddAttributeParent[i].SetActive(true);
                    
                    Sprite_AddAttribute[i].spriteName = sirenGrowthEffectList[i].EffectData.EffectRes;
                    Label_AddAttributeName[i].text = LanguageTextManager.GetString(sirenGrowthEffectList[i].EffectData.IDS);
                    Label_AddAttributeValue[i].text =HeroAttributeScale.GetScaleAttribute(sirenGrowthEffectList[i].EffectData,sirenGrowthEffectList[i].GrowthEffectValue).ToString();
                }
            }

            //显示奥义
            Label_Upanishads.text = sirenData._sirenPower.ToString();

            //收集数量、炼妖进度、奥义、打坐信息位置重置
            Label_Process.transform.parent.position = ProcessPos[growthItemLength+1].position;
            Label_NeedItemNum.transform.parent.position = ProcessPos[growthItemLength+1].position;
            Label_Upanishads.transform.parent.position = ProcessPos[growthItemLength].position;
            Label_Meditation.transform.parent.position = ProcessPos[growthItemLength + 2].position;

            //关闭对话框
            DialogManager_Siren.CloseDialogImmediately();

            //显示名字
            Sprite_SirenName.spriteName = playerSirenConfigData._nameRes;

            //消除残留炼化结果
            if (gEffRefineryResult != null)
            {
                Destroy(gEffRefineryResult);
            }
        }

        void EffUnlockFlashing()
        {
            if (Swith_EffUnlock.gameObject.activeInHierarchy == false)
            {
                CancelInvoke("EffUnlockFlashing");
            }
            else
            {
                int time = (int)(Time.time * (1 / EffUnlockFlashTime));
                int swith = time % 2;                
                Swith_EffUnlock.ChangeSprite(swith + 1);
            }

        }

        //播放按钮动画
        private void PlayButtonAnimation()
        {
            var tweenSceleArray = Button_Exit.transform.parent.GetComponentsInChildren<TweenScale>();
            tweenSceleArray.ApplyAllItem(p =>
            {
                p.Reset();
                p.Play(true);
            });
        }

        //处理炼化结果
        void LianHuaResultHandle(INotifyArgs arg)
        {
            m_LianHuaResult = (SMsgActionLianHua_SC)arg;                       
        }

        void AddColdWork(object obj)
        {
            ColdWorkInfo coldWork = (ColdWorkInfo)obj;
            if (coldWork.ColdClass == ColdWorkClass.Online)
            {
                SetCollectionNedanButtonCDTime();
            }
        }

        private void SetCollectionNedanButtonCDTime()
        {            
            var sirenData = m_SirenItemDict[m_CurSelectedSirenItemID].GetPlayerSirenConfigData();
            Int64 uid = PlayerManager.Instance.FindHeroDataModel().UID;
            ColdWorkInfo info = ColdWorkManager.Instance.GetColdWorkInfoClone(uid, ColdWorkClass.Online, (uint)sirenData._refiningColdTime);
            if (info != null && info.ColdTime > 0)
            {
                //cold
               // TraceUtil.Log("[Button_NedanCollection]" + Button_NedanCollection.gameObject.activeInHierarchy);
                
                Button_NedanCollection.SetBoxCollider(false);
                Button_NedanCollection.SetAlpha(0.5f);
                Icon_RefiningItem.alpha = 0.5f;
                Label_NeiDanNum.alpha = 0.5f;

                float coldTime = info.ColdTime / 1000f;

                m_NeiDanColdTime = coldTime;
                uint hour = (uint)(m_NeiDanColdTime / 3600);
                uint min = (uint)((m_NeiDanColdTime % 3600) / 60);
                uint sec = (uint)(m_NeiDanColdTime % 60);
                string timeStr = string.Format(LanguageTextManager.GetString("IDS_H1_513"), PraseClock(hour), PraseClock(min), PraseClock(sec));
                Label_NeiDanTime.text = timeStr;
            }
            else
            {
                m_NeiDanColdTime = 0;
                Button_NedanCollection.SetBoxCollider(true);
                Button_NedanCollection.SetAlpha(1);
                Icon_RefiningItem.alpha = 1;
                Label_NeiDanNum.alpha = 0.5f;
                Label_NeiDanTime.text = LanguageTextManager.GetString("IDS_H1_523");
            }
        }

        void RemoveColdWork(object obj)
        {
            SMsgActionColdWork_SC coldWork = (SMsgActionColdWork_SC)obj;

            if (coldWork.byClassID == (byte)ColdWorkClass.Online)
            {
                SetCollectionNedanButtonCDTime();
            }
        }
        private string PraseClock(uint time)
        {
            string str = time.ToString();
            if (time < 10)
            {
                str = "0" + str;
            }
            return str;
        }
        protected override void RegisterEventHandler()
        {
            //ItemPageManager_Siren.OnPageChanged += this.OnPageChanged;
            //AddEventHandler(EventTypeEnum.SirenBeSelected.ToString(), SirenBeSelectedHandle);
            AddEventHandler(EventTypeEnum.LianHuaResult.ToString(), LianHuaResultHandle);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.RemoveColdWork, RemoveColdWork);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.AddColdWork, AddColdWork);
        }
    }
}