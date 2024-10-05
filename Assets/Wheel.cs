using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Wheel : MonoBehaviour
{
    public GameObject slotPrefab;
    public Vector2 startMoveSpeed = new Vector2(15f, 50f);// Initial speed at which the objects will move down
    public float snapThreshold = 0.1f; // Range around y = 0 to check for snapping
    public float snapSpeed = 0.2f;
    public float smoothMoveDuration = 1.0f; // Duration for smooth movement

    private List<GameObject> symbols = new List<GameObject>();
    private List<Vector3> initialPositions = new List<Vector3>(); // New list to store initial positions
    private float currentMoveSpeed; // Current speed during the spin
    private float setSpeed;
    private float resetLimit = 2;
    private bool isSpinning = false; // Flag to check if spinning is active
    private bool symbolsSnap = false;

    void Start()
    {
        currentMoveSpeed = GetRandomSpeed();
        // Store initial positions of symbols
        foreach (GameObject symbol in symbols)
        {
            initialPositions.Add(symbol.transform.position);
        }

        var rnd = new System.Random();

        var slots = ModInventory.instance.GetMods().Where(i => i.GetType() == "slot").ToList();
        //slots = Shuffle(slots).ToList();

        resetLimit = Mathf.RoundToInt(slots.Count / 2) * 2;

        foreach (AdditionalSlot mod in slots)
        {
            var newSymbol = Instantiate(slotPrefab, new Vector3(transform.position.x, resetLimit, transform.position.z), Quaternion.identity, transform);
            newSymbol.GetComponent<SlotItem>().Initialize(mod);
            AddSymbol(newSymbol);
        }
    }
    
    void Update()
    {
        // Start spinning when "E" is pressed
        if (Input.GetKeyDown(KeyCode.E) && !isSpinning)
        {
            isSpinning = true;
            // Reset each symbol to its starting position
            for (int i = 0; i < symbols.Count; i++)
            {
                symbols[i].transform.position = initialPositions[i];
            }
            currentMoveSpeed = GetRandomSpeed();
            setSpeed = currentMoveSpeed;
            StartCoroutine(SpinAndSlowDown());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {            
            var newSymbol = Instantiate(slotPrefab, new Vector3(transform.position.x, resetLimit, transform.position.z), Quaternion.identity);
            AddSymbol(newSymbol);
        }

        if (isSpinning)
        {
            foreach (GameObject symbol in symbols)
            {
                symbol.transform.position += Vector3.down * currentMoveSpeed * Time.deltaTime;              
            }
        }

        if(!symbolsSnap)
        {
            foreach(GameObject symbol in symbols)
            {            
                // Reset position if y is -6 or smaller
                if (symbol.transform.position.y < resetLimit)
                {
                    symbol.transform.position = new Vector3(symbol.transform.position.x, resetLimit * -1, symbol.transform.position.z);
                }
            }
        }
    }

    // New function to set a GameObject at the lowest position y above the reset limit
    public void AddSymbol(GameObject obj)
    {
        float startLimit = resetLimit;
        resetLimit = resetLimit - 2; // Extended limit
        obj.transform.position = new Vector3(transform.position.x, startLimit, transform.position.z);
        symbols.Add(obj);
        initialPositions.Add(obj.transform.position);
    }

    private IEnumerator SpinAndSlowDown()
    {
        float duration = 3.0f; // Duration to slow down
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            currentMoveSpeed = Mathf.Lerp(setSpeed, 0, elapsedTime / duration); // Gradually reduce speed
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
            Debug.Log("Current Move Speed: " + currentMoveSpeed);
            
            // Check if currentMoveSpeed is less than 1 and any symbol is within the snapThreshold around y = 0
            if (currentMoveSpeed < snapSpeed)
            {
                GameObject closestSymbol = null;
                float closestDistance = float.MaxValue;

                // Find the closest symbol above y = 0
                foreach (GameObject symbol in symbols)
                {
                    if (symbol.transform.position.y > 0 && symbol.transform.position.y < closestDistance)
                    {
                        closestDistance = symbol.transform.position.y;
                        closestSymbol = symbol;
                    }
                }

                // If a closest symbol was found, smoothly move it to 0 and adjust others
                if (closestSymbol != null)
                {
                    isSpinning = false; // Stop spinning
                    StartCoroutine(SmoothMoveToZeroAndAdjustOthers(closestSymbol));
                    elapsedTime = Mathf.Infinity;
                }
            }
        }

        currentMoveSpeed = 0; // Ensure speed is set to 0 after the duration
    }

    private IEnumerator SmoothMoveToZeroAndAdjustOthers(GameObject closestSymbol)
    {
        symbolsSnap = true;
        float distanceToSnap = closestSymbol.transform.position.y; // Distance to snap to 0
        Vector3 targetPosition = new Vector3(closestSymbol.transform.position.x, 0, closestSymbol.transform.position.z);

        // Calculate target positions for all symbols
        Vector3[] targetPositions = new Vector3[symbols.Count];
        for (int i = 0; i < symbols.Count; i++)
        {
            if (symbols[i] != closestSymbol)
            {
                //int nearestMultiple = (int)System.Math.Round(( / (double)2), System.MidpointRounding.AwayFromZero) * 2;
                int nearest = Mathf.RoundToInt((symbols[i].transform.position.y - distanceToSnap) / 2) * 2;
                targetPositions[i] = new Vector3(symbols[i].transform.position.x, nearest, symbols[i].transform.position.z);
            }
            else
            {
                targetPositions[i] = targetPosition; // The closest symbol goes to 0
            }
        }

        // Smoothly move all symbols to their target positions
        float elapsedTime = 0f;
        while (elapsedTime < smoothMoveDuration)
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                symbols[i].transform.position = Vector3.Lerp(symbols[i].transform.position, targetPositions[i], elapsedTime / smoothMoveDuration);
            }
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure all symbols are exactly at their target positions after the movement
        for (int i = 0; i < symbols.Count; i++)
        {
            symbols[i].transform.position = targetPositions[i];
        }
        symbolsSnap = false;
    }

    private float GetRandomSpeed()
    {
        return Random.Range(startMoveSpeed.x, startMoveSpeed.y);
    }


    private IList<T> Shuffle<T>(IList<T> list)
    {
        //var newList = list.Select(item => (T)item.Clone()).ToList();

        System.Random rng = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }
}