using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class LateGameObject: Late<GameObject> {}
[Serializable] public class LateTexture2D: Late<Texture2D> {}
[Serializable] public class LateAnimationClip: Late<AnimationClip> {}

public class Late<T> where T: UnityEngine.Object
{
	[Late("_id")]
	public T _target;
	
	//[HideInInspector]
	public string _id;
		
	public static implicit operator T(Late<T> val)
	{
		return val.Target;
	}
		
	public T Target
	{
		get 
		{
			return AssetId.Resolve(_target, _id);
		}
	}
}

public class LateList<T> : IList<T> where T : UnityEngine.Object
{
	private IList<T> _sourceList;
	private IList<string> _sourceIds;
	
	public LateList(IList<T> sourceList, IList<string> sourceIds)
	{
		_sourceList = sourceList;
		_sourceIds = sourceIds;
	}
	
	#region IList[T] implementation
	public int IndexOf(T item)
	{
		throw new NotSupportedException();
	}

	public void Insert(int index, T item)
	{
		throw new NotSupportedException();
	}

	public void RemoveAt(int index)
	{
		throw new NotSupportedException();
	}

	public T this[int index] {
		get {
			return _sourceList[index] != null ? _sourceList[index] : AssetId.FromId<T>(_sourceIds[index]);
		}
		set {
			throw new NotSupportedException();
		}
	}
	#endregion

	#region IEnumerable implementation
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	#endregion

	#region IEnumerable[T] implementation
	public class Enumerator : IEnumerator<T>
	{
		private LateList<T> _list;
		private int index = -1;
		
		public Enumerator(LateList<T> list)
		{
			_list = list;
		}
		
		#region IEnumerator[T] implementation
		public T Current {
			get {
				return _list[index];
			}
		}
		#endregion

		#region IEnumerator implementation
		public bool MoveNext ()
		{
			if (index > _list.Count) 
				return false; 
			
			return ++index < _list.Count;		
		}

		public void Reset ()
		{
			index = -1;
		}

		object IEnumerator.Current {
			get {
				return Current;
			}
		}
		#endregion

		#region IDisposable implementation
		public void Dispose () { }
		#endregion
	}
	
	public IEnumerator<T> GetEnumerator()
	{
		return new Enumerator(this);
	}
	#endregion

	#region ICollection[T] implementation
	public void Add(T item)
	{
		throw new NotSupportedException();
	}

	public void Clear()
	{
		throw new NotSupportedException();
	}

	public bool Contains(T item)
	{
		throw new NotSupportedException();
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		throw new NotSupportedException();
	}

	public bool Remove(T item)
	{
		throw new NotSupportedException();
	}
	
	public int Count {
		get {
			return _sourceList.Count;
		}
	}

	public bool IsReadOnly {
		get {
			return true;
		}
	}
	#endregion

	public int Length {
		get {
			return _sourceList.Count;
		}
	}
}