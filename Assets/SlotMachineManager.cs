using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;
using System.Collections;

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
        controller.spins++;
        ModShop.instance.GenerateRandomMods();
        PlayerPrefs.SetInt("Spins", controller.spins);
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
            controller.GetComponent<AudioSource>().Play();
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
            controller.GetComponent<AudioSource>().Stop();
            controller.GetComponent<AudioSource>().PlayOneShot(controller.spinEndAudioClip);
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
    private Vector3 targetPosition; // Target position for acorns

    private int score = 0;
    private int mult = 0;

    public CalculatingResults(SlotMachineManager controller) : base(controller) { }

    public override void Enter()
    {
        score = 0;
        mult = 0;
        Debug.Log("Entering Calculating Results State");
        var slots = ModInventory.instance.GetMods().Where(i => i.GetType() == "multiplicator").ToList();

        foreach (var entry in controller.result)
        {
            Spot spot = entry.Key; // Get the spot
            AdditionalSlot additionalSlot = entry.Value; // Get the associated AdditionalSlot
            score += additionalSlot.INEEDMONEY(controller.score, spot);
        }

        foreach (Multiplicator multi in slots)
        {
            mult += multi.INEEDMONEY(score, controller.result);
            Debug.Log($"Current Score: {score}, Multiplicator Name: {multi.name}");
        }

        if (mult == 0)
        {
            mult = 1;
        }

        // Set the target position to the top right corner of the screen
        targetPosition = new Vector3(12, 6, 0);

        // Spawn acorns for each point scored
        SlotMachineManager.Instance.SpawnAcorns(score);
    }


    public override void Update()
    {
        // Move each acorn towards the target position
        for (int i = controller.spawnedAcorns.Count - 1; i >= 0; i--)
        {
            GameObject acorn = controller.spawnedAcorns[i];
            if (acorn != null)
            {
                // Move the acorn towards the target position
                float randomSpeed = Random.Range(controller.acornSpeed.x, controller.acornSpeed.y); // Get a random speed between x and y
                acorn.transform.position = Vector3.MoveTowards(acorn.transform.position, targetPosition, Time.deltaTime * randomSpeed); // Adjust speed as needed
                // Check if the acorn has reached the target position
                if (Vector3.Distance(acorn.transform.position, targetPosition) < 0.1f)
                {
                    Object.Destroy(acorn); // Destroy the acorn when it reaches the target
                    controller.spawnedAcorns.RemoveAt(i); // Remove from the list

                    if (controller.spawnedAcorns.Count <= 0)
                    {
                        controller.ChangeState(new Ready(controller));
                    }
                }
            }
        }
    }

    public override void Exit()
    {
        controller.score += score * mult;
        if (!controller.ScoreBiggerCost())
        {
            SceneManager.LoadScene("End");
        }
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

public class SetUp : StateBase
{
    private Vector3 targetPosition;
    private float duration = 2f; // Duration for the animation
    private float elapsedTime = 0f;
    private bool isAnimating = false;
    public SetUp(SlotMachineManager controller) : base(controller) { }

    public override void Enter()
    {
        base.Enter();
        targetPosition = new Vector3(0, 0, -8);
        elapsedTime = 0f;
        isAnimating = true;
    }

    public override void Update()
    {
        if (isAnimating)
        {
            elapsedTime += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, (elapsedTime / duration));

            // Check if the animation is complete
            if (elapsedTime >= duration)
            {
                Camera.main.transform.position = targetPosition; // Ensure the final position is set
                isAnimating = false; // Stop the animation
                controller.ChangeState(new Ready(controller));
            }
        }
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

    public GameObject acornPrefab;
    public Dictionary<Spot, AdditionalSlot> result = new Dictionary<Spot, AdditionalSlot>();
    public List<WheelSymbolManager> wheels = new List<WheelSymbolManager>();
    public Vector2 shuffleTimeRange = new Vector2(1f, 5f); // Minimum and maximum time for shuffling
    public float shuffleIntervalSteps = 3; // Number of steps for shuffle intervals
    public int startingCost = 2;
    public int startingScore = 3;
    public int spinCostIncreasesAfter = 3;
    public int spins = 0;
    public AudioClip spinEndAudioClip;

    [Header("UI")]
    public GameObject modShopUI;
    public TMP_Text scoreText;
    public Vector2 acornSpeed = new Vector2(5f, 15f);
    private int _score = 3; // Backing field for score

    public List<GameObject> spawnedAcorns = new List<GameObject>(); // New list to hold spawned acorns

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
        currentState = new SetUp(this);
        currentState.Enter();

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
        if (currentState is InShop)
        {
            ChangeState(new Ready(this));
        }
        else if (currentState is Ready)
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
        if (payCostCallCount >= spinCostIncreasesAfter)
        {
            CostToSpin += 1; // Increase CostToSpin by 3
            payCostCallCount = 0; // Reset the counter
        }
    }

    public bool ScoreBiggerCost()
    {
        return score >= costToSpin;
    }


    public void SpawnAcorns(int score)
    {
        // Spawn acorns for each point scored
        int t = 0;
        for (int i = 0; i < score; i++)
        {
            StartCoroutine(SpawnAcornEnumerator(t, i * 0.08f));
            t++;
            if (t >= wheels.Count)
            {
                t = 0;
            }
        }
    }

    IEnumerator SpawnAcornEnumerator(int t, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject acorn = Object.Instantiate(acornPrefab); // Spawn acorn
        spawnedAcorns.Add(acorn); // Add to the list

        Vector3 wheelPosition = wheels[t].transform.position; // Get the wheel's position
        acorn.transform.position = wheelPosition + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0); // Add random offset
        acorn.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f)); // Set random rotation around z-axis
    }
}