using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//all possible gamemodes, always should have one active
public enum GameMode
{
    DEFAULT,
    ARCHIVE,
    ARCHIVE2,
    MATCHING,
    CREATING,
    INSPECTING,
    ORDERING,
    PAUSED,
    REPORT,
    TUTORIAL
}

public class GameController : MonoBehaviour
{
    //game controller obj
    public static GameController GameControl;

    //player's save data
    public static SaveData SaveData;

    public bool testingMode = false;
    public bool lockPlayer = false;

    public int habitRange = 2; //in hours

    public GameMode gameMode = GameMode.DEFAULT;

    public float lerpTime = 0.5f;

    //generic prefabs that will get called a lot
    [SerializeField] public GameObject popup;
    [SerializeField] public GameObject scatteredCard;
    

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

    public void GameStart()
    {
        gameMode = GameMode.DEFAULT;
        Time.timeScale = 1f;
        FindObjectOfType<MuseumLoader>().MuseumRestore();
        FindObjectOfType<PlaceableHandler>().PlaceableRestore();
        FindObjectOfType<ArtifactControl>().ArtifactRestore();
        FindObjectOfType<TutorialControl>().TutorialInit();
        DeckManager.DeckManage.DateCheck();

        //open new rooms here if there are any that finish today - cut a ribbon maybe?
    }

}
