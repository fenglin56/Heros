using UnityEngine;
using System.Collections;

public class EFPlay : MonoBehaviour {
	
	public float waitTime = 2.0f;
	public GameObject EFPrefab;
	// Use this for initialization
	void Start () 
	{
		StartCoroutine(WaitAndPrint(waitTime)); 		
	}
	
	
	
	IEnumerator WaitAndPrint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
		createPrefab();
		EFPrefab.transform.parent = gameObject.transform;
		EFPrefab.transform.localPosition = new Vector3(0f,0f,0f);
    }
	
	private void createPrefab()
	{
		EFPrefab = (GameObject)GameObject.Instantiate(EFPrefab);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

