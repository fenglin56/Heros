using UnityEngine;
using System.Collections;
using UI;

public class VigourMessagePanelManager : MonoBehaviour 
{
	public GameObject BuyVigourMessagePrefab;
	private VigourMessagePanel BuyVigourMessagePanel;

	void Awake()
	{
		UIEventManager.Instance.RegisterUIEvent(UIEventType.NoEnoughActiveLife, ReceiveShowHandle);
	}
	void OnDestroy()
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.NoEnoughActiveLife, ReceiveShowHandle);
	}
	void ReceiveShowHandle(object obj)
	{
		if (BuyVigourMessagePanel == null) { BuyVigourMessagePanel = CreatObjectToNGUI.InstantiateObj(BuyVigourMessagePrefab, transform).GetComponent<VigourMessagePanel>(); }
		string ShowStr = string.Format(LanguageTextManager.GetString("IDS_H1_158"), CommonDefineManager.Instance.CommonDefine.EnergyAdd);
		BuyVigourMessagePanel.Show(string.Format("{0}\n{1}", LanguageTextManager.GetString("IDS_H1_166"), ShowStr));
	}	
}
