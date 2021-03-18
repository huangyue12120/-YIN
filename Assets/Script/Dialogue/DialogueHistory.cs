using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using System;

/**
 * 
 * @author  CSTG-工具寅
 * @data    2021-3-18
 * 
 */

/**
  * 从用户（也就是我自己）阅读体验的角度想了想，打算做一个历史消息界面
  * TODO: 1.目前无法做到动态生成，生成后就放在那里了，关掉界面也只是隐藏了而已，会占用内存，暂时不知道怎么办
  *       2.还有就是ScrollBar的控制部分没做（不知道咋搞）
  * 
  */
public class DialogueHistory : MonoBehaviour
{
    public static DialogueHistory _dialogueHistory;

    public static bool isScreenUp;
    public Transform histroyTran;
    public ItemUI item;
    public RectTransform contentRectTrans;


    private void Awake()
    {
        _dialogueHistory = this;
        isScreenUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isScreenUp && Input.GetKeyDown(KeyCode.Escape))
        {
            close();
        }
    }

    public void showHistory(int nowDialogueID)
    {
        for (int i = 0; i <= nowDialogueID; i++)
        {
            ItemUI ui = Instantiate(item, contentRectTrans);
            ui.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, - i * 100.0F);
            ui.UpdateData(i);
        }
    }

    public void open(int count)
    {
        if (!isScreenUp)
        {
            foreach (Transform item in DialoguePanel._dialoguePanel.historyTrans)
            {
                item.gameObject.SetActive(true);
            }
            isScreenUp = true;
            showHistory(count);
        }
        else
        {
            return;
        }
    }

    public void close()
    {
        isScreenUp = false;
        foreach(Transform item in DialoguePanel._dialoguePanel.historyTrans)
        {
            item.gameObject.SetActive(false);
        }
    }

}
