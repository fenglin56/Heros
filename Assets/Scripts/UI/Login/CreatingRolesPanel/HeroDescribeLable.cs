using UnityEngine;
using System.Collections;

public class HeroDescribeLable : MonoBehaviour {

    public UILabel NameLable;
    public UILabel DescribeLable01;
    public UILabel DescribeLable02;

    public void SetLableText(string DescribeStr)
    {
        if (!string.IsNullOrEmpty(DescribeStr))
        {
            //string[] Str = DescribeStr.Split('|');
            //this.NameLable.text = Str[0];
            //this.DescribeLable01.text = Str[1];
            //this.DescribeLable02.text = Str[2];
        }
    }

}
