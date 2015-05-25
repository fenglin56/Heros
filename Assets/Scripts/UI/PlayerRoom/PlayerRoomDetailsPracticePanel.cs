using UnityEngine;
using System.Collections;
namespace UI.PlayerRoom
{
    public class PlayerRoomDetailsPracticePanel : MonoBehaviour
    {

        public UILabel Label_AdditionByHomeowners;
        public UILabel Label_AdditionBySiren;
        public UILabel Label_AdditionByMembers;//单个房客修为增加值×当前房客人数
        public UILabel Label_PracticeSpeed;  //修炼速度=向下取整(60×房间修炼基础修为×(1+妖女修炼加成))
        public UILabel Label_Get;
        public SingleButtonCallBack Button_Sure;

        private int m_guideBtnID = 0;

        void Awake()
        {
            Button_Sure.SetCallBackFuntion(MakeSureHandle, null);
        }

        void Start()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Sure.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.MsgBox, out m_guideBtnID);
        }

        void OnDestroy()
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        public void ShowPanel()
        {
            transform.localPosition = Vector3.zero;
        }

        public void HidePanel()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="accoutConfigData">PlayerRoomAccoutConfigData</param>
        /// <param name="sirenAddition">妖女加成</param>
        /// <param name="memberNum">房客人数</param>
        /// <param name="practiceSpeed">修为速度(修为/小时)</param>
        /// <param name="practiceResult">现在修为所得</param>
        /// <param name="isHomer">是否房主</param>
        public void Show(PlayerRoomAccoutConfigData accoutConfigData,int sirenAddition, int memberNum,int practiceSpeed ,int practiceResult, bool isHomer)
        {
            if (isHomer)
            {
                Label_AdditionByHomeowners.text = accoutConfigData._ownerAddition.ToString() + "%";
            }
            else
            {
                Label_AdditionByHomeowners.text = LanguageTextManager.GetString("IDS_H1_508");
            }
            Label_AdditionBySiren.text = sirenAddition.ToString() + "%";
            Label_AdditionByMembers.text = (memberNum * accoutConfigData._guestAddition).ToString()+ "%";
            Label_PracticeSpeed.text = practiceSpeed.ToString() + LanguageTextManager.GetString("IDS_H1_475");
            Label_Get.text = practiceResult.ToString() + "/" + accoutConfigData._upperLimit.ToString();
            this.ShowPanel();
        }
       
        void MakeSureHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            this.HidePanel();
        }
    }
}