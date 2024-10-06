using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

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
        controller.PayCost();
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
            target.StartAllParticleSystems();
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
                // Perform shuffle if the interval has been reached
                if (shuffleTimers[i] >= shuffleIntervals[i])
                {
                    ShuffleSymbols(controller.wheels[i]); // Shuffle the current wheel
                    shuffleIntervals[i] += controller.shuffleIntervalSteps; // Increment the interval timer
                }
            }
            else
            {
                controller.wheels[i].StopAllParticleSystems();
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

        if (target.spawnedSymbols.Count <= 0)
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

        for (int i = 0; i < Mathf.Min(allSpots.Count, allGameObjects.Count); i++)
        {
            // Check if the spot already exists in the result dictionary
            if (!controller.result.ContainsKey(allSpots[i]))
            {
                controller.result.Add(allSpots[i], allGameObjects[i].GetComponent<SlotItem>()._slot); // Assigning the spot's position to the result
            }
            else
            {
                controller.result[allSpots[i]] = allGameObjects[i].GetComponent<SlotItem>()._slot;
            }
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
        int score = 0;
        int mult = 0;
        Debug.Log("Entering Calculating Results State");
        var slots = ModInventory.instance.GetMods().Where(i => i.GetType() == "multiplicator").ToList();

        foreach (var entry in controller.result)
        {
            Spot spot = entry.Key; // Get the spot
            AdditionalSlot additionalSlot = entry.Value; // Get the associated AdditionalSlot
            //Debug.Log($"Processing slot: {additionalSlot.name} at spot: {spot.name}. Is it main? {spot.isMain}");
            score += additionalSlot.INEEDMONEY(controller.score, spot);
        }

        foreach(Multiplicator multi in slots)
        {
            mult += multi.INEEDMONEY(score, controller.result);
            Debug.Log($"Current Score: {score}, Multiplicator Name: {multi.name}");
        }

        if (mult == 0)
        {
            mult = 1;
        }

        controller.score += score*mult;
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

public class InShop : StateBase
{
    public InShop(SlotMachineManager controller) : base(controller) { }

    public override void Enter()
    {
        base.Enter();
        controller.modShopUI.SetActive(true);
    }

    public override void Exit()
    {
        base.Exit();
        controller.modShopUI.SetActive(false);
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

    private StateBase currentState;
    public StateBase CurrentState => currentState;

    public Dictionary<Spot, AdditionalSlot> result = new Dictionary<Spot, AdditionalSlot>();
    public List<WheelSymbolManager> wheels = new List<WheelSymbolManager>();
    public Vector2 shuffleTimeRange = new Vector2(1f, 5f); // Minimum and maximum time for shuffling
    public float shuffleIntervalSteps = 3; // Number of steps for shuffle intervals
    public int startingCost = 2;
    public int startingScore = 3;
    
    [Header ("UI")]
    public GameObject modShopUI;
    public TMP_Text scoreText;
    private int _score = 3; // Backing field for score

    public int score 
    {
        get => _score;
        set
        {
            _score = value;
            scoreText.text = _score.ToString(); // Update the score text
        }
    }
    public TMP_Text costText;
    private int costToSpin = 0;
    public int CostToSpin // Public property to access and set costToSpin
    {
        get => costToSpin;
        set
        {
            costToSpin = value;
            costText.text = costToSpin.ToString(); // Update the cost text
        }
    }

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

    void Awake()
    {
        // Singelton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        currentState = new Ready(this);

        score = startingScore;
        CostToSpin = startingCost;
        modShopUI.SetActive(false);
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

    public void OpenShop()
    {
        if(currentState is InShop)
        {
            ChangeState(new Ready(this));
        }
        else if(currentState is Ready)
        {
            ChangeState(new InShop(this));
        }
    }

    public void ChangeState(StateBase newState)
    {
        currentState?.Exit(); // Call Exit on the current state
        newState.previous = currentState;
        currentState = newState; // Change to the new state
        currentState.Enter(); // Call Enter on the new state
    }

    private int payCostCallCount = 0; // Counter for PayCost calls

    public void PayCost()
    {
        payCostCallCount++; // Increment the call count
        score -= costToSpin;

        // Check if the method has been called three times
        if (payCostCallCount >= 3)
        {
            CostToSpin += 1; // Increase CostToSpin by 3
            payCostCallCount = 0; // Reset the counter
        }
    }

    public bool ScoreBiggerCost()
    {
        return score >= costToSpin;
    }
}