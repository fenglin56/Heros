using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using NetworkCommon;
using UnityEngine;

public class PackageHelper
{
    public static byte[] StructToBytes<T>(T structObj) where T : struct
    {
        //得到结构体的大小
        int size = Marshal.SizeOf(structObj);
        //创建byte数组
        byte[] bytes = new byte[size];
        //分配结构体大小的内存空间
        IntPtr structPtr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(structObj, structPtr, false);
        //从内存空间拷到byte数组ITPUB
        Marshal.Copy(structPtr, bytes, 0, size);
        //释放内存空间
        Marshal.FreeHGlobal(structPtr);
        //返回byte数组   

        return bytes;
    }
    //byte数组到结构体
    public static T BytesToStuct<T>(byte[] bytes) where T : struct
    {
        Type type = typeof(T);
        //得到结构体的大小
        int size = Marshal.SizeOf(type);
        //byte数组长度小于结构体的大小
        if (size > bytes.Length)
        {
            //返回空
            return default(T);
        }
        //分配结构体大小的内存空间
        IntPtr structPtr = Marshal.AllocHGlobal(size);
        //将byte数组拷到分配好的内存空间
        Marshal.Copy(bytes, 0, structPtr, size);
        //将内存空间转换为目标结构体
        T obj = (T)Marshal.PtrToStructure(structPtr, type);
        //释放内存空间
        Marshal.FreeHGlobal(structPtr);
        //返回结构体
        return obj;
    }
    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcBytes, out byte[] value, int count)
    {
        //value = srcBytes.Take(count).ToArray();
        return ReadData(srcBytes,out value,count,0);
    }

    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcByte, out float value)
    {
        //value = BitConverter.ToSingle(srcByte, 0);
        return ReadData(srcByte, out value, 0); 
    }
    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcByte, out long value)
    {
        //value = BitConverter.ToInt64(srcByte, 0);
        return ReadData(srcByte, out value, 0); 
    }
    public static int ReadData(byte[] srcByte, out ulong value)
    {
        //value = BitConverter.ToUInt64(srcByte, 0);
        return ReadData(srcByte, out value, 0); 
    }
    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcByte, out int value)
    {
        //value = BitConverter.ToInt32(srcByte, 0);
        return ReadData(srcByte, out value, 0); 
    }

    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcByte, out uint value)
    {
        //value = BitConverter.ToUInt32(srcByte, 0);
        return ReadData(srcByte, out value, 0); 
    }

    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcBytes, out short value)
    {
        //value = BitConverter.ToInt16(srcBytes, 0);
        return ReadData(srcBytes, out value, 0); 
    }

    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcBytes, out ushort value)
    {
        //value = BitConverter.ToUInt16(srcBytes, 0);
        return ReadData(srcBytes, out value, 0); 
    }
    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcBytes, out byte value)
    {
        //value = srcBytes[0];
        return ReadData(srcBytes, out value, 0); 
    }
    #region ReadData Override method
    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcBytes, out byte[] value, int count,int offset)
    {
        value = srcBytes.Skip(offset).Take(count).ToArray();
        return count;
    }

    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcByte, out float value, int offset)
    {
        value = BitConverter.ToSingle(srcByte, offset);
        return 4;
    }
    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcByte, out long value, int offset)
    {
        value = BitConverter.ToInt64(srcByte, offset);
        return 8;
    }
    public static int ReadData(byte[] srcByte, out ulong value, int offset)
    {
        value = BitConverter.ToUInt64(srcByte, offset);
        return 8;
    }
    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcByte, out int value, int offset)
    {
        value = BitConverter.ToInt32(srcByte, offset);
        return 4;
    }

    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcByte, out uint value, int offset)
    {
        value = BitConverter.ToUInt32(srcByte, offset);
        return 4;
    }

    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcBytes, out short value, int offset)
    {
        value = BitConverter.ToInt16(srcBytes, offset);
        return 2;
    }

    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcBytes, out ushort value, int offset)
    {
        value = BitConverter.ToUInt16(srcBytes, offset);
        return 2;
    }
    //从二进制数组中读取指定类型的数据到输出变量中，返回该类型的长度
    public static int ReadData(byte[] srcBytes, out byte value, int offset)
    {
        value = srcBytes[offset];
        return 1;
    }
    #endregion
    //写入指定类型的数据，返回二进制数组
    public static byte[] WriteData(byte[] value)
    {
        return value;
    }

    //写入指定类型的数据，返回二进制数组
    public static byte[] WriteData(float value)
    {
        return BitConverter.GetBytes(value);
    }

    //写入指定类型的数据，返回二进制数组
    public static byte[] WriteData(int value)
    {
        return BitConverter.GetBytes(value);
    }

    //写入指定类型的数据，返回二进制数组
    public static byte[] WriteData(uint value)
    {
        return BitConverter.GetBytes(value);
    }

    //写入指定类型的数据，返回二进制数组
    public static byte[] WriteData(short value)
    {
        return BitConverter.GetBytes(value);
    }

    //写入指定类型的数据，返回二进制数组
    public static byte[] WriteData(ushort value)
    {
        return BitConverter.GetBytes(value);
    }

    //Parse the data filed buffer to struct of T
    public static T ParseDataBufferToStruct<T>(byte[] databuff) where T : struct
    {
        T obj = default(T);
        //try
        //{
            Package package = ParseReceiveData(databuff);
            obj = PackageHelper.BytesToStuct<T>(package.Data);
        //}
        //catch
        //{
        //    //TraceUtil.Log("收到的字节数组解包异常");
        //}
        return obj;
    }
    //Parse  receive data buffer to BinPackage
    public static Package ParseReceiveData(byte[] databuff)
    {
        Package package = new Package();
        var headLength = 5;// Marshal.SizeOf(package.Head);
        package.Head = PackageHelper.BytesToStuct<PkgHead>(databuff.Take(headLength).ToArray());
        package.Data = databuff.Skip(headLength).ToArray().Take(package.Head.DataLength-3).ToArray();

        return package;
    }
    //Parse  Send object(BinPackage) to buffer 
    public static byte[] GetNetworkSendBuffer(Package sendObj)
    {
        //sendObj.SetDataLength();
        var headBuffer = PackageHelper.StructToBytes<PkgHead>(sendObj.Head);

        byte[] sendBuffer =sendObj.Data==null?headBuffer: headBuffer.Concat(sendObj.Data).ToArray();
        return sendBuffer;
    }
    public static byte[] GetByteFromArray(byte[] source, int length)
    {
        if (source != null)
            return source;
        else
            return new byte[length];

    }
}
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct PkgHead
{
    public ushort DataLength;  //C++的数据长度包括包头的主消息 1字节和子消息的2字节，所以内部会加3字节
    public byte MasterMsgType;  //主消息
    public short SubMsgType;    //子消息

    public PkgHead(byte masterMsgType,short subMsgType)
    {
        DataLength = 3;
        MasterMsgType = masterMsgType;
        SubMsgType = subMsgType;
    }

    public PkgHead(ushort len, byte masterMsgType, short subMsgType)
        :this(masterMsgType,subMsgType)
    {
        DataLength =(ushort)(len+3);       
    }   
}
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct Package
{
    private PkgHead m_head;
    private byte[] m_data;

    public PkgHead Head
    {
        get { return m_head; }
        set
        {
            m_head = value;
            if (m_data != null)
            {
                this.m_head.DataLength = (ushort)(m_data.Length + 3);
            }
        }
    }
    public byte[] Data
    {
        get
        {
            return m_data;
        }
        set
        {
            m_data = value;
            if (m_data != null)
            {
                this.m_head.DataLength = (ushort)(m_data.Length + 3);
            }
        }
    }
    public byte GetMasterMsgType()
    {
        return this.m_head.MasterMsgType;
    }
    public short GetSubMsgType()
    {
        return this.m_head.SubMsgType;
    }
    public void SetDataLength()
    {
        this.m_head.DataLength = (ushort)(m_data.Length + 3);//Marshal.SizeOf(this.m_head) - 2);
    }
}
