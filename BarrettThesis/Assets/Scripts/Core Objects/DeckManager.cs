using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DeckManager : MonoBehaviour
{
    public static DeckManager DeckManage;

    public List<Material> masteryMaterials;

    //[SerializeField] PlaceableHandler placeableHandler;

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

        //retrieve the new cards to add to the mix
        int startIndex = GameController.SaveData.newestIndex;
        int currentNew = GameController.SaveData.newQueue.Count;
        for (int i = 0; i < GameController.SaveData.newPerDay - currentNew; i++)
        {
            GameController.SaveData.newQueue.Add(GameController.SaveData.currentDeck.cards[startIndex + i]);
        }
        GameController.SaveData.newestIndex = startIndex + GameController.SaveData.newPerDay - 1;
        Debug.Log(GameController.SaveData.newQueue.Count + " new cards added to queue");

        int counter = 0;
        //retrieve all cards that are meant to appear (daysTilNext <= 1)
        foreach (Flashcard card in GameController.SaveData.currentDeck.cards)
        {
            if (card.discovered && card.daysTilNext <= 1)
            {
                GameController.SaveData.cardQueue.Add(card);
                card.daysTilNext = 0;
                counter++;
            }
            else
                card.daysTilNext--;
        }

        Debug.Log("Before dupe removal: " + GameController.SaveData.cardQueue.Count);
        GameController.SaveData.cardQueue = GameController.SaveData.cardQueue.Distinct().ToList();
        Debug.Log("After dupe removal: " + GameController.SaveData.cardQueue.Count);
        GameController.SaveData.cardCount = GameController.SaveData.cardQueue.Count + GameController.SaveData.newQueue.Count;

        //Debug.Log(cardQueue.Count + " total queue");

        //update all progression variables
        GameController.SaveData.refreshTime = DateTime.UtcNow.AddHours(24).ToString();
        GameController.SaveData.tasksComplete = false;
        GameController.SaveData.museumOpen = false;
        GameController.SaveData.orderedPlaceables.AddRange(GameController.SaveData.newOrders);
        GameController.SaveData.newOrders.Clear();
        GameController.SaveData.deliveriesToday = GameController.SaveData.orderedPlaceables.Count;
        GameController.SaveData.dayIndex++;
        GameController.SaveData.donationsToday = GameController.SaveData.jarBalance;

        //instantiate the tasks and checklist here
        GameObject test = GameObject.FindWithTag("Checklist");
        //Debug.Log(SceneManager.GetActiveScene().name);

        test.GetComponent<ChecklistDisplay>().TaskMenu();

        GameObject.FindWithTag("PlaceableHandler").GetComponent<PlaceableHandler>().Delivery();
    }

    //call this when the player completes their tasks for the day (when the museum opens)
    public void TasksComplete()
    {
        GameController.SaveData.museumOpen = true;
        GameController.SaveData.refreshTime = DateTime.UtcNow.AddHours(8).ToString();
        GameController.SaveData.completeDays++;
        GameObject.FindWithTag("VisitorSpawn").GetComponent<VisitorHandler>().VisitorSpawn();
    }

    //IMPORTANT - this function checks current time vs. last login, decides the state of current tasks
    public void DateCheck()
    {
        //check the lastLogin vs. currentTime
        DateTime refreshTime = GameController.SaveData.GetRefreshTime();
        TimeSpan diff = refreshTime - DateTime.UtcNow;
        double hourCount = diff.TotalHours;
        Debug.Log("Hour Count: " + hourCount);

        //previous day was finished
        if (GameController.SaveData.tasksComplete)
        {
            //case for too long since last login
            if (hourCount < -24)
            {
                //reset streak
                Debug.Log("STATUS: tasks completed, but streak reset, assigning new tasks");
                AssignTasks();
            }
            //case for login after museum has reclosed, time for new tasks
            else if (hourCount < 0)
            {
                Debug.Log("STATUS: streak upheld, assigning new tasks");
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
                //restore checklist progress
                GameObject.FindWithTag("Checklist").GetComponent<ChecklistDisplay>().TaskMenu();
                GameObject.FindWithTag("VisitorSpawn").GetComponent<VisitorHandler>().VisitorSpawn();
                Debug.Log("STATUS: museum still open, come back later for new tasks");
            }
        }
        else
        {
            //case for too long since last login - assign tasks and retain unfinished work
            if (hourCount < 0)
            {
                //reset streak
                Debug.Log("STATUS: tasks not completed, streak reset, assigning new tasks");
                AssignTasks();
            }
            else
            {
                //allow the user to continue their progress

                //restore the checklist progress
                GameObject.FindWithTag("Checklist").GetComponent<ChecklistDisplay>().TaskMenu();


                //if this is the first day, assignTasks for the first time
                if (GameController.SaveData.dayIndex == 0 || GameController.SaveData.newQueue == null)
                {
                    Debug.Log("STATUS: First day tasks being assigned");
                    GameController.SaveData.dayIndex = 0;
                    GameController.SaveData.newQueue = new List<Flashcard>();
                    GameController.SaveData.cardQueue = new List<Flashcard>();
                    AssignTasks();
                }
                else
                    Debug.Log("STATUS: tasks still remain, continue working");
            }
        }

    }
}
