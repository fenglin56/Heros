using UnityEngine;
using System.Collections;

public class PvpHeroTitle : MonoBehaviour {

	public SpriteSwith Profession;
	public UILabel Forcelable;
	public UILabel LevelLable;
	public UILabel NameLable;
	public void  ShowHeroTitle(int professionID,int Force,string Lv,string name)
	{
		Profession.ChangeSprite(professionID);
		Forcelable.SetText(Force);
		LevelLable.SetText(Lv);
		NameLable.SetText(name);
	}
}
