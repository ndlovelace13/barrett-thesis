using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//all possible gamemodes, always should have one active
public enum GameMode
{
    DEFAULT,
    ARCHIVE
}

public class GameController : MonoBehaviour
{
    //game controller obj
    public static GameController GameControl;

    //player's save data
    public static SaveData SaveData;

    public bool testingMode = false;
    public bool lockPlayer = false;

    public GameMode gameMode = GameMode.DEFAULT;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (testingMode)
        {
            if (Input.GetKeyDown(KeyCode.Keypad8))
                lockPlayer = !lockPlayer;
        }
    }

    private void Awake()
    {
        if (GameControl == null)
        {
            GameControl = this;
            DontDestroyOnLoad(gameObject);
            SaveData = new SaveData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
