using UnityEngine;
using System.Collections;

public class MoveSmooth  {
	
	private Vector3 m_Node1;
	private Vector3 m_Node2;
	private Vector3 m_Node3;
	private Vector3 m_Node4;
	
	private Vector3 m_A;
	private Vector3 m_B;
	private Vector3 m_C;
	private Vector3 m_D;

    private Vector3 m_currentVector;
    private Vector3 m_packetVector;
	
	
	private float m_TotalTime;
	
	public bool m_inited = false;
	
	
	
	
	
	public MoveSmooth()
	{
		
	}
	
	public void Init(Vector3 currentPos, Vector3 currentSpeedVector, Vector3 packetCurrentPos, Vector3 packetSpeedVector, float time)
	{
		m_inited = true;
		m_TotalTime = time;
		m_Node1 = currentPos;
		m_Node2 = currentPos + currentSpeedVector*time/5.0f;
		m_Node4 = packetCurrentPos + packetSpeedVector*time/5.0f;
		m_Node3 = m_Node4 - packetSpeedVector*time/5.0f;
		
		m_A = m_Node4 - 3*m_Node3 + 3*m_Node2 - m_Node1;
		m_B = 3*m_Node3 - 6*m_Node2 + 3*m_Node1;
		m_C = 3*m_Node2 - 3*m_Node1;
		m_D = m_Node1;
        m_currentVector = currentSpeedVector;
        m_packetVector = packetSpeedVector;
	}
	
	public Vector3 GetCurrentPos(float timePercentSinceStart)
	{
		Vector3 pos = Vector3.zero;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
		
		float t = timePercentSinceStart;
		
		pos = m_A*t*t*t + m_B*t*t + m_C*t + m_D;
		if(pos == Vector3.zero)
		{
			TraceUtil.Log("zero vector");
		}
		
		return pos;
		
	}

    public Quaternion GetCurrentQuaternion(float timePercentSinceStart)
    {
        Quaternion currentQ = Quaternion.LookRotation(m_currentVector);
        Quaternion packetQ = Quaternion.LookRotation(m_packetVector);
        return Quaternion.Lerp(currentQ, packetQ, timePercentSinceStart);

    }
	
	
}
