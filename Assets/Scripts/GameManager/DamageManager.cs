using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;

public class DamageManager : Controller ,IEntityManager,ISingletonLifeCycle{

    private List<EntityModel> m_damageList = new List<EntityModel>();
    private static DamageManager m_instance;

    private List<PlayerEquipAllocationAnimation> m_animationList = new List<PlayerEquipAllocationAnimation>();
    private List<PlayerEquipAllocationAnimation_V2> m_animationList_V2 = new List<PlayerEquipAllocationAnimation_V2>();

    public static DamageManager Instance
    {
        get 
        {
            if (m_instance == null)
            {
                m_instance = new DamageManager();
                SingletonManager.Instance.Add(Instance);
                EntityController.Instance.RegisteManager(TypeID.TYPEID_BOX, m_instance);
            }
            return m_instance;
        }
    }

    public static DamageManager GetInstance()
    {
        return Instance;
    }

    public void RegisteEntity(EntityModel damageData)
    {
        if (m_damageList.Exists(P => P.EntityDataStruct.SMsg_Header.uidEntity == damageData.EntityDataStruct.SMsg_Header.uidEntity))
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"收到服务器重复创建同ID的实体！");
        }
        else
        {
            m_damageList.Add(damageData);
        }
    }

    public List<EntityModel> GetDamageList()
    {
        return m_damageList;
    }

    public void AllocationToPlayer(Int64 heroUID, Int64 itemUID, int itemID)
    {
        if (itemID == 0)//分配失败
        {
            var damageModel = m_damageList.SingleOrDefault(p => p.EntityDataStruct.SMsg_Header.uidEntity == itemUID);
            if (damageModel != null)
            {                
                var heroModel = PlayerManager.Instance.FindPlayer(heroUID);
                if (heroModel != null)
                { 
					((DamageBehaviour)damageModel.Behaviour).AllocationFailure(damageModel.GO.transform.position);
                }
            }
            else
            {
                TraceUtil.Log("damageModel == null");
            }
        }
        else //分配成功 播放动画
        {            
            SoundManager.Instance.PlaySoundEffect("Sound_Msg_ItemGet");
            var equipData = ItemDataManager.Instance.GetItemData(itemID);
            if (equipData != null)
            {
                var heroModel = PlayerManager.Instance.FindPlayer(heroUID);
                if (heroModel != null)
                {
                    if (equipData._picPrefab != null)
                    {
                        //var damage = (GameObject)GameObject.Instantiate(equipData._picPrefab, heroModel.transform.position, Quaternion.identity);
                        //damage.transform.parent = PopupObjManager.Instance.UICamera.transform;
                        //damage.transform.localScale = new Vector3(45f, 45f, 1f);
                        //EquipAllocationAnimation equipAA = damage.AddComponent<EquipAllocationAnimation>();
                        //equipAA.Init(heroModel.gameObject, heroUID);
                        //AddEquipAllocationAnimation(heroUID, equipAA);
                        AddEquipAllocationAnimation(heroUID, new PickUpEff() { Index = equipData._ColorLevel, IconSpriteName = equipData.smallDisplay });                                                
                    }  
					UI.Battle.BattleMessangeManager.Instance.Show("1",LanguageTextManager.GetString(equipData._szGoodsName));
                }        
            }
        }                
    }

    private void AddEquipAllocationAnimation(Int64 uid, EquipAllocationAnimation eaa)
    {
        var playerEquipAllocationAnimation = m_animationList.SingleOrDefault(p => p.PlayerUID == uid);
        if (playerEquipAllocationAnimation == null)
        {
            playerEquipAllocationAnimation = new PlayerEquipAllocationAnimation();            
            playerEquipAllocationAnimation.PlayerUID = uid;
            m_animationList.Add(playerEquipAllocationAnimation);
        }
        playerEquipAllocationAnimation.Add(eaa);
    }

    private void AddEquipAllocationAnimation(Int64 uid, PickUpEff eff)
    {
        var animation = m_animationList_V2.SingleOrDefault(p => p.PlayerUID == uid);
        if (animation == null)
        {
            animation = new PlayerEquipAllocationAnimation_V2();
            animation.PlayerUID = uid;
            m_animationList_V2.Add(animation);
        }
        animation.Add(eff);
    }

    //public void PlayEquipAllocationAnimation(Int64 uid)
    //{
    //    var playerEquipAllocationAnimation = m_animationList.SingleOrDefault(p => p.PlayerUID == uid);
    //    if (playerEquipAllocationAnimation != null)
    //    {
    //        BattleManager.Instance.StartCoroutine(PlayNextAnimation(playerEquipAllocationAnimation));            
    //    }

    //    //BattleManager.Instance.StartCoroutine(
    //}
    //IEnumerator PlayNextAnimation(PlayerEquipAllocationAnimation peaa)
    //{
    //    yield return new WaitForEndOfFrame();
    //    yield return new WaitForSeconds(CommonDefineManager.Instance.CommonDefine.DropItem_IntervalTime);
    //    peaa.Play();
    //}

    //战斗结束要清理获得装备动画列表
    /// <summary>
    /// 
    /// </summary>
    public void ClearEquipAllocationAnimations()
    {
        m_animationList.Clear();
    }

    protected override void RegisterEventHandler()
    {
        
    }


    public void UnRegisteEntity(long uid)
    {
        var targetEntity = this.m_damageList.SingleOrDefault(P => ((DamageBehaviour)P.Behaviour).DamageDataModel.SMsg_Header.uidEntity == uid);

        if (targetEntity != null)
        {
            m_damageList.Remove(targetEntity);

            targetEntity.DestroyEntity();
        }
        //检查Factory中是否有未创建的缓存数据，如果有，一并清除
        GameManager.Instance.DamageFactory.UnRegister(uid);
    }

    public EntityModel GetEntityMode(long uid)
    {
        throw new NotImplementedException();
    }

	public void AllocationEquip(Int64 heroUID, Int64 itemUID, int itemID)
	{
		var damageModel = m_damageList.SingleOrDefault(p => p.EntityDataStruct.SMsg_Header.uidEntity == itemUID);
		if (damageModel != null)
		{                
			var heroModel = PlayerManager.Instance.GetEntityMode(heroUID);
			if (heroModel != null)
			{ 
				var boxBeAttract = damageModel.GO.GetComponent<BoxBeAttract>();
				if(boxBeAttract!= null)
				{
					boxBeAttract.PickOver(heroUID, itemUID,heroModel ,itemID);
				}
			}
		}
	}

	public void PickUpAllEquip()
	{
		m_damageList.ApplyAllItem(p=>{
			var boxBeAttract = p.GO.GetComponent<BoxBeAttract>();
			if(boxBeAttract!=null)
			{
				boxBeAttract.JudgeAndPickUp();
			}
		});
	}

    public void Instantiate()
    {
    }

    public void LifeOver()
    {
        m_instance = null;
    }

    public class PickUpEff
    {
        public int Index;
        public string IconSpriteName;
    }

	
    public class PlayerEquipAllocationAnimation_V2
    {
        public Int64 PlayerUID;
        public bool IsPlay = false;
        private List<PickUpEff> m_effList = new List<PickUpEff>();

        public void Add(PickUpEff eff)
        {
            if (!IsPlay)
            {
                CreateEffect(eff);
                IsPlay = true;
                BattleManager.Instance.StartCoroutine(LateShow());
            }
            else
            {
                m_effList.Add(eff);
            }
        }

        IEnumerator LateShow()
        {
            yield return new WaitForSeconds(CommonDefineManager.Instance.CommonDefine.DropItem_IntervalTime);
            Play();
        }

        private void Play()
        {
            if (m_effList.Count > 0)
            {
                CreateEffect(m_effList[0]);
                m_effList.RemoveAt(0);
                IsPlay = true;
                BattleManager.Instance.StartCoroutine(LateShow());
            }
            else
            {
                IsPlay = false;
            }
        }

        private void CreateEffect(PickUpEff data)
        {
            var hero = PlayerManager.Instance.FindPlayer(PlayerUID);
            if (hero != null)
            {
                var eff = (GameObject)GameObject.Instantiate(GameManager.Instance.DamageFactory.PickupEffs[data.Index]);
                eff.transform.parent = hero.transform;
                eff.transform.localPosition = Vector3.zero;
                PickUpEffectBehaviour pickupB = eff.GetComponent<PickUpEffectBehaviour>();
                pickupB.Begin(data.IconSpriteName);
            }            
        }
    }

    public class PlayerEquipAllocationAnimation
    {
        public Int64 PlayerUID;
        public bool IsPlay = false;
        private List<EquipAllocationAnimation> m_AnimationList = new List<EquipAllocationAnimation>();
        
        public void Add(EquipAllocationAnimation eaa)
        {
            if(false == IsPlay)
            {
                eaa.Begin();
                IsPlay = true;
                BattleManager.Instance.StartCoroutine(LateShow());
            }
            else
            {
                m_AnimationList.Add(eaa);
            }            
        }

        IEnumerator LateShow()
        {
            yield return new WaitForSeconds(CommonDefineManager.Instance.CommonDefine.DropItem_IntervalTime);
            Play();
        }

        private void Play()
        {
            for (int i = 0; i < m_AnimationList.Count; i++)
            {
                if (m_AnimationList[i] == null)
                {
                    m_AnimationList.RemoveAt(i);
                }
            }
            if (m_AnimationList.Count > 0)
            {
                m_AnimationList[0].Begin();
                m_AnimationList.RemoveAt(0);
                IsPlay = true;
                BattleManager.Instance.StartCoroutine(LateShow());
            }
            else
            {
                IsPlay = false;
            }
        }


    }

}
