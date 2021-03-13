using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectUI : MonoBehaviour
{
    //对话选项UI
    public string selectKey;//选项触发方法Key
    //刷新UI
    public void UpdateUI(string text, string selectKey)
    {
        this.selectKey = selectKey;
        transform.GetChild(0).GetComponent<Text>().text = text;//选项文字
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    //点击
    public void OnClick()
    {
        //触发对应事件
        SelectManager._selectManager.SendMessage(selectKey);
    }
}
