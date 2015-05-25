using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 寻路路点管理，负责管理服务器返回的路点，包括主角或AI（统一用ID识别）
/// </summary>
public class NavPathManager
{
    /// <summary>
    /// 路点容器，按角色ID保存KeyValue对。
    /// </summary>
    private Dictionary<int, Queue<Vector3>> m_navPathPoint = new Dictionary<int, Queue<Vector3>>();
    private static NavPathManager m_instance;

    public static NavPathManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new NavPathManager();
            }
            return m_instance;
        }
    }

    /// <summary>
    /// 添加角色的寻路路点
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="pathPoints">若干个寻路路点</param>
    public void AddNavPathPoint(int roleId, params Vector3[] pathPoints)
    {
        if(pathPoints.Length>0)
        {
            //TraceUtil.Log("Add Point"+pathPoints.Length);
        }
        if(!m_navPathPoint.ContainsKey(roleId))
        {
            m_navPathPoint.Add(roleId,new Queue<Vector3>());
        }
        var pointContainer = m_navPathPoint[roleId];
        for(int i=0;i<pathPoints.Length;i++)
        {
            pointContainer.Enqueue(pathPoints[i]);
        }        
    }
    /// <summary>
    /// 根据角色ID获得角色的下一个寻路路点.返回zero则表示没有路点可循
    /// </summary>
    /// <param name="roleId"></param>
    public Vector3? GetNextPathPoint(int roleId)
    {
        if (m_navPathPoint.ContainsKey(roleId)
            && m_navPathPoint[roleId].Count > 0)
        {
            return m_navPathPoint[roleId].Dequeue();
        }
        else
        {
            //TraceUtil.Log("No Point");
            return null;
        }

    }
}
