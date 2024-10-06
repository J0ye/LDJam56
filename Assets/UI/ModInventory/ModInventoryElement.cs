using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModInventoryElement : MonoBehaviour
{
    SlotMod _mod;

    [SerializeField]
    Image image;

    [SerializeField]
    TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(SlotMod mod)
    {
        _mod = mod;
        GetComponent<Image>().color = mod.color;

        if (mod.image != null)
        {
            image.sprite = mod.image;
        }
        else
        {
            image.enabled = false;
        }

        if (!string.IsNullOrWhiteSpace(mod.tinyText))
        {
            text.text = mod.tinyText;
        }
    }

    public SlotMod GetMod() => _mod;
}
