using UnityEngine;
using System.Collections;

public static class GUICommonTool {

    
    public static void SetText(this UILabel SourceObj,string str )
    {
        SourceObj.text = str.Replace("\\n","\n");
    }
    public static void SetText(this UILabel SourceObj, int str)
    {
        SourceObj.text = str.ToString();
    }

	//获取品质颜色
	public static UI.TextColor GetQuilityColor(int quilityID)
	{
		UI.TextColor NameTextColor = UI.TextColor.white;
		switch (quilityID)
		{
		case 0:
			NameTextColor = UI.TextColor.EquipmentGreen;
			break;
		case 1:
			NameTextColor = UI.TextColor.EquipmentBlue;
			break;
		case 2:
			NameTextColor = UI.TextColor.EquipmentMagenta;
			break;
		case 3:
			NameTextColor = UI.TextColor.EquipmentYellow;
			break;
		default:
			break;
		}
		return NameTextColor;
	}
}
