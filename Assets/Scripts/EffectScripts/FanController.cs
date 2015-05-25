using UnityEngine;
using System.Collections;

public class FanController : MonoBehaviour {
	
	public Transform m_fan;
	public Transform m_transLine1;
	public Transform m_transLine2;
	
	[SerializeField (), Range (0f, 180f)]
    public float m_Angel = 60f;
	
	private Material m_mat;
	
	// Use this for initialization
	
	
	void Start () {
		m_mat = m_fan.GetComponent<MeshRenderer>().material;
		m_mat.SetFloat("_Angel", m_Angel);
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_mat.SetFloat("_Angel", m_Angel);
		m_transLine1.localEulerAngles = new Vector3(0, (m_Angel/2), 0);
		m_transLine2.localEulerAngles = new Vector3(0, (-m_Angel/2), 0);
	}
}
