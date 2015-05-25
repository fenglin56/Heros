using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.MainUI;
public class RoleAttributePanel_Ranking : MonoBehaviour {
    public SingleRoleAtrribute[] SingleRoleAttributeList;
    private Dictionary<RoleAttributeType, SingleRoleAtrribute> RoleAtbList = new Dictionary<RoleAttributeType, SingleRoleAtrribute>();
    void Awake()
    {
        foreach (var child in SingleRoleAttributeList)
        {
            RoleAtbList.Add(child.roleAttributeType, child);
        }
       
    }
    public   void ShowAttribute(SMsgInteract_GetPlayerRanking_SC data)
    {
        RoleAtbList[RoleAttributeType.MaxHP].ResetInfo(data.dwMaxHp.ToString());
        RoleAtbList[RoleAttributeType.MaxMP].ResetInfo(data.dwMaxMp.ToString());
        RoleAtbList[RoleAttributeType.ATK].ResetInfo(data.dwAttack.ToString());
        RoleAtbList[RoleAttributeType.DEF].ResetInfo(data.dwdefend.ToString());
        RoleAtbList[RoleAttributeType.HIT].ResetInfo(data.dwNicety.ToString());
        RoleAtbList[RoleAttributeType.EVA].ResetInfo(data.dwJook.ToString());
        RoleAtbList[RoleAttributeType.Crit].ResetInfo(data.dwBurst.ToString());
        RoleAtbList[RoleAttributeType.ResCrit].ResetInfo(data.dwUnBurst.ToString());
    }
}
