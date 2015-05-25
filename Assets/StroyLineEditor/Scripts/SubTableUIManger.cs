using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StroyLineEditor
{
    public enum GroupType
    {
        MAPID,
        BGMUSIC,
        CONDITION,
        ECTYPEID,
    }

    public struct GroupItem
    {
        public GroupType GroupType;
        public StroyLineConfigData StroyLineData;
    }

    public class SubTableUIManger : MonoBehaviour
    {
        public GameObject ActionGroupItem;
        private List<GameObject> m_curObjectList = new List<GameObject>();

        // Use this for initialization
        void InitActionGroup(StroyLineConfigData initStroyLineData)
        {
            for (int i = 0; i < EditorDataManager.Instance.StroyLineTempData.Count; i++)
            {
                if (EditorDataManager.Instance.StroyLineTempData[i]._StroyLineID == initStroyLineData._StroyLineID)
                {
                    EditorDataManager.Instance.StroyLineTempData.RemoveAt(i);
                }
            }

            EditorDataManager.Instance.StroyLineTempData.Add(initStroyLineData);

            int iRow = 0;

            GameObject mapIDItem = (GameObject)Instantiate(ActionGroupItem);
            mapIDItem.transform.parent = this.transform;
            mapIDItem.transform.localScale = Vector3.one;
            mapIDItem.transform.localPosition = new Vector3(0, -20 + iRow * -40, 0);
            mapIDItem.GetComponent<ActionGroupItem>().InitGroupItem(initStroyLineData, GroupType.MAPID);
            m_curObjectList.Add(mapIDItem);

            iRow += 1;
            GameObject bgMusicItem = (GameObject)Instantiate(ActionGroupItem);
            bgMusicItem.transform.parent = this.transform;
            bgMusicItem.transform.localScale = Vector3.one;
            bgMusicItem.transform.localPosition = new Vector3(0, -20 + iRow * -40, 0);
            bgMusicItem.GetComponent<ActionGroupItem>().InitGroupItem(initStroyLineData, GroupType.BGMUSIC);
            m_curObjectList.Add(bgMusicItem);
            
            iRow += 1;
            GameObject conditionItem = (GameObject)Instantiate(ActionGroupItem);
            conditionItem.transform.parent = this.transform;
            conditionItem.transform.localScale = Vector3.one;
            conditionItem.transform.localPosition = new Vector3(0, -20 + iRow * -40, 0);
            conditionItem.GetComponent<ActionGroupItem>().InitGroupItem(initStroyLineData, GroupType.CONDITION);
            m_curObjectList.Add(conditionItem);

            iRow += 1;
            GameObject ectypeIDItem = (GameObject)Instantiate(ActionGroupItem);
            ectypeIDItem.transform.parent = this.transform;
            ectypeIDItem.transform.localScale = Vector3.one;
            ectypeIDItem.transform.localPosition = new Vector3(0, -20 + iRow * -40, 0);
            ectypeIDItem.GetComponent<ActionGroupItem>().InitGroupItem(initStroyLineData, GroupType.ECTYPEID);
            m_curObjectList.Add(ectypeIDItem);

            for (int i = 0; i < initStroyLineData._CameraGroup.Count; i++)
            {
                iRow += 1;
                GameObject cameraGroupItem = (GameObject)Instantiate(ActionGroupItem);
                cameraGroupItem.transform.parent = this.transform;
                cameraGroupItem.transform.localScale = Vector3.one;
                cameraGroupItem.transform.localPosition = new Vector3(0, -20 + iRow * -40, 0);
                cameraGroupItem.GetComponent<ActionGroupItem>().InitCameraGroup(initStroyLineData._CameraGroup[i], i);
                m_curObjectList.Add(cameraGroupItem);
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (EditorDataManager.Instance.IsUpdateStroyUI)
            {
                OnDestroy();
                InitActionGroup(EditorDataManager.Instance.CurSelectStroyData);
                EditorDataManager.Instance.IsUpdateStroyUI = false;
            }
        }

        void OnDestroy()
        {
            foreach (GameObject item in m_curObjectList)
            {
                Destroy(item);
            }
        }
    }
}