using UnityEngine;

public class BuildInfo : ScriptableObject
{
	public string _appVersion;
	public string _buildNumber;
	public string _buildDate;
	
	public bool _isBundleBased;
	public bool _bundlesIncluded;
	public bool _bundlesCacheIncluded;

	public string _companyName;
	public string _productName;
	
	public string _buildSubtarget;
	
	public bool _isStageBuild;
	
	public bool IsStageBuild{
		get{
			return _isStageBuild;	
		}
		set{
			_isStageBuild = value;	
		}
	}

	public bool IsBundleBased {
		get {
			return _isBundleBased;
		}
		set {
			_isBundleBased = value;
		}
	}

	public bool BundlesCacheIncluded {
		get {
			return _bundlesCacheIncluded;
		}
		set {
			_bundlesCacheIncluded = value;
		}
	}

	public bool BundlesIncluded {
		get {
			return _bundlesIncluded;
		}
		set {
			_bundlesIncluded = value;
		}
	}

	public string BuildDate {
		get {
			return _buildDate;
		}
		set {
			_buildDate = value;
		}
	}

	public string BuildNumber {
		get {
			return _buildNumber;
		}
		set {
			_buildNumber = value;
		}
	}

	public string AppVersion {
		get {
			return _appVersion;
		}
		set {
			_appVersion = value;
		}
	}

	public string CompanyName {
		get {
			return _companyName;
		}
		set {
			_companyName = value;
		}
	}

	public string ProductName {
		get {
			return _productName;
		}
		set {
			_productName = value;
		}
	}

	public string BuildSubtarget {
		get {
			return _buildSubtarget;
		}
		set {
			_buildSubtarget = value;
		}
	}
}
