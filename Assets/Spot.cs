using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spot : MonoBehaviour
{
    [HideInInspector]
    public bool isTaken; // Indicates if the spot is taken
    
    void Awake()
    {
        SpotList.Instance.AddSpot(this);
    }
}

public class SpotList
{
    private static SpotList _instance;
    private List<Spot> _spots = new List<Spot>();

    /// <summary>
    /// Gets the singleton instance of the SpotList class.
    /// If the instance does not exist, it creates a new one to ensure
    /// that there is only one SpotList managing all spots in the game.
    /// </summary>
    public static SpotList Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SpotList();
            }
            return _instance;
        }
    }

    private SpotList() { } // Private constructor to prevent instantiation

    public void AddSpot(Spot spot)
    {
        _spots.Add(spot);
    }

    // Method to get a random Spot that is not taken
    public Spot GetRandomAvailableSpot()
    {
        List<Spot> availableSpots = _spots.FindAll(spot => !spot.isTaken);
        if (availableSpots.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSpots.Count);
            return availableSpots[randomIndex];
        }
        return null; // Return null if no available spots
    }

    public void ResetAllSpots()
    {
        foreach (Spot spot in _spots)
        {
            spot.isTaken = false;
        }
    }

    public List<Spot> GetSpots()
    {
        return _spots;
    }
}