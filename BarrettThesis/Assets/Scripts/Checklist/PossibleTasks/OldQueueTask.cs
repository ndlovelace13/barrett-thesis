using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "OldQueueTask", menuName = "Custom/OldQueueTask")]
public class OldQueueTask : TaskControl
{
    // Start is called before the first frame update
    void Start()
    {
        tutorialKey = "firstPurchases";
        taskDescription = "Archives Reviewed";
        tasksComplete = GameController.SaveData.cardCount - GameController.SaveData.cardQueue.Count;
        tasksTotal = GameController.SaveData.cardCount;
        Debug.Log("OldQueueTask exists");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool UpdateTask()
    {
        tutorialKey = "firstPurchases";
        taskDescription = "Archives Reviewed";
        if (GameController.SaveData.newQueue.Count == 0)
            tasksComplete = GameController.SaveData.cardCount - GameController.SaveData.cardQueue.Count;
        else
            tasksComplete = 0;
        tasksTotal = GameController.SaveData.cardCount;
        Debug.Log("OldQueueTask Updated");
        return base.UpdateTask();

    }
}
