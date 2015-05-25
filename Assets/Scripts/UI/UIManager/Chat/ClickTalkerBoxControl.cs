using UnityEngine;
using System.Collections;
using System.Linq;
namespace Chat
{
    public class ClickTalkerBoxControl : MonoBehaviour
    {
        public LocalButtonCallBack Button_Send;
        public LocalButtonCallBack Button_MakeFriend;
        public LocalButtonCallBack Button_Close;
        //private int mTalkerActorID = 0;

		private UIPanel Panel_Send;
		private UIPanel Panel_MakeFriend;
		private Vector3 Pos_Send;
		private Vector3 Pos_MakeFriend;

        private TalkerInfo mTalkerInfo;

        void Awake()
        {
            Button_Send.SetCallBackFuntion(SendChat, null);
            Button_MakeFriend.SetCallBackFuntion(SendAddFriend, null);
            Button_Close.SetCallBackFuntion(CloseBox, null);

			Panel_Send =  Button_Send.GetComponent<UIPanel>();
			Panel_MakeFriend = Button_MakeFriend.GetComponent<UIPanel>();
			Pos_Send = Button_Send.transform.localPosition;
			Pos_MakeFriend = Button_MakeFriend.transform.localPosition;
        }

        public void SetTargetTalkerInfo(TalkerInfo talkerInfo)
        {
            this.mTalkerInfo = talkerInfo;

            bool isHasFriend = UI.Friend.FriendDataManager.Instance.GetFriendListData.Any(p => p.sMsgRecvAnswerFriends_SC.dwFriendID == talkerInfo.ActorID);
			Button_MakeFriend.gameObject.SetActive(!isHasFriend);				

			PlayAppearAnimation();
        }

		public void SetMakeFriendButtonActive()
		{
			if(gameObject.activeInHierarchy)
			{
				bool isHasFriend = UI.Friend.FriendDataManager.Instance.GetFriendListData.Any(p => p.sMsgRecvAnswerFriends_SC.dwFriendID == mTalkerInfo.ActorID);
				Button_MakeFriend.gameObject.SetActive(!isHasFriend);
			}
		}

        void SendChat(object obj)
        {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatPrivateChat");
            UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenPrivateChatWindow, this.mTalkerInfo);
            gameObject.SetActive(false);
        }
        void SendAddFriend(object obj)
        {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_ChatAddFriends");
            var playerData = PlayerManager.Instance.FindHeroDataModel();
            NetServiceManager.Instance.FriendService.SendAddFriendRequst(new SMsgAddFriends_CS()
            {
                dwActorID = (uint)playerData.ActorID,
                dwFriendID = (uint)mTalkerInfo.ActorID
            });
            gameObject.SetActive(false);
        }
        void CloseBox(object obj)
        {
            gameObject.SetActive(false);
        }

		//出现动画
		private void PlayAppearAnimation()
		{
			Panel_Send.alpha = 0;
			Panel_MakeFriend.alpha = 0;
			TweenPosition.Begin(Panel_Send.gameObject,0.2f,Pos_Send+Vector3.left * 20, Pos_Send,null);
			TweenAlpha.Begin(Panel_Send.gameObject,0.2f,1);

			if(Button_MakeFriend.gameObject.activeInHierarchy)
			{
				StartCoroutine(PlaySecondBtnAppearAnimation(0.1f));
			}
		}
		//先分出来
		IEnumerator PlaySecondBtnAppearAnimation(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			TweenPosition.Begin(Panel_MakeFriend.gameObject,0.2f,Pos_MakeFriend+Vector3.left * 20, Pos_MakeFriend,null);
			TweenAlpha.Begin(Panel_MakeFriend.gameObject,0.2f,1);
		}

    }
}