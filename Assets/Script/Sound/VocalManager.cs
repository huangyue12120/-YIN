using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VocalManager : MonoBehaviour
{
    public static VocalManager _vocalManager;
    public static AudioSource _vocalEffect;
    private void Awake()
    {
        _vocalManager = this;
        _vocalEffect = _vocalManager.GetComponent<AudioSource>();
    }

    public AudioClip setAudio(string name)
    {
        return Resources.Load<AudioClip>("Vocal/" + name);
    }

    public string getAudio(AudioClip audio)
    {
        return audio.name;
    }

    public void changeMusic(string audioName)
    {
        _vocalEffect.clip = setAudio(audioName);
        _vocalEffect.loop = false;
        _vocalEffect.Play();
    }
    public void stopMusic()
    {
        _vocalEffect.clip = null;
        _vocalEffect.Stop();
    }

    public void checkIfChange(int dialogueID)
    {
        if (SettingsConfigs.isVocalOpen)
        {
            if (DialogueManager.dialogueExcel.dataArray[dialogueID].VocalEffect == "null")
            {
                _vocalEffect.clip = null;
            }
            else
            {
                changeMusic(DialogueManager.dialogueExcel.dataArray[dialogueID].VocalEffect);
            }
        }
        else
        {
            stopMusic();
        }
    }
}
