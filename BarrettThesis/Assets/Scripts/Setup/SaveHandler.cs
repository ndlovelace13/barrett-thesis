using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class SaveHandler : MonoBehaviour
{
    public static SaveHandler SaveSystem;

    string saveFilePath;
    string deckFilePath;

    int saveIndex;
    public int saveCount;

    public string[] existingSaves;

    public bool loading = true;

    [SerializeField] TMP_Text display;
    // Start is called before the first frame update

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SaveSystem = this;
    }

    void Start()
    {
        //TO DO: allow this to be flexible for multiple save files (add a number at the end of playerData to indicate which save they want to access)
        RetrieveSaves();
        if (existingSaves.Length > 0)
        {
            SetSaveFile();
            //LoadGame();
        }
    }

    private void Update()
    {
        if (GameController.GameControl.testingMode)
        {
            if (Input.GetKey(KeyCode.Keypad9))
                DeleteGame();
            if (Input.GetKey(KeyCode.Keypad7))
                SaveGame();
        }
    }

    public void SaveGame()
    {
        try
        {
            if (GameController.SaveData == null)
                GameController.SaveData = new SaveData();
            //update the save time
            GameController.SaveData.saveTime = DateTime.UtcNow.ToString();    

            string playerData = JsonUtility.ToJson(GameController.SaveData);
            File.WriteAllText(saveFilePath, playerData);

            Debug.Log("Data saved at: " + saveFilePath);
        }
        catch
        {
            Debug.Log("Game Save did not work as intended");
        }
        
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string playerData = File.ReadAllText(saveFilePath);
            //Debug.Log(playerData);
            GameController.SaveData = JsonUtility.FromJson<SaveData>(playerData);
            Debug.Log("Save data successfully loaded!");
            GameController.SaveData.currentDeck.noteDictTranslate();
            //Debug.Log(GameController.SaveData.currentDeck.noteTypes.Count);
        }
        else
        {
            Debug.Log("There is no data to load!");
        }

    }

    public bool FindGame(int fileIndex)
    {
        string tempFilePath = Application.persistentDataPath + "/PlayerData" + fileIndex + ".json";
        if (File.Exists(tempFilePath))
            return true;
        else
            return false;
    }

    public void SetSaveFile()
    {
        saveIndex = PlayerPrefs.GetInt("recentSave");
        saveFilePath = Application.persistentDataPath + "/PlayerData" + saveIndex + ".json";
    }

    public void RetrieveSaves()
    {
        existingSaves = Directory.GetFiles(Application.persistentDataPath, "*.json");
        saveCount = existingSaves.Length;
        foreach (string file in existingSaves)
            Debug.Log(file);
    }

    public void DeleteGame()
    {
        if (File.Exists(existingSaves[0]))
        {
            File.Delete(existingSaves[0]);
            Debug.Log("Save filed deleted");
        }
        else
            Debug.Log("There is no file to delete!");
    }

    /*public void UpdateText()
    {
        display.text = GameController.SaveData.cardCount.ToString();
    }

    public void TestIncrement()
    {
        GameController.SaveData.cardCount++;
    }*/
}
