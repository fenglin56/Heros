using UnityEngine;
using System.Collections;

namespace UI
{

    public enum TextColor {EquipmentGreen=0, EquipmentBlue, EquipmentMagenta, EquipmentYellow, black, blue, cyan, gray, green, grey, red, white, yellow, orange, purple
		,ChatBlue,ChatGreen,ChatYellow,ChatOrange,DescriptionYellow}
    public class NGUIColor : MonoBehaviour
    {
        public static string SetTxtColor(string Text, TextColor color)
        {
            switch (color)
            {
                case TextColor.black:
                    return "[000000]" + Text + "[-]";
                case TextColor.blue:
                    return "[0042ff]" + Text + "[-]";
                case TextColor.cyan:
                    return "[00fffc]" + Text + "[-]";
                case TextColor.gray:
                    return "[d1d1d1]" + Text + "[-]";
                case TextColor.green:
                    return "[00ff00]" + Text + "[-]";
                case TextColor.grey:
                    return "[505050]" + Text + "[-]";
                case TextColor.red:
                    return "[ff0000]" + Text + "[-]";
                case TextColor.white:
                    return "[ffffff]" + Text + "[-]";
                case TextColor.yellow:
                    return "[ffde00]" + Text + "[-]";
                case TextColor.orange:
                    return "[ff6000]" + Text + "[-]";
                case TextColor.purple:
                    return "[942cff]" + Text + "[-]";
                case TextColor.EquipmentGreen:
                    return "[54e44f]" + Text + "[-]";
                case TextColor.EquipmentBlue:
                    return "[67b9ff]" + Text + "[-]";
                case TextColor.EquipmentMagenta:
                    return "[fc6cfa]" + Text + "[-]";
                case TextColor.EquipmentYellow:
                    return "[ff9860]" + Text + "[-]";
				case TextColor.ChatBlue:
					return "[b1dbff]"+ Text + "[-]";
				case TextColor.ChatGreen:
					return "[9bfd98]"+ Text + "[-]";
				case TextColor.ChatYellow:
					return "[fffa6f]"+ Text + "[-]";
				case TextColor.ChatOrange:
					return "[ff8400]"+ Text + "[-]";
				case TextColor.DescriptionYellow:
					return "[ece09e]"+ Text + "[-]";
                default:
                    return Text;
            }
        }

        public static string SetTxtColor(int Text, TextColor textColor)
        {
            return SetTxtColor(Text.ToString(), textColor);
        }
    }
}