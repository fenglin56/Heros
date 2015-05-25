using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonFx.Json;
using System.Runtime.InteropServices;

	public class JsonConvertor<T>
    {
        public static string Object2Json(T obj)
        {
            //serialize
            if (obj == null)
            {
                return string.Empty;
            }
            StringBuilder accountBuilder = new StringBuilder();
            JsonWriter writer = new JsonWriter(accountBuilder);

            writer.Write(obj.ToString());

            return accountBuilder.ToString();
        }
        public static T Json2Object(string json)
        {
            T obj = default(T);
            try
            {
                System.IO.StringReader stringReader = new System.IO.StringReader(json);
                JsonReaderSettings settings = new JsonReaderSettings();
                settings.AllowNullValueTypes = true;
                JsonDataReader jsonDataReader = new JsonDataReader(settings);
                obj = (T)jsonDataReader.Deserialize(stringReader, typeof(T));
            }
            catch { }

            return obj;
        }       
    }
    public class JsonToByteArray
    {
        static ASCIIEncoding m_dataEncoding = new ASCIIEncoding();
        public static string ByteArrayToString(byte[] buffer)
        {
            return m_dataEncoding.GetString(buffer);
        }
        public static Byte[] StringToByteArray(string jsonObj)
        {
            return m_dataEncoding.GetBytes(jsonObj);
        }        
    }
