using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// EnterPoint Scene GameManager
/// </summary>
using UI;


public class DamageFactory : MonoBehaviour {

    private bool m_createDamage;  //是否立即创建可破坏物
    private List<IEntityDataStruct> m_preCreateDamageStructCache;

    private Dictionary<string, GameObject> m_goCache;
    public GameObject[] DamageObj;

    public GameObject DiceObj;
    public GameObject EquipNamePrefab;
    public GameObject EquipNameShadowPrefab;
    public GameObject[] EffDrops;
    public GameObject[] PickupEffs;

    public GameObject SirenStoneEffPrefab;

	public GameObject Eff_AutomaticPick_Prefab;
	public GameObject Eff_AutomaticPick_Start_Prefab;
	public GameObject Eff_AutomaticPick_Fly_Prefab;
	public GameObject Eff_AutomaticPick_BePick_Prefab;
//    private Color[] NameFontColors = new Color[4]
//        {
//            Color.green,
//            Color.blue,
//            new Color(0.627f,0.125f,0.941f),
//            new Color(1f,0.84f,0)
//        };
	private TextColor[] m_TextColors =new TextColor[4]{
		UI.TextColor.EquipmentGreen,
		UI.TextColor.EquipmentBlue,
		UI.TextColor.EquipmentMagenta,
		UI.TextColor.EquipmentYellow
	};

    void Awake()
    {
        m_createDamage = false;
        m_preCreateDamageStructCache = new List<IEntityDataStruct>();

        m_goCache = new Dictionary<string, GameObject>();
        if (DamageObj != null && DamageObj.Length > 0)
        {
            for (int i = 0; i < DamageObj.Length; ++i)
            {
                string key = DamageObj[i].name;
                if (!m_goCache.ContainsKey(key))
                {
                    m_goCache.Add(key, DamageObj[i]);
                }
                else
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"有重复名称，请检查！");
                }
            }
        }
    }

    public GameObject FindByName(string prefabsName)
    {
        if (m_goCache.ContainsKey(prefabsName))
        {
            return m_goCache[prefabsName];
        }

        //TraceUtil.Log("未能找到名为" + prefabsName + "的物件");
        return null;
    }

    public void Register(IEntityDataStruct entityDataStruct)
    {
        if (!GameManager.Instance.CreateEntityIM)
        {
            m_preCreateDamageStructCache.Add(entityDataStruct);
        }
        else
        {
            CreateDamage(entityDataStruct);
        }
    }
    /// <summary>
    /// 当发生实体删除时，需要检查缓存里是否有未创建的实体数据，一并删除
    /// </summary>
    /// <param name="uid"></param>
    public void UnRegister(long uid)
    {
        m_preCreateDamageStructCache.RemoveAll(P => P.SMsg_Header.uidEntity == uid);
    }
    public void RegisterBox(IEntityDataStruct entityDataStruct)
    {
        if (!GameManager.Instance.CreateEntityIM)
        {
            m_preCreateDamageStructCache.Add(entityDataStruct);
        }
        else
        {
            CreateDamage(entityDataStruct);
        }        
    }
   
    public void CreateDamageGameObject()
    {
        this.m_createDamage = true;
        foreach(var dataStruct in this.m_preCreateDamageStructCache)
        {
            CreateDamage(dataStruct);
        }
        this.m_preCreateDamageStructCache.Clear();
    }

    public void PlayDiceAnimation()
    {
        var players = PlayerManager.Instance.PlayerList;
        players.ApplyAllItem(p =>
            {
                GameObject diceObj = (GameObject)Instantiate(DiceObj, p.GO.transform.position + new Vector3(0, 20f, 0), Quaternion.identity);
                diceObj.transform.parent = p.GO.transform;
            });
        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Random");
    }

    public void CreateSirenStone(Transform sirenTrans)
    {
        GameObject stone = (GameObject)Instantiate(SirenStoneEffPrefab, sirenTrans.position , Quaternion.identity);
		stone.transform.parent = sirenTrans;
    }

    //public void PlayGetGoodAnimation(Transform heroTrans, int goodID)
    //{
    //    var itemData = ItemDataManager.Instance.GetItemData(goodID);
    //    if (itemData != null)
    //    {
    //        Vector3 pos = heroTrans.position + new Vector3(0, 20f, 0);
    //        GameObject item = (GameObject)Instantiate(itemData._picPrefab, pos, Quaternion.identity);
    //        item.transform.parent = heroTrans.transform;
    //    }
    //}


    //计算掉落位置
    private Vector3 RandomPos(Vector3 pos)
    {
        int a = CommonDefineManager.Instance.CommonDefine.DropItem_Num_A / 10;
        int b = CommonDefineManager.Instance.CommonDefine.DropItem_Num_B / 10;
        Vector3 randomPos = new Vector3();
        randomPos.x = pos.x + (int)(Random.Range(-a, a) / b + 1) * b;
        randomPos.z = pos.z + (int)(Random.Range(-a, a) / b + 1) * b;
        randomPos.y = pos.y;
        //TraceUtil.Log("[randomPos]" + randomPos);
        return randomPos;
    }

    private Vector3 CountDropPos(Vector3 pos)
    {
        
        int time = 0;
        bool isAccord = false;
        Vector3 countPos = pos;
        float distance = 5f;
        while (false == isAccord)
        {
            time++;
            if (time > 20)
            {
                isAccord = true;
            }
            Vector3 randomPos = this.RandomPos(pos);
            if (false == SceneDataManager.Instance.IsPositionInBlock(randomPos))
            {
                if (false == DamageManager.Instance.GetDamageList().Any(p => p.GO != null && Vector3.Distance(p.GO.transform.position, randomPos) <= distance))
                {
                    countPos = randomPos;
                    isAccord = true;
                }
            }
        }
        //TraceUtil.Log("[CountDropPos] Initial: " + pos + "  , After: " + countPos);
        return countPos;
    }


    private void CreateDamage(IEntityDataStruct entityDataStruct)
    {
        var sMsgPropCreateEntity_SC_Damage = (BoxSubMsgEntity)entityDataStruct;
        //string damageName = "xiangzi";
        //var damagePrefab = this.FindByName(damageName);
        
        //\Edit by lee
        var damageList = EctypeConfigManager.Instance.DamageConfigList;
        if (!damageList.ContainsKey(sMsgPropCreateEntity_SC_Damage.BaseValues.OBJECT_FIELD_ENTRY_ID))
        {            
            return;
        }
        var damageData = damageList[sMsgPropCreateEntity_SC_Damage.BaseValues.OBJECT_FIELD_ENTRY_ID];
        var damagePrefab = damageData.DamagePrefab;


        var pos = Vector3.zero;
        var serverPos = pos.GetFromServer(sMsgPropCreateEntity_SC_Damage.BoxX, sMsgPropCreateEntity_SC_Damage.BoxY);
        pos = CountDropPos(serverPos);

        var damage = (GameObject)GameObject.Instantiate(damagePrefab, pos, damagePrefab.transform.rotation);
        var damageBehaviour = damage.GetComponent<DamageBehaviour>();

        GameObject m_popupWidget = null;//字体

        var itemData = ItemDataManager.Instance.GetItemData(damageData._correspondingItemID);
        if (itemData != null)
        {
            //武器特效
            var effDrops = GameManager.Instance.DamageFactory.EffDrops;
            if (itemData._ColorLevel < effDrops.Length)
            {
                GameObject effect = (GameObject)Instantiate(effDrops[itemData._ColorLevel]);
                effect.transform.parent = damage.transform;
                effect.transform.localPosition = new Vector3(0, 1f, 0);
            }

            //字体
            Vector3 uiPos = PopupTextController.GetPopupPos(damage.transform.position, PopupObjManager.Instance.UICamera);
            m_popupWidget = GameObjectPool.Instance.AcquireLocal(EquipNamePrefab, uiPos, Quaternion.identity);
            GameObject textShadow = GameObjectPool.Instance.AcquireLocal(EquipNameShadowPrefab, uiPos, Quaternion.identity);            
            EquipTextBehaviour etBehaviour = m_popupWidget.GetComponent<EquipTextBehaviour>();
            etBehaviour.Init(damage.transform);
            var scale = m_popupWidget.transform.localScale;
            var shadowScale = textShadow.transform.localScale;
            m_popupWidget.transform.parent = PopupObjManager.Instance.UICamera.transform;
            textShadow.transform.parent = m_popupWidget.transform;
            var uilabel = m_popupWidget.GetComponentInChildren<UILabel>();
			uilabel.text = NGUIColor.SetTxtColor(LanguageTextManager.GetString(itemData._szGoodsName),m_TextColors[itemData._ColorLevel]);
            //uilabel.color = NameFontColors[itemData._ColorLevel];
            m_popupWidget.transform.localScale = scale;
            textShadow.transform.localScale = shadowScale;    

			damageBehaviour.TitleRef = m_popupWidget;
        }

        //掉落音效
        switch (damageData._boxType)
        {
            case 2:
                if (itemData._GoodsClass == 1)
                {
                    if (itemData._ColorLevel >= 3)
                    {
                        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Drop_Equipment2");
                    }
                    else
                    {
                        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Drop_Equipment1");
                    }
                }
                else
                {
                    SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Drop_Item1");
                }
                break;
            case 4:
                SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Drop_Gold");
                break;
        }

        //掉落动画
        var dropBehaviour = damage.GetComponent<GoldBehaviour>();
        if (dropBehaviour != null)
        {
            Vector3 dropPos = serverPos + (pos - serverPos) * CommonDefineManager.Instance.CommonDefine.DropItem_Dis;
            dropBehaviour.Play(m_popupWidget, serverPos, dropPos);
        }

        if (damageBehaviour == null)
        {
            TraceUtil.Log(damageBehaviour.name);
        }
        damageBehaviour.DamageDataModel = sMsgPropCreateEntity_SC_Damage;

        EntityModel damageDataModel = new EntityModel();
        damageDataModel.GO = damage;
        damageDataModel.Behaviour = damageBehaviour;
        damageDataModel.EntityDataStruct = sMsgPropCreateEntity_SC_Damage;

        DamageManager.GetInstance();
        EntityController.Instance.RegisteEntity(sMsgPropCreateEntity_SC_Damage.SMsg_Header.uidEntity, damageDataModel);
                       

    }
}
