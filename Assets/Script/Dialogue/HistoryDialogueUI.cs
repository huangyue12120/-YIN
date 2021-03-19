using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryDialogueUI : MonoBehaviour
{
    public Image image;//头像
    public Text text;//文字

    //刷新UI
    public void UpdateData(int count)
    {
        image = transform.Find("Image").GetComponent<Image>();
        text = transform.Find("Text").GetComponent<Text>();

        image.sprite = Resources.Load<Sprite>("Sprite/Character/" + DialogueManager.dialogueExcel.dataArray[count].Character + "_head");
        if(image.sprite == null)
        image.gameObject.SetActive(false);
        text.text = DialogueManager.dialogueExcel.dataArray[count].Text;
    }
}
