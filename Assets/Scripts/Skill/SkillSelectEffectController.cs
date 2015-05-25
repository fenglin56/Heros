using UnityEngine;
using System.Collections;

public class SkillSelectEffectController : MonoBehaviour 
{
	public Transform m_arrow;
	public Transform m_circle;
	public Transform m_fan;
	
	private FanController m_fanController;
	// Use this for initialization
	void Start () {
		m_fanController = m_fan.GetComponent<FanController>();
		m_circle.gameObject.SetActive(false);
		m_arrow.gameObject.SetActive(false);
		m_fan.gameObject.SetActive(false);
	}
	
	public void HideAll()
	{
		StartCoroutine("HideEffects");
	}
	
	public void ShowArrow()
	{
		m_arrow.gameObject.SetActive(true);	
	}
	
	public void ShowCircle(int range, Transform ownerTrans)
	{
		m_circle.gameObject.SetActive(true);
		m_circle.localScale = new Vector3(range,range,range);
		m_circle.position = ownerTrans.TransformPoint(0.0f, 0.1f, 10.0f);
	}
	
	public void SetCirclePos(Vector3 worldPos)
	{
		m_circle.position = worldPos;	
	}
	
	public void ShowFan(int range, int angle)
	{
		m_fan.gameObject.SetActive(true);
		m_fanController.m_Angel = angle;
		m_fan.localScale = new Vector3(range, range, range);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator HideEffects()
	{
		m_circle.gameObject.SetActive(false);
		//yield return new WaitForSeconds(1.5f);
		m_arrow.gameObject.SetActive(false);
		m_fan.gameObject.SetActive(false);
		yield break;
	}
	
}
