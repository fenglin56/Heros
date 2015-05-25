using UnityEngine;
using System.Collections;
using UI;

/// <summary>
///	武学信息面板 
/// </summary>
public class MartialArtsInfoPanel : MonoBehaviour {

	public UILabel MartialNameLabel;	//武学名称
	public UILabel MartialDesLabel;		//武学描述
	public UILabel MartialEffDesLabel;	//武学效果描述
	public UILabel MartialLevelLabel;	//武学当前等级
	public UILabel MaxHonorLabel;		//最高荣誉数值
	public UILabel NeedConLabel;		//需要贡献数值

	public UILabel MaxHonorN;			//最高荣誉名字
	public UILabel NeedConN;			//需要贡献名字
	public UILabel MartialLevelN;		//武学等级名字

	public Transform IconPoint;		//武学图标
	public GameObject IconBG;			//武学图标背景

	public GameObject EffBackground;	//武学效果面板背景
	public GameObject LineMartialLevel;	//武学等级整行，未解锁的不显示
	public GameObject LineHonor;		//荣誉行
	public GameObject LineUpgradeLabel;	//升级条件字体
	public GameObject LineLearnLabel;	//学习条件字体
	public GameObject ConBackground;	//武学学习或升级条件背景

	public SingleButtonCallBack Button;	//学习或升级按钮

	private PlayerMartialArtsData MartialData;

	private bool IsLock = true;	//是否锁住，还没学习
	private int MartialId;	//武学Id
	private int MartialLevel;	//武学等级
	private int CurrentLevel;//武学当前等级
	private int PlayerMaxHonor = 100000;	//玩家当前最高荣誉
	private int PlayerCurrentContirbute = 100000;	//玩家当前贡献值 

	void Awake()
	{
		Button.SetCallBackFuntion(LearnOrUpgradeBtnCallback);
	}

	//更新某个武学信息
	public void UpdateInfo(int martialId)
	{
		MartialId = martialId;
		MartialLevel = PlayerMartialDataManager.Instance.GetMartialLevelByID(martialId);
		MartialIndex martialIndex = new MartialIndex(){MartialArtsID = MartialId, MartialArtsLevel = MartialLevel};
		MartialData = PlayerDataManager.Instance.GetPlayerMartialArtConfigData(martialIndex);	
		IconBG.GetComponent<SpriteSwith>().ChangeSprite(MartialLevel <= 1 ? 1 : MartialLevel);
		IconPoint.ClearChild();
		GameObject icon = UI.CreatObjectToNGUI.InstantiateObj(MartialData.MartialArtsIconPrefab, IconPoint);
		MartialNameLabel.text = LanguageTextManager.GetString(MartialData.MartialArtsName);
		MartialDesLabel.text = LanguageTextManager.GetString(MartialData.MartialArtsDes);
		MartialEffDesLabel.text = LanguageTextManager.GetString(MartialData.MartialArtsParamDes);
		NeedConLabel.text = MartialData.MartialArtsContribution.ToString();

		if(MartialLevel == 0)	//没解锁
		{
			EffBackground.transform.localScale = new Vector3(292, 74, 0);
			ConBackground.transform.localScale = new Vector3(292, 176, 0);
			LineMartialLevel.SetActive(false);
			LineHonor.SetActive(true);
			LineLearnLabel.SetActive(true);
			LineUpgradeLabel.SetActive(false);
			MaxHonorN.text = LanguageTextManager.GetString("IDS_I38_23");
			MaxHonorLabel.text = MartialData.MartialArtsMaxScore.ToString();
			NeedConN.text = LanguageTextManager.GetString("IDS_I38_24");
			//Button.GetComponentInChildren<UILabel>().text = LanguageTextManager.GetString("IDS_I38_26");	//学习
			Button.SetButtonText(LanguageTextManager.GetString("IDS_I38_26"));
		}
		else //已解锁
		{
			EffBackground.transform.localScale = new Vector3(292, 115, 0);
			ConBackground.transform.localScale = new Vector3(292, 137, 0);
			LineMartialLevel.SetActive(true);
			LineHonor.SetActive(false);
			LineLearnLabel.SetActive(false);
			LineUpgradeLabel.SetActive(true);
			MartialLevelN.text = LanguageTextManager.GetString("IDS_I38_25");
			MartialLevelLabel.text = MartialLevel.ToString() + "/" + MartialData.MartialArtsMaxLevels;
			//Button.GetComponentInChildren<UILabel>().text = LanguageTextManager.GetString("IDS_I38_27");	//升级
			Button.SetButtonText(LanguageTextManager.GetString("IDS_I38_27"));
		}
	}

	//学习或升级按钮回调
	void LearnOrUpgradeBtnCallback(object arg)
	{
		if(MartialLevel == 0)	//学习
		{
			//SoundManager.Instance.PlaySoundEffect("Sound_Button_PlayerMartialArts_Learn");
			//最高荣誉小于升级需求最高荣誉
			if(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAX_SCORE_VALUE < MartialData.MartialArtsMaxScore)
			//if(PlayerMaxHonor < MartialData.MartialArtsMaxScore)//测试 
			{
				MessageBox.Instance.ShowTips(4,LanguageTextManager.GetString("IDS_I38_21"),1f);
				Debug.Log("最高荣誉未达到需求");//测试
				return;
			}
			//当前贡献小于升级需求贡献
			if(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CONTRIB_VALUE < MartialData.MartialArtsContribution)
			//if(PlayerCurrentContirbute < MartialData.MartialArtsContribution)
			{
				MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_I38_22"), 1f);
				Debug.Log("贡献不足");	//测试
				return;
			}

			//可以学习，发送学习消息给服务端
			NetServiceManager.Instance.EntityService.SendWuXueStudyRequest(MartialId);
			//SMsgAcitonStudyWuXue_SC studyWuXue = new SMsgAcitonStudyWuXue_SC(){dwWuXueID = MartialId, byWuXueLevel = (byte)(MartialLevel+1)};
			//UIEventManager.Instance.TriggerUIEvent(UIEventType.MartialUpgrade, studyWuXue);	//测试
		}
		else //升级
		{
			//SoundManager.Instance.PlaySoundEffect("Sound_Button_PlayerMartialArts_Upgrade");
			if(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CONTRIB_VALUE < MartialData.MartialArtsContribution)
			//if(PlayerCurrentContirbute < MartialData.MartialArtsContribution)//测试
			{
				MessageBox.Instance.ShowTips(4, LanguageTextManager.GetString("IDS_I38_22"), 1f);
				return;
			}

			//可以升级，发送升级消息级服务端
			NetServiceManager.Instance.EntityService.SendWuXueStudyRequest(MartialId);
			//SMsgAcitonStudyWuXue_SC studyWuXue = new SMsgAcitonStudyWuXue_SC(){dwWuXueID = MartialId, byWuXueLevel = (byte)(MartialLevel+1)};
			//UIEventManager.Instance.TriggerUIEvent(UIEventType.MartialUpgrade, studyWuXue);//测试用
		}

	}

	//接收到升级成功回调
	public void ReceiveUpgradeCallback(int martialId)
	{
		//播放学习或升级成功音效 
		//SoundManager.Instance.PlaySoundEffect("Sound_UIEff_PlayerMartialArts_Upgrade");
		if(MartialLevel == 0)	//学习成功
		{
			IsLock = true;
			//Button.GetComponentInChildren<UILabel>().text = LanguageTextManager.GetString("IDS_I38_27");	//升级
			Button.SetButtonText(LanguageTextManager.GetString("IDS_I38_27"));
			//学习成功的武学图标、解锁武学图标位置同时播放武学图标升级闪烁动画
			//GameObject eff = UI.CreatObjectToNGUI.InstantiateObj(Eff_BoxOpen, Button_View.transform);
			//当前武学信息位置播放武学信息显示特效，更新武学信息内容

			//如果有战力提升，播放战力值提升动画（用现有战力值提升动画）
		}
		else
		{
			//升级武学图标播放武学图标升级闪烁动画
			//当前武学信息位置播放武学信息切换特效，更新武学信息内容
	
			//如果有战力提升，播放战力值提升动画（用现有战力值提升动画）
		}

		UpdateInfo(martialId);//ID由服务端返回
	}
}

