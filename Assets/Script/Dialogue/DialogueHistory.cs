using UnityEngine;
using System.Collections.Generic;


/**
 * 
 * @author  CSTG-工具寅
 * @data    2021-3-18
 *              
 */
public class DialogueHistory : MonoBehaviour
{
    public static DialogueHistory _dialogueHistory;
    public List<int> historyDialogueIDs = new List<int>();//用于保存已出现的对话
    public Transform histroyTran;//UI生成位置
    public GameObject historyDialogueUI;//历史对话UI

    private void Awake()
    {
        _dialogueHistory = this;

        gameObject.SetActive(false);
    }

    void Update()
    {
        //按ESC关闭界面
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    //打开并刷新历史对话界面
    public void OpenUI()
    {
        //显示界面
        gameObject.SetActive(true);
        //清空旧的
        foreach (Transform item in histroyTran)
        {
            Destroy(item.gameObject);
        }
        //生成新的
        for (int i = 0; i < historyDialogueIDs.Count; i++)
        {
            Instantiate(historyDialogueUI, histroyTran).GetComponent<HistoryDialogueUI>().UpdateData(historyDialogueIDs[i]);
        }
    }
    //关闭界面
    public void Close()
    {
        gameObject.SetActive(false);
    }

}
