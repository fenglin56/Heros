using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.MainUI;
public class SirenListUnit : MonoBehaviour {
    public AtrributePanelSingleSirenIcon[] SirenIconList;
    private SYaoNvRankingData[] YaonvData=new SYaoNvRankingData[5];

    public void InitData(RankingYaoNvFightData data)
    {
        ShowSirenItem(data.YaoNvData);
    }
    void ShowSirenItem(SYaoNvRankingData[] datas)
    {
   
        for (int i=0; i<YaonvData.Length; i++)
        {
            YaonvData[i]=new SYaoNvRankingData(){byYaoNvLevel=0,byYaoNvId=(byte)SirenDataManager.Instance.GetPlayerSirenList()[i]._sirenID};
        }
        
        for(int i = 0;i<SirenIconList.Length;i++)
        {
            if(datas[i].byYaoNvId!=0)
            {
                for(int j=0;j<YaonvData.Length;j++)
                {
                    if(YaonvData[j].byYaoNvId==datas[i].byYaoNvId)
                    {
                        YaonvData[j].byYaoNvLevel=datas[i].byYaoNvLevel;
                    }
                    
                }
            }
           
        }
        for (int i=0; i<YaonvData.Length; i++)
        {
            SirenIconList[i].Show(SirenDataManager.Instance.GetPlayerSirenList()[i]._nameRes,YaonvData[i].byYaoNvLevel);
        }
    }
}
