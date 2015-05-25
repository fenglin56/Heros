using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class MeridiansDragPanel : MonoBehaviour
    {
        public int PanelPositionID;//面板的位置ID，从0开始，用来确定位置
        public int MyKonfuLevel = 0;//改面板所对应的功夫ID
        public SingleMeridiansBtn[] SingleMeridiansBtnList;

        public PlayerKongfuData playerKongfuData { get; private set; }

        public MeridiansPanel MyParent { get; private set; }

        public bool IsUnlock { get; private set; }//本功夫面板是否解锁


        public void InitPanel(MeridiansPanel myParent)
        {
            playerKongfuData = myParent.PlayerMeridiansDataManager.GetKonfuData(MyKonfuLevel);
            MyParent = myParent;
            int MyMinMeridiansID = 100;
            for (int i = 0; i < SingleMeridiansBtnList.Length; i++)
            {
                int meridiansID =int.Parse(playerKongfuData.MeridiansList[i]);
                SingleMeridiansBtnList[i].Init(meridiansID,this);
                MyMinMeridiansID = MyMinMeridiansID < meridiansID ? MyMinMeridiansID : meridiansID;
            }
            transform.localPosition = new Vector3(850 *PanelPositionID, 0, 0);
            IsUnlock = MyMinMeridiansID <= PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MERIDIANS_LEVEL+1;
            //TraceUtil.Log("SetDragPanelIsUnlock:"+IsUnlock);
        }

        /// <summary>
        /// 检测该经脉ID是否在我这个面板中
        /// </summary>
        /// <param name="MeridiansID"></param>
        /// <returns></returns>
        public bool CheckIsInMyPanel(int MeridiansID)
        {
            return getMeridiansBtn(MeridiansID) != null;
        }

        /// <summary>
        /// 当某个经脉按钮点击的时候
        /// </summary>
        /// <param name="meridiansID"></param>
        public void OnMeridiansBtnClick(int meridiansID)
        {
            SingleMeridiansBtnList.ApplyAllItem(P=>P.OnSelectBtn(meridiansID));
        }

        public SingleMeridiansBtn getMeridiansBtn(int meridiansID)
        {
            return SingleMeridiansBtnList.FirstOrDefault(P=>P.m_MeridiansID == meridiansID);
        }

        /// <summary>
        ///  当修炼按钮点击的时候
        /// </summary>
        /// <param name="Flag"></param>
        public void OnAddMeridiansBtnClick(bool Flag)
        {
            if (CheckIsInMyPanel(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MERIDIANS_LEVEL+1))
            {
                SingleMeridiansBtnList.ApplyAllItem(P => P.AddLevelUpNeed(Flag));
            }
        }

        public void DisabelMyBtnsSelectActive()
        {
            SingleMeridiansBtnList.ApplyAllItem(P=>P.OnSelectBtn(-1));
        }

    }
}