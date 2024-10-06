using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public TMP_Text score;
    

    void Start()
    {
        score.text = PlayerPrefs.GetInt("Spins").ToString();
    }

    public void GoToMain()
    {
        SceneManager.LoadScene(0);
    }
}
