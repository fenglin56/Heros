using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class SkillActionData:ICloneable
{
	public int m_actionId;
	public string m_animationId;
	public float m_startTime;
	public Vector2 m_startPos;
	public int m_moveType;  //0,娑堝け     1,浣嶇Щ ,
	public float m_startSpeed; //鍒濋€熷害
	public float m_acceleration; //鍔犻€熷害
	public float m_angle;        //瑙掑害
	public float m_duration;     //鎸佺画鏃堕棿
	public int m_threshold;	//闇镐綋
	public float m_startAngle;
	public float m_endAngle;     //缁撴潫瑙掑害
	public int[] m_effectGroup;

    public string m_effectPath;

    //Rocky Add at 2013-6-11
    //[GameDataPostFlag(true)]
    //public GameObject m_effect_resource;   //鐗规晥ID
    /*
    //[Late]
    public GameObject m_effect_resource;
    [HideInInspector]
    public string m_effect_resourceId;

    public GameObject Effect_resourcePrefab
    {
        get
        {
            if (m_effect_resource != null)
            {
                return m_effect_resource;//m_effect_resource;
            }

            m_effect_resource = AssetId.Resolve(m_effect_resource, m_effect_resourceId);
            return m_effect_resource;


            //return AssetId.Resolve(_prefab,_prefabId);
        }
    }
    */
    public float m_effect_start_time; //鐗规晥鍑虹幇鏃堕棿锛堝欢杩燂紝鍗虫寔缁椂闂达級
    public Vector2 m_effect_start_pos;  //鐗规晥璧峰浣嶇疆
    public float m_effect_start_angel;   //鐗规晥璧峰浣嶇疆锛堝拰瑙掕壊鎸囧悜鐜╁閫夋嫨鐐圭殑灏勭嚎鐨勫す瑙掞級
    public float m_effect_move_speed;   //鐗规晥绉诲姩閫熷害
    public float m_effect_move_accleration;   //绉诲姩鍔犻€熷害
    public int m_effect_loop_time;          //寰幆鎾斁娆℃暟
    public byte m_ani_followtype;           //璺熼殢鍙傛暟 0 涓嶈窡闅忥紝涓嶆敼鍙樻柟鍚戙€?1 閿佸畾鐩爣锛屾敼鍙樻柟鍚?3 蹇呭畾鍒拌揪,4 鎶€鑳芥柦鏀句腑鍙互鎺ュ彈鐢ㄦ埛杈撳叆鏀瑰彉鏂瑰悜
	public Vector2 m_followPositionOffset;
	
	public int m_invincible; // 
	
	public int m_ironBody;   // can not be beat back, beat fly and absord;
	
	//xun add at 2013.8.8
	//sfx
	public string m_soundEffectName;
	//sfx delay
	public float m_sfxDelay;

	public bool IsIgnoreBlock; //0=不需要无视阻挡，1=需要无视阻挡；

    public object Clone()
    {
        SkillActionData skillActionData = (SkillActionData)this.MemberwiseClone();
        if(this.m_effectGroup!=null)
        {
            skillActionData.m_effectGroup = new int[this.m_effectGroup.Length];
            
            for (int i = 0; i < this.m_effectGroup.Length; i++)
            {
                skillActionData.m_effectGroup[i] = this.m_effectGroup[i];
            }
        }

        return skillActionData;
    }
}

public class SkillActionDataBase : ScriptableObject {

	public SkillActionData[] _dataTable;
}
