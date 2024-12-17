using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryTask : TaskControl
{
    // Start is called before the first frame update
    //This never runs idiot
    void Start()
    {
        taskDescription = "Unbox New Deliveries";
        tasksComplete = GameController.SaveData.deliveriesToday - GameController.SaveData.orderedPlaceables.Count;
        Debug.Log("Orders Ready = " + GameController.SaveData.orderedPlaceables.Count);
        tasksTotal = GameController.SaveData.deliveriesToday;
        Debug.Log("Delivery Task Created");
    }

    // Update is called once per frame
    public override bool UpdateTask()
    {
        
        tasksTotal = GameController.SaveData.deliveriesToday;
        tasksComplete = tasksTotal - GameController.SaveData.orderedPlaceables.Count;
        Debug.Log("Delivery Task Updated");
        return base.UpdateTask();
    }
}
