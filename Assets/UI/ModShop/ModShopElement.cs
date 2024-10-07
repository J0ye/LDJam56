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

    [SerializeField]
    TMP_Text sold;

    [SerializeField]
    public TMP_Text cantAfford;

    public bool isSold = false;

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
        EnableButton(false);
        SlotMachineManager.Instance.CostToSpin += _mod.drawback;
        isSold = true;
        sold.enabled = true;
        ModShop.instance.CheckModCost();
    }

    public void EnableButton(bool state)
    {
        button.interactable = state;
    }
}
