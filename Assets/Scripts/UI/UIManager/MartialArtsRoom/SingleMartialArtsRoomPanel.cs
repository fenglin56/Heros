using UnityEngine;
using System.Collections;

namespace UI.MainUI
{

    public class SingleMartialArtsRoomPanel : MonoBehaviour
    {


        public UILabel RoomNameLabel;
        public UILabel RoleNumberLabel;
        public UILabel SpeedLabel;
        public UILabel UpLimitLabel;
        public SingleButtonCallBack JoinBtn;

        public MartialArtsRoomListPanel MyParent { get; private set; }
        public EctypePraictice ectypePraictice { get; private set; }
        public EctypeContainerData EctypeContainerData { get; private set; }
        private bool IsShow = false;


        void Start()
        {
            JoinBtn.SetCallBackFuntion(OnJoinBtnClick);
        }


        public void Show(MartialArtsRoomListPanel myParent, EctypePraictice ectypePraictice)
        {
            IsShow = true;
            this.MyParent = myParent;
            this.ectypePraictice = ectypePraictice;
            this.gameObject.SetActive(true);
            TraceUtil.Log("解锁房间ID："+ectypePraictice.dwRoomID);
            this.EctypeContainerData = EctypeConfigManager.Instance.EctypeContainerConfigList[(int)ectypePraictice.dwEctypeID];
            RoomNameLabel.SetText(string.Format(LanguageTextManager.GetString("IDS_H1_474"),ectypePraictice.Name));
            RoleNumberLabel.SetText(string.Format("{0}/{1}",ectypePraictice.dwPlayerNum,EctypeContainerData.lMaxActorCount));
            SpeedLabel.SetText(string.Format("{0}{1}", ectypePraictice.dwPraicticeSpeed.ToString(),LanguageTextManager.GetString("IDS_H1_475")));
            UpLimitLabel.SetText(ectypePraictice.dwPraicticeMax.ToString());
        }

        //void OnClick()
        //{
        //    if (IsShow)
        //    {
        //        MyParent.MyParent.SendJoinRoomMsgToSever(this.ectypePraictice.dwRoomID);
        //    }
        //}

        public void Close()
        {
            IsShow = false;
            this.gameObject.SetActive(false);
        }

        void OnJoinBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_EnterRoom");
            MyParent.MyParent.SendJoinRoomMsgToSever(this.ectypePraictice.dwRoomID);
        }

    }
}