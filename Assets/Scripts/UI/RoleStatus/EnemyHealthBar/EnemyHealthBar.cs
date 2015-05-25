using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealthBar : MonoBehaviour {

    private bool m_isReady;
    private float m_MaxValue;
    private Transform m_TargetMP;
    private EntityModel m_EntityModel;
    private MonsterBehaviour m_enemyBehaviour;
    private bool m_hasShield=false;

    public UISlider HPSlider;
    public UISlider HPBackSlider;

    public UISlider ShieldSlider;    
    public UILabel HealthLabel;
    
	public GameObject Eff_BossHaveDefense;

	public SpriteSwith Switch_Camp;

    bool ShowHealthLabel = false;
    bool ShowShieldLabel = false;

	bool m_isBossHealth = false;

    void Awake()
    {
		ShowHealthLabel = GameManager.Instance.IsShowBloodLabel;
        m_hasShield = this.ShieldSlider != null;
		m_isBossHealth = Eff_BossHaveDefense != null;
    }
    public void InitMaxValue(Transform targetMP, int MaxBlood,EntityModel entityModel)
    {
        var scale = transform.localScale;
        transform.parent = PopupObjManager.Instance.UICamera.transform;
        transform.localScale = scale;

        this.m_TargetMP = targetMP;
        m_EntityModel = entityModel;
        m_enemyBehaviour = entityModel.Behaviour as MonsterBehaviour;
        m_enemyBehaviour.EnemyHealthBar = this;
        m_MaxValue=(float)MaxBlood;
        this.m_isReady = true;
		int camp = ((SMsgPropCreateEntity_SC_Monster)m_enemyBehaviour.RoleDataModel).MonsterUnitValues.UNIT_FIELD_FIGHT_HOSTILITY;
		Switch_Camp.ChangeSprite(camp);
    }   
    void Update()
    {
        if (this.m_isReady)
        {
            if (m_EntityModel != null)
            {
                var monsterData = ((SMsgPropCreateEntity_SC_Monster)m_EntityModel.EntityDataStruct).MonsterUnitValues;
                if (monsterData.UNIT_FIELD_CURHP <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    HPSlider.sliderValue = (float)monsterData.UNIT_FIELD_CURHP / m_MaxValue;
                    HPBackSlider.sliderValue = HPBackSlider.sliderValue - (HPBackSlider.sliderValue - HPSlider.sliderValue) * Time.deltaTime;
                    if (m_hasShield) EnemyShieldSlider(monsterData.UNIT_FIELD_SHARD);
                    string label = string.Empty;
                    if (ShowHealthLabel)
                    {
                        label +=monsterData.UNIT_FIELD_CURHP + "/" + m_MaxValue;
                    }
                    if (ShowShieldLabel)
                    {
                        if (ShowHealthLabel) label += "-";
                        label += monsterData.UNIT_FIELD_SHARD + "/" + m_enemyBehaviour.MonsterShieldValue;
                    }

					if(m_isBossHealth)
					{
						SetGameObjectActive(Eff_BossHaveDefense, monsterData.UNIT_FIELD_SHARD > 0);
					}


                    this.HealthLabel.SetText(label);
                }
            }

            transform.position = PopupTextController.GetPopupPos(this.m_TargetMP.position, PopupObjManager.Instance.UICamera)+Vector3.forward*0.5f;      
        }
        
    }

	private void SetGameObjectActive(GameObject obj, bool isActive)
	{
		if(obj.activeInHierarchy != isActive)
		{
			obj.SetActive(isActive);
		}
	}

    private void EnemyShieldSlider(float currentShield)
    {
        if (m_enemyBehaviour != null && m_enemyBehaviour.MonsterShieldValue>0)
        {
            ShieldSlider.sliderValue = currentShield/m_enemyBehaviour.MonsterShieldValue;
            if (ShieldSlider.sliderValue <= 0)
            {
                //破防动画
            }
        }
    }

}
