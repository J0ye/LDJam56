using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModInventoryElement : MonoBehaviour
{
    SlotMod _mod;

    [SerializeField]
    Image image;

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
        image.sprite = mod.image;
    }

    public SlotMod GetMod() => _mod;
}
