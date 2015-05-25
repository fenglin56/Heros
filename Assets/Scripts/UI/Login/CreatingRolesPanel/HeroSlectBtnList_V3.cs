using UnityEngine;
using System.Collections;

namespace UI.Login
{

    public class HeroSlectBtnList_V3 : MonoBehaviour
    {

        public SingleButtonCallBack[] heroSelectBtnCallBack;


        void Awake()
        {
            for (int i = 0; i < heroSelectBtnCallBack.Length; i++)
            {
                if (heroSelectBtnCallBack[i] != null && heroSelectBtnCallBack[i] is SingleButtonCallBack)
                {
                    heroSelectBtnCallBack[i].SetCallBackFuntion(OnHeroSelectBtnClick, i);
                }
            }
        }

        public void InitSelectBtn(int BtnID)
        {
            heroSelectBtnCallBack[BtnID -1].OnClick();
        }

        void OnHeroSelectBtnClick(object obj)
        {
            int BtnID = (int)obj;
            foreach (SingleButtonCallBack child in heroSelectBtnCallBack)
            {
                if (child == null) continue;
                if (child.ButtonCallBackInfo == obj)
                {
                    child.GetComponentInChildren<UISlicedSprite>().color =Color.white;
                }
                else
                {
                    child.GetComponentInChildren<UISlicedSprite>().color = Color.gray; 
                }
            }
            CreatingRolesUIPanel_V3.Instance.OnSelectRole(++BtnID);
        }

    }
}