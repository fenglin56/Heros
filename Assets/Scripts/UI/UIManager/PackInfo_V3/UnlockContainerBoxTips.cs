using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class UnlockContainerBoxTips : MonoBehaviour
    {

        ButtonCallBack SureBtnCallback;
        ButtonCallBack CancelBtnCallBack;

        public UILabel MsgLabel;

        public UILabel TakeMoneyLabel;

        public SingleButtonCallBack SureBtn;
        public SingleButtonCallBack CancelBtn;

        void Awake()
        {
            this.SureBtn.SetCallBackFuntion(OnSureBtnClick);
            this.CancelBtn.SetCallBackFuntion(OnCancelBtnClick);
            Close();

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            SureBtn.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_PackUnLock_Sure);
            CancelBtn.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_PackUnLock_Cancel);
        }
		
        public int GetActiveEnergyHaveGold(int UnlockBox)
        {
            // (向下取整 ((参数1×（购买次数）^2+参数2×购买次数+参数3)/参数4)×参数4)
            //CommonDefineManager.Instance.CommonDefine.BuyEnergyConsumption1
            int a = CommonDefineManager.Instance.CommonDefine.PackageUnlockConsumption1;
            int b = CommonDefineManager.Instance.CommonDefine.PackageUnlockConsumption2;
            int c = CommonDefineManager.Instance.CommonDefine.PackageUnlockConsumption3;
            int d = CommonDefineManager.Instance.CommonDefine.PackageUnlockConsumption4;
            //int times = PlayerDataManager.Instance.GetenergyPurchaseTimes() - PlayerManager.Instance.FindHeroDataModel ().PlayerValues.PLAYER_FIELD_CANBUYACTIVE_NUM;
            int val = Mathf.FloorToInt ( ((a*UnlockBox*UnlockBox + b*UnlockBox + c)/(float)d) )*d;
            return val;
        }

        public void Show(string Message, ButtonCallBack SureBtnCallBack, ButtonCallBack CancelBtnCallBack, string sureBtnStr, string cancelBtnStr)
        {
            this.MsgLabel.SetText(Message);
            this.SureBtn.SetButtonText(sureBtnStr);
            this.CancelBtn.SetButtonText(cancelBtnStr);
            this.CancelBtnCallBack = CancelBtnCallBack;
            transform.localPosition = new Vector3(0,0,-100);
            int UnlockBox = (ContainerInfomanager.Instance.GetContainerClientContsext(2).wMaxSize/4)-4;
            int tackGold = GetActiveEnergyHaveGold(UnlockBox);
            bool canBuy = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY >= tackGold;
            TakeMoneyLabel.SetText(NGUIColor.SetTxtColor(tackGold.ToString(),canBuy?TextColor.white:TextColor.red));
            this.SureBtnCallback =PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY>=tackGold?SureBtnCallBack:ShowNoMoneyTips;
        }

        void OnSureBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_BuyIngotConfirmation");
            if (this.SureBtnCallback != null)
            {
                this.SureBtnCallback(null);
            }
            Close();
        }

        void ShowNoMoneyTips(object obj)
        {
           // MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H2_44"),1);
			MessageBox.Instance.ShowNotEnoughGoldMoneyMsg();
        }

        void OnCancelBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_BuyIngotConfirmation");
            if (this.CancelBtnCallBack != null)
            {
                this.CancelBtnCallBack(null);
            }
            Close();
        }

        public void Close()
        {
            transform.localPosition = new Vector3(0,0,-1000);
        }
    }
}