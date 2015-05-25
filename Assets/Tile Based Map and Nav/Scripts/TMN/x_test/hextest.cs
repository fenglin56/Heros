using UnityEngine;
using System.Collections;

public class hextest : MonoBehaviour 
{
	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		DrawHex(Vector3.zero);
		DrawHex(new Vector3(1f, 0f, 0f));

	}

	private void DrawHex(Vector3 pos)
	{
		//float d = 1f;			// 2f/2f;
		float r = 0.5f;			// d/2f;
		float r2 = 0.25f;		// r * 0.5f;
		float a = 0.8660254f;	// Mathf.Sqrt(3f)/2f;
		float a2 = a * 0.5f;

		Gizmos.DrawLine(pos, pos + new Vector3(0f, 0f, +r));
		Gizmos.DrawLine(pos, pos + new Vector3(0f, 0f, -r));

		Gizmos.DrawLine(pos, pos + new Vector3(a2, 0f, +r2));
		Gizmos.DrawLine(pos, pos + new Vector3(a2, 0f, -r2));

		Gizmos.DrawLine(pos, pos + new Vector3(-a2, 0f, +r2));
		Gizmos.DrawLine(pos, pos + new Vector3(-a2, 0f, -r2));
	}
}
