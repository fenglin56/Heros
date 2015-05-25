using UnityEngine;
using System.Collections;
using System;
[RequireComponent(typeof(BoxCollider),typeof(UIEventListener))]
public abstract class BaseListItem : MonoBehaviour {
	public Action<BaseListItem> OnClickCallBack;


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
	public abstract void OnGetFocus();
	
	public abstract void OnLoseFocus();

}
