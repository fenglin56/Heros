using UnityEngine;
using System.Collections;

public class MeridiansDesTipsLabel : MonoBehaviour {

    public UISprite TwoAtbSprite;
    public UISprite ThreeAtbSprite;

    public UILabel NameLabel;
    public UILabel AtbNameLabel;
    public UILabel AtbNumberLabel;
    public SingleButtonCallBack NeedLabel;

    public void Show(string meridiansName, string addEffName, string addEffNum,string NeedAddNumber)
    {
        //TraceUtil.Log("显示两个");
        TwoAtbSprite.enabled = false;
        ThreeAtbSprite.enabled = true;
        NeedLabel.gameObject.SetActive(true);
        NameLabel.SetText(meridiansName);
        AtbNameLabel.SetText(addEffName);
        AtbNumberLabel.SetText("+"+addEffNum);
        NeedLabel.SetButtonText(NeedAddNumber);
    }
    public void Show(string meridiansName, string addEffName, string addEffNum)
    {
        //TraceUtil.Log("显示三个");
        TwoAtbSprite.enabled = true;
        ThreeAtbSprite.enabled = false;
        NeedLabel.gameObject.SetActive(false);
        NameLabel.SetText(meridiansName);
        AtbNameLabel.SetText(addEffName);
        AtbNumberLabel.SetText("+" + addEffNum);
    }
}
