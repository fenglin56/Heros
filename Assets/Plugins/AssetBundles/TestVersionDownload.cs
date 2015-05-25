using UnityEngine;
using System.Collections;

public class TestVersionDownload : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(DownLoadVersion());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator DownLoadVersion()
	{
		
		var www = new WWW("http://192.168.0.190/jianghu/test/version.txt");//("file://" + "D:/version.txt");
		yield return www;
		
		string text = www.text;
		Debug.Log(text);
		
	}
}
