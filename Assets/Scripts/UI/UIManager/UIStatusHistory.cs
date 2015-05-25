using UnityEngine;
using System.Collections;


namespace UI.MainUI
{

    public class UIStatusHistory
    {
        /// <summary>
        /// 用于记录UI的历史状态
        /// </summary>
        
        int[] UILevelStatus;

        public UIStatusHistory()
        {
            UILevelStatus = new int[4];
        }

        public void ResetStatus()
        {
            for (int i = 0;i < this.UILevelStatus.Length; i++)
            {
                this.UILevelStatus[i] = 0;
            }
        }

        public void SaveUIStatus(int PanelLevel, int PanelValue)//第几层面板，该层面板下的激活面板号
        {
            UILevelStatus[PanelLevel] = PanelValue;
        }

        public int[] getUIStatus()
        {
            return UILevelStatus;
        }

    }


}