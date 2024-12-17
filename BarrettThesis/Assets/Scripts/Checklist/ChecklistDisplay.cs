using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//enum to store all possible tasks 
public enum TaskType
{
    NewCards,
    ReviewCards,
    Donation,
    Delivery,
}

public class ChecklistDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text dayCounter;
    [SerializeField] TMP_Text currentFunds;

    [SerializeField] GameObject listHolder;

    //check prefab
    [SerializeField] GameObject checkItem;

    Dictionary<TaskType, TaskControl> taskDict;
    [SerializeField] List<TaskControl> taskControls;

    List<ChecklistItem> currentTasks;

    // Start is called before the first frame update
    void Start()
    {
        taskDict = new Dictionary<TaskType, TaskControl> ();
        int counter = 0;
        foreach (TaskType pieceType in Enum.GetValues(typeof(TaskType)))
        {
            if (counter < taskControls.Count)
            {
                taskDict.Add(pieceType, taskControls[counter]);
                counter++;
            }
        }
        Debug.Log("Counter at " + counter);

        //move this to the current scene controller when created
        GameController.GameControl.GameStart();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TaskMenu()
    {
        Debug.Log("Task Menu Reached");
        //set the necessary task menu for the day (for saving and retrieval)
        if (GameController.SaveData.taskMenu != null)
            GameController.SaveData.taskMenu.Clear();
        else
            GameController.SaveData.taskMenu = new List<TaskType>();

        //always add these two as there will always be cards to study
        GameController.SaveData.taskMenu.Add(TaskType.NewCards);
        GameController.SaveData.taskMenu.Add(TaskType.ReviewCards);

        //add in other necessary tasks for the day here

        //pickup delivery
        if (GameController.SaveData.deliveriesToday > 0)
            GameController.SaveData.taskMenu.Add(TaskType.Delivery);

        //claim donations
        if (GameController.SaveData.donationsToday > 0)
            GameController.SaveData.taskMenu.Add(TaskType.Donation);

        //place new art

        //etc.

        //fill in the checklist
        TaskFill();
    }

    //function to fill in the checklist for the current tasks for the day
    public void TaskFill()
    {
        dayCounter.text = "Day " + GameController.SaveData.dayIndex;
        currentFunds.text = "Current Funding: " + ((float)(GameController.SaveData.balance / 100f)).ToString("C0");
        
        if (currentTasks == null)
            currentTasks = new List<ChecklistItem>();
        else
            currentTasks.Clear();

        //Destroy any current tasks on the board
        foreach (Transform child in listHolder.transform)
            Destroy(child.gameObject);

        foreach (TaskType task in GameController.SaveData.taskMenu)
        {
            GameObject newTask = Instantiate(checkItem);
            newTask.transform.SetParent(listHolder.transform, false);

            //assign taskControl of type task from the dict here (might need assembly type shit?)
            //GameObject currentItem = Instantiate(taskDict[task]);
            newTask.GetComponent<ChecklistItem>().taskType = taskDict[task];

            currentTasks.Add(newTask.GetComponent<ChecklistItem>());
            Debug.Log("task added");
        }

        TaskUpdate();
    }

    //function to update the values of the checklist everytime it is enabled
    public void TaskUpdate()
    {
        currentFunds.text = "Current Funding: " + ((float)(GameController.SaveData.balance / 100f)).ToString("C0");
        Debug.Log("UpdatingTasks");

        bool taskCompletion = true;
        foreach (ChecklistItem item in currentTasks)
        {
            //update each task and check whether the task is complete
            bool complete = item.UpdateItem();
            if (!complete)
                taskCompletion = false;
        }
        //Task Completion will be true only if all tasks are complete
        GameController.SaveData.tasksComplete = taskCompletion;
        
        //This should be the only place the game is saved to minimize redundancy
        SaveHandler.SaveSystem.SaveGame();
    }
}