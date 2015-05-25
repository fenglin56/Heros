using UnityEngine;

public class LateResource : ScriptableObject
{
	public Object _target;
	
	public Object Target {
		get {
			return _target;
		}
		set {
			_target = value;
		}
	}
}
