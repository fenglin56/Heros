using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class SoundClip
{
	public string _name;
	public AudioClip _clip;
	public AudioClip Clip {
		get {
			return _clip;
		}
	}
	public int _rate = 100;
	public int _priority = 128;
	public int _repeate = 0;
}
	

