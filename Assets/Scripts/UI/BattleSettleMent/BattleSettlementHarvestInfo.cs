using UnityEngine;
using System.Collections;
using UI;

public class BattleSettlementHarvestInfo : MonoBehaviour 
{
	public Vector3 Scale_LongSize = new Vector3(345,17,1);
	public Vector3 Scale_ShortSize = new Vector3(185,17,1);

	public GameObject Eff_Flash;
	public UISprite UI_RewardIcon;
	public UISprite UI_RewardBackground;
	public UILabel Label_Reward;

	private Vector3 m_ShowPos = Vector3.zero;
	private Vector3 m_HidePos = Vector3.zero;

	
	public void SetInfo(int itemID, uint itemNum, Vector3 pos, float delayTime)
	{
		var itemData = ItemDataManager.Instance.GetItemData(itemID);
		Label_Reward.text = NGUIColor.SetTxtColor(LanguageTextManager.GetString(itemData._szGoodsName)
		                                   +"x"+itemNum.ToString(),(UI.TextColor)itemData._ColorLevel);
		if(itemData.smallDisplay.Contains("_Lv"))
		{
			int index = itemData.smallDisplay.LastIndexOf("_Lv");
			string str = itemData.smallDisplay.Substring(0,index);
			Debug.Log("itemData.smallDisplay : "+str);
			UI_RewardIcon.spriteName = str;
		}
		else
		{
			UI_RewardIcon.spriteName = itemData.smallDisplay;
		}

		UI_RewardIcon.alpha = 0;
		Label_Reward.alpha = 0;
		UI_RewardBackground.alpha = 0;
		m_ShowPos = pos;
		m_HidePos = pos + Vector3.right * 500;

		StartCoroutine("ShowDelay",delayTime);
	}

	IEnumerator ShowDelay(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		TweenPosition.Begin(gameObject,0.2f,m_HidePos,m_ShowPos,SetPosOver);
	}

	void SetPosOver(object obj)
	{
		Eff_Flash.SetActive(true);
		TweenFloat.Begin(0.3f,0,1f,SetAlpha);
	}

	void SetAlpha(float value)
	{
		UI_RewardIcon.alpha = value;
		Label_Reward.alpha = value;
		UI_RewardBackground.alpha = value;
	}

}
