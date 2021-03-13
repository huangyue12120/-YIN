using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager _dialogueManager;
    public static Dialogue dialogueExcel;//对话表
    
    void Awake() 
    {
        _dialogueManager = this;
        dialogueExcel = Resources.Load<Dialogue>("Excel/Dialogue");
    }

    void Start() 
    {
        //test use
        StartDialogue(0);
    }

    //根据对话ID触发对话
    public void StartDialogue(int dialogueId)
    {
        DialoguePanel._dialoguePanel.UpdateDialogueUI(dialogueId,dialogueExcel.dataArray[dialogueId].Character,dialogueExcel.dataArray[dialogueId].Text);
    }
}
