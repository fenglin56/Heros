using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//public abstract class NotifyArgs : EventArgs
//{
//    public NotifyArgs() { }
//    public NotifyArgs(int eventType)
//    {
//        this.EventArgsType = eventType;
//    }
//    public int EventArgsType { get; set; }
//    public virtual void ParsePackage(byte[] dataBuffer) { }
//    //public virtual BinPackage GeneratePackage() { return new BinPackage(); }
//}

public interface INotifyArgs
{
    int GetEventArgsType();
}

public class EventArgParser
{
    public static T Parse<T>(INotifyArgs e) where T : struct, INotifyArgs
    {
        T t = (T)e;

        return t;
    }
}

