using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioScript : MonoBehaviour
{
    public float volume
    {
        get
        {
            return AudioListener.volume;
        }
        set
        {
            AudioListener.volume = Mathf.Clamp(value,0,1);
            if (Bar != null)
                Bar.fillAmount = volume;
            else if(GameObject.Find("VolumeBar"))
            {
                Bar = GameObject.Find("VolumeBar").GetComponent<Image>();
                Bar.fillAmount = volume;
            }
        }
    }

    public Image Bar;
}
