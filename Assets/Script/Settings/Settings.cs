using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static Settings _settings;
    public static Toggle Music, Sound, Vocal;
    

    private void Awake()
    {
        _settings = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        //按ESC关闭界面
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    public void Open()
    {
        //显示界面
        gameObject.SetActive(true);
        GameObject.Find("Canvas/SettingsPanel/BGM").GetComponent<Toggle>().isOn = SettingsConfigs.isMusicOpen;
        GameObject.Find("Canvas/SettingsPanel/SoundEffect").GetComponent<Toggle>().isOn = SettingsConfigs.isSoundOpen;
        GameObject.Find("Canvas/SettingsPanel/VocalEffect").GetComponent<Toggle>().isOn = SettingsConfigs.isVocalOpen;
    }

    //关闭界面
    public void Close()
    {
        SettingsConfigs.isMusicOpen = GameObject.Find("Canvas/SettingsPanel/BGM").GetComponent<Toggle>().isOn;
        SettingsConfigs.isSoundOpen = GameObject.Find("Canvas/SettingsPanel/SoundEffect").GetComponent<Toggle>().isOn;
        SettingsConfigs.isVocalOpen = GameObject.Find("Canvas/SettingsPanel/VocalEffect").GetComponent<Toggle>().isOn;
        gameObject.SetActive(false);
    }
}
