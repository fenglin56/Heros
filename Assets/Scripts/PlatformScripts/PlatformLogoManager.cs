using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏城镇主界面放置平台Logo，点击可以平台跳转
/// TownUIScene.unity   TownUIRoot->Town_Panel ->PlatformLogoPos
/// </summary>
public class PlatformLogoManager : MonoBehaviour {

    public GameObject OppoLogo;
    public GameObject MiLogo;
    public GameObject UCLogo;

    private UIEventListener m_currentPlatformLogo;

    public void Start()
    {
        InitLogoViaPlatform();
    }
    private void InitLogoViaPlatform()
    {
        switch (GameManager.Instance.PlatformType)
        {
            case PlatformType.OPPO:
                m_currentPlatformLogo=NGUITools.AddChild(gameObject, OppoLogo.gameObject).GetComponent<UIEventListener>();
                m_currentPlatformLogo.onClick = (go) =>
                {
                    //链接到平台
                    JHPlatformConnManager.Instance.ShowPlatformInfo();
                };
                break;
            case PlatformType.MI:
                m_currentPlatformLogo = NGUITools.AddChild(gameObject, MiLogo.gameObject).GetComponent<UIEventListener>();
                break;
            case PlatformType.UC:
                m_currentPlatformLogo = NGUITools.AddChild(gameObject, UCLogo.gameObject).GetComponent<UIEventListener>();
                break;
        }

    }
}
