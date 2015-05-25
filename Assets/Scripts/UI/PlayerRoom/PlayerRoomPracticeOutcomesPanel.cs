using UnityEngine;
using System.Collections;
using System;
namespace UI.PlayerRoom
{
    public class PlayerRoomPracticeOutcomesPanel : MonoBehaviour
    {
        public enum LineType
        {
            Online,
            Offline,
        }

        public SingleButtonCallBack Button_Get;

        public GameObject Interface_Online;
        public UILabel Label_OnlineTime;
        public UILabel Label_OnlineValue;

        public GameObject Interface_Offline;
        public UILabel Label_OfflineTime;
        public UILabel Label_OfflineValue;
   
        private int m_lastXiuLianNum;

        public Action<int> PlayCollectAnimation;

        void Awake()
        {
            Button_Get.SetCallBackFuntion(GetPracticeOutcomesHandle, null);
        }

        public void SetAction(Action<int> playAction)
        {
            PlayCollectAnimation = playAction;
        }

        public void ShowPanel(LineType type, SMsgActionXiuLianInfo_SC xiuLianInfo)
        {
            transform.localPosition = Vector3.zero;
            if (type == LineType.Online)
            {
                Interface_Online.SetActive(true);
                Interface_Offline.SetActive(false);
                int time = xiuLianInfo.XiuLianTime;
                int hour = time / 3600;
                int min = (time - hour * 3600) / 60;
                int second = time % 60;
                Label_OnlineTime.text = string.Format(LanguageTextManager.GetString("IDS_H1_513"), ParseClock(hour), ParseClock(min), ParseClock(second));
                Label_OnlineValue.text = xiuLianInfo.XiuLianNum.ToString();
            }
            else
            {
                Interface_Online.SetActive(false);
                Interface_Offline.SetActive(true);
                int time = xiuLianInfo.XiuLianTime;
                if (time > 86400)
                {
                    int day = time / 86400;
                    Label_OfflineTime.text = day + LanguageTextManager.GetString("IDS_H1_510");
                }
                else
                {
                    int hour = time / 3600;
                    int min = (time - hour * 3600) / 60;
                    Label_OfflineTime.text = hour.ToString() + LanguageTextManager.GetString("IDS_H1_512") + ParseClock(min) + LanguageTextManager.GetString("IDS_H1_511");
                }                
                Label_OfflineValue.text = xiuLianInfo.XiuLianNum.ToString();
            }
            m_lastXiuLianNum = xiuLianInfo.XiuLianNum;
        }

        public void HidePanel()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
        }

        void GetPracticeOutcomesHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_RoomAward");            
            HidePanel();
            PlayCollectAnimation(m_lastXiuLianNum);
        }

        private string ParseClock(int time)
        {
            if (time < 10)
            {
                return "0" + time.ToString();
            }
            return time.ToString();
        }


    }
}