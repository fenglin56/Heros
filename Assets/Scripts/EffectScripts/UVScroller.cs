using System;
using UnityEngine;
public class UVScroller:MonoBehaviour
{
    public float scrollSpeedX = 0.1f;
	public float scrollSpeedY = 0.1f;

    void Update()
    {
        float offsetX = Time.time * scrollSpeedX;
        float offsetY =Time.time * scrollSpeedY;
        renderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
    }
}