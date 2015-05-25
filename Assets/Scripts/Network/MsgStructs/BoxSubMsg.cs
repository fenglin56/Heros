using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct BoxSubMsgEntity : INotifyArgs, IEntityDataStruct
{
    public SMsgPropCreateEntity_SC_Header m_sMsg_Header;

    public Int64 UID;
    public uint MapID;
    public int BoxX;
    public int BoxY;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
    public byte[] Name;         //盒子名称

    public SMsgPropCreateEntity_SC_BaseValue BaseValues;  //基础属性
    public SMsgPropCreateEntity_SC_BoxValue BoxValues; //单元基本属性

    public SMsgPropCreateEntity_SC_Header SMsg_Header
    {
        get
        {
            return this.m_sMsg_Header;
        }
    }

    public void UpdateValue(short index, int value)
    {
    }
         /// <summary>
    /// 从整体字节数组获得实体上下文的具体数据
    /// </summary>
    /// <param name="dataBuffer"></param>
    /// <returns></returns>
    public static BoxSubMsgEntity ParsePackage(byte[] dataBuffer, int offset)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);
        return ParsePackage(package, offset);
    }
    public static BoxSubMsgEntity ParsePackage(Package package, int offset)
    {
        var structLength = Marshal.SizeOf(typeof(BoxSubMsgEntity));
        var headLength = Marshal.SizeOf(typeof(SMsgPropCreateEntity_SC_Header));
        var offsetBuffer = package.Data.Skip(offset).Take(structLength).ToArray();

        BoxSubMsgEntity boxSubMsgEntity = new BoxSubMsgEntity();

        boxSubMsgEntity.m_sMsg_Header = SMsgPropCreateEntity_SC_Header.ParsePackage(package, offset);

        int of = headLength;
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out boxSubMsgEntity.UID);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out boxSubMsgEntity.MapID);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out boxSubMsgEntity.BoxX);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out boxSubMsgEntity.BoxY);
        of += PackageHelper.ReadData(offsetBuffer.Skip(of).ToArray(), out boxSubMsgEntity.Name, 19);


        //boxSubMsgEntity.UID = BitConverter.ToInt64(offsetBuffer, headLength);
        //boxSubMsgEntity.MapID = BitConverter.ToUInt32(offsetBuffer, headLength + 8);
        //boxSubMsgEntity.BoxX = BitConverter.ToInt32(offsetBuffer, headLength + 8 + 4);
        //boxSubMsgEntity.BoxY = BitConverter.ToInt32(offsetBuffer, headLength + 8 + 4 + 4);
        //boxSubMsgEntity.Name = offsetBuffer.Skip(headLength + 8 + 4 + 4 + 4).Take(19).ToArray();

        boxSubMsgEntity.BaseValues = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_BaseValue>
            (offsetBuffer.Skip(of).Take(4 * 2).ToArray());
        of += 4 * 2;
        boxSubMsgEntity.BoxValues = PackageHelper.BytesToStuct<SMsgPropCreateEntity_SC_BoxValue>
            (offsetBuffer.Skip(of ).Take(4 * 4).ToArray());

        return boxSubMsgEntity;
    }

    public int GetEventArgsType()
    {
        return 0;
    }
}
