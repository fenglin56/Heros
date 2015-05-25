using UnityEngine;
using System.Collections;

public class SignFruitReward : MonoBehaviour {
	public int index;
	public UILabel dayLabel;
	public UILabel goldLabel;
	// Use this for initialization
	public void Show (int days,string dayStr,int goldCount) {
		index = days;
		dayLabel.text = dayStr;
		goldLabel.text = goldCount.ToString ();
	}
}