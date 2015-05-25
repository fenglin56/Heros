using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.Team
{
    /// <summary>
    /// 组队主面板
    /// </summary>
    public class TeamOrganizePanel : View
    {
        public UIGrid MyUIGrid;                     //队伍信息UI的父体    
        public TeamInfoItem ATeamInfoItem;          //队伍信息项 

        //public UIPanel ClippingPanel;               //裁剪视口面板
        //private Vector4 mClipRange;                 //裁剪范围

        public ItemPagerManager ItemPageManager_Team;

        public LocalButtonCallBack RefreshTeamInfoButtonCallBack;       //刷新按钮    
        public UIFilledSprite RefreshCDSprite;

        public LocalButtonCallBack LookingForTeamButtonCallBack;        //寻找队伍
        public LocalButtonCallBack QuickJoinTeamButtonCallBack;         //快速加入
        public LocalButtonCallBack CreateTeamButtonCallBack;            //创建队伍
        public LocalButtonCallBack ChatButtonCallBack;                  //聊天
        public LocalButtonCallBack LookingForCaptainButtonCallBack;     //寻找队长
        public LocalButtonCallBack ReturnButtonCallBack;                //返回

        public UILabel Label_AreaTitle;

        public Transform SearchingEff;  //搜索特效
        public GameObject NoneTeamTip;

        private int[] m_guideBtnID = new int[4];

        private List<TeamInfoItem> TeamInfoItemList = new List<TeamInfoItem>(); //储存回收TeamInfoItem

        private bool m_isQuickJoinTeamBtnCD = false;

        //\假设网络过来的信息:
        private int mItemNum = 3;

        private const float m_RefreshCDTime = 10f;

        void Awake()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(ReturnButtonCallBack.gameObject, MainUI.UIType.TeamInfo, SubType.ButtomCommon, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(QuickJoinTeamButtonCallBack.gameObject, MainUI.UIType.TeamInfo, SubType.ButtomCommon, out m_guideBtnID[1]);
            //TODO GuideBtnManager.Instance.RegGuideButton(CreateTeamButtonCallBack.gameObject, MainUI.UIType.TeamInfo, SubType.ButtomCommon, out m_guideBtnID[2]);
            //TODO GuideBtnManager.Instance.RegGuideButton(RefreshTeamInfoButtonCallBack.gameObject, MainUI.UIType.TeamInfo, SubType.ButtomCommon, out m_guideBtnID[3]);
        }

        void Start()
        {            

            //mClipRange = ClippingPanel.clipRange;
            RefreshTeamInfoButtonCallBack.SetCallBackFuntion(OnRefreshWorldTeamInfoClick, null);
            ReturnButtonCallBack.SetCallBackFuntion(OnCloseTeamMainPanel, null);
            CreateTeamButtonCallBack.SetCallBackFuntion(OnCreateTeamClick, null);
            QuickJoinTeamButtonCallBack.SetCallBackFuntion(OnQuickJoinTeamClick, null);
            ChatButtonCallBack.SetCallBackFuntion(OnChatClick, null);

            RegisterEventHandler();

            ItemPageManager_Team.InitPager(1, 1, 0);
        }

        public void ShowPanel()
        {
            transform.localPosition = Vector3.zero;
            UpdateAreaTitleLabel();            
        }
       
        public void ClosePanel()
        {
            transform.localPosition = new Vector3(0, 0, -800);
        }

        public void UpdateAreaTitleLabel()
        {
            //更新副本名称显示
			var currentEctype = TeamManager.Instance.CurrentEctypeData;
			//Todo：onSelectEctypeData协议已经取消难度dwEctypeID和byDiff难度，如果使用需要从新更改
//            if (EctypeConfigManager.Instance.EctypeSelectConfigList.ContainsKey((int)currentEctype.dwEctypeID))
//            {
//                var ectypeSelect = EctypeConfigManager.Instance.EctypeSelectConfigList[(int)currentEctype.dwEctypeID];
//                var ectypeID = ectypeSelect._vectContainer[currentEctype.byDiff - 1];
//                var ectypeInfo = EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeID];
//                Label_AreaTitle.text = LanguageTextManager.GetString(ectypeInfo.lEctypeName);
//            }
            //按钮动画
            PlayButtonAnimation();
        }


        //刷新队伍
        public void OnRefreshWorldTeamInfoClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            //ClippingPanel.clipRange = mClipRange;
            //ClippingPanel.transform.localPosition = new Vector3(0, -1f * mClipRange.y, 0);            
            
            //清掉之前的队伍信息
            CreateTeamInfoItems(new SMsgTeamNum_SC()
            {
                wTeamNum = 0,
            });

            //TraceUtil.Log("===> 刷新");
            //上发匹配队伍请求
            var player = PlayerManager.Instance.FindHeroDataModel();
            var ectypeData = TeamManager.Instance.CurrentEctypeData;

            NetServiceManager.Instance.TeamService.SendGetTeamListMsg(new SMSGGetTeamList_CS() 
            { 
				uidEntity = player.UID, 
				//Todo：onSelectEctypeData协议已经取消难度dwEctypeID和byDiff难度，如果使用需要从新更改
//                dwEctypeID = ectypeData.dwEctypeID, 
//                byDifficulty = ectypeData.byDiff 
            });

            if (obj == null)
            {
                StartCoroutine("RefreshCDTimeRestore");
                SearchingEff.gameObject.SetActive(true);                
                SearchingEff.animation.Play("JH_Eff_UI_TeamUpdate 1");
                StartCoroutine("Researching");
                NoneTeamTip.SetActive(false);
            }
        }

        IEnumerator RefreshCDTimeRestore()
        {
            RefreshCDSprite.fillAmount = 1;
            RefreshTeamInfoButtonCallBack.SetButtonActive(false);
            float i = 0;
            float rate = 1f / m_RefreshCDTime;
            while (i < 1f)
            {
                i += Time.deltaTime * rate;
                //
                RefreshCDSprite.fillAmount = 1 - i;

                yield return null;
            }
            RefreshCDSprite.fillAmount = 0;
            RefreshTeamInfoButtonCallBack.SetButtonActive(true);
        }
        IEnumerator Researching()
        {
            float time = CommonDefineManager.Instance.CommonDefine.LookingForTeamTime;
            yield return new WaitForSeconds(time);
            SearchingEff.animation.Play("JH_Eff_UI_TeamUpdate 2");
            time = SearchingEff.animation["JH_Eff_UI_TeamUpdate 2"].length;
            yield return new WaitForSeconds(time);
            //SearchingEff.animation.Stop();
            SearchingEff.gameObject.SetActive(false);
            if (TeamInfoItemList.Count <= 0)
            {
                NoneTeamTip.SetActive(true);
            }
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }
        //返回
        void OnCloseTeamMainPanel(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            transform.parent.transform.localPosition = new Vector3(0, 0, -800);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI,UI.MainUI.UIType.Battle);
        }
        //快速加入队伍
        void OnQuickJoinTeamClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            if (m_isQuickJoinTeamBtnCD)
                return;
            var player = PlayerManager.Instance.FindHeroDataModel();
            var ectypeData = TeamManager.Instance.CurrentEctypeData;
            NetServiceManager.Instance.TeamService.SendTeamFastJoinMsg(new SMsgTeamFastJoin_CS()
			                                                           {
				//Todo：onSelectEctypeData协议已经取消dwEctypeID和byDiff难度，如果使用需要从新更改
                //dwEctypeId = ectypeData.dwEctypeID,
                dwActorId = (uint)player.ActorID,
                //byEctypeDiff = ectypeData.byDiff 
            });
            StartCoroutine("QuickJoinTeamBtnCD");
        }
        IEnumerator QuickJoinTeamBtnCD()
        {
            m_isQuickJoinTeamBtnCD = true;
            yield return new WaitForSeconds(2f);
            m_isQuickJoinTeamBtnCD = false;
        }

        //创建队伍
        void OnCreateTeamClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            NetServiceManager.Instance.TeamService.SendTeamCreateMsg();
        }

        void OnChatClick(object obj)
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenWorldChatWindow, null);
        }

        public void CreateTeamInfoItems(SMsgTeamNum_SC sMsgTeamNum)
        {            
            mItemNum = sMsgTeamNum.wTeamNum;    //队伍数量

            if (mItemNum <= 0)
            {
                NoneTeamTip.SetActive(true);
            }
            else
            {
                NoneTeamTip.SetActive(false);
            }
            /*
            if (mItemNum > TeamInfoItemList.Count)
            {
                int addNum = mItemNum - TeamInfoItemList.Count;
                for (int i = 0; i < addNum; i++)
                {
                    GameObject obj = ((GameObject)Instantiate(ATeamInfoItem.gameObject));
                    TeamInfoItem item = obj.GetComponent<TeamInfoItem>();
                    item.InitInfo(i, MyUIGrid.transform);
                    TeamInfoItemList.Add(item);

                    //\
                    item.UpdateInfo(sMsgTeamNum.SMsgTeamProps[i]);
                }
                //TeamInfoItemList.ApplyAllItem(p => p.UpdateInfo());
            }
            else
            {
                int num = 0;
                TeamInfoItemList.ApplyAllItem(p =>
                {
                    if (num < mItemNum)
                    {
                        //\
                        p.UpdateInfo(sMsgTeamNum.SMsgTeamProps[num]);
                    }
                    else
                    {
                        p.Close();
                    }
                    num++;
                });
            }
            //排列
            MyUIGrid.repositionNow = true;

            */
            #region new page
            /*
            if (mItemNum > TeamInfoItemList.Count)
            {
                int addNum = mItemNum - TeamInfoItemList.Count;
                for (int i = 0; i < addNum; i++)
                {
                    GameObject obj = ((GameObject)Instantiate(ATeamInfoItem.gameObject));
                    TeamInfoItem item = obj.GetComponent<TeamInfoItem>();
                    item.InitInfo(i, ItemPageManager_Team.transform);
                    TeamInfoItemList.Add(item);
                    item.UpdateInfo(sMsgTeamNum.SMsgTeamProps[i]);
                }                
            }
            else
            {
                int num = 0;
                TeamInfoItemList.ApplyAllItem(p =>
                {
                    if (num < mItemNum)
                    {
                        p.UpdateInfo(sMsgTeamNum.SMsgTeamProps[num]);
                    }
                    else
                    {
                        p.Close();
                    }
                    num++;
                });
            }
            */
            #endregion

            TeamInfoItemList.ApplyAllItem(p =>
                {
                    Destroy(p.gameObject);
                });
            TeamInfoItemList.Clear();
            for (int i = 0; i < mItemNum; i++)
            {
                GameObject obj = ((GameObject)Instantiate(ATeamInfoItem.gameObject));
                TeamInfoItem item = obj.GetComponent<TeamInfoItem>();
                item.InitInfo(i, ItemPageManager_Team.transform);
                TeamInfoItemList.Add(item);               
                item.UpdateInfo(sMsgTeamNum.SMsgTeamProps[i]);
            }

            mItemNum = Mathf.Clamp(mItemNum, 1, 100);
            ItemPageManager_Team.InitPager(mItemNum, 1, 0);

           

                 
        }
        public void ItemPageChanged(PageChangedEventArg pageSmg)
        {
            TeamInfoItemList.ApplyAllItem(p =>
            {
                p.transform.position = new Vector3(-2000, 0, 0);
                //p.gameObject.SetActive(false);
            });
            int size = ItemPageManager_Team.PagerSize;
            var showTeamInfoArray = TeamInfoItemList.Skip((pageSmg.StartPage - 1) * size).Take(size).ToArray();
            //showTeamInfoArray.ApplyAllItem(p =>
            //    {
            //        p.gameObject.SetActive(true);
            //        var tweenScele = p.GetComponentInChildren<TweenScale>();
            //        tweenScele.Reset();
            //        tweenScele.Play(true);                                                    
            //    });
            ItemPageManager_Team.UpdateItems(showTeamInfoArray, "teamList");

            //加入百叶窗效果            
            int arrayLength = showTeamInfoArray.Length;
            for (int i = 0; i < arrayLength; i++)
            {
                showTeamInfoArray[i].PlayShutterAnimation(i * 0.1f);
            }
        }
        //播放按钮动画
        private void PlayButtonAnimation()
        {
            var tweenSceleArray =  ReturnButtonCallBack.transform.parent.GetComponentsInChildren<TweenScale>();
            tweenSceleArray.ApplyAllItem(p =>
            {
                p.Reset();
                p.Play(true);
            });
            var tweenPositionArray = ReturnButtonCallBack.transform.parent.GetComponentsInChildren<TweenPosition>();
            tweenPositionArray.ApplyAllItem(p =>
                {
                    p.Reset();
                    p.Play(true);
                });
        }

        protected override void RegisterEventHandler()
        {
            
        }
    }
}
