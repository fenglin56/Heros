using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.MainUI;
public class HeroAttributePanel_Ranking : MonoBehaviour {

    public SpriteSwith VocationSprite;
    public UILabel AtkLabel;
    public UILabel ExpLabel;
    public UISlider ExpSliderBar;
    public RoleAttributePanel_Ranking RoleAttributePanel;
    public AtrributePanelSingleSirenIcon[] SirenIconList;


    public bool IsShow{get;private set;}
    private GameObject TweenObj;
    private SYaoNvRankingData[] YaonvData=new SYaoNvRankingData[5];

    
    public void UpdateAttribute(SMsgInteract_GetPlayerRanking_SC data)
    {
        RoleAttributePanel.ShowAttribute(data);
        ExpLabel.SetText(string.Format("{0}/{1}",data.dwCurExp ,data.dwMaxExp));
        ExpSliderBar.sliderValue = (float)data.dwCurExp/(float)data.dwMaxExp;
        VocationSprite.ChangeSprite(data.byKind);
        AtkLabel.SetText(data.dwActorFinght.ToString());
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
//            else
//            {
//                string nameRes=SirenDataManager.Instance.GetPlayerSirenList()[i]._nameRes;
//                SirenIconList[i].Show(nameRes,0);
//            }

        }
        for (int i=0; i<YaonvData.Length; i++)
        {
            SirenIconList[i].Show(SirenDataManager.Instance.GetPlayerSirenList()[i]._nameRes,YaonvData[i].byYaoNvLevel);
        }
    }

}
