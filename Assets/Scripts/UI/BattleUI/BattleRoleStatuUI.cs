using UnityEngine;
using System.Collections;

namespace UI.Battle
{

    public class BattleRoleStatuUI : MonoBehaviour
    {

        //public GameObject TeammateUIPreafab;
        public GameObject HeroStatusUIPrefab;

        private HeroStatusUI HeroStatusScript;
        //private TeammateUIManager teammateUIManager;



        public void ShowRoStatusUI()
        { 
            if (HeroStatusScript == null)
            {
                HeroStatusScript = CreatObjectToNGUI.InstantiateObj(HeroStatusUIPrefab,BattleUIManager.Instance.GetScreenTransform(ScreenPositionType.TopLeft)).GetComponent<HeroStatusUI>();
            }
            //HeroStatusScript.Show();
            //if (teammateUIManager == null)
            //{
            //    teammateUIManager = CreatObjectToNGUI.InstantiateObj(TeammateUIPreafab, BattleUIManager.Instance.GetScreenTransform(ScreenPositionType.BottomLeft)).GetComponent<TeammateUIManager>();
            //}
            //teammateUIManager.ShowTeammate(); 
        }


    }
}