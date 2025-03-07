using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseBehavior : MonoBehaviour
{
    [SerializeField] Canvas pauseCanvas;

    //vars
    GameMode storedState;

    // Start is called before the first frame update
    void Start()
    {
        pauseCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pauseCanvas.enabled)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Debug.Log("Game Paused");

        Time.timeScale = 0f;
        pauseCanvas.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //store the game state
        storedState = GameController.GameControl.gameMode;
        GameController.GameControl.gameMode = GameMode.PAUSED;
    }

    public void ResumeGame()
    {
        Debug.Log("Game Resumed");

        pauseCanvas.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        //restore the game state
        GameController.GameControl.gameMode = storedState;
    }

    public void Options()
    {
        //call options menu here
        Debug.Log("Options Called Lol");
    }

    public void MenuReturn()
    {
        Debug.Log("Return to Menu Pressed");

        SaveHandler.SaveSystem.SaveGame();
        //ResumeGame();

        //return to the main menu
        SceneManager.LoadScene("MainMenu");
    }

    public void GameQuit()
    {
        Debug.Log("Quitting Game");

        SaveHandler.SaveSystem.SaveGame();

        //quit the game entirely
        Application.Quit();
    }
}
