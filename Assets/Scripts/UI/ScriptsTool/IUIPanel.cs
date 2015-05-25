using UnityEngine;
using System.Collections;


public abstract class IUIPanel : ViewNotifier
{
    public abstract void Show();
    public abstract void Close();
    public abstract void DestroyPanel();   
}
