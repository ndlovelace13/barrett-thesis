using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string playerName;
    public int cardCount;
    public int balance;


    // Start is called before the first frame update
    void Start()
    {
        playerName = "Hugh Mungus";
        cardCount = 0;
        balance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
