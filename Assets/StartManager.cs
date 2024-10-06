using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.SetInt("Spins", 0);
    }
    
    public void LoadFirstScene()
    {
        SceneManager.LoadScene("Game"); // Replace "FirstScene" with the actual name of your first scene
    }
}
