using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class IllegalNameData
{
    public int ID1;
    public string Name;
}
public class IllegalNameDataBase:ConfigBase
{
    public IllegalNameData[] Datas;

    public override void Init(int length, object dataList)
    {
        Datas = new IllegalNameData[length];

        var realData = dataList as List<IllegalNameData>;
        for (int i = 0; i < length; i++)
        {
            Datas[i] = realData[i];
        }
    }

    public bool ValidCharacter(string srcChar)
    {
        bool flag = true;
        foreach (IllegalNameData illegalChar in Datas)
        {
            if (srcChar.Equals(illegalChar.Name,StringComparison.OrdinalIgnoreCase))
            {
                flag = false;
                break;
            }
        }
        return flag;
    }
}
