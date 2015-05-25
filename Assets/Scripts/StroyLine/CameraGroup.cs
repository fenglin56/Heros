using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraGroup {

    private Dictionary<int, List<AnimationClip>> m_AnimationList = new Dictionary<int, List<AnimationClip>>();
	public void SetRolePosition(StroyAction npcAction, int AnimID)
	{
		Vector3 pos = StroyLineConfigManager.Instance.GetStroyActionConfig [AnimID]._StartPosition;
		float dis = Vector3.Distance (pos, Vector3.zero);
		if ( dis > 0.1f ) {
			if (StroyLineDataManager.Instance.GetNpcList.ContainsKey (npcAction.NpcID)) {
				Transform npcTrans = StroyLineDataManager.Instance.GetNpcList [npcAction.NpcID].transform;
				npcTrans.localPosition = pos;
			}
		}
	}
    /// <summary>
    /// 创建玩家对象
    /// </summary>
    /// <param name="npcAction"></param>
    /// <param name="AnimID"></param>
    public void CreateRoleData(StroyAction npcAction, int AnimID)
    {
        StroyActionConfigData actionData = StroyLineConfigManager.Instance.GetStroyActionConfig[AnimID];
        Vector3 pos = Vector3.zero;
        pos = StroyLineConfigManager.Instance.GetStroyActionConfig[AnimID]._StartPosition;
        //pos.z = StroyLineConfigManager.Instance.GetStroyActionConfig[AnimID]._StartPosition.z;
        GameObject npcGo = null;
        Quaternion roleInitDirection = Quaternion.Euler(0, (float)actionData._StartAngle, 0);

        if (StroyLineDataManager.Instance.GetNpcList.ContainsKey(npcAction.NpcID))
        {
            Transform npcTrans = StroyLineDataManager.Instance.GetNpcList[npcAction.NpcID].transform;
            npcTrans.position = pos;
            npcTrans.forward = roleInitDirection * Vector3.forward;
            return;
        }

        switch (npcAction.RoleType)
        {   
            case 1: ///生成玩家
                var playerData = PlayerDataManager.Instance.GetBattleItemData((byte)npcAction.RoleResID);
                npcGo = AssemblyPlayer(playerData, pos);
                npcGo.transform.forward = roleInitDirection * Vector3.forward;                
                break;
            case 2:///生成怪物
                var monsterData = MapResManager.Instance.GetMapMonsterPrefab(npcAction.RoleResID);//BattleConfigManager.Instance.GetMonsterData(npcAction.RoleResID);
                //npcGo = (GameObject)GameObject.Instantiate(monsterData.MonsterPrefab, pos, roleInitDirection);
                //npcGo.RemoveComponent<MonsterBehaviour>("MonsterBehaviour");
                //npcGo.RemoveComponent<MonsterBehaviour>("HurtFlash");
                break;
            case 3:///生成NPC

                var npcPrefab = MapResManager.Instance.GetMapMonsterPrefab(npcAction.RoleResID);//NPCConfigManager.Instance.NPCConfigList[npcAction.RoleResID].NPCPrefab;
                if (npcPrefab != null)
                {
                    npcGo = (GameObject)GameObject.Instantiate(npcPrefab, pos, roleInitDirection);
                }
                else
                    Debug.LogWarning("剧情系统：发现将要生成的NpcPrefab为空。");
                break;
            case 4:///生成当前英雄
                var curHeroVocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
                if (curHeroVocation == 0) ///当在剧情编辑器状态下，无法获得用户职业
                    curHeroVocation = (byte)npcAction.RoleResID;
                var heroData = PlayerDataManager.Instance.GetBattleItemData((byte)curHeroVocation);
                if (heroData != null)
                {
                    npcGo = AssemblyPlayer(heroData, pos);
                    npcGo.transform.forward = roleInitDirection * Vector3.forward;
                }
                else
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"没有获得当前英雄配置数据");
                break;
            default:
                break;
        }

        if (npcGo != null)
        {
            StroyNpcBehaviour npcBehaviour;
			int size;

            var childs = npcGo.GetChildTransforms();
            childs.ApplyAllItem(P => P.gameObject.layer = 30);
//			Transform skinObj = npcGo.transform.Find("SkinnedMesh");//
			SkinnedMeshRenderer[] skinRend = npcGo.GetComponentsInChildren<SkinnedMeshRenderer>();
			if(skinRend != null && skinRend.Length != 0)
			{
				foreach(SkinnedMeshRenderer skin in skinRend)
				{
					skin.updateWhenOffscreen = true;
				}
			}

            if (npcAction.RoleType == 2)
            {
				var child = npcGo.transform.GetChild(0);
				npcGo.SetActive(true);
                npcBehaviour = child.gameObject.AddComponent<StroyNpcBehaviour>();
				size = child.animation.GetClipCount();

                if (size > 0)
                {
                    m_AnimationList.Add(npcAction.NpcID, new List<AnimationClip>());
                    foreach (AnimationState states in child.animation)
                    {
                        m_AnimationList[npcAction.NpcID].Add(child.animation.GetClip(states.name));
                    }
                }
            }
            else
            {
                npcBehaviour = npcGo.AddComponent<StroyNpcBehaviour>();
				size = npcGo.animation.GetClipCount();

                if (size > 0)
                {
                    m_AnimationList.Add(npcAction.NpcID, new List<AnimationClip>());
                    foreach (AnimationState states in npcGo.animation)
                    {
                        m_AnimationList[npcAction.NpcID].Add(npcGo.animation.GetClip(states.name));
                    }
                }
            }

            StroyLineDataManager.Instance.GetNpcList.Add(npcAction.NpcID, npcBehaviour);
        }
        else
            Debug.LogWarning("剧情系统：没有创建ID为" + npcAction.NpcID +"NPC");
    }
	
    /// <summary>
    /// 设置玩家阴影
    /// </summary>
    /// <param name="go"></param>
    /// <param name="shadowEffect"></param>
	void SetPlayerShadow(GameObject go, GameObject shadowEffect)
	{
		var shadowEff =  GameObject.Instantiate(shadowEffect) as GameObject;
        shadowEff.name = "shadow";
        shadowEff.transform.parent = go.transform;
        shadowEff.transform.localPosition = shadowEff.transform.localPosition + Vector3.up * 1;
	}
	
    /// <summary>
    /// 获取角色的Anim列表
    /// </summary>
    public Dictionary<int, List<AnimationClip>> GetNpcAnimClip
    {
        get { return m_AnimationList; }
    }

    /// <summary>
    /// 装配角色
    /// </summary>
    private GameObject AssemblyPlayer(PlayerGenerateConfigData configData, Vector3 pos)
    {
        string defaultAnim, defaultAvatar;
        string[] attachAnims;//, weaponPosition;

        defaultAnim = configData.DefaultAnim;
        attachAnims = configData.Animations;
        defaultAvatar = configData.DefaultAvatar;
        //defaultWeapon = configData.DefaultWeapon;
        //weaponPosition = configData.WeaponPosition;

        var player = RoleGenerate.GenerateRole(configData.PlayerName, defaultAvatar, true);
        player.name = configData.PlayerName;

        RoleGenerate.AttachAnimation(player, configData.PlayerName, defaultAnim, attachAnims);

        PlayerBehaviour playerBehaviour = player.GetComponent<PlayerBehaviour>();
        if (playerBehaviour != null)
        {
            player.RemoveComponent<PlayerBehaviour>("PlayerBehaviour");
        }
        

        player.transform.position = pos;
        player.animation.CrossFade("BIdle");

        //SetPlayerShadow(player, configData.ShadowEffect);

		//挂武器 jamfing
		/*int vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
		StroyLineKey key = new StroyLineKey{ VocationID = vocation,ConditionID = GameManager.Instance.GetStoryConfigData._TriggerCondition,
			EctypeID = GameManager.Instance.GetStoryConfigData._EctypeID };*/
		if (StroyLineConfigManager.Instance.GetStroyLineConfig.ContainsKey(StroyLineDataManager.Instance.curStroyLineKey))
		{
			if(StroyLineConfigManager.Instance.GetStroyLineConfig[StroyLineDataManager.Instance.curStroyLineKey].WeaponType == 1)
			{
				string[] ItemWeaponPosition = configData.WeaponPosition;
				var weaponObj = PlayerFactory.Instance.GetWeaponPrefab(configData.DefaultWeapon);
				RoleGenerate.AttachWeapon(player,ItemWeaponPosition,weaponObj,null);
			}
		}
        return player;
    }
    /// <summary>
    /// 创建动作特效
    /// </summary>
    /// <param name="actionID"></param>
    /// <param name="npcID"></param>
    public void CreateActionEffect(int actionID, int npcID)
    {		
        StroyActionConfigData actionData = StroyLineConfigManager.Instance.GetStroyActionConfig[actionID];
		
		Transform npcTrans;
		if(StroyLineDataManager.Instance.GetNpcList.ContainsKey(npcID))
        	npcTrans = StroyLineDataManager.Instance.GetNpcList[npcID].transform;
		else
			return;
        Vector3 startPos = npcTrans.TransformPoint(actionData._EffectPosition.z, npcTrans.localPosition.y + actionData._EffectPosition.y, actionData._EffectPosition.x);  //配置表中的X对应3D中的Z，z对应3D中和X,

		if(actionData._EffectGo == null)
			return;
        GameObject actionEffect = GameObjectPool.Instance.AcquireLocal(actionData._EffectGo, startPos, Quaternion.Euler(0, actionData._EffectStartAngle, 0));//Quaternion.Euler(0, rotationY, 0));
        
        var childs = actionEffect.GetChildTransforms();
        childs.ApplyAllItem(P => P.gameObject.layer = 30);
        
        StroyActionEffect actionEffectBehaviour = actionEffect.GetComponent<StroyActionEffect>();
        if (actionEffectBehaviour == null)
        {
            actionEffectBehaviour = actionEffect.AddComponent<StroyActionEffect>();
        }

        actionEffectBehaviour.InitDataConfig(actionData._EffectLoopTimes);
    }

    /// <summary>
    /// 执行动作
    /// </summary>
    /// <param name="npcID"></param>
    /// <param name="stroyAction"></param>

    public void Act(StroyAction stroyAction)
    {
        if (StroyLineDataManager.Instance.GetNpcList.ContainsKey(stroyAction.NpcID))
        {
            StroyLineDataManager.Instance.GetNpcList[stroyAction.NpcID].SetActionData(stroyAction);
        }
    }
}
