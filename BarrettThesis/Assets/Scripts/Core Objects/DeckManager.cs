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

    //call this when the player opens on a new day
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
        Debug.Log(cardQueue.Count + " new cards added to queue");

        int counter = 0;
        //retrieve all cards that are meant to appear (daysTilNext <= 1)
        foreach (var card in GameController.SaveData.currentDeck.cards)
        {
            if (card.discovered && card.daysTilNext <= 1)
            {
                cardQueue.Add(card);
                card.daysTilNext = 0;
                counter++;
            }
            else
                card.daysTilNext--;
        }
        Debug.Log(counter + " old cards added to queue");

        Debug.Log(cardQueue.Count + " total queue");

        //update all progression variables
        GameController.SaveData.refreshTime = DateTime.UtcNow.AddHours(24).ToString();
        GameController.SaveData.tasksComplete = false;
        GameController.SaveData.dayIndex++;
    }

    //call this when the player completes their tasks for the day (when the museum opens)
    public void TasksComplete()
    {
        GameController.SaveData.refreshTime = DateTime.UtcNow.AddHours(8).ToString();
        GameController.SaveData.completeDays++;
        GameController.SaveData.tasksComplete = true;
    }

    //IMPORTANT - this function checks current time vs. last login, decides the state of current tasks
    public void DateCheck()
    {
        //check the lastLogin vs. currentTime
        DateTime refreshTime = DateTime.Parse(GameController.SaveData.refreshTime);
        TimeSpan diff = refreshTime - DateTime.UtcNow;
        double hourCount = diff.TotalHours;

        //previous day was finished
        if (GameController.SaveData.tasksComplete)
        {
            //case for too long since last login
            if (hourCount < -24)
            {
                //reset streak
                AssignTasks();
            }
            //case for login after museum has reclosed, time for new tasks
            else if (hourCount < 0)
            {
                DateTime completeTime = refreshTime.AddHours(-8);
                TimeSpan habitCheck = completeTime - DateTime.UtcNow;
                //check if login is within 2 hours of the same time yesterday, habit points gained if true
                if (Math.Abs(habitCheck.TotalHours) < GameController.GameControl.habitRange)
                {
                    //enable habit bonus
                }
                AssignTasks();
            }
            //case for museum is still open, come back later - set the clock to trigger AssignTasks when museum closes
            else
            {

            }
        }
        else
        {
            //case for too long since last login - assign tasks and retain unfinished work
            if (hourCount > 0)
            {
                //reset streak
                AssignTasks();
            }
            else
            {
                //allow the user to continue their progress

                //if this is the first day, assignTasks for the first time
                if (GameController.SaveData.dayIndex == 0)
                {
                    AssignTasks();
                }
            }
        }

    }
}
