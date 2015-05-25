using UnityEngine;
using System.Collections;


//武学item
public class MartialItem : MonoBehaviour 
{
	public UISprite ArrowSprite;	//升级箭头
	public UISprite OutLineSprite;	//选中时的外框
	public UISprite Background;		//武学图标背景
	public Transform IconPoint;		//武学图标
	public UILabel LevelLabel;		//武学等级
	public GameObject LevelIcon;	//等级图标

	private int MartialId;		//武学ID
	private int MartialLevel;
	private PlayerMartialArtsData PlayerMartialData;	//
	public int MartialID
	{
		get{return this.MartialId;}
	}

	#region 临时代码
	public int PLAYER_FIELD_CONTRIB_VALUE = 100;
	public int PLAYER_FIELD_MAX_SCORE_VALUE = 100;
	#endregion

	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="SendWuXueData">Send wu xue data.</param>
	/// <param name="Callback">Callback.</param>
	public void Init(SSendWuXueData SendWuXueData, ButtonCallBack Callback)
	{
		MartialIndex martialIndex = new MartialIndex(){MartialArtsID = SendWuXueData.dwWuXueID, MartialArtsLevel = SendWuXueData.byWuXueLevel};
		PlayerMartialData = PlayerDataManager.Instance.GetPlayerMartialArtConfigData(martialIndex);	
		MartialId = SendWuXueData.dwWuXueID;
		GameObject Icon = UI.CreatObjectToNGUI.InstantiateObj(PlayerMartialData.MartialArtsIconPrefab, IconPoint);
		this.gameObject.GetComponent<SingleButtonCallBack>().SetCallBackFuntion(Callback, MartialId);
		UpdateIconBg(SendWuXueData.byWuXueLevel);
		UpdateLevel(SendWuXueData.dwWuXueID);
		UpdateArrow();
	}

	//更新升级箭头标志
	void UpdateArrow()
	{
		//当前贡献大于等于武学升级所需贡献（MartialArtsContribution），且最高荣誉大于等于武学升级所需最高荣誉（MartialArtsMaxScore），对应武学图标显示 升级提示箭头；
		int currentContribution = 10, maxHonor = 10;
		if(PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CONTRIB_VALUE  >= PlayerMartialData.MartialArtsContribution
		   && PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_MAX_SCORE_VALUE  >= PlayerMartialData.MartialArtsMaxScore)
		{
			ArrowSprite.gameObject.SetActive(true);
		}
		else
		{
			ArrowSprite.gameObject.SetActive(false);
		}
	}

	//更新等级标志
	void UpdateLevel(int martialId)
	{
		int level = PlayerMartialDataManager.Instance.GetMartialLevelByID(martialId);
		if(level > 0)
		{
			LevelIcon.SetActive(true);
			LevelLabel.text = level.ToString();
		}
		else
		{
			LevelIcon.SetActive(false);
		}
	}

	//更新图标背景，不同等级有不同背景
	void UpdateIconBg(int level)
	{
		Background.gameObject.GetComponent<SpriteSwith>().ChangeSprite(level <= 1 ? 1 : level);
	}

	/// <summary>
	/// 被选中
	/// </summary>
	/// <param name="obj">Object.</param>
	public void BeSelected(object obj)
	{
		bool isSelected = (bool)obj;
		OutLineSprite.gameObject.SetActive(isSelected);
	}

	/// <summary>
	/// 服务器返回学习升级成功，更新等级数字和升级箭头
	/// </summary>
	/// <param name="martialId">Martial identifier.</param>
	public void LevelUpdateCallback(int martialId)
	{
		int level = PlayerMartialDataManager.Instance.GetMartialLevelByID(martialId);
		UpdateIconBg(level);
		UpdateLevel(martialId);
		UpdateArrow();
	}

}

