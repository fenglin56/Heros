using UnityEngine;
using System.Collections;

public class DragModel : SpinWithMouse
{


    void OnPress(bool isPressed)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
    }
}
