using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AssetBundlesDatabase : ScriptableObject
{
	[System.Serializable]
	public class BundleData
	{
		public string _name;
		public string _filename;
		public int _dataLevel;
		
		public byte[] _hash;
		public int _size;
		public string[] _sceneNames;
		public List<string> _assetsIds;
		
		public string _cacheName;
		
		public int DataLevel
		{
			get { return _dataLevel; }
			set { _dataLevel = value; }
		}
		
		
		public List<string> AssetsIds {
			get { return _assetsIds; }
			set { _assetsIds = value; }
		}

		public string[] SceneNames {
			get { return _sceneNames; }
			set { _sceneNames = value; }
		}

		public byte[] Hash {
			get { return _hash; }
			set { _hash = value; }
		}

		public int Size {
			get { return _size; }
			set { _size = value; }
		}

		public string Name {
			get { return _name; }
			set { _name = value; }
		}
	
		public string CacheName {
			get { return _cacheName; }
			set { _cacheName = value; }
		}

		public string Filename {
			get { return _filename; }
			set { _filename = value; }
		}

		public bool IsScene {
			get { 
				return _sceneNames != null && _sceneNames.Length > 0;
			}
		}		
	}
	
	public string _id;
	
	public string Id {
		get {
			return _id;
		}
		set {
			_id = value;
		}
	}

	public byte[] _hash;
	
	public byte[] Hash {
		get {
			return _hash;
		}
		set {
			_hash = value;
		}
	}
	
	public int _size;
	
	public int Size {
		get {
			return _size;
		}
		set {
			_size = value;
		}
	}


	public List<BundleData> _bundles;

	public List<BundleData> Bundles {
		get {
			if (_bundles == null)
				_bundles = new List<BundleData>();
			
			return _bundles;
		}
		set
		{
			_bundles = value;	
		}
	}
	
	private Dictionary<string, BundleData> _bundlesMap;
	
	public Dictionary<string, BundleData> BundlesMap
	{
		get {
			if (_bundlesMap == null)
			{
				_bundlesMap = new Dictionary<string, BundleData>();
				foreach(var bundle in Bundles)
				{
					_bundlesMap.Add(bundle.Name, bundle);
				}
			}
			
			return _bundlesMap;
		}
	}
	
	public BundleData this[string name] 
	{
		get {
			
			BundleData ret;
			if (BundlesMap.TryGetValue(name, out ret))
				return ret;
			
			return null;
		}
	}
}


