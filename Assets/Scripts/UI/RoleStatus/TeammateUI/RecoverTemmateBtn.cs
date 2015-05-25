using UnityEngine;
using System.Collections;

namespace UI.Battle
{

    public class RecoverTemmateBtn : SingleButtonCallBack
    {
        public UILabel RecoverMoneyLabel;

        public void ShowBtn(int RecoverMoney,string btnString)
        {
            SetButtonText(btnString);
            SetTextColor(Color.white);
            GetComponent<BoxCollider>().enabled = true;
            CreatIconPoint.gameObject.SetActive(true);
            this.RecoverMoneyLabel.text = "x"+ RecoverMoney.ToString();
            SetImageButtonComponentActive(true);
            SetButtonBackground(1);
        }

        public void GrayButton(string BtnText)
        {
            SetButtonText(BtnText);
            SetTextColor(Color.gray);
            SetImageButtonComponentActive(false);
            SetButtonBackground(2);
            GetComponent<BoxCollider>().enabled = false;
            CreatIconPoint.gameObject.SetActive(false);
        }
        
    }
}