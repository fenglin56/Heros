using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{
    public enum LuckDrawItemFlashState   // 点亮对应选项的状态
    {
        Normal,   //只有一盏灯点
        Double,   //双龙戏珠，主项和对称项点亮
        TripleHit, //连中三元,三个连续项点亮
        GetAll,    //所有中奖项闪烁


    }


    public class PlayerLuckDrawManager : BaseUIPanel 
    {
        //common title
        public CommonPanelTitle m_commonTitle;

        //item anchors
        public Transform[] m_itemAnchors;

        //buttons
        public SingleButtonCallBack m_backButton;
        public SingleButtonCallBack m_luckDrawOnce;
        public SingleButtonCallBack m_luckDrawTenTimes;

        //text
        public UILabel m_luckDrawLeftTimesText;
        public UILabel m_luckDrawLeftTimesNumber;

        public UILabel m_vipLevel;
        public UILabel m_luckDrawVipAddTmesText;
        public UILabel m_luckDrawVipAddTmesNumber;


        public UILabel m_luckDrawDescribeText;
        public UILabel m_luckDrawResetTips;

        public UILabel m_costOffText;
        public UILabel m_tenTimeCostText;

        public UILabel m_oneTimeCostText;

        //vip icon anchor
        public Transform m_vipIconAnchor;
        //effect on enter
        public GameObject m_OnPanelShowFlashEffectPrefab;
        //drop anim when begin to draw
        public GameObject m_OnDrawBeganEffectPrefab;
        //anim when draw end
        public GameObject m_OnDrawEndEffectPrefab;
        //get all effect prefab, play it on all get items
        public GameObject m_GetAllEffectPrefab;

        //effect anchor for both m_OnPanelShowFlashEffectPrefab and m_OnDrawEndEffectPrefab;
        public Transform m_CenterEffectAnchor;
        //effect anchor for m_OnDrawBeganEffectPrefab;
        public Transform m_DrawBeganEffectAnchor;



        //tip anchor
        public Transform m_SpecialTipAnchor;

        public GameObject m_luckDrawItemPrefab;

        public PlayerLuckDrawDataBase m_luckDrawDataBase;

        private GameObject m_specialTipObj;

        private Dictionary<int,LuckDrawItem> m_itemList = new Dictionary<int, LuckDrawItem>();
        private bool m_itemCreated = false;

        //anim params
        int m_currentSelectedItemIndex = 0;
        
        //anim step 1
        int m_startItemIndex;
        int m_endItemIndex;
        
        //anim step2
        int m_endItemIndex02;

        float m_startAnimLength;

        SMsgLuckDrawResult_SC m_currentResult;

        LuckDrawResultType m_currentDrawType;

        LuckDrawItemFlashState m_currentItemFlashState = LuckDrawItemFlashState.Normal;
        void Awake()
        {
            //init buttons
            m_backButton.SetCallBackFuntion(OnBackButtonClick);
            m_luckDrawOnce.SetCallBackFuntion(OnLuckDrawOnceClick);
            m_luckDrawTenTimes.SetCallBackFuntion(OnLuckDrawTenTimesClick);


            RegisterEventHandler();
            m_startAnimLength = (float)(CommonDefineManager.Instance.CommonDefine.LotteryThrowTime )/1000.0f;
            PlayEnterFlashAnim();

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            m_backButton.gameObject.RegisterBtnMappingId(UIType.PlayerLuckDraw, BtnMapId_Sub.PlayerLuckDraw_Back);
            m_luckDrawOnce.gameObject.RegisterBtnMappingId(UIType.PlayerLuckDraw, BtnMapId_Sub.PlayerLuckDraw_OneTime);
            m_luckDrawTenTimes.gameObject.RegisterBtnMappingId(UIType.PlayerLuckDraw, BtnMapId_Sub.PlayerLuckDraw_TenTimes);
        }

        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.LuckDrawResult, ReceiveLuckDrawResultHandel);
        }

        public void ReceiveLuckDrawResultHandel(object arg)
        {
            SMsgLuckDrawResult_SC sMsgLuckDrawResult_SC = (SMsgLuckDrawResult_SC)arg;
            m_currentResult = sMsgLuckDrawResult_SC;
            PlayResultAnim((LuckDrawResultType)(sMsgLuckDrawResult_SC.byType));

            if((LuckDrawResultType)sMsgLuckDrawResult_SC.byType == LuckDrawResultType.Normal)
            {
                if(sMsgLuckDrawResult_SC.resultInfoList[0].byID == 3 
                   || sMsgLuckDrawResult_SC.resultInfoList[0].byID == 7
                   || sMsgLuckDrawResult_SC.resultInfoList[0].byID == 12
                   || sMsgLuckDrawResult_SC.resultInfoList[0].byID == 16)
                {
                    MessageBox.Instance.ShowTips(3, "Wrong luck draw result", 2.0f);
                }
            }

        }

        void PlayResultAnim(LuckDrawResultType type)
        {
            ClearSpecialEffectTip();
            SetAllItemCountOrigin();
            StopAllCoroutines();
            m_currentItemFlashState = LuckDrawItemFlashState.Normal;


            switch(type)
            {
                case LuckDrawResultType.Normal:
                {
                    int endIndex = (int)(m_currentResult.resultInfoList[0].byID);

                    StartCoroutine(StartNormalAnim(endIndex));
                }

                    break;
                case LuckDrawResultType.TripleHit:
                {
                    int endIndex = (int)(m_currentResult.resultInfoList[0].byID);
                    int nextIndex = (int)(m_currentResult.resultInfoList[1].byID);
                    StartCoroutine(StartTripleHitAnim(endIndex, nextIndex));
                }
                    
                    break;
                case LuckDrawResultType.Double:
                {
                    int endIndex = (int)(m_currentResult.resultInfoList[0].byID);
                    int nextIndex = (int)(m_currentResult.resultInfoList[1].byID);
                    StartCoroutine(StartDoubleAnim(endIndex, nextIndex));
                }
                    
                    break;
                case LuckDrawResultType.RewardMultiple:
                {
                    int endIndex = (int)(m_currentResult.resultInfoList[0].byID);
                    int nextIndex = (int)(m_currentResult.resultInfoList[1].byID);
                    StartCoroutine(StartRewardMultipleAnim(endIndex, nextIndex));
                }
                    
                    break;
                case LuckDrawResultType.GetAll:
                {
                    int endIndex = (int)(m_currentResult.resultInfoList[0].byID);

                    StartCoroutine(StartGetAllAnim((int)(endIndex)));
                }
                    
                    break;
                case LuckDrawResultType.TenTimeDrawResult:
                {
                    PlayItemGet();
                }
                    
                    break;
                default:
                    break;

            }
        }


        void PlayItemGet()
        {
            foreach(SLuckDrawResultInfo info in m_currentResult.resultInfoList)
            {
                if(info.dwGoodsID != 0 && info.dwGoodsNum != 0)
                {
                    GoodsMessageManager.Instance.Show((int) info.dwGoodsID, (int)info.dwGoodsNum);
                }
            }
        }

        //连中三元动画
        IEnumerator StartTripleHitAnim(int endIndex,int  nextEndIndex)
        {
            PlayLuckDrawBeginAnim();
            yield return new WaitForSeconds(m_startAnimLength);
            
            yield return StartCoroutine(StartMainItemMoveAnim(m_currentSelectedItemIndex, endIndex));
            TryShowSpecialEffectTip();
            yield return new WaitForSeconds(1f);
            m_currentItemFlashState = LuckDrawItemFlashState.TripleHit;
            FreshItemFlash();
            yield return StartCoroutine(StartMainItemMoveAnim(m_currentSelectedItemIndex, nextEndIndex));
            PlayLuckDrawEndAnim();
            PlayItemGet();
        }

        //双龙戏珠动画
        IEnumerator StartDoubleAnim(int endIndex,int  nextEndIndex)
        {
            PlayLuckDrawBeginAnim();
            yield return new WaitForSeconds(m_startAnimLength);
            
            yield return StartCoroutine(StartMainItemMoveAnim(m_currentSelectedItemIndex, endIndex));
            TryShowSpecialEffectTip();
            yield return new WaitForSeconds(1f);
            m_currentItemFlashState = LuckDrawItemFlashState.Double;
            FreshItemFlash();
            yield return StartCoroutine(StartMainItemMoveAnim(m_currentSelectedItemIndex, nextEndIndex));
            PlayLuckDrawEndAnim();
            PlayItemGet();
        }

        //三倍奖励动画
        IEnumerator StartRewardMultipleAnim(int endIndex,int  nextEndIndex)
        {
            PlayLuckDrawBeginAnim();
            yield return new WaitForSeconds(m_startAnimLength);
            
            yield return StartCoroutine(StartMainItemMoveAnim(m_currentSelectedItemIndex, endIndex));
            TryShowSpecialEffectTip();
            SetAllItemCountMultiple();
            yield return new WaitForSeconds(1f);
            m_currentItemFlashState = LuckDrawItemFlashState.Normal;
            FreshItemFlash();
            yield return StartCoroutine(StartMainItemMoveAnim(m_currentSelectedItemIndex, nextEndIndex));
            PlayLuckDrawEndAnim();
            PlayItemGet();
            yield return new WaitForSeconds(1f);
            SetAllItemCountOrigin();
        }


        //乾坤一掷动画
        IEnumerator StartGetAllAnim(int endIndex)
        {
            PlayLuckDrawBeginAnim();
            yield return new WaitForSeconds(m_startAnimLength);

            yield return StartCoroutine(StartMainItemMoveAnim(m_currentSelectedItemIndex, endIndex));
            TryShowSpecialEffectTip();
            yield return new WaitForSeconds(1f);
            PlayGetAllFlash();
            yield return new WaitForSeconds(2.5f);
            PlayLuckDrawEndAnim();
            PlayItemGet();
        }

        void PlayGetAllFlash()
        {
            for(int i = 1; i < m_currentResult.resultInfoList.Count; i++)
            {
                GameObject flashObj = Instantiate(m_GetAllEffectPrefab) as GameObject;
                flashObj.AddComponent<DestroySelf>().Time = 2.5f;
                Transform flashObjTrans = flashObj.transform;
                flashObjTrans.parent = m_itemAnchors[(int)(m_currentResult.resultInfoList[i].byID)];
                flashObjTrans.localPosition = new Vector3(0, 0, -10);
                flashObjTrans.localScale = m_GetAllEffectPrefab.transform.localScale;
            }
        }


        //普通中奖动画
        IEnumerator StartNormalAnim(int endIndex)
        {
            m_currentItemFlashState = LuckDrawItemFlashState.Normal;
            PlayLuckDrawBeginAnim();
            yield return new WaitForSeconds(m_startAnimLength);

            yield return StartCoroutine(StartMainItemMoveAnim(m_currentSelectedItemIndex, endIndex));
            PlayLuckDrawEndAnim();
            PlayItemGet();
        }


        [ContextMenu("Play Draw Begin")]
        public void PlayLuckDrawBeginAnim()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_LotteryThrow");
            GameObject beginEffect = NGUITools.AddChild(m_DrawBeganEffectAnchor.gameObject,m_OnDrawBeganEffectPrefab);
            beginEffect.AddComponent<DestroySelf>().Time = 3.0f;
        }

        [ContextMenu("Play Draw End")]
        public void PlayLuckDrawEndAnim()
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_LotteryDrop");
            GameObject endEffect = NGUITools.AddChild(m_CenterEffectAnchor.gameObject,m_OnDrawEndEffectPrefab);
            endEffect.AddComponent<DestroySelf>().Time = 3.0f;
        }


        [ContextMenu("Play Enter Flash")]
        public void PlayEnterFlashAnim()
        {
            GameObject enterEffect = NGUITools.AddChild(m_CenterEffectAnchor.gameObject, m_OnPanelShowFlashEffectPrefab);
            //enterEffect.AddComponent<DestroySelf>().Time = 3.0f;
        }

        public void FreshItemFlash()
        {
            switch(m_currentItemFlashState)
            {
                case LuckDrawItemFlashState.Normal:
                {
                    for(int i = 0; i < m_itemList.Count; i++)
                    {
                        if(i == m_currentSelectedItemIndex)
                        {
                            m_itemList[i].SetSelected(true);
                        }
                        else
                        {
                            m_itemList[i].SetSelected(false);
                        }
                    }
                }
                    break;
                case LuckDrawItemFlashState.Double:
                {
                    int secondIndex = GetDoubleSecond(m_currentSelectedItemIndex);
                    for(int i = 0; i < m_itemList.Count; i++)
                    {
                        if(i == m_currentSelectedItemIndex || i == secondIndex)
                        {
                            m_itemList[i].SetSelected(true);
                        }
                        else
                        {
                            m_itemList[i].SetSelected(false);
                        } 
                    }
                }
                    break;
                case LuckDrawItemFlashState.TripleHit:
                {
                    int previousIndex = m_currentSelectedItemIndex - 1;
                    if(previousIndex < 0)
                    {
                        previousIndex = m_itemList.Count - 1;
                    }
                    int nextIndex = m_currentSelectedItemIndex + 1;
                    if(nextIndex > m_itemList.Count - 1)
                    {
                        nextIndex = 0;
                    }
                    for(int i = 0; i < m_itemList.Count; i++)
                    {
                        if(i == m_currentSelectedItemIndex || i == previousIndex || i == nextIndex)
                        {
                            m_itemList[i].SetSelected(true);
                        }
                        else
                        {
                            m_itemList[i].SetSelected(false);
                        } 
                           
                    }
                }
                    break;
                case LuckDrawItemFlashState.GetAll:
                {
                    for(int i = 0; i < m_itemList.Count; i++)
                    {
                        
                    }
                }
                    break;
            }

        }


        int GetDoubleSecond(int index)
        {
            int result = 0;
            int itemCount = m_itemList.Count;
            if(index >= (itemCount + 1)/2)
            {
                result = index - (itemCount + 1)/2;
            }
            else
            {
                result = index + (itemCount + 1)/2;
            }
            return result;
        }

        void Start()
        {

        }

        void Update()
        {
            int a = 0;
        }

        void Init()
        {


        }

        void SetupItems()
        {

        }

        void ResetItems()
        {

        }



        void SetupRightArea()
        {
            SetupRightAreaTextAndCost();
            RefreshDrawTime();
        }

        void SetupRightAreaTextAndCost()
        {
            //vip emblem
            GameObject vipEmblemPrefab = PlayerDataManager.Instance.GetCurrentVipEmblemPrefab();
            if(vipEmblemPrefab != null)
            {
                NGUITools.AddChild(m_vipIconAnchor.gameObject, vipEmblemPrefab);
            }


            m_luckDrawLeftTimesText.SetText(LanguageTextManager.GetString("IDS_I14_5"));
            m_luckDrawVipAddTmesText.SetText(LanguageTextManager.GetString("IDS_I14_6"));



            m_luckDrawDescribeText.SetText(LanguageTextManager.GetString("IDS_I14_7"));
            m_luckDrawResetTips.SetText(LanguageTextManager.GetString("IDS_I14_8"));


            m_costOffText.SetText(LanguageTextManager.GetString("IDS_I14_3"));
            m_oneTimeCostText.SetText(CommonDefineManager.Instance.CommonDefine.LotteryOneCost.ToString());
            m_tenTimeCostText.SetText(CommonDefineManager.Instance.CommonDefine.LotteryTenCost.ToString());
        }

        bool IsLeftDrawTimeEnough(int needTime)
        {
            var heroDataModel = PlayerManager.Instance.FindHeroDataModel();
            int leftDrawTime = heroDataModel.PlayerValues.PLAYER_FIELD_LUCKDRAW_VALUE;
            if(needTime <= leftDrawTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void RefreshDrawTime()
        {
            var heroDataModel = PlayerManager.Instance.FindHeroDataModel();
            int leftDrawTime = heroDataModel.PlayerValues.PLAYER_FIELD_LUCKDRAW_VALUE;
            int vipAdd = PlayerDataManager.Instance.GetVipAddDrawTimes();

            m_luckDrawLeftTimesNumber.SetText(leftDrawTime.ToString());
            m_luckDrawVipAddTmesNumber.SetText(vipAdd.ToString());


        }

        void TryShowSpecialEffectTip()
        {
            int item = (int)(m_currentResult.resultInfoList[0].byID);
            if(m_itemList[item].Data.m_tipPrefab != null)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_UIEff_LotteryHit");
                m_specialTipObj = UI.CreatObjectToNGUI.InstantiateObj(m_itemList[item].Data.m_tipPrefab, m_SpecialTipAnchor);
            }
        }

        void ClearSpecialEffectTip()
        {
            if(m_specialTipObj != null)
            {
                DestroyImmediate(m_specialTipObj);
                m_specialTipObj = null;
            }
        }

        void CreateItems()
        {
            foreach(PlayerLuckDrawData data in m_luckDrawDataBase.m_dataTable)
            {
                GameObject itemObj = Instantiate(m_luckDrawItemPrefab) as GameObject;
                LuckDrawItem item = itemObj.GetComponent<LuckDrawItem>();
                item.Setup(data);
                m_itemList[data.m_luckId] = item;

                itemObj.transform.parent = m_itemAnchors[data.m_luckId];
                itemObj.transform.localScale = Vector3.one;
                itemObj.transform.localPosition = new Vector3(0f, 0f, -0.1f);
            }
        }

        void ClearItems()
        {
            foreach(int key in m_itemList.Keys)
            {
                Destroy(m_itemList[key].gameObject);
            }
            m_itemList.Clear();
        }


        void SetItemFlash(List<int> itemIndexList, bool flash)
        {
            foreach(int index in itemIndexList)
            {
                SetItemFlash(index, flash);
            }
        }

        void SetItemFlash(int itemIndex, bool flash)
        {
            int indexInList = itemIndex;
            if(indexInList >=0 && indexInList < m_itemList.Count)
            {
                m_itemList[indexInList].SetSelected(flash);
            }
        }

        void SetAllItemCountMultiple()
        {
            foreach(LuckDrawItem item in m_itemList.Values)
            {
                item.MakeItemCountMultiple();
            }
        }

        void SetAllItemCountOrigin()
        {
            foreach(LuckDrawItem item in m_itemList.Values)
            {
                item.MakeItemCountOrigion();
            }
        }


        public override void Show(params object[] value)
        {
//           if(!m_itemCreated)
//            {
//                CreateItems();
//                m_itemCreated = true;
//            }
            ClearItems();
            CreateItems();
            m_currentSelectedItemIndex = 0;
            m_currentItemFlashState = LuckDrawItemFlashState.Normal;
            FreshItemFlash();
            ClearSpecialEffectTip();
            SetAllItemCountOrigin();
            SetupRightArea();
            m_commonTitle.TweenShow();
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_LotteryShine");

            base.Show(value);
        }
        
        public override void Close()
        {
            SetAllItemCountOrigin();
            StopAllCoroutines();
            m_commonTitle.tweenClose();
            base.Close();
        }


        void OnBackButtonClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_LotteryExit");
            Close();
        }


        void OnLuckDrawOnceClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_LotteryOneTime");
            if(!IsLeftDrawTimeEnough(1))
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I14_4"), 1f);
                return;
            }
            int pay = CommonDefineManager.Instance.CommonDefine.LotteryOneCost;
            if(!PlayerManager.Instance.IsBindPayEnough(pay))
            {
                MessageBox.Instance.ShowNotEnoughGoldMoneyMsg();
                return;
            }
            if (UI.MainUI.ContainerInfomanager.Instance.GetEmptyPackBoxNumber() < 1)
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_2"), 1);//背包已满
                return;
            }

            SMsgLuckDraw_CS sMsgLuckDraw_CS = new SMsgLuckDraw_CS();
            sMsgLuckDraw_CS.byType = 1;
            NetServiceManager.Instance.EquipStrengthenService.SendSMsgLuckDraw_CS(sMsgLuckDraw_CS);
        }


        void OnLuckDrawTenTimesClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_LotteryTenTime");
            if(!IsLeftDrawTimeEnough(10))
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_I14_4"), 1f);
                return;
            }
            int pay = CommonDefineManager.Instance.CommonDefine.LotteryTenCost;
            if(!PlayerManager.Instance.IsBindPayEnough(pay))
            {
                MessageBox.Instance.ShowNotEnoughGoldMoneyMsg();
                return;
            }

            if (UI.MainUI.ContainerInfomanager.Instance.GetEmptyPackBoxNumber() < 1)
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_2"), 1);//背包已满
                return;
            }

            SMsgLuckDraw_CS sMsgLuckDraw_CS = new SMsgLuckDraw_CS();
            sMsgLuckDraw_CS.byType = 10;
            NetServiceManager.Instance.EquipStrengthenService.SendSMsgLuckDraw_CS(sMsgLuckDraw_CS);
        }

        public void UpdateViaNotify(INotifyArgs inotifyArgs)//设置次数等属性
        {
            EntityDataUpdateNotify notify = (EntityDataUpdateNotify)inotifyArgs;
            if(notify.nEntityClass == TypeID.TYPEID_PLAYER && notify.IsHero)
            {
                RefreshDrawTime();
            }
        }


        
        
        IEnumerator StartMainItemMoveAnim(int startIndex, int endIndex)
        {
            float V = CommonDefineManager.Instance.CommonDefine.LotteryVelocity;
            float a = CommonDefineManager.Instance.CommonDefine.LotteryAcceleration;

            int N = m_itemAnchors.Length;
            int P1 = startIndex;
            int P2 = endIndex;
            int F = (int)(V/a);
            int S = (int)(0.5*a*F*F);
            int P3 = (P1 + S) % N;
            int P4 = P2 - S;
            while(P4 < 0)
            {
                P4 += N;
            }

            int S1 = P4 - P3;
            if(S1 < 0)
            {
                S1 += N;
            }
            int S2 = N + S1;
            int F1 = (int)(S2/V);




            float timeAfterMoveStart = 0;

            int tempIndex = 0;
            int frame = 0;
            bool end = false;
            while(!end)
            {
                if( frame <= F)
                {
                    tempIndex = (int)( (int)(P1 + 0.5f*a*frame*frame) %N );
                    if(frame == F)
                    {
                        int tt = tempIndex;
                    }
                }
                else if(frame > F && frame <= ( F1 + F ))
                {
                    tempIndex = ((int)(P3 + (frame-F)*V) %N);
                    if(frame == F1 + F)
                    {
                        int tt = tempIndex;
                    }
                }
                else if(frame > ( F1 + F ) && frame < (F1 + 2*F))
                {
                    int tempF = frame - ( F + F1);
                    tempIndex = ((int)(P4 + V *tempF - 0.5*a*tempF*tempF  )) %N;
                    if(frame == F1 + 2*F)
                    {
                        int tt = tempIndex;
                    }
                }
                else if(frame >= (F1 + 2*F))
                {
                    tempIndex = endIndex;
                    end = true;
                    //break;
                }
                if(tempIndex != m_currentSelectedItemIndex)
                {
                    m_currentSelectedItemIndex = tempIndex;
                    FreshItemFlash();
                    SoundManager.Instance.PlaySoundEffect("Sound_UIEff_LotteryShift");
                }
                timeAfterMoveStart += Time.deltaTime;
                frame = (int)(timeAfterMoveStart/0.033f);
                yield return null;
            }
            
            
            
            
        }


        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.LuckDrawResult, ReceiveLuckDrawResultHandel);
        }


        #region test
        [ContextMenu("Test")]
        public void TestLuckDraw()
        {
            SMsgLuckDrawResult_SC sMsgLuckDrawResult_SC;
            sMsgLuckDrawResult_SC.byType = 0;
            sMsgLuckDrawResult_SC.byNum = 1;
            SLuckDrawResultInfo info;
            info.byID = 11;
            info.dwGoodsID = 0;
            info.dwGoodsNum = 1;
            sMsgLuckDrawResult_SC.resultInfoList = new List<SLuckDrawResultInfo>();
            sMsgLuckDrawResult_SC.resultInfoList.Add(info);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.LuckDrawResult, sMsgLuckDrawResult_SC);

        }


        #endregion
    }
}
