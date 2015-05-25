using UnityEngine;
using System.Collections;

public class DestroySelf : MonoBehaviour {
    public float Time = 3f;
	// Use this for initialization
	void Start () {
        StartCoroutine(Clean());   
	}

    IEnumerator Clean()
    {
        yield return new WaitForSeconds(Time);
        Destroy(this.gameObject);
    }
	
}
