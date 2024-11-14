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

    //settings
    public int newPerDay = 15;


    public string saveTime;
    public List<int> nextDayIndex;

    public SaveData()
    {
        newestIndex = 0;
        balance = 0;
        playerName = "Hugh Mungus";
        completeDays = 0;
        saveTime = DateTime.UtcNow.ToString();
        nextDayIndex = new List<int>();
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
