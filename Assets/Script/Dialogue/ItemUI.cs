using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image head;
    public Text Logs;

    //刷新UI
    public void UpdateData(int count)
    {
        if (DialogueManager.dialogueExcel.dataArray[count].Character.Equals("null"))
        {
            head.gameObject.SetActive(false);
            Logs.gameObject.SetActive(true);
            Logs.text = DialogueManager.dialogueExcel.dataArray[count].Text;
        }
        else
        {
            head.gameObject.SetActive(true);
            Logs.gameObject.SetActive(true);
            head.sprite = Resources.Load<Sprite>("Sprite/Character/" + DialogueManager.dialogueExcel.dataArray[count].Character + "_head");
            Logs.text = DialoguePanel._dialoguePanel.characterName[DialogueManager.dialogueExcel.dataArray[count].Character] + ": " + DialogueManager.dialogueExcel.dataArray[count].Text;
        }
    }

}
