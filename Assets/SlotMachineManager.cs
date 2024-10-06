using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase
{
    public StateBase previous;

    public virtual void Enter() {}
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void Calculate() { }
}

public class Ready : StateBase
{
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SlotMachineManager.Instance.ChangeState(new Spinning());
        }
    }
}

public class Spinning : StateBase
{
    public override void Update()
    {        
        // DO BIG SPIN
        // YAS
    }
}

/// <summary>
/// Singleton class for managing the Slot Machine state.
/// Ensures that only one instance of SlotMachineManager exists in the game,
/// providing global access to it through the Instance property.
/// </summary>
public class SlotMachineManager : MonoBehaviour
{
    private static SlotMachineManager instance; // Singleton instance

    public static SlotMachineManager Instance // Public property to access the instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("There is no slot machine");
            }
            return instance;
        }
    }

    private StateBase currentState;

    void Start()
    {
        // Singelton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Update()
    {
        currentState?.Update(); // Call the update function of the current state
    }

    public void ChangeState(StateBase newState)
    {
        currentState?.Exit(); // Call Exit on the current state
        newState.previous = currentState;
        currentState = newState; // Change to the new state
        currentState.Enter(); // Call Enter on the new state
    }
}