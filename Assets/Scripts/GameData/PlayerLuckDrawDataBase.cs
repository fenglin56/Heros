using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ItemCountGroup
{
    public GameObject m_icon;
    public int m_itemCount;
}

[System.Serializable]
public class PlayerLuckDrawData
{
    //id
    public int m_luckId;
    
    //掉落数量
    public List<ItemCountGroup> m_goodsNum;
    
    //
    //public GameObject m_iconPrefab;

    public GameObject m_tipPrefab;


    public int GetItemCount(int playerLevel)
    {
        if(m_goodsNum.Count < 1)
        {
            return 0;
        }
        else 
        {

            int playerLevelParam = (playerLevel - 1)/10 + 1;
            if(playerLevelParam > m_goodsNum.Count)
            {
                playerLevelParam = m_goodsNum.Count;
            }



            return m_goodsNum[playerLevelParam - 1].m_itemCount;
        }
    }

    public GameObject GetItemIconPrefab(int playerLevel)
    {
        int playerLevelParam = (playerLevel - 1)/10 + 1;
        if(playerLevelParam > m_goodsNum.Count)
        {
            playerLevelParam = m_goodsNum.Count;
        }
        
        
        
        return m_goodsNum[playerLevelParam - 1].m_icon;
    }

}



public class PlayerLuckDrawDataBase : ScriptableObject 
{
    public PlayerLuckDrawData[] m_dataTable;
	
}
