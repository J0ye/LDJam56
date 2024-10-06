using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModShopElement : MonoBehaviour
{
    SlotMod _mod;

    [SerializeField]
    Image image;

    [SerializeField]
    Button button;

    [SerializeField]
    Image background;

    [SerializeField]
    TMP_Text shopName;

    [SerializeField]
    TMP_Text tinyText;

    [SerializeField]
    TMP_Text description;

    [SerializeField]
    TMP_Text drawback;

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
        if (mod.image != null)
        {
            image.sprite = mod.image;
        }
        else
        {
            image.enabled = false;
        }
        background.color = mod.color;
        shopName.text = mod.name;
        description.text = mod.description;

        if (mod.drawback > 0)
        {
            drawback.text = "+" + mod.drawback.ToString() + "cost";
        }

        if (!string.IsNullOrWhiteSpace(mod.tinyText))
        {
            tinyText.text = mod.tinyText;
        }

        if (mod is Multiplicator)
        {
            var mod2 = mod as Multiplicator;
            //if (mod2.specifi)
            //{
                
            //}
        }
    }

    public SlotMod GetMod() => _mod;

    public void BuyMod()
    {
        ModInventory.instance.AddMod(_mod);
        button.interactable = false;
    }
}
