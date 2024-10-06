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
    private float[] shuffleDurations = new float[3]; // Array to hold durations for each shuffle
    private float[] shuffleTimers = new float[3]; // Array to hold timers for each shuffle
    private float[] shuffleIntervals = new float[3]; // Array to hold intervals for each shuffle

    public override void Enter()
    {
        base.Enter();
        shuffleDurations[0] = Random.Range(controller.shuffleTimeRange.x, controller.shuffleTimeRange.y);
        shuffleDurations[1] = Random.Range(controller.shuffleTimeRange.x, controller.shuffleTimeRange.y);
        shuffleDurations[2] = Random.Range(controller.shuffleTimeRange.x, controller.shuffleTimeRange.y);
        
        // Initialize timers and intervals
        for (int i = 0; i < shuffleTimers.Length; i++)
        {
            shuffleTimers[i] = 0f; // Reset all timers
            shuffleIntervals[i] = 0f; // Reset all intervals
        }

        foreach (WheelSymbolManager target in controller.wheels)
        {
            target.UpdateSymbols(); // Ensure symbols are updated when entering the spinning state
        }
    }

    public override void Update()
    {
        for (int i = 0; i < shuffleTimers.Length; i++)
        {
            shuffleTimers[i] += Time.deltaTime; // Increment the timer for each shuffle

            // Check if the shuffle duration has been reached
            if (shuffleTimers[i] < shuffleDurations[i])
            {
                shuffleIntervals[i] += Time.deltaTime; // Increment the interval timer

                // Perform shuffle if the interval has been reached
                if (shuffleIntervals[i] >= controller.shuffleIntervalSteps)
                {
                    ShuffleSymbols(controller.wheels[i]); // Shuffle the current wheel
                    shuffleIntervals[i] = 0f; // Reset the interval timer
                }
            }
        }

        // Check if all shuffles are completed
        if (shuffleTimers[0] >= shuffleDurations[0] && 
            shuffleTimers[1] >= shuffleDurations[1] && 
            shuffleTimers[2] >= shuffleDurations[2])
        {
            controller.ChangeState(new CalculatingResults(controller)); // Change state after all shuffles
        }
    }

    private void ShuffleSymbols(WheelSymbolManager target)
    {
        // Get all spots
        List<Spot> allSpots = target.spots;

        if(target.spawnedSymbols.Count <= 0)
        {
            target.UpdateSymbols();
        }
        List<GameObject> allGameObjects = target.spawnedSymbols;
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
    public List<WheelSymbolManager> wheels = new List<WheelSymbolManager>();
    public Vector2 shuffleTimeRange = new Vector2(1f, 5f); // Minimum and maximum time for shuffling
    public float shuffleIntervalSteps = 3; // Number of steps for shuffle intervals


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
        currentState = new Ready(this);
    }

    void Update()
    {
        currentState?.Update(); // Call the update function of the current state
    }

    /// <summary>
    /// Attempts to change to the Spinning state if the current state is Ready.
    /// </summary>
    public void StartSpinning()
    {
        if (currentState is Ready)
        {
            ChangeState(new Spinning(this));
        }
    }

    public void ChangeState(StateBase newState)
    {
        currentState?.Exit(); // Call Exit on the current state
        newState.previous = currentState;
        currentState = newState; // Change to the new state
        currentState.Enter(); // Call Enter on the new state
    }
}