using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.Forging;


[System.Serializable]
public class ForgeRecipeData {
    public int ForgeID;
    public int ForgeEquipmentID;
    public int[] ForgeProfession;
    public Recipe[] ForgeCost;
    public string ForgeIDS;
    public ForgingType ForgeType;
}
public class ForgeRecipeDataBase:ScriptableObject
{
    public  ForgeRecipeData[] ForgeRecipeDataList;
}
[System.Serializable]
public class Recipe
{
   public  int RecipeID;
    public int count;
}