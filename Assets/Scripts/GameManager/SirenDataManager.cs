using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SirenDataManager : MonoBehaviour {

    private static SirenDataManager m_instance;
    public static SirenDataManager Instance { get { return m_instance; } }

    public PlayerSirenConfigDataBase playerSirenConfigDataBase;

    private List<PlayerSirenConfigData> PlayerSirenList = new List<PlayerSirenConfigData>();

	public SirenItemControl_V3 CurSelectSiren{ set; get;}

    void Awake()
    {
        m_instance = this;
        InitPlayerSirenConfigData();
    }

    void OnDestroy()
    {
        m_instance = null;        
    }

    void InitPlayerSirenConfigData()
    {
        foreach (PlayerSirenConfigData data in playerSirenConfigDataBase._dataTable)
        {
            PlayerSirenList.Add(data);
        }
    }

    public List<PlayerSirenConfigData> GetPlayerSirenList()
    {
        return PlayerSirenList;
    }

	/// <summary>
	/// 获取所有妖女战力总和
	/// </summary>
	/// <returns>The sirens combat value.</returns>
	public int GetSirensCombatValue()
	{
		int allValue = 0;
		var sirenList = SirenManager.Instance.GetYaoNvList();
		for(int i = 0 ; i< sirenList.Count;i++)
		{
			int value = 0;
			var playerSirenData = PlayerSirenList.SingleOrDefault(p=>p._sirenID == sirenList[i].byYaoNvID);
			var sirenData = playerSirenData._sirenConfigDataList.SingleOrDefault(p=>p._growthLevels == sirenList[i].byLevel);
			string[] growthItem = sirenData._GrowthEffect.Split('|');

			int growthItemLength = growthItem.Length;
			List<SirenGrowthEffect> effectList = new List<SirenGrowthEffect>();
			for (int j = 0; j < growthItemLength; j++)
			{
				string[] growthEffect = growthItem[j].Split('+');
				var effectData = ItemDataManager.Instance.GetEffectData(growthEffect[0]);
				if (effectData != null)
				{
					value+=Convert.ToInt32(growthEffect[1])*effectData.CombatPara;
				}
			}
			allValue += (int)(value / 1000);
		}
		//战斗力总值 =int（（ 生命值*生命值战力系数 + 真气值*真气值战力系数 + 攻击*攻击战力系数+防御*防御战力系数 + 命中*命中战力系数 + 闪避*闪避战力系数 + 暴击*暴击战力系数+ 抗暴击*抗暴击战力系数）/1000）
		return allValue;
	}

	/// <summary>
	/// 计算单个妖女战力
	/// </summary>
	/// <returns>The siren combat value.</returns>
	/// <param name="sirenID">Siren I.</param>
	public int GetSirenCombatValue(int sirenID)
	{
		int value = 0;
		var sirenLevelData = SirenManager.Instance.GetYaoNvList().SingleOrDefault(p=>p.byYaoNvID == sirenID);

		var playerSirenData = PlayerSirenList.SingleOrDefault(p=>p._sirenID == sirenID);
		var sirenData = playerSirenData._sirenConfigDataList.SingleOrDefault(p=>p._growthLevels == sirenLevelData.byLevel);
		string[] growthItem = sirenData._GrowthEffect.Split('|');
		
		int growthItemLength = growthItem.Length;
		List<SirenGrowthEffect> effectList = new List<SirenGrowthEffect>();
		for (int j = 0; j < growthItemLength; j++)
		{
			string[] growthEffect = growthItem[j].Split('+');
			var effectData = ItemDataManager.Instance.GetEffectData(growthEffect[0]);
			if (effectData != null)
			{
				value+=Convert.ToInt32(growthEffect[1])*effectData.CombatPara;
			}
		}
		value = (int)(value / 1000);
		return value;
	}
}
