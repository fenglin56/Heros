using UnityEngine;
using System.Collections;

public class EndLessRankItem : MonoBehaviour {
	public ItemIconInfo itemInfo;
	public GameObject lockObj;
	public GameObject besgObj;
	public UILabel labelWave;
	public void Init(ItemData item,int count,int wave)
	{
		itemInfo.Init (item,count.ToString());
		labelWave.text = string.Format(LanguageTextManager.GetString ("IDS_I20_4"), wave);
	}
	public void Show(bool isLock,bool isBest)
	{
		if(lockObj.activeSelf != isLock)
			lockObj.SetActive (isLock);
		if(besgObj.activeSelf != isBest)
			besgObj.SetActive (isBest);
	}
}
