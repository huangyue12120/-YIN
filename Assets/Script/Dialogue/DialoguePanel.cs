using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    public static DialoguePanel _dialoguePanel;

    public Dictionary<string,string> characterName = new Dictionary<string, string>();

    public Image image;//人物图像
    public Text dialogueText;//对话文本
    public Transform selectTrans;//选项生成位置
    public GameObject selectUI;//选项UI

    int nowDialogue;//当前对话id

    void Awake() 
    {
        _dialoguePanel = this;

        LoadCharacterName();//加载人物名字字典
    }

    void Update() 
    {
        //鼠标点击进行下一轮对话
        if(Input.GetMouseButtonDown(0))
        {
            NextDialogue();
        }
    }
    
    //刷新对话界面
    public void UpdateDialogueUI(int dialogueId,string character,string dialogue)
    {
        nowDialogue = dialogueId;//当前对话
        image.sprite = Resources.Load<Sprite>("Sprite/Character/" + character);//头像
        dialogueText.text = "<color=green>" + characterName[character] + "</color> :" + " " +dialogue;//文本
        //关闭选项
        foreach (Transform item in selectTrans)
        {
            Destroy(item.gameObject);
        }
    }

    //下一个对话
    public void NextDialogue()
    {
        switch (DialogueManager.dialogueExcel.dataArray[nowDialogue].Next)
        {
            case "continue":
            DialogueManager._dialogueManager.StartDialogue(DialogueManager.dialogueExcel.dataArray[nowDialogue].Jumpid);
            break;

            case "select":
            if(selectTrans.childCount == 0)
            for (int i = 0; i < DialogueManager.dialogueExcel.dataArray[nowDialogue].Selects.Length; i++)
            {
                Instantiate(selectUI,selectTrans).GetComponent<SelectUI>().UpdateUI(SelectManager.selectExcel.dataArray[DialogueManager.dialogueExcel.dataArray[nowDialogue].Selects[i]].Text,SelectManager.selectExcel.dataArray[DialogueManager.dialogueExcel.dataArray[nowDialogue].Selects[i]].Key);
            }
            break;

            case "end":
            gameObject.SetActive(false);
            break;
        }
    }

    //加载人物名字字典
    public void LoadCharacterName()
    {
        characterName.Add("yinzi","寅子");
    }
}
