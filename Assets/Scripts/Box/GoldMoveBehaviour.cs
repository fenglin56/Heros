using UnityEngine;
using System.Collections;

public class GoldMoveBehaviour : MonoBehaviour 
{
    public float mTime;
    public Vector3 Form;
    public Vector3 To;

    private Transform thisTransform;

    public void Begin(Vector3 form, Vector3 to, float time)
    {
        thisTransform = this.transform;
        this.Form = form;
        this.To = to;
        this.mTime = time;
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        float i = 0;
        float rate = 1.0f / mTime;
        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(Form, To, i);
            yield return null;
        }
    }

}
