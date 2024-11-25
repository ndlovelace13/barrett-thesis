using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "OldQueueTask", menuName = "Custom/OldQueueTask")]
public class OldQueueTask : TaskControl
{
    // Start is called before the first frame update
    void Start()
    {
        taskDescription = "Archives Reviewed";
        tasksComplete = GameController.SaveData.cardCount - GameController.SaveData.cardQueue.Count - 15;
        tasksTotal = GameController.SaveData.cardCount;
        Debug.Log("OldQueueTask exists");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UpdateTask()
    {
        base.UpdateTask();
        taskDescription = "Archives Reviewed";
        tasksComplete = GameController.SaveData.cardCount - GameController.SaveData.cardQueue.Count - 15;
        tasksTotal = GameController.SaveData.cardCount;
        Debug.Log("OldQueueTask Updated");
    }
}
