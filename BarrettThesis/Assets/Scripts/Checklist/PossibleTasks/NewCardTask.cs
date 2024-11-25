using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NewCardTask", menuName = "Custom/NewCardTask")]
public class NewCardTask : TaskControl
{
    // Start is called before the first frame update
    void Start()
    {
        taskDescription = "Acquire New Works";
        tasksComplete = GameController.SaveData.newPerDay - GameController.SaveData.newQueue.Count;
        tasksTotal = GameController.SaveData.newPerDay;
        Debug.Log("NewCardTask Exists");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UpdateTask()
    {
        base.UpdateTask();
        taskDescription = "Acquire New Works";
        tasksComplete = GameController.SaveData.newPerDay - GameController.SaveData.newQueue.Count;
        tasksTotal = GameController.SaveData.newPerDay;
        Debug.Log("NewCardTask Updated");
    }
}
