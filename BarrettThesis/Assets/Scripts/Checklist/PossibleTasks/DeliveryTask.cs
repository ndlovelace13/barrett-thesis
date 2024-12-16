using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryTask : TaskControl
{
    // Start is called before the first frame update
    void Start()
    {
        taskDescription = "Unbox New Deliveries";
        tasksComplete = 0;
        tasksTotal = GameController.SaveData.orderedPlaceables.Count;
    }

    // Update is called once per frame
    void Update()
    {
        tasksComplete = tasksTotal - GameController.SaveData.orderedPlaceables.Count;
    }
}
