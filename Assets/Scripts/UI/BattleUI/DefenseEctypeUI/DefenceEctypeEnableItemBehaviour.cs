using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class DefenceEctypeEnableItemBehaviour : MonoBehaviour {
	public UILabel ItemName;
	public UILabel RemainNum;
	public UILabel AwardDesc;
	public UILabel AwardNum;
	public UILabel ConsumeDesc;
	public UILabel ConsumeNum;
	public SpriteSwith EctypePic;
	public SpriteSwith AwardIcon;
	public UISprite FocusEff;

	[HideInInspector]
	public EctypeContainerData EctypeContainerData;
	public Action<DefenceEctypeEnableItemBehaviour> CallBackAct;
    [HideInInspector]
    public SingleButtonCallBack CallBackBtn;

	void Awake()
	{
		CallBackBtn=GetComponentInChildren<SingleButtonCallBack>();

		CallBackBtn.SetCallBackFuntion((obj)=>
		                                 {
			if(CallBackAct!=null)
			{
				CallBackAct(this);
			}
		});
	}
	public void Init(EctypeContainerData ectypeContainerData)
	{
		EctypeContainerData=ectypeContainerData;
		int ectypePos=int.Parse(ectypeContainerData.lEctypePos[2]);

		ItemName.text=LanguageTextManager.GetString(ectypeContainerData.lEctypeName);
        var heroPlayerVals=PlayerManager.Instance.FindHeroDataModel().PlayerValues;

		EctypePic.ChangeSprite(ectypePos);
		AwardIcon.ChangeSprite(ectypePos);

		switch(ectypePos)
		{
			case 1:    //经验关
				AwardDesc.text=LanguageTextManager.GetString("IDS_I15_24");
                 RemainNum.text=heroPlayerVals.PLAYER_FIELD_EXPDEFIEND_VALUE.ToString();
				break;
			case 2:		//铜币关
			AwardDesc.text=LanguageTextManager.GetString("IDS_I15_25");
                 RemainNum.text=heroPlayerVals.PLAYER_FIELD_COINDEFINED_VALUE.ToString();
				break;
			case 3:		//元宝关
			AwardDesc.text=LanguageTextManager.GetString("IDS_I15_26");
            RemainNum.text = heroPlayerVals.PLAYER_FIELD_GOLDDEFINED_VALUE.ToString();
				break;
		}
		ConsumeDesc.text=LanguageTextManager.GetString("IDS_I15_23");
		AwardNum.text=ectypeContainerData.DefenceLevelLoot[1];
		ConsumeNum.text=ectypeContainerData.lCostEnergy;

		gameObject.name=ectypeContainerData.lEctypePos[2];//定义物体名称，用于在UIGrid中排序
	}
	public void SetFocus(bool flag)
	{
		FocusEff.gameObject.SetActive(flag);
	}
}
