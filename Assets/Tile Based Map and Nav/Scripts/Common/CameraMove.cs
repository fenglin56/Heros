// ====================================================================================================================
// Simple movement of the camera around the scene
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;

public class CameraMove : MonoBehaviour
{
	public float speed = 10f;
	public Transform target;			// target to follow (cam is fixed to following this around till it is NULL)
	public bool followTarget = false;	// follow the target? (only if target is not NULL)
	public bool allowInput = true;		// the cam wont read keyinput if set to false
	public Transform camTr;
	public Vector2 min_xz;
	public Vector2 max_xz;
	private Transform tr;

	public delegate void CamMaunallyMoved();
	public CamMaunallyMoved OnCamManuallyMoved = null;

	private bool moved = false;// helper

	void Start()
	{
		tr = this.transform;
		if (target && followTarget) tr.position = target.position;
	}

	void Update()
	{
		if (Input.anyKey && allowInput)
		{
			moved = false;
			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) { moved = true; Translate(Vector3.forward * Time.deltaTime * speed); }
			if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) { moved = true; Translate(Vector3.back * Time.deltaTime * speed);}
			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) { moved = true; Translate(Vector3.left * Time.deltaTime * speed);}
			if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) { moved = true; Translate(Vector3.right * Time.deltaTime * speed); }

			if (OnCamManuallyMoved != null && moved)
			{
				Vector3 pos = tr.position;
				if (pos.x < min_xz.x) pos.x = min_xz.x;
				if (pos.x > max_xz.x) pos.x = max_xz.x;
				if (pos.z < min_xz.y) pos.z = min_xz.y;
				if (pos.z > max_xz.y) pos.z = max_xz.y;
				tr.position = pos;

				OnCamManuallyMoved(); // call callback
			}
		}
	}

	void LateUpdate()
	{
		if (target && followTarget)
		{
			Vector3 difference = target.position - tr.position;
			tr.position = Vector3.Slerp(tr.position, target.position, Time.deltaTime * Mathf.Clamp(difference.magnitude, 0f, 2f));
		}
	}

	private void Translate(Vector3 pos)
	{
		followTarget = false; // stop follow mode if manually moved

		// if SHIFT is held, move at double speed
		if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) pos *= 2.5f;

		// apply
		Vector3 r = camTr.eulerAngles;
		r.x = 0; tr.position += Quaternion.Euler(r) * pos;
	}

	public void Follow(bool doFollowCurrentTarget)
	{
		followTarget = doFollowCurrentTarget;
	}

	public void Follow(Transform t)
	{
		target = t;
		followTarget = true;
	}

	// ====================================================================================================================
}
