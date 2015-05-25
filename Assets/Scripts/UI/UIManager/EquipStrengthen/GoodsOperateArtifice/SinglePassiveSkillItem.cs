using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class SinglePassiveSkillItem : MonoBehaviour
    {

        public GameObject ResetSkillEffectPrefab;
        public Transform CreatSkillEffectPoint;

        public UILabel NameLabel;
        public UILabel DesLabel;
        public Transform CreatItemIconTransform;

        public PassiveSkillData MyPassiveSkillData { get; private set; }
        public GoodsOperateArtificePanel MyParent { get; private set; }

        public void Show(int skillID, int skillLevel, PassiveSkillDataBase passiveSkillData,GoodsOperateArtificePanel myParent)
        {
            TraceUtil.Log("SkillID:" + skillID + "," + skillLevel);
            this.MyParent = myParent;
            MyPassiveSkillData = passiveSkillData._dataTable.FirstOrDefault(P => P.SkillID == skillID && P.SkillLevel == skillLevel);
            SetMyselfActive(MyPassiveSkillData != null);
            if (MyPassiveSkillData==null)
                return;
            Show(MyPassiveSkillData);
        }

        void Show(PassiveSkillData passiveSkillData)
        {
            ResetWidgetInfo();
            NameLabel.SetText(LanguageTextManager.GetString(passiveSkillData.SkillName));
            DesLabel.SetText(LanguageTextManager.GetString(passiveSkillData.SkillDis));
            CreatObjectToNGUI.InstantiateObj(passiveSkillData.SkillIconPrefab, CreatItemIconTransform);
            if (MyParent.IsResetSkill)
            {
                TraceUtil.Log("显示技能刷新特效");
                CreatResetSkillEffect();
            }
        }

        void CreatResetSkillEffect()
        {
            GameObject skillEffect = CreatObjectToNGUI.InstantiateObj(ResetSkillEffectPrefab, CreatSkillEffectPoint);
            DoForTime.DoFunForTime(2, DestroySkillObj, skillEffect);
        }

        void DestroySkillObj(object obj)
        {
            GameObject destroyObj = obj as GameObject;
            if (destroyObj != null)
            {
                Destroy(destroyObj);
            }
        }

        public void ResetWidgetInfo()
        {
            NameLabel.SetText(string.Empty);
            DesLabel.SetText(string.Empty);
            CreatItemIconTransform.ClearChild();
        }

        void SetMyselfActive(bool flag)
        {
            this.gameObject.SetActive(flag);
        }
    }
}