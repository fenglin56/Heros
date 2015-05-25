using UnityEngine;
using System.Collections;

public class Delete : MonoBehaviour
{
    public float totaltime = 6f;
    public float detachTime = 0;
    public bool detach = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        totaltime -= Time.deltaTime;
        if (totaltime < 0)
        {
            DestroyObject(gameObject);
        }

        if (detach)
        {
            detachTime -= Time.deltaTime;
            if (detachTime < 0)
            {
                transform.parent = null;
            }
        }

    }
}
