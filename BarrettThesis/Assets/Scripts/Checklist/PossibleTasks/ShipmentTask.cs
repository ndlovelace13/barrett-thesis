using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipmentTask : TaskControl
{
    // Start is called before the first frame update
    void Start()
    {
        taskDescription = "Unbox New Card Shipment";
        tasksComplete = 0;
        tasksTotal = 1;
    }

    // Update is called once per frame
    public override bool UpdateTask()
    {
        taskDescription = "Unbox New Card Shipment";
        if (GameController.SaveData.cardsUnboxed)
            tasksComplete = 1;
        Debug.Log("Shipment Task Updated");
        return base.UpdateTask();
    }
}
