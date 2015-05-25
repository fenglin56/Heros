using UnityEngine;
using System.Collections;

public class MiniMapUIFlag : MonoBehaviour {

    float BlinkTime = 1;
    Vector3 FromScal = Vector3.zero;
    Vector3 ToScal = Vector3.zero;
    float Fromalpha = 1;
    float Toalpha = 0;

    public static void Begin(GameObject gameobject,Vector3 fromScal,Vector3 ToScal)
    {
        if (gameobject.GetComponent<MiniMapUIFlag>() == null)
        {
            gameobject.AddComponent<MiniMapUIFlag>();
        }
        MiniMapUIFlag miniMapUIFlag = gameobject.GetComponent<MiniMapUIFlag>();
        miniMapUIFlag.FromScal = fromScal;
        miniMapUIFlag.ToScal = ToScal;
        miniMapUIFlag.BeginBlik();
    }

    public static void StopBlik(GameObject gameobject)
    {
        MiniMapUIFlag miniMapUIFlag = gameobject.GetComponent<MiniMapUIFlag>();
        if (miniMapUIFlag != null)
        {
            miniMapUIFlag.StopBlink();
        }
    }

    public void BeginBlik()
    {
        StartCoroutine(BeginBlinkThread());
    }

    IEnumerator BeginBlinkThread()
    {
        Beginalpha(BlinkTime, 1, 0);
        BeginScal(BlinkTime,FromScal,ToScal);
        yield return new WaitForSeconds(BlinkTime);
        StartCoroutine(BeginBlinkThread());
    }

    public void StopBlink()
    {
        StopAllCoroutines();
    }

    void Beginalpha( float duration,float Fromalpha ,float Toalpha)
    {
        TweenAlpha comp = UITweener.Begin<TweenAlpha>(gameObject, duration);
        comp.from = Fromalpha;
        comp.to = Toalpha;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
    }

    void BeginScal(float duration,Vector3 Fromscale, Vector3 Toscale)
    {
        TweenScale comp = UITweener.Begin<TweenScale>(gameObject, duration);
        comp.from = Fromscale;
        comp.to = Toscale;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
    }

}
