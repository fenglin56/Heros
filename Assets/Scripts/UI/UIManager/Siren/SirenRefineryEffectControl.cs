using UnityEngine;
using System.Collections;

public class SirenRefineryEffectControl : MonoBehaviour 
{
    public enum Refinery
    {
        Complete,
        Success,
        Fail,
        Underway,
    }

    public GameObject Eff_Refining_Success;
    public GameObject Eff_Refining_Fail;
    public GameObject Eff_Refining_Complete;
    public GameObject Eff_Refining_Underway;

    public Transform EffEmissionTrans;

    private GameObject gLastEff = null;

    public GameObject PlayEff(Refinery refinery)
    {
        GameObject eff = null;
        switch(refinery)
        {
            case Refinery.Complete:
                eff = Eff_Refining_Complete;
                break;
            case Refinery.Fail:
                eff = Eff_Refining_Fail;
                break;
            case Refinery.Success:
                eff = Eff_Refining_Success;
                break;
            case Refinery.Underway:
                eff = Eff_Refining_Underway;
                break;
        }

        if (gLastEff != null)
        {
            Destroy(gLastEff);
            gLastEff = null;
        }
        gLastEff = (GameObject)Instantiate(eff);
        gLastEff.transform.parent = EffEmissionTrans;
        gLastEff.transform.localPosition = Vector3.zero;
        gLastEff.transform.localScale = eff.transform.localScale;

        return gLastEff;
    }

}
