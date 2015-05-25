using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SystemMailConfigData
{
    public int MailType;
    public string MailTitle;
    public string MailText;
}
public class SystemMailConfigDataBase:ScriptableObject
{
    public SystemMailConfigData[] SystemMailConfigDataList;
}
