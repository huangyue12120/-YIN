using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEvents : MonoBehaviour
{
    public static DialogueEvents _dialogueEvents;

    void Awake()
    {
        _dialogueEvents = this;
    }
    
    public void Thunder()
    {
        print("轰隆");
    }
}
