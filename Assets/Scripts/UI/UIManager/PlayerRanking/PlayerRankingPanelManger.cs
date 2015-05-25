using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;
using System;
namespace UI.Ranking
{
    public class PlayerRankingPanelManger :  BaseUIPanel{
        //public ItemInfoTipsManager TipsMananger; 
        public SingleButtonCallBack BackButton;
        public SingleButtonCallBack NextButton;
        public SingleButtonCallBack PerButton;
        public UILabel MyRankLable;
        public UISprite NotInRankingSpring;
        public UILabel RankCountdownLable_des;
        public UILabel RankCountdownLable;
        public UILabel MyForceLable_des;
        public UISprite MyForceSprite;
        public UILabel MyForceLable;
        public SingleButtonCallBack PlayerRankingBtn;
        public SingleButtonCallBack SirenRankingBtn;
        public SingleButtonCallBack WeaponRankingBtn;
        public PlayerDetail_Ranking PlayerDetail;
        private PlayerRankingListPanel CurrentRankingListPnel;
//        private int PlayerRankingBasePageIndex=100;
//        private int SirenRankingBasePageIndex=100;
//        private int WeaponRankingBasePageIndex=100;
        private RankingType CurrentRankingType=RankingType.PlayerRanking;
        private int CurrentPageindex;
        private int PerPageIndex;
        private int PageCount;
        public  PlayerRankingListPanel SC_PlayerRankingList;
        public SirenRankingList SC_SirenRankingList;
        public WeaponRankingList SC_WeaponRankingList;

        private static PlayerRankingPanelManger instance;
        public static PlayerRankingPanelManger GetInstance ()
        {
            if (!instance) {
                instance = (PlayerRankingPanelManger)GameObject.FindObjectOfType (typeof(PlayerRankingPanelManger));
                if (!instance)
                    Debug.LogError ("没有附加JewelBesetManager脚本的gameobject在场景中");
            }
            return instance;
        }
        void Awake()
        {
            //RankCountdownLable_des.text=LanguageTextManager.GetString("IDS_I25_4");
            //MyForceLable_des.text=LanguageTextManager.GetString("IDS_I25_1");
            BackButton.SetCallBackFuntion(OnBackButtonClick);
            NextButton.SetCallBackFuntion(OnNextButtonClick);
            PerButton.SetCallBackFuntion(OnPerButtonClick);
            PlayerRankingBtn.SetCallBackFuntion(OnPlayerRankingBtnClick);
            SirenRankingBtn.SetCallBackFuntion(OnSirenRankingBtnClick);
            WeaponRankingBtn.SetCallBackFuntion(OnWeaponRankingBtnClick);
            RegisterEventHandler();
            CreatListPanel();

        }

        void CreatListPanel()
        {
         
            SC_PlayerRankingList.InitList(RankingType.PlayerRanking);
           
            SC_SirenRankingList.InitList(RankingType.SirenRanking);
           
            SC_WeaponRankingList.InitList(RankingType.WeaponRanking);
        }
        /// <summary>
        /// 是否显示下一页按钮，每次获取到数据后调用
        /// </summary>
        /// <param name="Ishow">If set to <c>true</c> ishow.</param>
        void CheckNextButton(bool Ishow)
        {
            if (Ishow)
            {
                NextButton.gameObject.SetActive(true);
            }
            else
            {
                NextButton.gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 是否显示上一页按钮，每次获取到数据后调用
        /// </summary>
        /// <param name="Ishow">If set to <c>true</c> ishow.</param>
        void CheckPerButton(bool Ishow)
        {
            if (Ishow)
            {
                PerButton.gameObject.SetActive(true);
            }
            else
            {
                PerButton.gameObject.SetActive(false);
            }
        }

        void GetCurrentPageData()
        {
            switch (CurrentRankingType)
            {
                case RankingType.PlayerRanking:
                {
                    var list = PlayerRankingDataManager.Instance.GetPlayerRankingListFromLocal(CurrentPageindex);
                  
                    if (list == null || list.Count == 0)
                    {
//                        if(PlayerRankingDataManager.Instance.PlayerRankingListDic.Count==0)
//                            CurrentPageindex=0;
                        PlayerRankingDataManager.Instance.GetListFromService(RankingType.PlayerRanking, CurrentPageindex);
                        LoadingUI.Instance.Show();
                      
                    }
                    else
                    {
                       SC_PlayerRankingList.StartRefershList(list);
                         CheckNextButton(CurrentPageindex<PlayerRankingDataManager.Instance.PlayerRankingPageCount);
                        CheckPerButton(CurrentPageindex!=1);
                        SetMyRanking(PlayerRankingDataManager.Instance.MyPlayerRanking);
                    }
                   
                }
                    break;
                case RankingType.SirenRanking:
                {
                    var list = PlayerRankingDataManager.Instance.GetSirenRankingListFromLocal(CurrentPageindex);
                   
                    if (list == null || list.Count == 0)
                    {
//                        if(PlayerRankingDataManager.Instance.PlayerRankingListDic.Count==0)
//                            CurrentPageindex=0;
                        PlayerRankingDataManager.Instance.GetListFromService(RankingType.SirenRanking, CurrentPageindex);
                        LoadingUI.Instance.Show();
                       
                    }
                    else
                    {

                        CheckNextButton(CurrentPageindex<PlayerRankingDataManager.Instance.SirenRankingPageCount);
                        CheckPerButton(CurrentPageindex!=1);
                        SC_SirenRankingList.StartRefershList(list);
                        SetMyRanking(PlayerRankingDataManager.Instance.MySirenRanking);
                    }
                  
                }
                    break;
                case RankingType.WeaponRanking:
                {
                    var list = PlayerRankingDataManager.Instance.GetWeaponRankingListFromLocal(CurrentPageindex);
                   
                    if (list == null || list.Count == 0)
                    {
//                        if(PlayerRankingDataManager.Instance.PlayerRankingListDic.Count==0)
//                            CurrentPageindex=0;
                        PlayerRankingDataManager.Instance.GetListFromService(RankingType.WeaponRanking, CurrentPageindex);
                        LoadingUI.Instance.Show();
                    }
                    else
                    {
                       
                         CheckNextButton(CurrentPageindex<PlayerRankingDataManager.Instance.WeaponRankingPageCount);
                        CheckPerButton(CurrentPageindex!=1);
                        SC_WeaponRankingList.StartRefershList(list);
                        SetMyRanking(PlayerRankingDataManager.Instance.MyWeaponRanking);
                    }
                }
                    break;
            }
          
        }

 

        void OnBackButtonClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_Exit");
            Close();
        }

        void OnNextButtonClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_SwitchPage");
            PerPageIndex=CurrentPageindex;
            CurrentPageindex++;
            GetCurrentPageData();
            if (CurrentPageindex == 10)
            {
                CheckNextButton(false);
            }
        }

        void OnPerButtonClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_SwitchPage");
            PerPageIndex=CurrentPageindex;
            CurrentPageindex--;
            GetCurrentPageData();
        }

        void OnPlayerRankingBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_SwitchPage");
            SC_PlayerRankingList.gameObject.SetActive(true);
            SC_SirenRankingList.gameObject.SetActive(false);
            SC_WeaponRankingList.gameObject.SetActive(false);
            CurrentPageindex=PlayerRankingDataManager.Instance.DefultPlayerRankingPage;
            PerPageIndex=0;
            CurrentRankingType=RankingType.PlayerRanking;
            GetCurrentPageData();
            PlayerRankingBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(2));
            SirenRankingBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(1));
            WeaponRankingBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(1));



            SetMyForce(RankingType.PlayerRanking);
        }

        void OnSirenRankingBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_SwitchPage");
            SC_PlayerRankingList.gameObject.SetActive(false);
            SC_SirenRankingList.gameObject.SetActive(true);
            SC_WeaponRankingList.gameObject.SetActive(false);
            CurrentPageindex=PlayerRankingDataManager.Instance.DefultSirenRankingPage;
            PerPageIndex=0;
            CurrentRankingType=RankingType.SirenRanking;
            GetCurrentPageData();
            PlayerRankingBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(1));
            SirenRankingBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(2));
            WeaponRankingBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(1));

            SetMyForce(RankingType.SirenRanking);
        }

        void OnWeaponRankingBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Ranking_SwitchPage");
            SC_PlayerRankingList.gameObject.SetActive(false);
            SC_SirenRankingList.gameObject.SetActive(false);
            SC_WeaponRankingList.gameObject.SetActive(true);
            CurrentPageindex=PlayerRankingDataManager.Instance.DefultWeaponRankingPage;
            PerPageIndex=0;
            CurrentRankingType=RankingType.WeaponRanking;
            GetCurrentPageData();
            PlayerRankingBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(1));
            SirenRankingBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(1));
            WeaponRankingBtn.spriteSwithList.ApplyAllItem(p=>p.ChangeSprite(2));

            SetMyForce(RankingType.WeaponRanking);
        }

    

        public void SetMyRanking(int rank)
        {
            if(rank<=0||rank>100)
            {
                NotInRankingSpring.gameObject.SetActive(true);
                MyRankLable.gameObject.SetActive(false);
            }
            else
            {
                NotInRankingSpring.gameObject.SetActive(false);
                MyRankLable.gameObject.SetActive(true);
                MyRankLable.SetText(rank);

            }
        }

        public void SetMyForce(RankingType type)
        {
            switch(type)
            {
                case RankingType.PlayerRanking:
                    MyForceSprite.spriteName="JH_UI_Typeface_1337";
                    MyForceSprite.transform.localScale=GetSpriteSize(MyForceSprite);
                   
                    MyForceLable.SetText( PlayerDataManager.Instance.GetHeroForce());
                        break;
                case RankingType.SirenRanking:
                    MyForceSprite.spriteName="JH_UI_Typeface_1338";
                    MyForceSprite.transform.localScale=GetSpriteSize(MyForceSprite);
                    MyForceLable.SetText( SirenDataManager.Instance.GetSirensCombatValue());
                    break;
                case RankingType.WeaponRanking:
                    MyForceSprite.spriteName="JH_UI_Typeface_1339";
                     MyForceSprite.transform.localScale=GetSpriteSize(MyForceSprite);
                    MyForceLable.SetText( GetWeaponForce());
                    break;

            }


        }


        Vector3 GetSpriteSize(UISprite sprite)
        {
          Rect rect=  sprite.GetAtlasSprite().outer;
            return new Vector3(rect.width,rect.height,1);
        }
        int GetWeaponForce()
        {
            int force=0;
            ItemFielInfo itemfileInfo=ContainerInfomanager.Instance.GetCurrentWeaponItemInfo();

            if(itemfileInfo!=null)
            {
                force=(int)EquipItem.GetEquipForce(itemfileInfo);
            }
            return force;
        }
        public void InitPanel()
        {
            OnPlayerRankingBtnClick(null);
            //OnSirenRankingBtnClick(null);
        }

	    public override void Show(params object[] value)
        {
            base.Show(value);
         
            InvokeRepeating("UpdateTime",0,0.5f);
            InitPanel();
        }


        float preTime = 0;
         void UpdateTime()
        {
           // float dddd = Time.realtimeSinceStartup - preTime;
            if (((Time.realtimeSinceStartup - PlayerRankingDataManager.Instance.RankUpateTimeSinceGameStart) >= PlayerRankingDataManager.Instance.UpdateRankInterval)&&PlayerRankingDataManager.Instance.IfNeedGetDataClearData())
            {
                PlayerRankingDataManager.Instance.ClearAllData();
                GetCurrentPageData();
                PlayerRankingDataManager.Instance.RankUpateTimeSinceGameStart = Time.realtimeSinceStartup;
            } else
            {
                preTime=PlayerRankingDataManager.Instance.UpdateRankInterval-(Time.realtimeSinceStartup - PlayerRankingDataManager.Instance.RankUpateTimeSinceGameStart);
                int hour = (int)preTime/3600;
                int minue = ((int)preTime%3600)/60;
                int second = (int)preTime%3600%60;
                RankCountdownLable.text = string.Format("{0:d2}:{1:d2}:{2:d2}", hour, minue, second);
            }
        }

        public override void Close()
        {
         
            base.Close();
            PlayerDetail.CloseDatailPanel();
            CancelInvoke("UpdateTime");
           // PlayerRankingDataManager.Instance.RankUpateTimeSinceGameStart=Time.realtimeSinceStartup;
        }


        void GetRankingListHandel(object obj)
        {
            LoadingUI.Instance.Close();
            SMsgInteract_RankingList_SC data=(SMsgInteract_RankingList_SC)obj;
          
            if(data.byRankingNum>0)
            {
            CurrentPageindex= data.byIndex;
            GetCurrentPageData();
            }
            else
            {
                CurrentPageindex=PerPageIndex;
            }
           
        }

        public void ShowDetailePanel(RankingType type,uint OtherId)
        {
            PlayerDetail.ShowPanel();
            PlayerRankingDataManager.Instance.GetPlayerDetailFromService(type,OtherId,(uint)PlayerManager.Instance.FindHeroDataModel().ActorID);
        }
        protected override void RegisterEventHandler()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ReceiveRankingListRes,GetRankingListHandel);
        }
        void OnDestroy()
        {
            instance=null;
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ReceiveRankingListRes,GetRankingListHandel);
        }
    }
}
