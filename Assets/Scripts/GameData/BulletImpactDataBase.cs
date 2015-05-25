using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class BulletImpactData
{
	public int m_id;
	public int m_damage_type;
	public int m_beatBackLevel;
	public int m_beatBackDir;
	public float m_beatBackDuration;
	public float m_beatBackSpeed;
	public float m_beatBackAcceleration;
	public float m_beatFlyLevel;
	public float m_beatFlySpeed;
	public float m_beatFlyAcceleration;
	public float m_beatFlyVerticalSpeed;
    public float m_teleportLevel;
    public Vector3 m_teleportDestination;
    public Vector3 m_teleportArea;
    public float m_teleportAngle;

	public int[] m_affect_src;
	public int[] m_affect_prop;
}


public class BulletImpactDataBase : ScriptableObject 
{
	public BulletImpactData[] _dataTable;
}
