using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace UI.MainUI
{

    public class SingleTrialsAtbPanel : MonoBehaviour
    {

        public UILabel TitleDesLabel;
        public List<SingleRoleAtrribute> SingleAtrributeLsit;

        public bool IsShow = false;
        Vector3 NormalScale;
        Vector3 SmallScale = new Vector3(0.01f,0.01f,0.01f);

        void Start()
        {
            NormalScale = transform.localScale;
            gameObject.SetActive(true);
            transform.localScale = SmallScale;
        }

        public bool Show(EctypeContainerData ectypeData,bool isPass)
        {
            
            string Line01Str = LanguageTextManager.GetString("IDS_H1_487");
            string Line02Str = LanguageTextManager.GetString(isPass?"IDS_H1_489":"IDS_H1_488");
            TitleDesLabel.SetText(string.Format("{0}\n{1}",Line01Str,Line02Str));
            bool flag = false;
            if (!IsShow)
            {
                IsShow = true;
                flag = true;
                gameObject.SetActive(IsShow);
                SetPanelAttribute(ectypeData);
                TweenShowPanel();
            }
            return flag;
        }

        public bool Close()
        {
            bool flag = false;
            if (IsShow)
            {
                IsShow = false;
                flag = true;
                TweenClosePanel();
            }
            return flag;
        }

        void SetPanelAttribute(EctypeContainerData ectypeData)
        {
            string[] addEffectStr = ectypeData.TrialsAward.Split('|');
            Dictionary<int, int> addEffectList = new Dictionary<int, int>();
            foreach (var child in addEffectStr)
            {
                string[] str = child.Split('+');
                EffectData addData = ItemDataManager.Instance.EffectDatas._effects.First(P=>P.m_SzName == str[0]);
                addEffectList.Add(addData.BasePropView,int.Parse(str[1]));
            }
            foreach(var child in SingleAtrributeLsit)
            {
                int addNumber = 0;
                addEffectList.TryGetValue(child.EffectBasePropID, out addNumber);
                addNumber = HeroAttributeScale.GetScaleAttribute(child.EffectBasePropID, addNumber);
                child.ResetInfo(addNumber.ToString());
            }
        }

        void TweenShowPanel()
        {
            gameObject.SetActive(true);
            Vector3 fromScale = transform.localScale;
            Vector3 toScale = NormalScale;
            TweenScale.Begin(gameObject, 0.2f, fromScale, toScale, TweenShowPanelComplete);
        }

        void TweenClosePanel()
        {
            Vector3 fromScale = transform.localScale;
            Vector3 toScale = SmallScale;
            TweenScale.Begin(gameObject, 0.2f, fromScale, toScale, TweenClosePanelComplete);
        }

        void TweenShowPanelComplete(object obj)
        {
        }

        void TweenClosePanelComplete(object obj)
        {
            gameObject.SetActive(false);
        }
    }
}