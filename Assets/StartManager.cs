using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public AudioMixer master;
    void Start()
    {
        PlayerPrefs.SetInt("Spins", 0);
    }
    
    public void LoadFirstScene()
    {
        SceneManager.LoadScene("Game"); // Replace "FirstScene" with the actual name of your first scene
    }

    public void AdjustAudioVolume(float val)
    {
        master.SetFloat("Master", val);
    }
}
