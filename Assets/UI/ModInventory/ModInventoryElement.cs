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

    [SerializeField]
    TMP_Text ammountText;

    int count = 0;


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

        gameObject.name = mod.name + " Mod";
    }
    
    public void UpdateCount(int amount)
    {
        count += amount;

        // Clear text if count is less than or equal to 0
        if (count <= 0)
        {
            ammountText.text = string.Empty;
        }
        else
        {
            ammountText.text = count.ToString();
        }
    }


    public SlotMod GetMod() => _mod;
}
