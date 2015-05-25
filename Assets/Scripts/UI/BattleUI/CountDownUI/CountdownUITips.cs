using UnityEngine;
using System.Collections;

public class CountdownUITips : MonoBehaviour {

    public UILabel TipsLabel;
    public UILabel timeLabel;

    private bool ShowAnim = false;

    void Awake()
    {
        this.timeLabel.SetText(LanguageTextManager.GetString("IDS_H1_197"));
        timeLabel.SetText(0);
    }

    public void Show(int time)
    {
        if (!ShowAnim&&time <= 10)
        {
            timeLabel.gameObject.animation.Play();
            ShowAnim = true;
        }
        this.timeLabel.SetText(time);
    }


}
