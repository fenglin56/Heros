// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;

public class SelectionIndicator : MonoBehaviour 
{

	public Vector3 offset = Vector3.zero; // how it should be offset from position it is placed at

	void Start()
	{
		Hide();
	}

	/// <summary>Hides the selector, also unparent from any transform it might have been following if set</summary>
	public void Hide()
	{
		this.transform.parent = null;
#if	UNITY_3_0_0 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		this.gameObject.SetActiveRecursively(false);
#else
		this.gameObject.SetActive(false);
#endif		
		
	}

	/// <summary>Hides the selector. Only unlink with transform it is following if unlink is set to true</summary>
	public void Hide(bool unlink)
	{
		if (unlink) this.transform.parent = null;
#if	UNITY_3_0_0 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		this.gameObject.SetActiveRecursively(false);
#else
		this.gameObject.SetActive(false);
#endif		
		
	}


	/// <summary>Show it at given pos (offset, set in properties, is applied)</summary>
	public void Show(Vector3 pos)
	{
		this.transform.position = pos + offset;
#if	UNITY_3_0_0 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		this.gameObject.SetActiveRecursively(true);
#else
		this.gameObject.SetActive(true);
#endif			
	}

	/// <summary>Show it at the location of the transform it will follow around (offset, set in properties, is applied)</summary>
	public void Show(Transform follow)
	{
		this.transform.parent = follow;
		this.transform.localPosition = offset;
#if	UNITY_3_0_0 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		this.gameObject.SetActiveRecursively(true);
#else
		this.gameObject.SetActive(true);
#endif			
		
	}

	// ====================================================================================================================
}
