using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class TreasureTreesButtonToolManager : View
    {
        public enum FlashBtnType {WateringBtn,PickUpBtn }

        //public SingleButtonCallBack wateringFruitBtn;
        public SingleButtonCallBack PickUpFruitBtn;
        public SingleButtonCallBack UseManaBtn;

        private Vector3 WateringBtnPosition;
        private Vector3 PickUpBtnPosition;

        //private GameObject TweenWateringFruitBtnObj;
        //private GameObject TweenPickUpBtnObj;

        private bool CanWatering = false;
        private bool CanPickUp = false;

        TreasureTreesPanelManager MyParent;

        private int[] m_guideBtnID = new int[3];

        void Awake()
        {
            //WateringBtnPosition = wateringFruitBtn.transform.localPosition;
            PickUpBtnPosition = PickUpFruitBtn.transform.localPosition;
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
            //wateringFruitBtn.SetCallBackFuntion(OnWateringFruitBtnClick);
            PickUpFruitBtn.SetCallBackFuntion(OnPickUpFruitBTnClick);
            UseManaBtn.SetCallBackFuntion(OnUseManaBtnClick);

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            PickUpFruitBtn.gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_Catch);
            UseManaBtn.gameObject.RegisterBtnMappingId(UIType.Treasure, BtnMapId_Sub.Treasure_Amrita);
        }

        void Start()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(this.wateringFruitBtn.gameObject, UIType.Treasure, SubType.TreasureTreesMainButton, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(this.PickUpFruitBtn.gameObject, UIType.Treasure, SubType.TreasureTreesMainButton, out m_guideBtnID[1]);
            //TODO GuideBtnManager.Instance.RegGuideButton(this.UseManaBtn.gameObject, UIType.Treasure, SubType.TreasureTreesMainButton, out m_guideBtnID[2]);
        }

        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        public void Init(TreasureTreesPanelManager MyParent)
        {
            this.MyParent = MyParent;
            SetAddManaBtnStatus();
        }

        public void UpdateViaNotify(INotifyArgs inotifyArgs)//设置各种属性
        {
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
                SetAddManaBtnStatus();
            }
        }

        void SetAddManaBtnStatus()
        {
            this.UseManaBtn.SetButtonText(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MANNA_NUM.ToString());
            //TraceUtil.Log(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MANNA_NUM + "," + PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAXMANNA_BUYNUM);
        }

        public void ResetBtnStatus()
        {
            CanWatering = TreasureTreesData.Instance.FruitDataList.Exists(P => P.byFruitDryStatus == 1&&P.byFruitStatus!=(byte)FruitPrucStatusType.RIPEN_FRUIT_STATUS_TYPE);
            CanPickUp = TreasureTreesData.Instance.FruitDataList.Exists(P=>P.byFruitStatus == (byte)FruitPrucStatusType.RIPEN_FRUIT_STATUS_TYPE);
            //wateringFruitBtn.BackgroundSprite.color = CanWatering?Color.white: Color.gray;
            PickUpFruitBtn.BackgroundSprite.color = CanPickUp?Color.white: Color.gray;
            if (CanWatering)
            {
                //FlashBtn(FlashBtnType.WateringBtn);
            }
            else
            {
                //StopFlashBtn(FlashBtnType.WateringBtn);
            }
            if (CanPickUp)
            {
                FlashBtn(FlashBtnType.PickUpBtn);
            }
            else
            {
                StopFlashBtn(FlashBtnType.PickUpBtn);
            }
        }

        void OnWateringFruitBtnClick(object obj)
        {
            if (CanWatering)
            {
                MyParent.WateringAllFruit();
            }
        }

        void OnPickUpFruitBTnClick(object obj)
        {
            if (CanPickUp)
            {
                MyParent.PickUpAllFruit();
            }
        }

        void OnUseManaBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Tree_Buy");
            int buyNum = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAXMANNA_BUYNUM;
            bool CanBuyMana = buyNum < CommonDefineManager.Instance.CommonDefine.FruitMannan_CountMax;
            int leftTime = CommonDefineManager.Instance.CommonDefine.FruitMannan_CountMax - PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAXMANNA_BUYNUM;
            //TraceUtil.Log("是否可以继续购买仙露：" + PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAXMANNA_BUYNUM);
            if (CanBuyMana)//如果可以购买仙露
            {
                bool CanBuy = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY >= CommonDefineManager.Instance.CommonDefine.FruitMannan_Pay;//元宝是否足够
                string formatStr = LanguageTextManager.GetString("IDS_I28_16").Replace("\\n", "\n");
                string ShowMsg = string.Format(formatStr, CommonDefineManager.Instance.CommonDefine.FruitMannan_Count, leftTime);
                string sureBtnStr = LanguageTextManager.GetString("IDS_H2_55");
                string cancelBtnStr = LanguageTextManager.GetString("IDS_H2_28");

                int A = CommonDefineManager.Instance.CommonDefine.BuyFruitMannanConsumption1;
                int B = CommonDefineManager.Instance.CommonDefine.BuyFruitMannanConsumption2;
                int C = CommonDefineManager.Instance.CommonDefine.BuyFruitMannanConsumption3;
                int D = CommonDefineManager.Instance.CommonDefine.BuyFruitMannanConsumption4;
                int buyTimes = buyNum/CommonDefineManager.Instance.CommonDefine.FruitMannan_Count + 1;
                int buyPrice = Mathf.FloorToInt((A * buyTimes * buyTimes + B * buyTimes + C)/(float)D) * D;

                MyParent.ShowCostMoneyMessageBox(CanBuy, EMessageCoinType.EGoldType, buyPrice, ShowMsg, sureBtnStr, cancelBtnStr, SendBuyAmritaToSever, null);
            }
            else
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_470"), 1);//超出购买上限提示
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Tree_BuyFail");
            }

        }

        //void ShowGoldMoneyNotEnoughTips()//元宝不足提示
        //{
        //    MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_44"), 1);
        //}

        void SendBuyAmritaToSever()
        {
			bool CanBuy = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY >= CommonDefineManager.Instance.CommonDefine.FruitMannan_Pay;//鍏冨疂鏄惁瓒冲
            if (CanBuy)
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Tree_BuySuccess");
                MyParent.SendBuyAmritaToSever(5);
            }
            else
            {
                //MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_H2_44"), 1);//元宝不足提示
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Tree_BuyFail");
                MessageBox.Instance.ShowNotEnoughGoldMoneyMsg();
            }
        }

        public void StopFlashBtn(FlashBtnType flashBtnType)
        {
            //TraceUtil.Log("StopFlashBtn:"+flashBtnType);
            switch (flashBtnType)
            {
                case FlashBtnType.PickUpBtn:
                    TweenPosition.Begin(PickUpFruitBtn.gameObject, 0.3f, PickUpFruitBtn.transform.localPosition, PickUpBtnPosition, null);
                    break;
                case FlashBtnType.WateringBtn:
                    //TweenPosition.Begin(wateringFruitBtn.gameObject, 0.3f, wateringFruitBtn.transform.localPosition, WateringBtnPosition, null);
                    break;
                default:
                    break;
            }
        }

        public void FlashBtn(FlashBtnType flashBtnType)
        {
            //TraceUtil.Log("FlashBtn:"+flashBtnType);
            switch (flashBtnType)
            {
                case FlashBtnType.PickUpBtn:
                    Vector3 fromPosition = PickUpBtnPosition;
                    Vector3 ToPosition = fromPosition + new Vector3(0, 10, 0);
                    TweenPosition.Begin(PickUpFruitBtn.gameObject,0.3f,fromPosition,ToPosition,MoveDown);
                    break;
                case FlashBtnType.WateringBtn:

                    break;
                default:
                    break;
            }
        }

        void MoveDown(object obj)
        {
            //TraceUtil.Log("MoveDown");
            GameObject gameobj = obj as GameObject;
            Vector3 fromPosition = gameobj.transform.localPosition;
            Vector3 toPosition = fromPosition + new Vector3(0,-10,0);
            TweenPosition.Begin(gameobj, 0.3f, fromPosition, toPosition,MoveUp);
        }

        void MoveUp(object obj)
        {
            //TraceUtil.Log("MoveUp");
            GameObject gameobj = obj as GameObject;
            Vector3 fromPosition = gameobj.transform.localPosition;
            Vector3 toPosition = fromPosition + new Vector3(0, +10, 0);
            //TweenPosition.Begin(gameobj, 0.3f, fromPosition, toPosition, MoveDown1);
            TweenPosition.Begin(gameobj, 0.3f, fromPosition, toPosition, MoveDown);
        }

        void MoveDown1(object obj)
        {
            //TraceUtil.Log("MoveDown1");
            GameObject gameobj = obj as GameObject;
            Vector3 fromPosition = gameobj.transform.localPosition;
            Vector3 toPosition = fromPosition + new Vector3(0, -10, 0);
            TweenPosition.Begin(gameobj, 0.3f, fromPosition, toPosition,null);
        }

        protected override void RegisterEventHandler()
        {
            throw new System.NotImplementedException();
        }
    }
}
