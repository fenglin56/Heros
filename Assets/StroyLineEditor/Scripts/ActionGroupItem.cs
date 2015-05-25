using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StroyLineEditor
{
    public class ActionGroupItem : MonoBehaviour
    {
        public GameObject OptionInput;
        public GameObject Checkbox;
        public GameObject TitleItem;
        public UILabel UILabel;
        public GameObject Tween;
        private StroyLineConfigData m_stroyLineData;
        private CameraGroupConfigData m_cameraGroup;

        // Use this for initialization
        public void InitGroupItem(StroyLineConfigData item, GroupType groupType)
        {
            m_stroyLineData = item;
            int iRow = 0;

            switch (groupType)
            {
                case GroupType.MAPID:
                    UILabel.text = "场景地图";
                    GameObject OptionMapID = (GameObject)Instantiate(OptionInput);
                    OptionMapID.transform.parent = this.Tween.transform;
                    OptionMapID.transform.localScale = Vector3.one;
                    OptionMapID.transform.localPosition = new Vector3(0, -45 + iRow * -40, 0);
                    OptionMapID.GetComponent<OptionInputPanel>().InitInput(item._SceneMapID.ToString(), "地图", groupType);
                    break;
                case GroupType.BGMUSIC:
                    UILabel.text = "背景音乐";
                    GameObject OptionMusic = (GameObject)Instantiate(OptionInput);
                    OptionMusic.transform.parent = this.Tween.transform;
                    OptionMusic.transform.localScale = Vector3.one;
                    OptionMusic.transform.localPosition = new Vector3(0, -45 + iRow * -40, 0);
                    OptionMusic.GetComponent<OptionInputPanel>().InitInput(item._BgMusic, "音乐", groupType);
                    break;
                case GroupType.CONDITION:
                    UILabel.text = "触发条件";
                    GameObject OptionCondition = (GameObject)Instantiate(OptionInput);
                    OptionCondition.transform.parent = this.Tween.transform;
                    OptionCondition.transform.localScale = Vector3.one;
                    OptionCondition.transform.localPosition = new Vector3(0, -45 + iRow * -40, 0);
                    OptionCondition.GetComponent<OptionInputPanel>().InitInput(item._TriggerCondition.ToString(), "条件", groupType);
                    break;
                case GroupType.ECTYPEID:
                    UILabel.text = "触发副本";
                    GameObject OptionEctype = (GameObject)Instantiate(OptionInput);
                    OptionEctype.transform.parent = this.Tween.transform;
                    OptionEctype.transform.localScale = Vector3.one;
                    OptionEctype.transform.localPosition = new Vector3(0, -45 + iRow * -40, 0);
                    OptionEctype.GetComponent<OptionInputPanel>().InitInput(item._EctypeID.ToString(), "副本", groupType);
                    break;
                default:
                    break;
            }
        }

        void OnClick(object obj)
        {
            EditorDataManager.Instance.CurSelectCameraGroup = m_cameraGroup;
        }

        public void InitCameraGroup(int cameraGroupID, int index)
        {
            UILabel.text = cameraGroupID.ToString();
            if (!StroyLineConfigManager.Instance.GetCameraGroupConfig.ContainsKey(cameraGroupID))
                return;

            m_cameraGroup = StroyLineConfigManager.Instance.GetCameraGroupConfig[cameraGroupID];

            int iRow = 0;
            GameObject titleItemA = (GameObject)Instantiate(TitleItem);
            titleItemA.transform.parent = this.Tween.transform;
            titleItemA.transform.localScale = Vector3.one;
            titleItemA.transform.localPosition = new Vector3(25, -45 + iRow * -40, 0);
            titleItemA.GetComponent<TitleItem>().text.text = "镜头片段";

            for (int i = 0; i < m_cameraGroup._CameraID.Count; i++)
            {
                GameObject CameraList = (GameObject)Instantiate(Checkbox);
                CameraList.transform.parent = titleItemA.transform;
                CameraList.transform.localScale = Vector3.one;
                CameraList.transform.localPosition = new Vector3(-20, -45 + i * -40, 0);
                CameraList.GetComponent<EditorCheckbox>().InitCheckbox(m_cameraGroup._CameraID[i],0, m_cameraGroup._CameraID[i].ToString(), CheckboxType.CameraID, -1);
                UICheckbox uiCheckbox = CameraList.GetComponent<UICheckbox>();
                uiCheckbox.radioButtonRoot = titleItemA.transform;
                
            }

            iRow = m_cameraGroup._CameraID.Count;
            GameObject titleItemB = (GameObject)Instantiate(TitleItem);
            titleItemB.transform.parent = this.Tween.transform;
            titleItemB.transform.localScale = Vector3.one;
            titleItemB.transform.localPosition = new Vector3(25, -45 + ++iRow * -40, 0);
            titleItemB.GetComponent<TitleItem>().text.text = "角色序列";

            //克隆动作列表，每个镜头组包含8个动作列表
            for (int i = 0; i < m_cameraGroup._ActionList.Length; i++)
            {
                GameObject ActionList = (GameObject)Instantiate(Checkbox);
                ActionList.transform.parent = titleItemA.transform;
                ActionList.transform.localScale = Vector3.one;
                ActionList.transform.localPosition = new Vector3(-20, -45 + iRow++ * -40, 0);
                ActionList.GetComponent<EditorCheckbox>().InitCheckbox(m_cameraGroup._ActionList[i].NpcID, 0,m_cameraGroup._ActionList[i].NpcID.ToString(), CheckboxType.NpcID, -1);
                UICheckbox uiCheckbox = ActionList.GetComponent<UICheckbox>();
                uiCheckbox.radioButtonRoot = titleItemA.transform;
                
            }
        }
    }
}