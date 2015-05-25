using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 实现思路（备忘及帮助理解）
/// 1、单例模式
/// 2、聚合TouchTime类，TouchTime储存触屏信息（触屏移动长度、开始时间、结束时间、双击间隔，是否触发点击。重置方法，判断点击次数方法）-- 双击也会触发单击
///     重置方法：把每个属性都置为float.NaN
///     判断点击次数方法：如果 移动长度为 float.NaN并且已经触发点击。
///                             如果双击间隔有值，并且间隔时间小于等于间隔配置时间，则触发双击。
///                             否则触发单击。
///                       否则不触发。
/// 3、InputUtil公开ClickAmount属性，使用者需要在Update方法里逐帧调用，根据获得的值判断用户是否点击（单击，双击）
///     在ClickAmount属性中，通过CatchTouchBegin()捕捉用户的Touch触屏，处理后更新TouchTime对象的值，然后调用TouchTime对象的“判断点击次数方法(GetClickAmount)”.返回点击检测结果。
///     CatchTouchBegin()方法实现：
///     获得用户Touches（忽略两个触点），取第一个触点位置。判断触点阶段(phase)
///         Began  更新TouchTime类的开始时间，长度为Nan，记下点击位置
///         Ended   更新TouchTime类的结束时间，如果发生移动，并且移动距离大开移动配置距离，则重置TouchTime。
///                                             否则 重置移动长度，接下来判断是否有双击间隔，如果上一次的EndTime为NaN，则双击间隔为NaN。否则双击间隔为这次结束-上次结束
///                                                     标TouchTime的是否触发标记为真，把本次结束时间设为TouchTime的结束时间
///         Moved   计算亲更新TouchTime的移动长度
///         Canceled    重置移动长度，并以最后点为点击点。并标TouchTime的是否触发标记为真
///                                      
/// </summary>
public class InputUtil
{
	private Vector2? TouchStartPosition = null;
	private float? touchMoveDistance;
	public static InputUtil m_instance;
	public static InputUtil Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = new InputUtil();
			}
			return m_instance;
		}
	}
	public TouchTime TouchTime=new TouchTime();
	public InputUtil()
	{
		this.TouchTime.Init();
	}
	public Vector2 TouchPosition;
	//获取的时候会去检测点击
	public byte ClickAmount
	{
		get
		{
			CatchTouchBegin();
			byte amount = TouchTime.GetClickAmount();
			return amount;
		}
	}
	
	public string ShowTouchTime()
	{
		return TouchTime.ToString();
	}
	private void CatchTouchBegin()
	{
		var touchs = Input.touches;
		switch (touchs.Length)
		{
		case 1:
		case 2:
			var touchPoint = touchs[0].position;
			switch (touchs[0].phase)
			{
			case TouchPhase.Began:
				this.TouchTime.BeganTime = Time.realtimeSinceStartup;
				this.TouchTime.MoveTackLength=float.NaN;
				TouchStartPosition = touchPoint;
				touchMoveDistance = null;
				break;
			case TouchPhase.Ended:
				var touchEndTime = Time.realtimeSinceStartup;
				//if (this.TouchTime.MoveTackLength > InputUtilDefineManager.SlideTrackLength)
				//{
				//    this.TouchTime.Init();
				//}
				//else
				//{
				this.TouchTime.MoveTackLength = float.NaN;
				if (float.IsNaN(this.TouchTime.EndTime))
				{
					this.TouchTime.DoubleClickInternal = float.NaN;
				}
				else
				{
					this.TouchTime.DoubleClickInternal = touchEndTime - this.TouchTime.EndTime;
				}
				this.TouchTime.IsClickInvoke = true;
				if(touchMoveDistance.HasValue&&touchMoveDistance.Value > 15)
				{
					this.TouchTime.IsClickInvoke = false;
				}
				this.TouchTime.EndTime = touchEndTime;
				TouchPosition = touchPoint;
				TouchStartPosition = null;
				//}
				break;
			case TouchPhase.Moved:
				if (!TouchStartPosition.HasValue)
				{
					TouchStartPosition = touchPoint;
				}
				this.TouchTime.MoveTackLength = Vector2.Distance(touchPoint, TouchStartPosition.Value);
				if(!touchMoveDistance.HasValue || (touchMoveDistance.HasValue && touchMoveDistance.Value < this.TouchTime.MoveTackLength))
				{
					touchMoveDistance = this.TouchTime.MoveTackLength;
				}
				break;
			case TouchPhase.Canceled:
				//this.TouchTime.MoveTackLength = float.NaN;
				//this.TouchTime.IsClickInvoke = true;
				//this.TouchTime.EndTime = Time.realtimeSinceStartup;
				//TouchPosition = touchPoint;
				//TouchStartPosition = null;
				touchMoveDistance = null;
				TouchStartPosition = null;
				this.TouchTime.Init();
				break;
			}
			break;
		default:
			break;
			
		}
	}
	
}
public struct TouchTime
{
	private float m_moveTackLength;
	public float BeganTime;
	public float EndTime;
	public float DoubleClickInternal;
	public float MoveTackLength
	{
		get
		{
			return m_moveTackLength;
		}
		set
		{
			this.m_moveTackLength = value;
		}
	}
	public bool IsClickInvoke;
	public void Init()
	{
		this.BeganTime = float.NaN;
		this.EndTime = float.NaN;
		this.DoubleClickInternal = float.NaN;
		this.m_moveTackLength = float.NaN;
	}
	public byte GetClickAmount()
	{
		if (float.IsNaN(this.m_moveTackLength)
		    && IsClickInvoke)
		{
			IsClickInvoke = false;
			if (!float.IsNaN(DoubleClickInternal) && DoubleClickInternal <= InputUtilDefineManager.DoubleInternal)
			{
				Init();
				return 2;
				
			}
			else
			{
				return 1;
			}
		}
		
		return 0;
	}
	public override string ToString()
	{
		string content = string.Format("Began:{0},End:{1},Internal:{2},DoubleInternal:{3},MoveLength:{4}", BeganTime, EndTime, EndTime - BeganTime, DoubleClickInternal, MoveTackLength);
		
		return content;
	}
}