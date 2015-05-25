using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkManager
{
    public class EventArgsConvertor<T> where T : INotifyArgs
    {
        //public static Package Args2Package(T obj)
        //{
        //    return obj.GeneratePackage();
        //}
        //public static void Struct2Args(ref T obj,byte[] dataBuffer)
        //{
        //    try
        //    {
        //        obj.ParsePackage(dataBuffer);
        //    }
        //    catch { }
        //}
    }   
    public static class ExtensionFunManager
    {
        public static List<T> Array2List<T>(this T[] tArray)
        {
            List<T> tList = new List<T>();
            if (tArray != null && tArray.Length > 0)
            {
                tList.AddRange(tArray);
            }

            return tList;
        }
    }
}
