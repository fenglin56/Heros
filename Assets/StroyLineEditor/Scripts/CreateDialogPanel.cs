using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StroyLineEditor
{
    public class CreateDialogPanel : MonoBehaviour
    {
        enum CreateType
        {
            NONE,
            STROYLINE,
            ACTION,
            CAMERACLIP,
        }

        private CreateType m_createType = CreateType.NONE;

        void CancelEvent(GameObject go)
        {
            HidePanel();
        }

        void OkEvent(GameObject go)
        {
            HidePanel();
            switch (m_createType)
            {
                case CreateType.STROYLINE:
                    StroyLineConfigData stroyline = new StroyLineConfigData();
                    CreateStroyLine(stroyline);
                    break;
                case CreateType.ACTION:
                    StroyActionConfigData actionData = new StroyActionConfigData();
                    CreateStroyAction(actionData);
                    break;
                case CreateType.CAMERACLIP:
                    StroyCameraConfigData cameraData = new StroyCameraConfigData();
                    CreateStroyCameraClip(cameraData);
                    break;
                default:
                    break;
            }

            HidePanel();
        }

        void CreateStroyLine(StroyLineConfigData stroyline)
        {
            stroyline._BgMusic = "";
            stroyline._CameraGroup = new List<int>();
            stroyline._EctypeID = 0;
            stroyline._SceneMapID = 0;
            stroyline._TriggerCondition = 1;
            stroyline._StroyLineID = 0;

            StroyEditorUIManager.Instance.CreateStroyLineData(stroyline);
        }

        void CreateStroyAction(StroyActionConfigData actionData)
        {
            actionData._ActionID = 0;
            actionData._ActionName = "0";
            actionData._ActionType = 0;
            actionData._Acceleration = 0;
            actionData._Duration = 0;
            actionData._EffectGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            actionData._EffectLoopTimes = 0;
            actionData._EffectPosition = Vector2.zero;
            actionData._EffectStartTime = 0;
            actionData._SoundName = "0";
            actionData._SoundTime = 0;
            actionData._Speed = 0;
            actionData._StartAngle = 0;
            actionData._StartPosition = Vector3.zero;
            StroyEditorUIManager.Instance.SetActionUiState(actionData);
        }

        void CreateStroyCameraClip(StroyCameraConfigData cameraData)
        {
            cameraData._CameraID = 0;
            cameraData._ActionTime = 0;
            cameraData._TargetID = 0;
            cameraData._TargetPos = Vector2.zero;
            cameraData._TargetType = 0;
            cameraData._Params = new CameraParam[3];
            for (int i = 0; i < cameraData._Params.Length; i++)
            {
                cameraData._Params[i] = new CameraParam();
                cameraData._Params[i]._EquA = 0;
                cameraData._Params[i]._EquB = 0;
                cameraData._Params[i]._EquC = 0;
                cameraData._Params[i]._EquD = 0;
            }

            StroyEditorUIManager.Instance.SetCameraUiState(cameraData);
        }

        void OnSelectionChange(string createType)
        {
            switch (createType)
            {
                case "创建剧情":
                    m_createType = CreateType.STROYLINE;
                    break;
                case "创建Action":
                    m_createType = CreateType.ACTION;
                    break;
                case "创建CameraClip":
                    m_createType = CreateType.CAMERACLIP;
                    break;
                default:
                    Debug.LogWarning("未知选项，请检查是否有错误！");
                    break;
            }
        }

        void HidePanel()
        {
            transform.localPosition = new Vector3(0, 0, 1000);
        }

        void OnDestroy()
        {
            Destroy(this.gameObject);
        }
    }
}
