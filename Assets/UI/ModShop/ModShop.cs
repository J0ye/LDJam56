using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModShop : MonoBehaviour
{
    public static ModShop instance { get; private set; }

    [SerializeField]
    ModShopElement _prefab;

    [SerializeField]
    List<SlotMod> _availableMods;

    [SerializeField]
    Button refreshButton;

    [Header("Values")]
    public int rerollCost = 1;

    private int[] PROBABILITY_RATIOS = new int[4]
    {
        60,
        30,
        13,
        5
    };

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateRandomMods();

        UpdateRerollCostText();
    }

    public void GenerateRandomMods()
    {
        //Delete all existing
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < 5; i++)
        {
            var slotMod = GetRandomMod();
            var modShopElement = Instantiate(_prefab, transform);
            modShopElement.Initialize(slotMod);
        }

        CheckModCost();
    }

    private SlotMod GetRandomMod()
    {
        int probabilityWeight = Random.Range(1, 100);
        int probCategory = 1;
        for (int i = 0; i < PROBABILITY_RATIOS.Length; i++)
        {
            if (probabilityWeight < PROBABILITY_RATIOS[i] && (i == PROBABILITY_RATIOS.Length - 1 || probabilityWeight > PROBABILITY_RATIOS[i + 1]))
            {
                probCategory = i + 1;
            }
        }

        var filtered = _availableMods.Where(i => i.probability == probCategory).ToList();
        return filtered[Random.Range(0, filtered.Count)];
    }

    public void RefreshShop()
    {
        if (SlotMachineManager.Instance.score -rerollCost > SlotMachineManager.Instance.CostToSpin)
        {
            SlotMachineManager.Instance.score -= rerollCost;
            GenerateRandomMods();
        }
    }

    public void DisableRefresh()
    {
        refreshButton.interactable = false;
    }

    public void EnableRefresh()
    {
        refreshButton.interactable = true;
    }

    public void CheckModCost()
    {
        foreach (Transform child in transform)
        {
            var el = child.gameObject.GetComponent<ModShopElement>();
            int cost = el.GetMod().drawback;

            if (!el.isSold)
            {
                if (SlotMachineManager.Instance.score - cost >= SlotMachineManager.Instance.CostToSpin)
                {
                    el.cantAfford.enabled = false;
                    el.EnableButton(true);
                }
                else
                {
                    el.cantAfford.enabled = true;
                    el.EnableButton(false);
                }
            }
        }
    }

    private void UpdateRerollCostText()
    {
        foreach(Transform child in refreshButton.transform)
        {
            if(child.TryGetComponent<TMP_Text>(out TMP_Text costText))
            {
                costText.text = rerollCost.ToString();
            }
        }
    }
}
