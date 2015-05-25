using UnityEngine;
using System.Collections;

namespace UI.Battle
{

    public class BattleSettlementRewardPanel_V3 : MonoBehaviour
    {
        public UIPanel TweenShowElement;
        public UISprite Background;
        public SingleRewardPanel_V3[] RewardPanelList;
        public SingleButtonCallBack BackButton;
        public SingleButtonCallBack TimeLeftLabel;

        public SMSGEctypeSettleAccounts ShowData { get; private set; }
        public BattleSettlementPanel_V3 MyParent { get; private set; }

        void Awake()
        {
            TweenShowElement.alpha =0;
            BackButton.SetCallBackFuntion(OnBackButtonClick);
            Background.alpha = 0;
            RewardPanelList.ApplyAllItem(P => P.gameObject.GetComponent<UIPanel>().alpha = 0);
        }

        public void Show(SMSGEctypeSettleAccounts sMSGEctypeSettleAccounts, BattleSettlementPanel_V3 myParent)
        {
            this.MyParent = myParent;
            this.ShowData = sMSGEctypeSettleAccounts;
            TweenShowBackground(null);
            TweenMoveSingleRewariPanel(null);
        }

        void TweenShowBackground(object obj)
        {
            TweenAlpha.Begin(Background.gameObject, 0.3f, 0, 1, null);
        }

        void TweenMoveSingleRewariPanel(object obj)
        {
            float showTime = 0.3f;
            Vector3 fromPosition01 = RewardPanelList[0].transform.localPosition + new Vector3(0,120,0);// new Vector3(-182, 104, 0);
            Vector3 toPosition01 = RewardPanelList[0].transform.localPosition;//new Vector3(-182, -20, 0);
            Vector3 fromPosition02 = RewardPanelList[1].transform.localPosition + new Vector3(0, -120, 0);// new Vector3(-22, -200, 0);
            Vector3 toPosition02 = RewardPanelList[1].transform.localPosition;//new Vector3(-22, -20, 0);
            Vector3 fromPosition03 = RewardPanelList[2].transform.localPosition + new Vector3(0, 120, 0);// ;new Vector3(137, 104, 0);
            Vector3 toPosition03 = RewardPanelList[2].transform.localPosition;// new Vector3(137, -20, 0);
            TweenPosition.Begin(RewardPanelList[0].gameObject, showTime, fromPosition01, toPosition01, null);
            TweenPosition.Begin(RewardPanelList[1].gameObject, showTime, fromPosition02, toPosition02, null);
            TweenPosition.Begin(RewardPanelList[2].gameObject, showTime, fromPosition03, toPosition03, ShowRewardPanel);
            TweenAlpha.Begin(RewardPanelList[0].gameObject, showTime, 0, 1, null);
            TweenAlpha.Begin(RewardPanelList[1].gameObject, showTime, 0, 1, null);
            TweenAlpha.Begin(RewardPanelList[2].gameObject, showTime, 0, 1, null);
            StartCoroutine(ShowTimeLeftLabel(MyParent.EctypeData.BattleVictoryLotteryTime));
            TweenFloat.Begin(0.5f,0,1,SetElementAlpha);
        }

        void SetElementAlpha(float value)
        {
            TweenShowElement.alpha = value;
        }

        void ShowRewardPanel(object obj)
        {
            int panelIndex = 0;
//            for (int i = 0; i < ShowData.RoleAccountList.Count; i++)
//            {
//                if (this.RewardPanelList.Length > i)
//                {
//                    if (PlayerManager.Instance.GetEntityMode(ShowData.RoleAccountList[i].uidPlayer) == null)
//                    {
//                        TraceUtil.Log("找不到实体UID：" + ShowData.RoleAccountList[i].uidPlayer+"MyUID:"+PlayerManager.Instance.FindHeroDataModel().UID);
//                    }
//                    else
//                    {
//                        RewardPanelList[panelIndex].Show(new SingleRewardPanel_V3.RewardData(ShowData.RoleAccountList[i]), this);
//                        panelIndex++;
//                    }
//                }
//            }
            //for (int i = 0; i < this.RewardPanelList.Length; i++)
            //{
            //    if (ShowData.RoleAccountList.Count >= (panelIndex + 1) && PlayerManager.Instance.GetEntityMode(ShowData.RoleAccountList[panelIndex].uidPlayer) != null)
            //    {
            //        RewardPanelList[i].Show(new SingleRewardPanel_V3.RewardData(ShowData.RoleAccountList[panelIndex]), this);
            //        panelIndex++;
            //        //RewardPanelList[i].Show(new SingleRewardPanel_V3.RewardData(), this);
            //    }
            //}
        }

        public void OnBackButtonClick(object obj)
        {
            TraceUtil.Log("返回城镇");
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            long UIDEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
            NetServiceManager.Instance.EctypeService.SendEctypeRequestReturnCity(UIDEntity);
        }

        IEnumerator ShowTimeLeftLabel(int leftTime)
        {
            TimeLeftLabel.SetButtonText(leftTime.ToString());
            yield return new WaitForSeconds(1);
            if (leftTime > 0)
            {
                leftTime--;
                StartCoroutine(ShowTimeLeftLabel(leftTime));
            }
        }

        public void Close()
        {
        }
    }
}