using UnityEngine;
using System.Collections;

namespace UI.Battle
{

    public class SingleTrialsSettlementPanel : MonoBehaviour
    {
        public UILabel RoundLabel;
        public GameObject NextRoundIconPrefab;
        public Transform CreatIconPoint;


        public void Show(SMSGEctypeTrialsSubResult_SC sMSGEctypeTrialsSubResult_SC)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_UIEff_NextBattle");
            transform.localPosition = Vector3.zero;
            //RoundLabel.SetText(sMSGEctypeTrialsSubResult_SC.dwProgress+1);
            CreatObjectToNGUI.InstantiateObj(NextRoundIconPrefab, CreatIconPoint);
            StartCoroutine(CloseForTime(2));
        }

        IEnumerator CloseForTime(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            Close();
        }

        public void Close()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
            CreatIconPoint.ClearChild();
        }

    }
}