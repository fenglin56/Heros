using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// 装备及背包右边按钮控制，挂在Prefab_RightBtn预设件上
/// </summary>
[RequireComponent(typeof(SingleButtonCallBack))]
public class RightBtnBehaviour : MonoBehaviour {

	public SpriteSwith TitleSpriteSwitch;
	public SpriteSwith BgSpriteSwitch;
	private PackBtnType m_packBtnType;
	private SingleButtonCallBack m_callBack;
	void Awake()
	{
		m_callBack=GetComponent<SingleButtonCallBack>();
	}
	public void Init(PackBtnType packBtnType,Action<PackBtnType> clickHandle)
	{
		m_packBtnType=packBtnType;
		gameObject.name=string.Format("{0}{1}",(int)packBtnType, packBtnType);
		ChangeSprite(m_packBtnType);
		//监听点击，向上冒泡
		m_callBack.SetCallBackFuntion((obj)=>{if(clickHandle!=null)clickHandle(packBtnType);},m_packBtnType);
		//按下去或松开 效果
		m_callBack.SetPressCallBack((isPressed)=>
		 {
			ChangeSprite(isPressed);
		});
	}
	public void ChangeSprite(PackBtnType packBtnType)
	{
		ChangeSprite(packBtnType,false);
	}
	public void ChangeSprite(bool isFocus)
	{
		ChangeSprite(m_packBtnType,isFocus);
	}
	public void ChangeSprite(PackBtnType packBtnType,bool focus)
	{
		int bgId=1;
		//下面两行代码，把枚举Int值转成SpriteId。
		int sprite=(int)m_packBtnType;
		sprite=sprite+(sprite+1);
		if(focus)
		{
			sprite++;
			bgId++;
		}
		TitleSpriteSwitch.ChangeSprite(sprite);
		BgSpriteSwitch.ChangeSprite(bgId);
        TitleSpriteSwitch.transform.localScale=GetSpriteSize( TitleSpriteSwitch.transform.GetComponent<UISprite>());
	}
    Vector3 GetSpriteSize(UISprite sprite)
    {
        Rect rect=  sprite.GetAtlasSprite().outer;
        return new Vector3(rect.width,rect.height,1);
    }
}
