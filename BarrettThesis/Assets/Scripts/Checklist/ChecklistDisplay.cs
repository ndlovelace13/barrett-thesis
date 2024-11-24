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

    [SerializeField] Dictionary<TaskType, TaskControl> taskDict;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TaskMenu()
    {
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

        foreach (TaskType task in GameController.SaveData.taskMenu)
        {
            GameObject newTask = Instantiate(checkItem);
            newTask.transform.SetParent(listHolder.transform);
            
            //assign taskControl of type task from the dict here (might need assembly type shit?)
        }
    }

    //function to update the values of the checklist everytime it is enabled
    public void TaskUpdate()
    {

    }
}
