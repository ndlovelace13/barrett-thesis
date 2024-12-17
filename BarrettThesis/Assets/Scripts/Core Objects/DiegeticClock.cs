using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiegeticClock : MonoBehaviour
{

    [SerializeField] TMP_Text timeDisplay;
    [SerializeField] TMP_Text description;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //display the time between now and refresh time
        TimeSpan timeDiff = GameController.SaveData.GetRefreshTime() - DateTime.UtcNow;

        //check whether the timespan is greater than zero
        if (timeDiff > TimeSpan.Zero)
        {
            timeDisplay.text = timeDiff.ToString(@"hh\:mm\:ss");
        }
        else
        {
            if (GameController.SaveData.museumOpen)
                DeckManager.DeckManage.AssignTasks();
            else
                timeDisplay.text = "OVERTIME";
        }
            
        if (GameController.SaveData.museumOpen)
            description.text = "Time until Museum Closure";
        else
            description.text = "Time until Next Delivery";
    }
}
