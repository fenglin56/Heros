using UnityEngine;
using System.Collections;

public class LoadingSceneUIManager : IUIPanel {

    //public GameObject LoadTownPanel;
    public GameObject LoadBattlePanel;

    //private IUIPanel LoadBattleUI, LoadTownUI;

    private GameObject LoadBattleUI, LoadTownUI;

    void Awake()
    {
        GameDataManager.Instance.dataEvent.RegisterEvent(DataType.LoadingSceneData,ShowLoadingPanel);
    }

    void Destroy()
    {
        //Debug.LogWarning("DestroyGameObj:"+gameObject.name);
        GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.LoadingSceneData, ShowLoadingPanel);
    }

    void ShowLoadingPanel(object obj)
    {
        if (this == null)
        {
            Destroy();
            return;
        }
        LoadSceneData loadSceneData = obj as LoadSceneData;
        if (loadSceneData == null) return;
        switch (loadSceneData.loadSceneType)
        {
            case LoadSceneData.LoadSceneType.Town:
			case LoadSceneData.LoadSceneType.StoryLine:
			case LoadSceneData.LoadSceneType.Battle:
                //TraceUtil.Log("Transform:"+transform);
                if (LoadBattleUI == null) { LoadBattleUI = UI.CreatObjectToNGUI.InstantiateObj(LoadBattlePanel, null); }
                //if (LoadTownUI == null) { LoadTownUI = UI.CreatObjectToNGUI.InstantiateObj(LoadTownPanel, null); }
                //LoadTownUI.Show();
                break;
            default:
                break;
        }
    }

    public override void Show()
    {
    }

    public override void Close()
    {
    }

    public override void DestroyPanel()
    {
        throw new System.NotImplementedException();
    }
}
