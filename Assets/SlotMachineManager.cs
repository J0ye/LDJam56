using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase
{
    public SlotMachineManager controller;
    public StateBase previous;

    // Removed the constructor to allow inheriting classes to define their own
    public StateBase(SlotMachineManager newController)
    {
        controller = newController;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void Calculate() { }

    protected void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}

public class Ready : StateBase
{
    public Ready(SlotMachineManager controller) : base(controller) { }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            controller.ChangeState(new Spinning(controller));
        }
    }
}

public class Spinning : StateBase
{
    public Spinning(SlotMachineManager controller) : base(controller) { }

    private float shuffleTimer = 0f;
    private int shuffleInterval = 1;

    /// <summary>
    /// Overrides the Update method from StateBase to implement specific behavior for the Spinning state.
    /// This method increments the shuffle timer and checks if it has reached the shuffle interval.
    /// If the interval is reached, it either increases the interval or changes the state to Ready.
    /// </summary>
    public override void Update()
    {
        shuffleTimer += Time.deltaTime;
        if (shuffleTimer >= shuffleInterval)
        {
            ShuffleSymbols();
            if (shuffleInterval < 4)
            {
                shuffleInterval++;
            }
            else
            {
                controller.ChangeState(new CalculatingResults(controller)); // Change state
            }
            shuffleTimer = 0f;
        }
    }

    private void ShuffleSymbols()
    {
        // Get all spots
        List<Spot> allSpots = SpotList.Instance.GetSpots();

        // ########################################################################
        List<GameObject> allGameObjects = new List<GameObject>(controller.result.Keys); // TO DO: Should be list from mods i think
        foreach (GameObject gameObject in allGameObjects)
        {
            gameObject.SetActive(false);
        }

        // Shuffle the list of GameObjects to ensure randomness
        Shuffle(allGameObjects);

        // Assign each spot to a random GameObject
        for (int i = 0; i < Mathf.Min(allSpots.Count, allGameObjects.Count); i++)
        {
            controller.result[allGameObjects[i]] = allSpots[i].transform.position; // Assigning the spot's position to the result
            allGameObjects[i].transform.position = allSpots[i].transform.position;
            allGameObjects[i].SetActive(true);
        }
    }
}

public class CalculatingResults : StateBase
{
    public CalculatingResults(SlotMachineManager controller) : base(controller) { }

    public override void Enter()
    {
        Debug.Log("Entering Calculating Results State");
    }

    public override void Update()
    {
        controller.ChangeState(new Ready(controller));
    }

    public override void Exit()
    {
        Debug.Log("Exiting Calculating Results State");
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

    public Dictionary<GameObject, Vector3> result = new Dictionary<GameObject, Vector3>();

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