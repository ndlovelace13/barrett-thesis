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
    public int studyStreak;
    public int dayIndex = 0;
    public int donationsToday;
    public bool cardsUnboxed;


    //achievement shit
    public int highestMastery;
    public int unlockedCardCount;
    public int highestStudyStreak;
    public int placeableCount;
    public int additionalRooms;
    

    public List<Flashcard> newQueue;
    public List<Flashcard> cardQueue;
    public List<Flashcard> artQueue;
    public List<TaskType> taskMenu;

    public bool tasksComplete;

    //tutorial shit
    public List<string> completedTutorials;

    //museum orientation
    public List<Placeable> placeables;
    public List<PlaceableControl> placeableControls;

    //museum expansions
    public List<RoomData> roomData;
    public int currentRoomCost;

    //achievements
    public List<ArtifactData> artifactData;
    public int unlockedArtifacts;

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
        studyStreak = 0;
        saveTime = DateTime.UtcNow.ToString();
        refreshTime = DateTime.UtcNow.AddHours(24).ToString();
        cardsUnboxed = false;
        
        //tutorial init
        completedTutorials = new List<string>();

        //achievement init
        highestMastery = 0;
        unlockedCardCount = 0;
        highestStudyStreak = 0;
        placeableCount = 0;
        additionalRooms = 0;

        maxVisitors = 2;

        newQueue = new List<Flashcard>();
        cardQueue = new List<Flashcard>();
        artQueue = new List<Flashcard>();
        placeables = new List<Placeable>();
        
        //artifact init
        artifactData = new List<ArtifactData>();
        unlockedArtifacts = 0;

        //expansion init
        roomData = new List<RoomData>();
        currentRoomCost = 10000;
        InitRoom();

        //order init
        orderedPlaceables = new List<Placeable>();
        newOrders = new List<Placeable>();
        placeableControls = new List<PlaceableControl>();
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

    public void InitRoom()
    {
        if (roomData.Count == 0)
        {
            //instantiate the starting room
            RoomData firstRoom = new RoomData(0, 0, 0);
            roomData.Add(firstRoom);

            //instantiate the office
            RoomData office = new RoomData(1);
            roomData.Add(office);

            //connect the two
            firstRoom.LinkLeft(office);

            //save game
            //SaveHandler.SaveSystem.SaveGame();
        }
    }
}
