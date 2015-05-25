using UnityEngine;
using System.Collections;
using UI.Friend;
namespace UI.PlayerRoom
{
    public class PlayerRoomFriendPanel : MonoBehaviour
    {
        public SocialUIManager SocialUIManagerPanel;
        public SingleButtonCallBack Button_Near;
        public SingleButtonCallBack Button_Close;

        void Awake()
        {
            Button_Near.SetCallBackFuntion(SocialUIManagerPanel.ShowNearlyPlayerPanel, null);
            Button_Close.SetCallBackFuntion(ClosePanelHandle, null);
        }

        public void ShowPanel()
        {
            transform.localPosition = Vector3.zero;
            //SocialUIManagerPanel.SetSocialPanelActive(new int[] { 6 });
            SocialUIManagerPanel.Show();
        }

        public void HidePanel()
        {            
            transform.localPosition = new Vector3(0, 0, -1000);
        }
        
        void ClosePanelHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            HidePanel();
        }
    }
}