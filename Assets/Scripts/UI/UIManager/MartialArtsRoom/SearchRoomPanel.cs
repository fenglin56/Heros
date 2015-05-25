using UnityEngine;
using System.Collections;


namespace UI.MainUI
{

    public class SearchRoomPanel : MonoBehaviour
    {


        public UIInput InputLabel;
        public SingleButtonCallBack SureBtn;
        public SingleButtonCallBack CancelBtn;
        public MartialArtsRoomListPanel MyParent { get; private set; }

        bool IsShow = false;

        private int[] m_guideBtnID = new int[3];

        void Awake()
        {
            SureBtn.SetCallBackFuntion(OnSureBtnClick);
            CancelBtn.SetCallBackFuntion(OnCancelBtnClick);
            Close(null);
        }

        void Start()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(SureBtn.gameObject, UIType.MartialArtsRoom, SubType.MartialSearchRoom, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(CancelBtn.gameObject, UIType.MartialArtsRoom, SubType.MartialSearchRoom, out m_guideBtnID[1]);
            //TODO GuideBtnManager.Instance.RegGuideButton(InputLabel.gameObject, UIType.MartialArtsRoom, SubType.MartialSearchRoom, out m_guideBtnID[1]);
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        void Update()
        {
            if (InputLabel.text.IndexOf("-") != -1)
            {
                InputLabel.text = InputLabel.text.Substring(1);
            }
        }

        void OnSureBtnClick(object obj)
        {
            string inputLabelStr = InputLabel.text;
            if (inputLabelStr.Length > 0)
            {
                uint searchRoomID = uint.Parse(inputLabelStr);
                MyParent.MyParent.SendJoinRoomMsgToSever(searchRoomID);
                Close(null);
            }
            else
            {
                MessageBox.Instance.ShowTips(3, LanguageTextManager.GetString("IDS_H1_476"), 1);
            }
        }

        void OnCancelBtnClick(object obj)
        {
            Close(null);
        }

        public void Show(MartialArtsRoomListPanel myParent)
        {
            this.MyParent = myParent;
            transform.localPosition = new Vector3(0, 0, -100);
            IsShow = true;
        }

        public void Close(object obj)
        {
            transform.localPosition = new Vector3(0, 0, -1000);
            IsShow = false;
        }

    }
}