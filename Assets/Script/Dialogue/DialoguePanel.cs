using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 
 * @author       一念
 * @changed by   CSTG-工具寅
 * @change date  2021-3-18
 * @change log:
 * 2021-3-18    添加了人物立绘左侧显示的设置，正在编写历史对话界面（摸）
 *              添加了下一对话对于常用按键（空格、回车、鼠标滚轮的适配）
 * 
 */

public class DialoguePanel : MonoBehaviour
{
    public static DialoguePanel _dialoguePanel;

    public Dictionary<string,string> characterName = new Dictionary<string, string>();

    public Image image;//人物图像
    public Text dialogueText;//对话文本
    public Text dialogueTextLong;//独白文本
    public Transform selectTrans;//选项生成位置
    public Transform textTrans;//人物对话位置
    public GameObject selectUI;//选项UI
    public DialogueHistory dialogueHistory;//历史对话
    int nowDialogue;//当前对话id

    void Awake() 
    {
        _dialoguePanel = this;
        LoadCharacterName();//加载人物名字字典
    }

    void Update() 
    {
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

        }
        //根据文本高度修改对齐
        dialogueText.alignment = dialogueText.preferredHeight > 41 ? TextAnchor.UpperLeft : TextAnchor.UpperCenter;
        dialogueTextLong.alignment = dialogueText.preferredHeight > 41 ? TextAnchor.UpperLeft : TextAnchor.UpperCenter;
    }
    
    //刷新对话界面
    public void UpdateDialogueUI(int dialogueId,string character,string dialogue, bool isShowLeft)
    {
        //关闭选项
        foreach (Transform item in selectTrans)
        {
            Destroy(item.gameObject);
        }
        dialogueHistory.Close();
        //赋值当前对话
        nowDialogue = dialogueId;
        //头像
        if(Resources.Load<Sprite>("Sprite/Character/" + character) != null)
        {
            image.gameObject.SetActive(true);
            //如果头像显示在左侧，则将对话文字、头像的x坐标变为相反数字
            if (isShowLeft)
            {
                image.GetComponent<RectTransform>().anchoredPosition = new Vector2(-757, 72);
                textTrans.GetComponent<RectTransform>().anchoredPosition = new Vector2(165, 55);
            }
            else
            {
                image.GetComponent<RectTransform>().anchoredPosition = new Vector2(757, 72);
                textTrans.GetComponent<RectTransform>().anchoredPosition = new Vector2(-165, 55);
            }
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
        //触发事件
        if(DialogueManager.dialogueExcel.dataArray[nowDialogue].Key != "null")
        DialogueEvents._dialogueEvents.SendMessage(DialogueManager.dialogueExcel.dataArray[nowDialogue].Key);

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
        characterName.Add("huzi","胡子");
        characterName.Add("nvsheng","女生们");
        characterName.Add("zhujue","主角");
        characterName.Add("nver","女二");
    }
}
