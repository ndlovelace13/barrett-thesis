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
    public int completeDays;
    public int dayIndex = 0;

    public List<Flashcard> newQueue;
    public List<Flashcard> cardQueue;
    public List<TaskType> taskMenu;

    public bool tasksComplete;

    //settings
    public int newPerDay = 15;


    public string saveTime;
    public string refreshTime;

    public SaveData()
    {
        newestIndex = 0;
        dayIndex = 0;

        tasksComplete = false;
        balance = 0;
        playerName = "Hugh Mungus";
        completeDays = 0;
        saveTime = DateTime.UtcNow.ToString();
        refreshTime = DateTime.UtcNow.AddHours(24).ToString();

        newQueue = new List<Flashcard>();
        cardQueue = new List<Flashcard>();
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
}
