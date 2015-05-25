using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;

//		About Rules:  ** TODO: Update the documentation 
//		- Pattern is a .net regular expression: http://msdn.microsoft.com/en-us/library/az24scfc.aspx
//		- Every asset path will be matched against the rules, the first match will be used
//		- Only assets to be included in a normal build will be processed (ie. it must be referenced from any scene)
//		- If more than one asset uses the same BundleName both all of then will be put in the same bundle
//		- Regex substitutions are allowed in the BundleName
//		- All the paths will be lowercase and using normal (/) slash
//		- BundleOrder defines the order to pack the bundles, the bundles only shares resources with previously packed bundles
//		- Every asset not matched by any rule will be directly added to the scene it is referenced
//		- Scenes are automatically bundled in the end and don't share their own resources with the rest of the bundles
//		- Check the editor log for detailed information about which files are added to each bundle

public class AssetBundlesConfig : ScriptableObject
{
    [MenuItem("Tools/Utils/Create Asset Bundles Config")]
    static void CreateItemDataList()
    {
		EditorUtils.CreateScriptable<AssetBundlesConfig>();
    }	
	
	public enum RuleType
	{
		Include,
		Exclude
	}
	
	[Serializable]
	public class RegexOptionsWrapper
	{
		public bool _ignoreCase = true;
		public bool _rightToLeft;
		public bool _ignoreWhitespace;
		
		public static implicit operator RegexOptions(RegexOptionsWrapper wrapper)
		{
			RegexOptions options = RegexOptions.None;
			
			if (wrapper._ignoreCase)
				options |= RegexOptions.IgnoreCase;
			
			if (wrapper._ignoreWhitespace)
				options |= RegexOptions.IgnorePatternWhitespace;
			
			if (wrapper._rightToLeft)
				options |= RegexOptions.RightToLeft; 
			    
			return options;
		}
	}
	
	public enum RegexReplaceModifier
	{
		None = 0,	// someExample
		Lowercase,  // someexample
		Uppercase,  // SOMEEXAMPLE
	}
	
	[Serializable]
	public class Rule
	{
		public string _name;
		public bool _enabled = true;
		public RuleType _type;
		public string _pattern;
		public RegexOptionsWrapper _patternOptions;
		public string _bundleName;
		public RegexReplaceModifier _patternReplaceModifier;
		public int _bundleOrder;
		public bool _isolate;
		
		private Regex _regex;
	
		public string Name {
			get { return _name; }
		}

		public bool Enabled {
			get { return _enabled; }
		}

		public RuleType Type {
			get {
				return _type;
			}
		}

		public string BundleName {
			get { return _bundleName; }
		}

		public int BundleOrder {
			get { return _bundleOrder; }
		}

		public string Pattern {
			get { return _pattern; }
		}

		public bool Isolate {
			get {
				return _isolate;
			}
		}

		public Regex Regex {
			get {
				if (_regex == null)
					_regex = new Regex(_pattern, RegexOptions.Compiled | _patternOptions);
				
				return _regex;
			}
		}
		
		public string GetReplacedBundleName(string assetPath)
		{
			switch (_patternReplaceModifier)
			{
			case RegexReplaceModifier.Lowercase:
				return Regex.Replace(assetPath.ToLower(), BundleName);
				
			case RegexReplaceModifier.Uppercase:
				return Regex.Replace(assetPath.ToUpper(), BundleName);
				
			default:
				return Regex.Replace(assetPath, BundleName);
			}
		}
	}
	
	public List<Rule> _rules;
		
	public IEnumerable<Rule> Rules {
		get { return _rules.Where(r => r.Enabled); }
	}

	public Rule MatchRules(string input)
	{
		foreach(var rule in Rules)
		{
			if (rule.Regex.IsMatch(input))
				return rule;
		}
		
		return null;
	}
}