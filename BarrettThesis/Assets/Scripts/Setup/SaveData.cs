using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string playerName;
    //public Flashcard testCard;
    public Deck currentDeck;

    //progression
    public int newestIndex;
    public int cardCount;
    public int balance;
    public int jarBalance;
    public int completeDays;
    public int dayIndex = 0;
    public int donationsToday;

    public List<Flashcard> newQueue;
    public List<Flashcard> cardQueue;
    public List<TaskType> taskMenu;

    public bool tasksComplete;

    //museum orientation
    public List<Placeable> placeables;
    public int maxPaintings = 2;
    public int maxPillars = 0;
    public int maxSeating = 0;

    //orders
    public List<Placeable> orderedPlaceables;
    public List<Placeable> newOrders;
    public int deliveriesToday;
    //public bool ordersReady = false;

    //visitors
    public bool museumOpen;
    public int maxVisitors;
    public int visitorsToday;

    //settings
    public int newPerDay = 15;


    public string saveTime;
    public string refreshTime;

    public SaveData()
    {
        newestIndex = 0;
        dayIndex = 0;

        tasksComplete = false;
        museumOpen = false;

        balance = 0;
        jarBalance = 0;
        playerName = "Hugh Mungus";
        completeDays = 0;
        saveTime = DateTime.UtcNow.ToString();
        refreshTime = DateTime.UtcNow.AddHours(24).ToString();

        maxVisitors = 2;

        newQueue = new List<Flashcard>();
        cardQueue = new List<Flashcard>();
        placeables = new List<Placeable>();

        //order init
        orderedPlaceables = new List<Placeable>();
        newOrders = new List<Placeable>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public DateTime GetSaveTime()
    {
        DateTime lastSave = DateTime.Parse(saveTime);
        return lastSave;
    }

    public DateTime GetRefreshTime()
    {
        DateTime refreshDate = DateTime.Parse(refreshTime);
        return refreshDate;
    }
}
