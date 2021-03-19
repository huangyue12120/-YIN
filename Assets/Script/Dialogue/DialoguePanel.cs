﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    public static DialoguePanel _dialoguePanel;

    public Dictionary<string,string> characterName = new Dictionary<string, string>();

    public Image image;//人物图像
    public Text dialogueText;//对话文本
    public Text dialogueTextLong;//独白文本
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
<<<<<<< Updated upstream
        //鼠标点击进行下一轮对话
        if(Input.GetMouseButtonDown(0))
        {
            NextDialogue();
=======
        if (!DialogueHistory._dialogueHistory.gameObject.activeInHierarchy)
        {
            //鼠标点击、按空格、按回车、鼠标滚轮向下滚动时进行下一轮对话
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                NextDialogue();
            }
            //滚轮向上打开历史对话界面
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                dialogueHistory.OpenUI();
            }
>>>>>>> Stashed changes
        }

        //根据文本高度修改对齐
        dialogueText.alignment = dialogueText.preferredHeight > 41 ? TextAnchor.UpperLeft : TextAnchor.UpperCenter;
        dialogueTextLong.alignment = dialogueText.preferredHeight > 41 ? TextAnchor.UpperLeft : TextAnchor.UpperCenter;
    }
    
    //刷新对话界面
    public void UpdateDialogueUI(int dialogueId,string character,string dialogue)
    {
        //关闭选项
        foreach (Transform item in selectTrans)
        {
            Destroy(item.gameObject);
        }
        //赋值当前对话
        nowDialogue = dialogueId;
        //头像
        if(Resources.Load<Sprite>("Sprite/Character/" + character) != null)
        {
            image.gameObject.SetActive(true);
            image.sprite = Resources.Load<Sprite>("Sprite/Character/" + character);
            dialogueText.gameObject.SetActive(true);//正常文本框
            dialogueTextLong.gameObject.SetActive(false);
        }else
        {
            image.gameObject.SetActive(false);
            dialogueText.gameObject.SetActive(false);//独白文本框
            dialogueTextLong.gameObject.SetActive(true);
        }
        //文本
        dialogueText.text = dialogueTextLong.text =
        !character.Equals("null")
        ?
        "<color=green>" + characterName[character] + ":</color>" + " " +dialogue
        :
        dialogue;
        //添加到历史对话
        DialogueHistory._dialogueHistory.historyDialogueIDs.Add(dialogueId);
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
<<<<<<< Updated upstream

        DialogueEvents._dialogueEvents.SendMessage(DialogueManager.dialogueExcel.dataArray[nowDialogue].Key);
=======
        //触发对话结束后的事件
        //DialogueEvents._dialogueEvents.SendMessage(DialogueManager.dialogueExcel.dataArray[nowDialogue].Key);
>>>>>>> Stashed changes
    }

    //加载人物名字字典
    public void LoadCharacterName()
    {
        characterName.Add("huzi","胡子");
        characterName.Add("nvsheng","女生们");
        characterName.Add("zhujue","主角");
        characterName.Add("nver","女二");
    }
}
