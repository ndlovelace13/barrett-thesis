using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimeDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text timeDisplay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        timeDisplay.text = "Current Time: " + DateTime.UtcNow.ToString() + " | " + "Last Save: " + GameController.SaveData.GetSaveTime().ToString();
    }
}
