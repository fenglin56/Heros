using UnityEngine;
using System;


    public class CharacterNameDataBase : ScriptableObject
    {
        public CharacterData[] CharacterDataList;

    }

    [Serializable]
    public class CharacterData
    {
        public int FamilyNameID;
        public string FamilyName;
        public int MaleNameID;
        public string MaleName;
        public int FemaleNameID;
        public string FemaleName;
    }
