using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class IllegalCharacterData
{
    [HideInDataReader]
    public int Index;
    [FieldMap(SheetNameOfData="BanWords")]
    public string IllegalCharacter;
}
public class IllegalCharacterDataBase : ConfigBase
{
    public IllegalCharacterData[] IllegalCharacterDataTable;
    /// <summary>
    /// 检验srcChar是否包含非法字符
    /// </summary>
    /// <param name="srcChar">源字符串</param>
    /// <returns></returns>
    public bool ValidCharacter(string srcChar)
    {
        bool flag = true;
        foreach (IllegalCharacterData illegalChar in IllegalCharacterDataTable)
        {
            if (srcChar.IndexOf(illegalChar.IllegalCharacter) != -1)
            {
                flag = false;
                break;
            }
        }
        return flag;
    }

	/// <summary>
	/// 替换srcChar包含的非法字符
	/// </summary>
	/// <returns>The character.</returns>
	/// <param name="srcChar">Source char.</param>
	public string ReplaceCharacter(string srcChar)
	{
		if(!this.ValidCharacter(srcChar))
		{
			foreach (IllegalCharacterData illegalChar in IllegalCharacterDataTable)
			{
				srcChar = srcChar.Replace(illegalChar.IllegalCharacter,"*");
			}
		}
		return srcChar;
	}


    public override void Init(int length, object dataList)
    {
        IllegalCharacterDataTable = new IllegalCharacterData[length];

        var realData = dataList as List<IllegalCharacterData>;
        for (int i = 0; i < realData.Count; i++)
        {
            IllegalCharacterDataTable[i] = (IllegalCharacterData)realData[i];
            IllegalCharacterDataTable[i].Index = i;
        }
    }
}
