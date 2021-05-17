using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager _musicManager;
    public static AudioSource _backgroundMusic;
    // Start is called before the first frame update
    void Awake()
    {
        _musicManager = this;
        _backgroundMusic = _musicManager.GetComponent<AudioSource>();
    }

    public AudioClip setAudio(string name)
    {
        return Resources.Load<AudioClip>("BGM/" + name);
    }

    public string getAudio(AudioClip audio)
    {
        return audio.name;
    }

    public void changeMusic(string audioName, bool isLoop)
    {
        _backgroundMusic.clip = setAudio(audioName);
        _backgroundMusic.loop = isLoop;
        _backgroundMusic.Play();
    }

    public void stopMusic()
    {
        _backgroundMusic.clip = null;
        _backgroundMusic.Stop();
    }

    public void checkIfChange(int dialogueID)
    {
        if (SettingsConfigs.isMusicOpen)
        {
            if (DialogueManager.dialogueExcel.dataArray[dialogueID].MusicEffect == "null")
            {
                _backgroundMusic.clip = null;
            }else if (_backgroundMusic.clip == null)
            {
                changeMusic(DialogueManager.dialogueExcel.dataArray[dialogueID].MusicEffect, true);
            }
            else if (DialogueManager.dialogueExcel.dataArray[dialogueID].MusicEffect.Equals(getAudio(_backgroundMusic.clip)))
            {
                return;
            }
            else
            {
                changeMusic(DialogueManager.dialogueExcel.dataArray[dialogueID].MusicEffect, true);
            }
        }
        else
        {
            stopMusic();
        }
    }

}
