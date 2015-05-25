using UnityEngine;
using System.Collections;

public class Gamelauncher : MonoBehaviour {
	
	public string _nextSceneToLoad;

	// Use this for initialization
	void Start () 
	{
		Application.LoadLevelAsync(_nextSceneToLoad);
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
