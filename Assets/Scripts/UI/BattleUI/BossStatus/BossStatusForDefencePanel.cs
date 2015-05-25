using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UI;

public class BossStatusForDefencePanel :  BossStatusPanel_V3 
{
	public UILabel FightRound;// 波数
	public GameObject[] RoadblockDamagedIcon;  //三个路障被破坏的标记
	public GameObject SpecProperty;    //特殊属性
	//public Dictionary<long,GameObject> RoadblockDamagedMap=new Dictionary<long, GameObject>();

	protected override void RegisterEventHandler()
	{
		base.RegisterEventHandler();
	}
}