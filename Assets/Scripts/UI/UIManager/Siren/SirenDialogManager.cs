using UnityEngine;
using System.Collections;

public class SirenDialogManager : MonoBehaviour 
{
    public GameObject DialogBox;
    public SpriteSwith Switch_DialogBg;
    public UILabel Label;

    private const float fShowTime = 2f;

    public void ShowDialog(SirenDialogConfigData dialogData)
    {
        if (DialogBox.activeInHierarchy)
        {
            StopAllCoroutines();
        }

        Switch_DialogBg.ChangeSprite(dialogData.Rows);
        Label.text = LanguageTextManager.GetString(dialogData.IDS);
        DialogBox.transform.localPosition = new Vector3(dialogData.Pos.x, dialogData.Pos.y, 0);
        DialogBox.SetActive(true);
        StartCoroutine("CloseDialog");
    }

    IEnumerator CloseDialog()
    {
        yield return new WaitForSeconds(fShowTime);
        DialogBox.SetActive(false);
    }

    /// <summary>
    /// 关闭对话框
    /// </summary>
    public void CloseDialogImmediately()
    {
        if (DialogBox.activeInHierarchy)
        {
            StopAllCoroutines();
            DialogBox.SetActive(false);
        }        
    }

}
