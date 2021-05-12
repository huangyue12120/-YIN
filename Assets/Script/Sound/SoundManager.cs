using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager _soundManager;

    public AudioSource thunder;

    private void Awake() 
    {
        _soundManager = this;
    }


}
