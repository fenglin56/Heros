using UnityEngine;
using System.Collections;

public class defenceEctypedisableItemBehaviour : MonoBehaviour {

	public UILabel UnLockLev;
	public UILabel Title;
	public SpriteSwith ItemPic;
	/// <summary>
	/// 初始化副本容器
	/// </summary>
	/// <param name="ectypeContainerData">Ectype container data.</param>
	public void Init(EctypeContainerData ectypeContainerData)
	{
		UnLockLev.text=ectypeContainerData.lMinActorLevel.ToString();
		Title.text=LanguageTextManager.GetString(ectypeContainerData.lEctypeName);

		ItemPic.ChangeSprite(int.Parse(ectypeContainerData.lEctypePos[2]));

		gameObject.name=ectypeContainerData.lEctypePos[2];//定义物体名称，用于在UIGrid中排序
	}
}
