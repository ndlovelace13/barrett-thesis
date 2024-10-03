using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //game controller obj
    public static GameController GameControl;

    //player's save data
    public static SaveData SaveData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        GameControl = this;
        DontDestroyOnLoad(gameObject);
    }
}
