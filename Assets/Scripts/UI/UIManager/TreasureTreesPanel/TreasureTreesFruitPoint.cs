using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public enum FruitPrucStatusType
    {
        NONE_FRUIT_STATUS_TYPE,			//默认状态
        SEED_FRUIT_STATUS_TYPE,			//种子阶段
        FLOWER_FRUIT_STATUS_TYPE,		//开花阶段
        GROW_FRUIT_STATUS_TYPE,			//结果阶段
        RIPEN_FRUIT_STATUS_TYPE,		//成熟阶段
    };

    public class TreasureTreesFruitPoint : MonoBehaviour
    {
        public UILabel FruitTitle;//果实提示
        public UISlider FruitProgressTitle;//果实生长进度条
        public SpriteSwith ProgressBarSwith;
        public UISprite FruitIcon;//果实图标
        public GameObject LockICon;//锁住状态图标
        public GameObject OpenWithHC; //元宝开启图片文字
        //public GameObject FruitDryStatusIcon;//干旱状态图标
        public GameObject FruitPickStatusIcon;//可摘取状态图标
        public GameObject WateringFruitAnimPrefab;//浇水动画图标预置体
        public GameObject FlashEffectPrefab;//闪动特效预置体
        public GameObject PickUpFruitAnimPrefab;//摘取动画图标预置体
        public GameObject UsingAmritaAnimPrefab;//使用仙露动画预置体
        public GameObject UnLockAnimPrefab;//解锁动画Prefab

        public Transform CreatFlashEffectPoint;
        public Transform CreatCenterAnimPoint;
        public Transform CreatLeftUpAnimPoint;

        public int MyPositionID { get; private set; }

        TreasureTreesPanelManager myParent;


        private bool IsDroughtStatus = false;

        public LocalTreasureTreesData MyTreeData { get; private set; }
        public SMsgActionFruitContext_SC MyFruitData { get; private set; }
        public FruitData MyLocalFruitData { get; private set; }

        private Vector3 m_LocalPosition;

        private int m_guideBtnID = 0;

        public void Init(TreasureTreesPanelManager myParent,int myPosition)
        {
            this.myParent = myParent;
            MyPositionID = myPosition;
            //TraceUtil.Log("InitPosition:"+myPosition);
            MyTreeData = myParent.TreasureTreesDataBase.TreasureTreesDataList.First(P=>P.PositionID == myPosition);
            //TODO GuideBtnManager.Instance.RegGuideButton(this.gameObject, UIType.Treasure, SubType.TreasureTreesItem, out m_guideBtnID);
            switch (myPosition)
            {
                case 1:
                    gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_Fruit1);
                    break;
                case 2:
                    gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_Fruit2);
                    break;
                case 3:
                    gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_Fruit3);
                    break;
                case 4:
                    gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_Fruit4);
                    break;
                case 5:
                    gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_Fruit5);
                    break;
                case 6:
                    gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_Fruit6);
                    break;
                case 7:
                    gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_Fruit7);
                    break;
                case 8:
                    gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_Fruit8);
                    break;
            }
        }

        IEnumerator Start()
        {
            m_LocalPosition = transform.localPosition;
            yield return new WaitForSeconds(Random.Range(0f, 2f));
            MoveUp(null);
        }

        public void UpdateFruitPointStatus()
        {
            MyFruitData = TreasureTreesData.Instance.FruitDataList.FirstOrDefault(P => P.byFruitPosition == MyPositionID);
            //TraceUtil.Log("下发果实ID：" + MyFruitData.dwFruitID + ",MyPositionID:" + MyPositionID);
            MyLocalFruitData = myParent.TreasureTreesDataBase.FruitDataList.FirstOrDefault(P => P.FruitID == MyFruitData.dwFruitID);
            ResetFruitPointStatus();
            if (MyFruitData.dwFruitID != 0)
            {
                //TraceUtil.Log("刷新果实状态,是否干旱："+MyFruitData.byFruitDryStatus+"，是否成熟:"+MyFruitData.byFruitStatus);
                LockICon.SetActive(false);
                CreatFlashEffectPoint.ClearChild();
                ShowFruitIcon();
                if (!ShowIsDroughtStatus())
                {
                    //TraceUtil.Log("果实状态:" + MyFruitData.byFruitStatus+",是否干旱："+MyFruitData.byFruitDryStatus);
                    switch ((FruitPrucStatusType)MyFruitData.byFruitStatus)
                    {
                        case FruitPrucStatusType.NONE_FRUIT_STATUS_TYPE:
                            break;
                        case FruitPrucStatusType.SEED_FRUIT_STATUS_TYPE:
                            FruitIcon.spriteName = MyLocalFruitData.SeedModelID;
                            ShwoGrowingStates();
                            break;
                        case FruitPrucStatusType.FLOWER_FRUIT_STATUS_TYPE:
                            FruitIcon.spriteName = MyLocalFruitData.FlowerModelID;
                            ShwoGrowingStates();
                            break;
                        case FruitPrucStatusType.GROW_FRUIT_STATUS_TYPE:
                            FruitIcon.spriteName = MyLocalFruitData.GrowModelID;
                            ShwoGrowingStates();
                            break;
                        case FruitPrucStatusType.RIPEN_FRUIT_STATUS_TYPE:
                            FruitIcon.spriteName = MyLocalFruitData.RipenModelID;
                            ShowCantPickStatus();
                            break;
                    }
                }
            }
            else
            {
                //锁住状态
                LockICon.SetActive(true);

                int UnlockLevel = myParent.TreasureTreesDataBase.TreasureTreesDataList.First(P => P.PositionID == MyPositionID).UnlockLevel;

                if(UnlockLevel > 0)
                {
                    FruitTitle.SetText( string.Format(LanguageTextManager.GetString("IDS_H1_462"), UnlockLevel));
                    OpenWithHC.SetActive(false);
                }
                else
                {
                    FruitTitle.SetText("");
                    OpenWithHC.SetActive(true);
                }
            }
        }

        void ShowFruitIcon()
        {
            switch ((FruitPrucStatusType)MyFruitData.byFruitStatus)
            {
                case FruitPrucStatusType.NONE_FRUIT_STATUS_TYPE:
                    break;
                case FruitPrucStatusType.SEED_FRUIT_STATUS_TYPE:
                    FruitIcon.spriteName = MyLocalFruitData.SeedModelID;
                    break;
                case FruitPrucStatusType.FLOWER_FRUIT_STATUS_TYPE:
                    FruitIcon.spriteName = MyLocalFruitData.FlowerModelID;
                    break;
                case FruitPrucStatusType.GROW_FRUIT_STATUS_TYPE:
                    FruitIcon.spriteName = MyLocalFruitData.GrowModelID;
                    break;
                case FruitPrucStatusType.RIPEN_FRUIT_STATUS_TYPE:
                    FruitIcon.spriteName = MyLocalFruitData.RipenModelID;
                    break;
            }
        }

        /// <summary>
        /// 重置为默认状态
        /// </summary>
        void ResetFruitPointStatus()
        {
            FruitTitle.SetText("");
            LockICon.SetActive(true);
            //FruitDryStatusIcon.SetActive(false);
            FruitPickStatusIcon.SetActive(false);
            FruitProgressTitle.gameObject.SetActive(false);
        }
        /// <summary>
        /// 显示是否干旱状态
        /// </summary>
        /// <returns></returns>
        bool ShowIsDroughtStatus()
        {
            IsDroughtStatus = MyFruitData.byFruitDryStatus == 1 && MyFruitData.byFruitStatus != (byte)FruitPrucStatusType.RIPEN_FRUIT_STATUS_TYPE;
            if (IsDroughtStatus)
            {
                //FruitDryStatusIcon.SetActive(true);
            }
            return IsDroughtStatus;
        }

        /// <summary>
        /// 显示果实生长进度
        /// </summary>
        void ShwoGrowingStates()
        {
            ProgressBarSwith.ChangeSprite(MyLocalFruitData.FruitLevel+1);
            FruitTitle.SetText(LanguageTextManager.GetString(MyLocalFruitData.FruitName));
            FruitProgressTitle.gameObject.SetActive(true);
            CancelInvoke();
            InvokeRepeating("SetProgressBar",0,5f);
        }

        void SetProgressBar()
        {
            long nowTime = TreasureTreesData.Instance.GetNowTimes();
            FruitProgressTitle.sliderValue = (float)(nowTime - MyFruitData.dwStartTimes) / (float)(MyFruitData.dwEndTime - MyFruitData.dwStartTimes);
        }

        /// <summary>
        /// 显示果实可摘取状态
        /// </summary>
        void ShowCantPickStatus()
        {
            FruitPickStatusIcon.SetActive(true);
            CreatObjectToNGUI.InstantiateObj(FlashEffectPrefab,CreatFlashEffectPoint);
        }

        void OnClick()
        {
            switch ((FruitPrucStatusType)MyFruitData.byFruitStatus)
            {
                case FruitPrucStatusType.NONE_FRUIT_STATUS_TYPE:
                    int unlockLevel = myParent.TreasureTreesDataBase.TreasureTreesDataList.First(P=>P.PositionID == MyPositionID).UnlockLevel;
                    if (unlockLevel < 0)
                    {
                        string ShowMsg = unlockLevel > 0 ? string.Format(LanguageTextManager.GetString("IDS_H1_464"), unlockLevel) : LanguageTextManager.GetString("IDS_H1_465");
                        //MessageBox.Instance.Show(3, "", ShowMsg, LanguageTextManager.GetString("IDS_H2_55"), LanguageTextManager.GetString("IDS_H2_28"), SendUnlockMyFruitPositionToSever, null);
                        bool CanBuy = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY >= MyTreeData.UnlockCost;//元宝是否足够
                        myParent.ShowCostMoneyMessageBox(CanBuy, EMessageCoinType.EGoldType, MyTreeData.UnlockCost, ShowMsg,
                            LanguageTextManager.GetString("IDS_H2_11"), LanguageTextManager.GetString("IDS_H2_28"), SendUnlockMyFruitPositionToSever, null);
                    }
                    break;
                case FruitPrucStatusType.SEED_FRUIT_STATUS_TYPE:
                    OnFruitClick();
                    break;
                case FruitPrucStatusType.FLOWER_FRUIT_STATUS_TYPE:
                    OnFruitClick();
                    break;
                case FruitPrucStatusType.GROW_FRUIT_STATUS_TYPE:
                    OnFruitClick();
                    break;
                case FruitPrucStatusType.RIPEN_FRUIT_STATUS_TYPE:
                    myParent.PickUpFruit((byte)MyPositionID);
                    break;
            }
        }

        void OnFruitClick()
        {
            if (IsDroughtStatus)
            {
                myParent.WateringFruit(MyFruitData.byFruitPosition);
            }
            else
            {
                bool haveAmrita = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MANNA_NUM > 0;
                if (haveAmrita)//如果有仙露
                {
                    myParent.SendUseAmritaToSever((byte)MyPositionID);
                }
                else
                {
                    int buyNum = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAXMANNA_BUYNUM;
                    bool CanBuyMana = buyNum < CommonDefineManager.Instance.CommonDefine.FruitMannan_CountMax;
                    if (CanBuyMana)//如果可以购买仙露
                    {
                        int leftTime = CommonDefineManager.Instance.CommonDefine.FruitMannan_CountMax - PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAXMANNA_BUYNUM;
                        bool CanBuy = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY >= CommonDefineManager.Instance.CommonDefine.FruitMannan_Pay;//元宝是否足够

                        string formatStr = LanguageTextManager.GetString("IDS_I28_16").Replace("\\n", "\n");
                        string ShowMsg = string.Format(formatStr, CommonDefineManager.Instance.CommonDefine.FruitMannan_Count, leftTime);

                        //string ShowMsg = string.Format("{0}\n{1}",LanguageTextManager.GetString("IDS_H1_466"),LanguageTextManager.GetString("IDS_H1_467"));
                        string sureBtnStr = LanguageTextManager.GetString("IDS_H2_55");
                        string cancelBtnStr = LanguageTextManager.GetString("IDS_H2_28");
                        int A = CommonDefineManager.Instance.CommonDefine.BuyFruitMannanConsumption1;
                        int B = CommonDefineManager.Instance.CommonDefine.BuyFruitMannanConsumption2;
                        int C = CommonDefineManager.Instance.CommonDefine.BuyFruitMannanConsumption3;
                        int D = CommonDefineManager.Instance.CommonDefine.BuyFruitMannanConsumption4;
                        int buyTimes = buyNum/CommonDefineManager.Instance.CommonDefine.FruitMannan_Count + 1;
                        int buyPrice = Mathf.FloorToInt((A * buyTimes * buyTimes + B * buyTimes + C)/(float)D) * D;

                        myParent.ShowCostMoneyMessageBox(CanBuy, EMessageCoinType.EGoldType, buyPrice, ShowMsg, sureBtnStr, cancelBtnStr, SendBuyAmritaToSever, null);
                    }
                    else
                    {
                        SoundManager.Instance.PlaySoundEffect("Sound_Button_Tree_BuyFail");
                        MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_H1_470"),1);//超出购买上限提示
                    }
                }
            } 
        }

        void SendBuyAmritaToSever()
        {
            bool CanBuy = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY >= CommonDefineManager.Instance.CommonDefine.FruitMannan_Pay;//元宝是否足够
            if(CanBuy)
            {
                myParent.SendBuyAmritaToSever(5);
            }
            else
            {
                MessageBox.Instance.ShowNotEnoughGoldMoneyMsg();
            }
        }

        void ShowGoldMoneyNotEnoughTips()//元宝不足提示
        {
            MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_44"), 1);
        }

        void SendUnlockMyFruitPositionToSever()
        {
            bool CanBuy = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY >= MyTreeData.UnlockCost;//元宝是否足够
            if (CanBuy)
            {
                myParent.SendUnlockFruitPointToSever((byte)MyPositionID);
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Tree_BuySuccess");
            }
            else
            {
                MessageBox.Instance.ShowNotEnoughGoldMoneyMsg();
                //ShowGoldMoneyNotEnoughTips();
            }
        }

        public IEnumerator WateringFruit()
        {
            TraceUtil.Log("WateringFruit");
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_FruitWatering");
            myParent.FruitLogMessageWindow.AddWateringFruitLogInfo(this);
            CreatLeftUpAnimPoint.ClearChild();
            CreatFlashEffectPoint.ClearChild();
            CreatObjectToNGUI.InstantiateObj(FlashEffectPrefab,CreatFlashEffectPoint);
            CreatObjectToNGUI.InstantiateObj(WateringFruitAnimPrefab, CreatLeftUpAnimPoint);
            yield return new WaitForSeconds(1);
            CreatLeftUpAnimPoint.ClearChild();
            yield return new WaitForSeconds(1.2f);
            if ((FruitPrucStatusType)MyFruitData.byFruitStatus != FruitPrucStatusType.RIPEN_FRUIT_STATUS_TYPE)
            {
                CreatFlashEffectPoint.ClearChild();
            }
        }

        public IEnumerator UsingAmrita()
        {
            TraceUtil.Log("UsingAmrita");
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_FruitWatering");
            myParent.FruitLogMessageWindow.AddUserAmritaLogInfo(this);
            CreatLeftUpAnimPoint.ClearChild();
            CreatObjectToNGUI.InstantiateObj(UsingAmritaAnimPrefab, CreatLeftUpAnimPoint);
            yield return new WaitForSeconds(1);
            CreatLeftUpAnimPoint.ClearChild();
        }

        public IEnumerator PickUpFruit()
        {
            TraceUtil.Log("PickUpFruit");
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_FruitGain");
            myParent.FruitLogMessageWindow.AddGetFruitLogInfo(this);
            CreatCenterAnimPoint.ClearChild();
            CreatObjectToNGUI.InstantiateObj(PickUpFruitAnimPrefab, CreatCenterAnimPoint);
            yield return new WaitForSeconds(1);
            CreatCenterAnimPoint.ClearChild();
        }

        public IEnumerator UnLockFruitPoint()
        {
            TraceUtil.Log("UnLockFruitPoint");
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_FruitUnlock");
            CreatCenterAnimPoint.ClearChild();
            LockICon.SetActive(false);
            CreatObjectToNGUI.InstantiateObj(UnLockAnimPrefab,CreatCenterAnimPoint);
            yield return new WaitForSeconds(1);
            CreatCenterAnimPoint.ClearChild();
        }

        void MoveUp(object obj)
        {
            Vector3 fromPosition = m_LocalPosition;
            Vector3 toPosotion = fromPosition+new Vector3(0,10,0);
            TweenPosition.Begin(gameObject,2f,fromPosition,toPosotion,MoveDown);
        }

        void OnDestroy()
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        void MoveDown(object obj)
        {
            Vector3 fromPosition = transform.localPosition;
            Vector3 toPosotion = fromPosition + new Vector3(0, -10, 0);
            TweenPosition.Begin(gameObject, 2f, fromPosition, toPosotion, MoveUp);
        }
    }
}