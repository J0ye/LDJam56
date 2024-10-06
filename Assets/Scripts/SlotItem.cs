using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotItem : MonoBehaviour
{
    private AdditionalSlot _slot;

    public Vector3 targetPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(AdditionalSlot slot)
    {
        _slot = slot;
    } 
}
