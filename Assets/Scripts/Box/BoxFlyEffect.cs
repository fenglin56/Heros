using UnityEngine;
using System.Collections;

public class BoxFlyEffect : MonoBehaviour {

	private EntityModel m_target;
	private Transform m_thisTransfrom;
	private float m_fly_speed;
	private float m_moneyDropRadius;
	private bool m_isFly = false;

	void Update () 
	{
		if(m_isFly)
		{
			Fly();
		}
	}

	public void Init(EntityModel target, float speed, float dropRadius)
	{
		m_target = target;
		m_fly_speed = speed;
		m_moneyDropRadius = dropRadius;

		m_thisTransfrom = this.transform;

		m_isFly = true;
	}

	private void Fly()
	{
		if(m_target==null)
			return;
		Vector3 targetPos = m_target.GO.transform.position;
		//targetPos.y = 0;
		Vector3 myPos = m_thisTransfrom.position;
		//myPos.y = 0;
		Vector3 v = targetPos - myPos;
		Vector3 move = (v.normalized * m_fly_speed * Time.deltaTime);
		m_thisTransfrom.position += new Vector3(move.x,0,move.z);

		Vector3 compareMyPos = new Vector3(m_thisTransfrom.position.x,0,m_thisTransfrom.position.z);
		Vector3 compareTargetPos = new Vector3(targetPos.x,0,targetPos.z);
		if (Vector3.Distance(compareMyPos, compareTargetPos) <= m_moneyDropRadius)
		{
			GameObject pickEff = UI.CreatObjectToNGUI.InstantiateObj(GameManager.Instance.DamageFactory.Eff_AutomaticPick_BePick_Prefab,m_target.GO.transform);
			pickEff.AddComponent<DestroySelf>();

			m_isFly = false;
			Destroy(gameObject);
		}
	}
}
