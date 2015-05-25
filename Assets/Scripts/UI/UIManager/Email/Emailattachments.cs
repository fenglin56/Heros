using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;


public class Emailattachments : MonoBehaviour {
	public UILabel Title_des;
	public UILabel num_des;
	public Transform IconPoint;
	public UILabel Title;
	public UILabel num;
    public SpriteSwith Background_spriteSwith;

	void Awake()
	{
		Title_des.SetText(LanguageTextManager.GetString("IDS_I22_15"));
		num_des.SetText(LanguageTextManager.GetString("IDS_I22_16"));
	}
	public void Init(SEmailSendUint email)
	{
        IconPoint.ClearChild();
		ItemData data=ItemDataManager.Instance.GetItemData(System.Convert.ToInt32( email.dwGoodsID)) ;
        Title.SetText(NGUIColor.SetTxtColor( LanguageTextManager.GetString( data._szGoodsName),(TextColor)data._ColorLevel));
		//NGUITools.AddChild(IconPoint.gameObject,data._picPrefab);
        Background_spriteSwith.ChangeSprite(data._ColorLevel+1);
        UI.CreatObjectToNGUI.InstantiateObj(data._picPrefab,IconPoint);
		num.SetText(email.dwGoodsNum.ToString());

	}
}
