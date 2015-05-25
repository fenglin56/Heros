using UnityEngine;
using System.Collections;


namespace UI.Battle
{
    public class HeroResurrectionUI : MonoBehaviour
    {

        public SingleButtonCallBack FullHealthResurgenceBtn;
        public SingleButtonCallBack QuarterHealthResurgenceBtn;
        public SingleButtonCallBack CancelBtn;
        public int FullHealthPay = 10, QuarteHealthPay = 300;

        void Start()
        {
            FullHealthResurgenceBtn.SetCallBackFuntion(OnFullHealthBtnClick);
            QuarterHealthResurgenceBtn.SetCallBackFuntion(OnQuarterHealthClick);
            CancelBtn.SetCallBackFuntion(OnCancelBtnClick);
        }

        void OnFullHealthBtnClick(object obj)
        {
            int CurrentPay = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;
            TraceUtil.Log("玩家当前元宝："+CurrentPay);
            if (CurrentPay < FullHealthPay)
            {
                GrayButton(FullHealthResurgenceBtn);
                FullHealthResurgenceBtn.SetButtonText(LanguageTextManager.GetString("IDS_H2_44"));//("元宝不足");
            }
            else
            {
                SendFullHealthToSever();
            }
        }

        void SendFullHealthToSever()
        {
            int actorID = PlayerManager.Instance.FindHeroDataModel().ActorID;            
            //NetServiceManager.Instance.EctypeService.SendEctypeRequestRevive(UID, UID, EctypeRevive.ER_FULLSTATE);
            NetServiceManager.Instance.EntityService.SendActionRelivePlayer(actorID, actorID, (byte)EctypeRevive.ER_PREFECT);
            TraceUtil.Log("发送满状态复活请求");
            this.CloseMyself();
        }

        void OnQuarterHealthClick(object obj)
        {
            int CurrentMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
            TraceUtil.Log("玩家当前铜币：" + CurrentMoney);
            if (CurrentMoney < QuarteHealthPay)
            {
                GrayButton(QuarterHealthResurgenceBtn);
                QuarterHealthResurgenceBtn.SetButtonText(LanguageTextManager.GetString("IDS_H2_35"));//("铜币不足");
            }
            else
            {
                SendQuarterHealthToSever();
            }
        }

        void SendQuarterHealthToSever()
        {
            int actorID = PlayerManager.Instance.FindHeroDataModel().ActorID;      
            //NetServiceManager.Instance.EctypeService.SendEctypeRequestRevive(UID,UID,EctypeRevive.ER_QUARTERSTATE);
            NetServiceManager.Instance.EntityService.SendActionRelivePlayer(actorID, actorID, (byte)EctypeRevive.ER_NORMAL);
            TraceUtil.Log("发送1/4状态复活请求");
            this.CloseMyself();
        }

        void OnCancelBtnClick(object obj)
        {
            CloseMyself();
        }

        void GrayButton(SingleButtonCallBack button)
        {
            button.SetImageButtonComponentActive(false);
            button.SetTextColor(Color.gray);
            button.SetButtonBackground(2);
        }

        void RecoverButton(SingleButtonCallBack button)
        {
            button.SetImageButtonComponentActive(true);
            button.SetTextColor(Color.white);
            button.SetButtonBackground(1);
        }

        public void ShowMyself()
        {
            transform.localPosition = new Vector3(0,0,-100);
        }

        public void CloseMyself()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
        }

    }
}