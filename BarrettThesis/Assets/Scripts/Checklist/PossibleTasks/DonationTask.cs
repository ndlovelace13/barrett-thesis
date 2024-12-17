using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonationTask : TaskControl
{
    // Start is called before the first frame update
    void Start()
    {
        taskDescription = "Collect Visitor Donations";
        tasksComplete = 0;
        tasksTotal = 1;
        Debug.Log("Donation Task Exists");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool UpdateTask()
    {
        if (GameController.SaveData.jarBalance == 0)
        {
            tasksComplete = 1;
        }
        Debug.Log("Donation Task Updated");
        return base.UpdateTask();
    }
}
