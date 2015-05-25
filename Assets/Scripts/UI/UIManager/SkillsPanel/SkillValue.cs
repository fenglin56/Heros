using UnityEngine;
using System.Collections;


public class SkillValue {

    public static int GetSkillValue(int SkillLevel,string SkillValueArray)
    {
        string[] value = SkillValueArray.Split('+');
        int[] numberArray = new int[4] ;
        for (int i = 0; i < numberArray.Length;i++ )
        {
            numberArray[i] =System.Int32.Parse(value[i]);
        }
        int ValueA = 0;
        int ValueB = 0;
        ValueA = (int)((numberArray[0] * SkillLevel * SkillLevel + numberArray[1] * SkillLevel + numberArray[2]) / numberArray[3]);
        ValueB = ValueA*numberArray[3];
        return ValueB;
    }

    public static int GetSkillValue(int SkillLevel, int[] SkillValueArray)
    {
        //string[] value = SkillValueArray.Split('+');
        //int[] numberArray = new int[4];
        //for (int i = 0; i < numberArray.Length; i++)
        //{
        //    numberArray[i] = System.Int32.Parse(value[i]);
        //}
        int ValueA = 0;
        int ValueB = 0;
        ValueA = (int)((SkillValueArray[0] * SkillLevel * SkillLevel + SkillValueArray[1] * SkillLevel + SkillValueArray[2]) / SkillValueArray[3]);
        ValueB = ValueA * SkillValueArray[3];
        return ValueB;
    }

    public static int GetSkillValue(int SkillLevel, float[] SkillValueArray)
    {
        //string[] value = SkillValueArray.Split('+');
        //int[] numberArray = new int[4];
        //for (int i = 0; i < numberArray.Length; i++)
        //{
        //    numberArray[i] = System.Int32.Parse(value[i]);
        //}
        int ValueA = 0;
        int ValueB = 0;
        ValueA = (int)((SkillValueArray[0] * SkillLevel * SkillLevel + SkillValueArray[1] * SkillLevel + SkillValueArray[2]) / SkillValueArray[3]);
        ValueB = ValueA * (int)SkillValueArray[3];
        return ValueB;
    }

}
