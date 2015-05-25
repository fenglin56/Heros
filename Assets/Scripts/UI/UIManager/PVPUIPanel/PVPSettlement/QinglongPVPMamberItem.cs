using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Result
{
	Success = 0,
	Failed = 1,
}

/// <summary>
/// 青龙会pvp成员
/// </summary>
public class QinglongPVPMamberItem : MonoBehaviour 
{
	public UISprite PlayerHead;		//玩家头像
	public UISprite Career;			//职业
	public UILabel 	PlayerName;		//玩家名字
	public UILabel 	KillAndDie;		//杀敌和死亡
	public UILabel 	Damage;			//造成伤害
	public UISlider DamageSlider;
	public UILabel 	Injured;		//受到伤害
	public UISlider InjuredSlider;
	public UILabel 	Honor;			//荣誉
	public Transform WinTimes1;		//首胜，二胜...
	public UILabel 	ExtraHonor;		//额外荣誉
	public UILabel 	Contribute;		//贡献
	public Transform WinTimes2;		//首胜，二胜
	public UILabel 	ExtraContribute;//额外贡献

	public List<GameObject> TweenAlphaList;	//要透明渐变出现的

	private readonly float DelayTime = 0.6f;
	private readonly float Duration = 0.6f;

	private Result result;	//胜负结果
	private SPVPResult sPVPResult;
	public const string PROFESSION_CHAR = "JH_UI_Typeface_9115_0";

	#region 临时代码
	public int PLAYER_FIELD_WIN_VALUE = 100;

	#endregion

	/// <summary>
	/// 战斗成员信息，参数分别为 信息结构体，胜利方还是失败方
	/// </summary>
	/// <param name="sPVPResult">S PVP result.</param>
	/// <param name="result">Result.</param>
	public void Init(SPVPResult sPVPResult, Result result)
	{
		this.result = result;
		var entityModel = PlayerManager.Instance.FindPlayerByActorId(sPVPResult.dwActorID);
		if(entityModel.EntityDataStruct.SMsg_Header.IsHero)
		{
			var entityDataStruct = (SMsgPropCreateEntity_SC_MainPlayer)entityModel.EntityDataStruct;
			int vocation = entityDataStruct.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
			int fashion = entityDataStruct.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
			var headRes = CommonDefineManager.Instance.CommonDefine.HeroIcon_BattleReward.FirstOrDefault(P=>P.VocationID == vocation &&P.FashionID == fashion);

			PlayerHead.spriteName = headRes.ResName;
			Career.spriteName = PROFESSION_CHAR + vocation.ToString();
			PlayerName.text = entityDataStruct.m_name.ToString();
		}
		else
		{
			var entityDataStruct = (SMsgPropCreateEntity_SC_OtherPlayer)entityModel.EntityDataStruct;
			int vocation = entityDataStruct.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
			int fashion = entityDataStruct.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
			var headRes = CommonDefineManager.Instance.CommonDefine.HeroIcon_BattleReward.FirstOrDefault(P=>P.VocationID == vocation &&P.FashionID == fashion);

			PlayerHead.spriteName = headRes.ResName;
			Career.spriteName = PROFESSION_CHAR + vocation.ToString();
			PlayerName.text = entityDataStruct.m_name.ToString();
		}

		KillAndDie.text = sPVPResult.dwKillEnemy + "/" + sPVPResult.dwDeathTimes;
		Damage.text = sPVPResult.dwDamage.ToString();
		Injured.text = sPVPResult.dwInjured.ToString();
		Honor.text = sPVPResult.dwHonor.ToString();

		if(sPVPResult.dwHonorExtra != 0)
		{
			Honor.transform.localPosition = new Vector3(-2.0f, 11.0f, -7.0f);
			ExtraHonor.transform.localPosition = new Vector3(15.0f, -9.6f, -7.0f);
			ExtraHonor.text = sPVPResult.dwHonorExtra.ToString();
			//实例化首胜prefab
			int WinValue = PLAYER_FIELD_WIN_VALUE;
			if(WinValue > 0 || WinValue < 11)
			{
				GameObject IconPre = CommonDefineManager.Instance.CommonDefine.PVPBattleWinIconPrefab[WinValue];
				GameObject Icon = UI.CreatObjectToNGUI.InstantiateObj(IconPre, WinTimes1);
			}
		}
		else
		{
			Honor.transform.localPosition = new Vector3(-2.0f, -2.0f, -7.0f);
		}

		Contribute.text = sPVPResult.dwContribute.ToString();
		if(sPVPResult.dwContributeExtra != 0)
		{
			Contribute.transform.localPosition = new Vector3(-2.0f, 11.0f, -7.0f);
			ExtraContribute.transform.localPosition = new Vector3(15.0f, -9.6f, -7.0f);
			ExtraContribute.text = sPVPResult.dwContributeExtra.ToString();
			//实例化首胜prefab
			int WinValue = PLAYER_FIELD_WIN_VALUE;
			if(WinValue > 0 || WinValue < 11)
			{
				GameObject IconPre = CommonDefineManager.Instance.CommonDefine.PVPBattleWinIconPrefab[WinValue];
				GameObject Icon = UI.CreatObjectToNGUI.InstantiateObj(IconPre, WinTimes1);
				Icon.GetComponent<UISprite>().color = new Color(0, 255, 255, 255);
			}
		}
		{
			Contribute.transform.localPosition = new Vector3(-2.0f, -2.0f, -7.0f);
		}

		TweenAlphaList.ApplyAllItem(p=>{
			p.GetComponent<UIPanel>().alpha = 0;
		});
	}

	public void Show()
	{
		StartCoroutine(StartTweenAlpha(Duration, DelayTime));
	}

	IEnumerator StartTweenAlpha(float duration, float delay)
	{
		for(int i = 0; i < TweenAlphaList.Count; i++)
		{
			if(i == 2 || i == 3)
			{
				TweenFloatAndSlider(i);
			}
			TweenAlpha.Begin(TweenAlphaList[i], duration, 0.0f, 1.0f, null);
			yield return new WaitForSeconds(delay);
		}
		yield return null;
	}

	void TweenFloatAndSlider(int index)
	{
		//造成伤害显示音效，音效3秒内循环播放
		SoundManager.Instance.PlaySoundEffect("造成伤害显示音效", true);
		DoForTime.DoFunForTime(3.0f, p=>{SoundManager.Instance.StopSoundEffect("造成伤害显示音效");}, null);

		if(index == 2)
		{
			TweenFloat.Begin(3.0f, 0, float.Parse(Damage.text), SetDamage);
			float sliderValue;
			if(result == Result.Success)
			{
				sliderValue = float.Parse(Damage.text) / (float)QinglongPVPSettlementPanel.TotalDamageSuc;
			}
			else
			{
				sliderValue = float.Parse(Damage.text) / (float)QinglongPVPSettlementPanel.TotalDamageFai;
			}

			TweenFloat.Begin(3.0f, 0, sliderValue, SetDamageSliderValue);
		}
		else
		{
			TweenFloat.Begin(3.0f, 0, float.Parse(Injured.text), SetInjured);
			float sliderValue;
			if(result == Result.Success)
			{
				sliderValue = float.Parse(Injured.text) / (float)QinglongPVPSettlementPanel.TotalInjuredSuc;
			}
			else
			{
				sliderValue = float.Parse(Injured.text) / (float)QinglongPVPSettlementPanel.TotalInjuredFai;
			}

			TweenFloat.Begin(3.0f, 0, sliderValue, SetInjuredSliderValue);
		}
	}

	void SetDamage(float value)
	{
		Damage.text = value.ToString();
	}

	void SetDamageSliderValue(float value)
	{
		DamageSlider.sliderValue = value;
	}

	void SetInjured(float value)
	{
		Injured.text = value.ToString();
	}

	void SetInjuredSliderValue(float value)
	{
		InjuredSlider.sliderValue = value;
	}
}

