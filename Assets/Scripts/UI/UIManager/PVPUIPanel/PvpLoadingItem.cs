using UnityEngine;
using System.Collections;
using System.Text;
public class PvpLoadingItem : MonoBehaviour {
	public SpriteSwith bg;
	public UISprite spriteIcon;
	public SpriteSwith assassinVacation;
	public SpriteSwith swordsmanVacation;
	public SpriteSwith infoMark;
	public UILabel infoName;
	private Color nameColor = new Color(128/255f,40/255f,0f,1f);
	public SGroupMemberInfo roleInfo;
	// Use this for initialization
	private bool isRead = false;
	void Init () {
		if (isRead)
			return;
		isRead = true;
	}
	
	// Update is called once per frame
	public void Show (SGroupMemberInfo info) {
		roleInfo = info;
		Init ();
		ShowPanel ();
	}
	public void UpdateInfo(bool isOver)
	{
		SetRoleState (isOver);
	}
	void ShowPanel()
	{
		foreach(RoleIconData roleData in CommonDefineManager.Instance.CommonDefine.pvpLoadingIcon)
		{
			if(roleData.VocationID == roleInfo.dwVocation && roleData.FashionID == roleInfo.dwFashion)
			{
				spriteIcon.spriteName = roleData.ResName;
				break;
			}
		}
		SetRoleState (false);
	}
	void SetRoleState(bool isOver)
	{
		//1是剑客,4是刺客//
		if (roleInfo.dwVocation == 1) {
			assassinVacation.gameObject.SetActive(true);
			swordsmanVacation.gameObject.SetActive(false);
			assassinVacation.ChangeSprite(isOver?0:1);
		} else {
			assassinVacation.gameObject.SetActive(false);
			swordsmanVacation.gameObject.SetActive(true);
			swordsmanVacation.ChangeSprite(isOver?0:1);
		}
		infoMark.ChangeSprite (isOver?0:1);
		infoName.text = Encoding.UTF8.GetString(roleInfo.szName);
		infoName.color = isOver?nameColor:Color.white;
		bg.ChangeSprite (isOver?0:1);
	}
}






































