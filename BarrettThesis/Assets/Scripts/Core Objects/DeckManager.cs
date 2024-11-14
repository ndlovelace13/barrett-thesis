using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager DeckManage;

    public List<Flashcard> cardQueue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DeckManage = this;
    }

    public void AssignTasks()
    {
        //create a new cardQueue
        cardQueue = new List<Flashcard>();

        //retrieve the new cards to add to the mix
        int startIndex = GameController.SaveData.newestIndex;
        for (int i = 0; i < GameController.SaveData.newPerDay; i++)
        {
            cardQueue.Add(GameController.SaveData.currentDeck.cards[startIndex + i]);
        }
        GameController.SaveData.newestIndex = startIndex + GameController.SaveData.newPerDay - 1;

        Debug.Log(cardQueue.Count);

    }

    public void DateCheck(DateTime lastLogin)
    {
        //check the lastLogin vs. currentTime
    }
}
