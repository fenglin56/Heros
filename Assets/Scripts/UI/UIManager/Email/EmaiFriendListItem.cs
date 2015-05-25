using UnityEngine;
using System.Collections;
using System;
using UI.Friend;
using System.Linq;
[RequireComponent(typeof(BoxCollider),typeof(UIEventListener))]
public class EmaiFriendListItem : MonoBehaviour {
	public Action<EmaiFriendListItem> OnClickCallBack;
	public UILabel Level_des;
	public UISprite Icon_spring;
	public UILabel Name;
	public UILabel Level;
	public UISprite Selected_spring;
	public PanelElementDataModel Friend;
    public SpriteSwith profession;
	//public Action<EmaiFriendListItem> OnItemClick;

	void Awake()
	{
		GetComponent<UIEventListener>().onClick=OnItemClick;
	}
	
	void OnItemClick(GameObject obj)
	{
		if(OnClickCallBack!=null)
		{
			OnClickCallBack(this);
		}
	}
	
	public virtual void BeSelected()
	{
		OnClickCallBack(this);
	}
	public  void OnGetFocus() 
	{
		Selected_spring.gameObject.SetActive(true);
	}
	
	public  void OnLoseFocus() 
	{
		Selected_spring.gameObject.SetActive(false);
	}

	public void InitItemData(PanelElementDataModel item)
	{
		Friend=item;
		Level_des.SetText(LanguageTextManager.GetString("IDS_I22_38"));
        profession.ChangeSprite((int)item.sMsgRecvAnswerFriends_SC.dProfession);
		Icon_spring.spriteName=CommonDefineManager.Instance.CommonDefineFile._dataTable.HeroIcon_MailFriend.Where(p=> p.VocationID==item.sMsgRecvAnswerFriends_SC.dProfession&&p.FashionID==item.sMsgRecvAnswerFriends_SC.dbSysActorImageHeadID).First().ResName;
		Name.SetText(item.sMsgRecvAnswerFriends_SC.Name);
		Level.SetText(item.sMsgRecvAnswerFriends_SC.sActorLevel.ToString());
	}




}
