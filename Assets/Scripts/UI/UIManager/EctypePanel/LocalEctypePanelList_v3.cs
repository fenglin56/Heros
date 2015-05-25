using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace UI.MainUI
{
    public class LocalEctypePanelList_v3 : MonoBehaviour
    {

        List<LocalEctypePanel_v3> LocalPanelList = new List<LocalEctypePanel_v3>();

        public EctypeSelectConfigData ectypeSelectConfigData;

        public GameObject LocalEctypeCarPrefab;

        EctypePanel_V4 myParent;

        public int PositionIndex;

        private bool m_IsFirstEctypeUnlock = false;

        public void InitPanel(int index, EctypeSelectConfigData ectypeSelectConfigData, EctypePanel_V4 myParent)
        {
            this.PositionIndex = index;
            this.myParent = myParent;
            if (transform.childCount > 0) { transform.ClearChild(); }
            this.ectypeSelectConfigData = ectypeSelectConfigData;
            this.ectypeSelectConfigData.InitectContainer();
            for (int i = 0; i < this.ectypeSelectConfigData._vectContainer.Length; i++)
            {
                //add by lee 如果是封妖副本 break
                if (ectypeSelectConfigData._vectContainer[i] == ectypeSelectConfigData._sirenEctypeContainerID)
                    break;
                GameObject EctypeCard = null;
                EctypeCard = CreatObjectToNGUI.InstantiateObj(LocalEctypeCarPrefab, transform);
                LocalEctypePanel_v3 ectypeCard = EctypeCard.GetComponent<LocalEctypePanel_v3>();
                ectypeCard.ContainerID = ectypeSelectConfigData._lEctypeID;
                ectypeCard.InitPanel(this.ectypeSelectConfigData._vectContainer[i], myParent);
                this.LocalPanelList.Add(ectypeCard);
            }
            SetMyPosition(index);
        }

//        public void UnlockMyPanel(SMSGEctypeData_SC sMSGEctypeData_SC)
//        {
//            if(sMSGEctypeData_SC.dwEctypeID == this.ectypeSelectConfigData._lEctypeID)
//            {
//                int EctypeID = 0;
//                if(this.ectypeSelectConfigData.VectContainerList.TryGetValue(sMSGEctypeData_SC.byDiff,out EctypeID))
//                {
//                    var EctypeCard = LocalPanelList.SingleOrDefault(P=>P.ectypeContainerData.lEctypeContainerID == EctypeID);
//                    if (EctypeCard != null)
//                    {
//                        EctypeCard.UnlockMyself(sMSGEctypeData_SC);
//                        m_IsFirstEctypeUnlock = true;
//                    }
//                }
//            }
//        }
//
//        public EctypeContainerData GetSingleEctypeData(SMSGEctypeData_SC sMSGEctypeData_SC)
//        {
//            EctypeContainerData ectypeContainerData;
//            ectypeContainerData = this.LocalPanelList.SingleOrDefault(P => P.sMSGEctypeData_SC.dwEctypeID == sMSGEctypeData_SC.dwEctypeID&& P.sMSGEctypeData_SC.byDiff == sMSGEctypeData_SC.byDiff).ectypeContainerData;
//            return ectypeContainerData;
//        }
//
//        public void SelectPanel(SMSGEctypeData_SC sMSGEctypeData_SC)
//        {
//            var SelectPanel = LocalPanelList.FirstOrDefault(P => P.sMSGEctypeData_SC.dwEctypeID == sMSGEctypeData_SC.dwEctypeID && P.sMSGEctypeData_SC.byDiff == sMSGEctypeData_SC.byDiff);
//            if (SelectPanel != null)
//            {
//                SelectPanel.SelectMyself();
//            }
//            //for (int i = 0; i < LocalPanelList.Count; i++)
//            //{
//            //    if (LocalPanelList[i].SelectMyself())
//            //        return;
//            //}
//        }        
//
//        public void OnSelectEctypeCard(SMSGEctypeData_SC sMSGEctypeData_SC)
//        {
//            foreach (var child in LocalPanelList)
//            {
//                child.UnSelectMyself(sMSGEctypeData_SC);
//            }
//        }

        /// <summary>
        /// 出现妖女副本
        /// </summary>
        /// <param name="regionID">区域id</param>
        /// <param name="ectypeID">副本id</param>
        /// <param name="time">时间(毫秒)</param>
        public void AppearSirenEctype(int regionID, int ectypeID, int time)
        {
            if (this.LocalPanelList.Any(p => p.ectypeContainerData.lEctypeContainerID == ectypeID))
            {
                return;
            }			           

            GameObject EctypeCard = null;

            EctypeCard = CreatObjectToNGUI.InstantiateObj(LocalEctypeCarPrefab, transform);
            LocalEctypePanel_v3 ectypeCard = EctypeCard.GetComponent<LocalEctypePanel_v3>();
            ectypeCard.ContainerID = this.ectypeSelectConfigData._lEctypeID;
            ectypeCard.CreateSirenPanel(regionID, ectypeID, myParent, time);
            this.LocalPanelList.Add(ectypeCard);
        }

        /// <summary>
        /// 删除妖女副本
        /// </summary>
        /// <param name="regionID"></param>
        public void DeleteSirenEctype()
        {
            for (int i = 0; i < LocalPanelList.Count; i++)
            {
                if (LocalPanelList[i].IsSirenEctype == true)
                {
                    Destroy(LocalPanelList[i].gameObject);
                    LocalPanelList.RemoveAt(i);
                }
            }            
        }

        /// <summary>
        /// 当前容器有未有副本解锁
        /// </summary>
        /// <returns></returns>
        public bool IsFirstEctypeUnlock()
        {
            return m_IsFirstEctypeUnlock;
        }

        public void SetMyPosition(int Index)
        {
            transform.localPosition = new Vector3(myParent.PageDistance* Index, 0, 0);
        }
    }
}