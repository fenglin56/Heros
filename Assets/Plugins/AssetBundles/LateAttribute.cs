using System; 
using System.Reflection;

[AttributeUsage(AttributeTargets.Field)]
public class LateAttribute : Attribute
{
	public string TargetIdField;
	
	public LateAttribute()
	{
	}

	public LateAttribute(string targetIdField)
	{
		TargetIdField = targetIdField;
		
	}
}