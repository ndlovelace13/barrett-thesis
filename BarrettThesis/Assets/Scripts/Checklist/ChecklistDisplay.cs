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
    TipCollection,
    Delivery,
    Order

}

public class ChecklistDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text dayCounter;

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
        GameController.SaveData.taskMenu = new List<TaskType>();
        GameController.SaveData.taskMenu.Add(TaskType.NewCards);
        GameController.SaveData.taskMenu.Add(TaskType.ReviewCards);

        //add in other necessary tasks for the day here

        //pickup delivery

        //claim donations

        //place new art

        //etc.

        //fill in the checklist
        TaskFill();
    }

    //function to fill in the checklist for the current tasks for the day
    public void TaskFill()
    {
        dayCounter.text = "Day " + GameController.SaveData.dayIndex;
        currentTasks = new List<ChecklistItem>();

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
       Debug.Log("UpdatingTasks");
       foreach (ChecklistItem item in currentTasks)
       {
            item.UpdateItem();
       }
    }
}