using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class TrialsEctypePanelList : MonoBehaviour
    {

        public GameObject SingleTrialsEctypePanelPrefab;

        public TrialsEctypePanelManager MyParent { get; private set; }

        List<SingleTrialsEctypepanel> SingleTrialsEctypepanelList = new List<SingleTrialsEctypepanel>();


        int PanelID = 0;

        public void InitPanel(int PanelID,TrialsEctypePanelManager myParent)
        {
            this.MyParent = myParent;
            this.PanelID = PanelID;
            transform.ClearChild();
            transform.localPosition = new Vector3((PanelID-1)*800,0,0);
            SingleTrialsEctypepanelList.Clear();
            List<EctypeContainerData> myEctypeDataList = EctypeConfigManager.Instance.EctypeContainerConfigFile.ectypeContainerDataList.Where(P => P.lEctypeType == 5 && P.lEctypePos[0] == PanelID.ToString()).ToList();
            if (myEctypeDataList != null && myEctypeDataList.Count > 0)
            {
                for (int i = 0; i < myEctypeDataList.Count; i++)
                {
                    SingleTrialsEctypepanel creatPanel = CreatObjectToNGUI.InstantiateObj(SingleTrialsEctypePanelPrefab, transform).GetComponent<SingleTrialsEctypepanel>();
                    creatPanel.InitPanel(int.Parse(myEctypeDataList[i].lEctypePos[2]), myEctypeDataList[i],this);
                    SingleTrialsEctypepanelList.Add(creatPanel);
                }
            }
        }

        public void UnLockPanel(Dictionary<int, SEctypeTrialsInfo> EctypeDataList)
        {
            SingleTrialsEctypepanelList.ApplyAllItem(P => P.UnlockPanel(EctypeDataList));
        }

    }
}