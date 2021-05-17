using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager _soundManager;
    public static AudioSource _soundEffect;
    // Start is called before the first frame update
    void Awake()
    {
        _soundManager = this;
        _soundEffect = _soundManager.GetComponent<AudioSource>();
    }

    public AudioClip setAudio(string name)
    {
        return Resources.Load<AudioClip>("Sound/" + name);
    }

    public string getAudio(AudioClip audio)
    {
        return audio.name;
    }

    public void changeMusic(string audioName)
    {
        _soundEffect.clip = setAudio(audioName);
        _soundEffect.loop = false;
        _soundEffect.Play();
    }

    public void stopMusic()
    {
        _soundEffect.clip = null;
        _soundEffect.Stop();
    }

    public void checkIfChange(int dialogueID)
    {
        if (SettingsConfigs.isVocalOpen)
        {
            if (DialogueManager.dialogueExcel.dataArray[dialogueID].SoundEffect == "null")
            {
                _soundEffect.clip = null;
            }
            else
            {
                changeMusic(DialogueManager.dialogueExcel.dataArray[dialogueID].SoundEffect);
            }
        }
        else
        {
            stopMusic();
        }
    }
}
