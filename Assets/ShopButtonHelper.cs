using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ShopButtonHelper : MonoBehaviour
{
    public string alternateText = "Close Shop";
    private TMP_Text textObject;
    private string startText ="";
    void Awake()
    {
        textObject = transform.GetChild(0).GetComponent<TMP_Text>();
        startText = textObject.text;
    }

    public void SwitchText()
    {
        if(textObject.text == startText)
        {
            textObject.text = alternateText;
        }
        else
        {
            textObject.text = startText;
        }
    }
}
