using UnityEngine;
using System.Collections;

using UI.MainUI;

/// <summary>
/// 青龙武学界面
/// </summary>
public class QinglongMartialPanel : BaseUIPanel 
{
	public SingleButtonCallBack BackButton;	//返回按钮

	public MartialItemListPanel_V2 MartialItemList;	//武学列表面板
	public MartialArtsInfoPanel MartialItemInfo;	//武学信息面板

	public UILabel RankingLabel;		//最高荣誉
	public UILabel MaxContributeLabel;	//最高贡献

	private int CurrentSelectedMartial;	//当前被选中的武学ID

	#region 临时代码
	public int PLAYER_FIELD_PVP_RANKINDEX_VALUE = 100;
	public int PLAYER_FIELD_CONTRIB_VALUE = 100;
	#endregion

	void Awake()
	{
		BackButton.SetCallBackFuntion(OnBackButtonClick);
		RegisterEventHandler();
	}

	//引导按钮注册
	private void TackGuideBtnRegister()
	{

	}

	public override void Show(params object[] value)
	{
		base.Show(value);
		RankingLabel.text =  PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAX_SCORE_VALUE.ToString();
		MaxContributeLabel.text = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CONTRIB_VALUE.ToString();
		MartialItemList.show(MartialType.XINFA);
	}

	//测试用
	public void Init()
	{
		RankingLabel.text =  PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_SCORE_VALUE.ToString();
		MaxContributeLabel.text = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CONTRIB_VALUE.ToString();
		MartialItemList.show(MartialType.XINFA);
	}

	//点击某个武学
	public void OnItemClick(object obj)
	{
		int martialIds = (int)obj;
		Debug.Log("点击 " + martialIds + " 武学图标");
		if(martialIds == CurrentSelectedMartial)
		{
			return;
		}
		CurrentSelectedMartial = martialIds;

		MartialItemList.OnSelectItemCallback(martialIds);
		MartialItemInfo.UpdateInfo(martialIds);
	}

	//学习升级武学成功响应
	public void MartialUpgradeCallback(object obj)
	{
		SMsgAcitonStudyWuXue_SC studyWuXue = (SMsgAcitonStudyWuXue_SC)obj;
		//PlayerMartialDataManager.Instance.UpdatePlayerMartialData(studyWuXue);	//测试
		//更新武学信息面板
		MartialItemInfo.ReceiveUpgradeCallback(studyWuXue.dwWuXueID);
		//更新武学列表面板
		MartialItemList.UpdateMartialList(studyWuXue.dwWuXueID);
	}

	public override void Close()
	{
		base.Close();
	}

	//返回按钮点击回调
	void OnBackButtonClick(object arg)
	{
		//SoundManager.Instance.PlaySoundEffect("Sound_Button_PackgeClose");
		BackButton.BackgroundSwithList.ApplyAllItem(p=>{
			p.ChangeSprite(1);
		});
		Close();
	}

	void HonorUpdate(object arg)
	{
		int honor = (int)arg;
		RankingLabel.text = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAX_SCORE_VALUE.ToString();
	}

	void ContributeUpdate(object arg)
	{
		int contribute = (int)arg;
		MaxContributeLabel.text = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CONTRIB_VALUE.ToString();
	}

	//注册事件
	protected override void RegisterEventHandler()
	{
		UIEventManager.Instance.RegisterUIEvent(UIEventType.MartialUpgrade, MartialUpgradeCallback);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.PVPHonorUpdate, HonorUpdate);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.PVPContributeUdate, ContributeUpdate);
	}
}


