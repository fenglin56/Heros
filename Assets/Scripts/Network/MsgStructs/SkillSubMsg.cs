using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SkillEquipEntity
{
    public Dictionary<byte, ushort> Skills;  //技能装备栏上的技能集合
   
    public static SkillEquipEntity ParsePackage(byte[] data)
    {
        SkillEquipEntity skillEquipEntity = new SkillEquipEntity();
        skillEquipEntity.Skills = new Dictionary<byte, ushort>();
        int off = 0;
        ushort skill1,skill2,skill3,skill4;
        off+=PackageHelper.ReadData(data.Skip(off).ToArray(),out skill1);
        off += PackageHelper.ReadData(data.Skip(off).ToArray(), out skill2);
        off += PackageHelper.ReadData(data.Skip(off).ToArray(), out skill3);
        off += PackageHelper.ReadData(data.Skip(off).ToArray(), out skill4);

        skillEquipEntity.Skills.Add(0, skill1);
        skillEquipEntity.Skills.Add(1, skill2);
        skillEquipEntity.Skills.Add(2, skill3);
        skillEquipEntity.Skills.Add(3, skill4);

        return skillEquipEntity;
    }
	//没有发送了//
    public Package GeneratePackage()
    {
        Package pkg = new Package();
        pkg.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_EQUIP_SKILL);
        pkg.Data = new byte[8];
        for (byte i = 0; i < 4; i++)
        {
            ushort skillId = 0;

            if (this.Skills != null && this.Skills.Count > i)
            {
                skillId = this.Skills[i];
            }
            if(i==0)
            {
                pkg.Data =PackageHelper.WriteData(skillId) ;
            }
            else
            {
                pkg.Data = pkg.Data.Concat(PackageHelper.WriteData(skillId)).ToArray();
            }
        }
        return pkg;
    }
}
//技能初始化数据//
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgSkillInit_SC
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public ushort[] wSkillEquipList;//当前装备的技能
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
    public List<SSkillInfo> sInfos;


    public static SMsgSkillInit_SC ParsePackage(byte[] data)
    {
        SMsgSkillInit_SC sMsgSkillInit_SC = new SMsgSkillInit_SC();
		List<SSkillInfo> sSkillInfoList=new List<SSkillInfo>();
        int off = 0;
        ushort skill;
        sMsgSkillInit_SC.wSkillEquipList = new ushort[10];
        for (int i = 0; i < 10; i++)
        {
            off += PackageHelper.ReadData(data.Skip(off).ToArray(), out skill);
            sMsgSkillInit_SC.wSkillEquipList[i] = skill;
        }
        for (int b = 0; b < 15; b++)
        {
			ushort skillID = 0;
			off += PackageHelper.ReadData(data.Skip(off).ToArray(), out skillID);
			if(skillID == 0)
			{
				break;
			}
			SSkillInfo sSkillInfo = new SSkillInfo();
			sSkillInfo.wSkillID = skillID;
			off += PackageHelper.ReadData(data.Skip(off).ToArray(), out sSkillInfo.wSkillLV);
			off += PackageHelper.ReadData(data.Skip(off).ToArray(), out sSkillInfo.byStrengthenLv);
			sSkillInfoList.Add(sSkillInfo);
        }
        sMsgSkillInit_SC.sInfos = sSkillInfoList;

        return sMsgSkillInit_SC;
    }
	//装备技能//
    public void UpdateAssembleSkill(SkillEquipEntity skillEquipEntity)
    {
        for (byte index=0; index < wSkillEquipList.Length;index++ )
        {
            if (skillEquipEntity.Skills.ContainsKey(index))
            {
                wSkillEquipList[index] = skillEquipEntity.Skills[index];
            }
        }
		SkillModel.Instance.DealSkillAdUpStrengthen();
    }
	//技能升级
    public void UpgradeSkill(SSkillInfo sSkillInfo)
    {
        if (sInfos.Exists(P => P.wSkillID == sSkillInfo.wSkillID))
        {
            var targetskill = sInfos.SingleOrDefault(P => P.wSkillID == sSkillInfo.wSkillID);
			sInfos.Remove(targetskill);
            targetskill.wSkillLV = sSkillInfo.wSkillLV;
			//targetskill.byStrengthenLv = sSkillInfo.byStrengthenLv;//升级时，这个字段不能动，保持原值
			sInfos.Add(targetskill);
        }
        else
        {
			//不可能在这里
            sInfos.Add(sSkillInfo);
        }
		SkillModel.Instance.DealSkillAdUpStrengthen();
    }
	//技能解锁
	public void UnlockSkill(SMsgSkillUnLock_SC sSkillInfo)
	{
		//一般不可能存在
		if (sInfos.Exists(P => P.wSkillID == sSkillInfo.wSkillId))
		{
			var targetskill = sInfos.SingleOrDefault(P => P.wSkillID == sSkillInfo.wSkillId);
			sInfos.Remove(targetskill);
			SSkillInfo newInfo = new SSkillInfo() { wSkillID = (ushort)sSkillInfo.wSkillId, wSkillLV = sSkillInfo.byUpgradeLv,byStrengthenLv=sSkillInfo.byStrengthenLv };
			sInfos.Add(newInfo);
		}
		else
		{
			SSkillInfo newInfo = new SSkillInfo() { wSkillID = (ushort)sSkillInfo.wSkillId, wSkillLV = sSkillInfo.byUpgradeLv,byStrengthenLv=sSkillInfo.byStrengthenLv };
			sInfos.Add(newInfo);
		}
		SkillConfigData configData = SkillDataManager.Instance.GetSkillConfigData ((int)(sSkillInfo.wSkillId));
		if (configData.PreSkill != 0) {
			UIEventManager.Instance.TriggerUIEvent (UIEventType.SkillAdvanceEvent, sSkillInfo);
		}
		SkillModel.Instance.DealSkillAdUpStrengthen();
	}

	//技能强化
	public void StrengthenSkill(SMsgSkillStrengthen_SC sSkillInfo)
	{
		if (sInfos.Exists(P => P.wSkillID == sSkillInfo.wSkillId))
		{
			var targetskill = sInfos.SingleOrDefault(P => P.wSkillID == sSkillInfo.wSkillId);
			byte skillLev = targetskill.wSkillLV;
			sInfos.Remove(targetskill);
			targetskill.wSkillLV = skillLev;
			targetskill.byStrengthenLv = sSkillInfo.byStrengthenLv;
			sInfos.Add(targetskill);
		}
		else
		{
			TraceUtil.Log("StrengthenSkill data error!!!!");
		}
		SkillModel.Instance.DealSkillAdUpStrengthen ();
	}

    public void WaskSkill(bool washResult)
    {
        if (washResult)
        {
            List<SSkillInfo> tempList=new List<SSkillInfo>();
            foreach (var item in sInfos)
            {
                tempList.Add(new SSkillInfo() { wSkillID = item.wSkillID, wSkillLV = 1 });
            }
			sInfos.Clear();
            sInfos.AddRange(tempList);
        }
    }
}
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SSkillInfo 
{
    public ushort wSkillID;
    public byte wSkillLV;//等级
	public byte byStrengthenLv;//强化等级

}


#region new add skill
// 升级技能
//MSG_ACTION_STRENGTHEN_SKILL			=	58,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgSkillUpgrade_CS
{
	public ushort wSkillId;
	public Package GeneratePackage()
	{
		Package Pak = new Package();
		Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_UPGRADE_SKILL);
		Pak.Data = PackageHelper.StructToBytes<SMsgSkillUpgrade_CS>(this);
		return Pak;
	}
};

//进阶技能
//MSG_ACTION_ADVANCED_SKILL				=	57,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgSkillAdvanced_CS
{
	public ushort wSkillId;
	public Package GeneratePackage()
	{
		Package Pak = new Package();
		Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_ADVANCED_SKILL);
		Pak.Data = PackageHelper.StructToBytes<SMsgSkillAdvanced_CS>(this);
		return Pak;
	}
	public static SMsgSkillAdvanced_CS ParsePackage(byte[] dataBuffer)
	{
		SMsgSkillAdvanced_CS sMsgSkillAdvanced_CS = new SMsgSkillAdvanced_CS();
		int of = 0;
		PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgSkillAdvanced_CS.wSkillId);
		return sMsgSkillAdvanced_CS;
	}
};
// 强化技能
//MSG_ACTION_STRENGTHEN_SKILL			=	58,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgSkillStrengthen_CS
{
	public ushort wSkillId;
	public Package GeneratePackage()
	{
		Package Pak = new Package();
		Pak.Head = new PkgHead((byte)MasterMsgType.NET_ROOT_THING, CommonMsgDefineManager.MSG_ACTION_STRENGTHEN_SKILL);
		Pak.Data = PackageHelper.StructToBytes<SMsgSkillStrengthen_CS>(this);
		return Pak;
	}
};
// 强化技能
//MSG_ACTION_STRENGTHEN_SKILL			=	58,
[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgSkillStrengthen_SC
{
	public ushort wSkillId;
	public byte byStrengthenLv;
	public static SMsgSkillStrengthen_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgSkillStrengthen_SC sMsgSkillStrengthen_SC = new SMsgSkillStrengthen_SC();
		int of = 0;
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgSkillStrengthen_SC.wSkillId);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgSkillStrengthen_SC.byStrengthenLv);
		return sMsgSkillStrengthen_SC;
	}
};

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct SMsgSkillUnLock_SC
{
	public ushort wSkillId;
	public byte byUpgradeLv;
	public byte byStrengthenLv;
	public static SMsgSkillUnLock_SC ParsePackage(byte[] dataBuffer)
	{
		SMsgSkillUnLock_SC sMsgSkillStrengthen_SC = new SMsgSkillUnLock_SC();
		int of = 0;
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgSkillStrengthen_SC.wSkillId);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgSkillStrengthen_SC.byUpgradeLv);
		of += PackageHelper.ReadData(dataBuffer.Skip(of).ToArray(), out sMsgSkillStrengthen_SC.byStrengthenLv);
		return sMsgSkillStrengthen_SC;
	}
};
#endregion
