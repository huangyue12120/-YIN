using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public static SelectManager _selectManager;
    public static Select selectExcel;//选项表

    void Awake() 
    {
        _selectManager = this;
        selectExcel = Resources.Load<Select>("Excel/Select");
    }

    public void Test()
    {
        DialogueManager._dialogueManager.StartDialogue(0);
    }
}
