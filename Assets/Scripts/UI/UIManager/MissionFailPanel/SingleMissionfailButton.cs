using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class SingleMissionfailButton : MonoBehaviour
    {

        public UIPanel MyPanel;
        public BoxCollider MyCollider;
        public Transform CreatIconPoint;
        public UILabel NameLabel;
        public UILabel DesLabel;
        public MissionFailData MyMissionFailData { get; private set; }
        public MissionFailPanel MyParent { get; private set; }

        public void Init()
        {
            gameObject.SetActive(false);
        }

        public void SetBtnActive(MissionFailData myData,MissionFailPanel myParent,bool isActive)
        {
            gameObject.SetActive(true);
            MyParent = myParent;
            MyMissionFailData = myData;
            SetMyselfActive(isActive);
            CreatIconPoint.ClearChild();
            CreatObjectToNGUI.InstantiateObj(myData.IconPrefab,CreatIconPoint);
            NameLabel.SetText(LanguageTextManager.GetString(myData.NameIDS));
            DesLabel.SetText(LanguageTextManager.GetString(myData.ButtonExplainIDS));
        }

        void SetMyselfActive(bool flag)
        {
            MyPanel.alpha = flag ? 1 : 0.5f;
            MyCollider.enabled = flag;
        }

        void OnClick()
        {
            MyParent.OnSingleBtnClick(MyMissionFailData);
        }

    }
}