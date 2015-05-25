using UnityEngine;
using System.Collections;

public class PlayerFight : View
{
    private float m_speed;
    private float m_accel;
    private int m_angle;
    private float m_attackedTime;
    private bool m_beAttacked;
    private float m_time;
    private Vector3 m_attackDire;
    void Awake()
    {
        RegisterEventHandler();
    }
	// Use this for initialization
	void Start () {
        ResetAttackParameter();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_beAttacked)
        {
            var t = Time.deltaTime;
            var s = m_speed * t + m_accel * Mathf.Pow(t, 2) * 0.5f;

            m_time += t;

            m_speed += m_accel * t;

            animation.CrossFade("dead");
            transform.Translate(m_attackDire * s);
            
            if (m_time>=m_attackedTime || m_speed <= 0)
            {
                ResetAttackParameter();
            }
        }
	}
    void OnGUI()
    {
        if (GUILayout.Button("被击飞",GUILayout.Width(100)))
        {
            m_beAttacked = true;

            m_attackDire = Quaternion.Euler(0, m_angle, 0) * Vector3.right;

            transform.LookAt(transform.TransformDirection(m_attackDire)*-1);
        }
    }
    protected override void RegisterEventHandler()
    {
    }
    void ResetAttackParameter()
    {
        m_speed = 50f;
        m_accel = -50f;
        m_angle = 60;
        m_time = 0;
        m_attackedTime = 3f;
        m_beAttacked = false;
    }
}
