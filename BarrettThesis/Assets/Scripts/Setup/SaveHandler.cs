using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Security.Cryptography;

public class SaveHandler : MonoBehaviour
{
    public static SaveHandler SaveSystem;

    string saveFolderPath;
    string saveFilePath;
    string paintingPath;

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
        RetrieveSaves();
        if (saveCount > 0)
        {
            SetSaveFile();
            //LoadGame();
        }
    }

    private void Update()
    {
        if (GameController.GameControl.testingMode)
        {
            //if (Input.GetKey(KeyCode.Keypad9))
                //DeleteGame();
            if (Input.GetKey(KeyCode.Keypad7))
                SaveGame();
            if (Input.GetKeyDown(KeyCode.Keypad1))
                TimeSkip(new TimeSpan(1, 0, 0));
            if (Input.GetKeyDown(KeyCode.Keypad2))
                TimeSkip(new TimeSpan(0, 1, 0));
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

    public void SavePainting(Texture2D painting, Flashcard card)
    {
        if (!Directory.Exists(paintingPath))
        {
            Directory.CreateDirectory(paintingPath);
            Debug.Log("Painting Directory Created");
        }

        //store to bytes and write to a png
        byte[] paintingBytes = painting.EncodeToPNG();

        //establish the path for this particular painting
        string currentPainting = card.cardId + ".png";
        string writePath = Path.Combine(paintingPath, currentPainting);
        File.WriteAllBytes(writePath, paintingBytes);

        card.customArt = currentPainting;
        card.useCustom = true;
    }

    public Texture2D GetPainting(string paintFile)
    {
        string paintPath = Path.Combine(paintingPath, paintFile);
        byte[] paintBytes = File.ReadAllBytes(paintPath);
        Texture2D returnedPainting = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
        returnedPainting.filterMode = FilterMode.Point;
        returnedPainting.LoadImage(paintBytes);
        return returnedPainting;
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
        string tempPath = Path.Combine(Application.persistentDataPath, "Slot" + (fileIndex + 1));
        tempPath = Path.Combine(tempPath, "PlayerData.json");
        if (File.Exists(tempPath))
            return true;
        else
            return false;
    }

    public void SetSaveFile()
    {
        saveIndex = PlayerPrefs.GetInt("recentSave");
        saveFolderPath = Path.Combine(Application.persistentDataPath, "Slot" + (saveIndex + 1));
        saveFilePath = Path.Combine(saveFolderPath, "PlayerData.json");
        paintingPath = Path.Combine(saveFolderPath, "Paintings");
    }

    public void RetrieveSaves()
    {
        if (Directory.GetDirectories(Application.persistentDataPath).Length < 1)
        {
            for (int i = 1; i <= 3; i++)
            {
                string dirPath = Path.Combine(Application.persistentDataPath, "Slot" + i);
                Directory.CreateDirectory(dirPath);
            }
        }
        else
        {
            Debug.Log("Save Slot Directories exist");
        }

        saveCount = 0;
        string[] paths = Directory.GetDirectories(Application.persistentDataPath);
        for (int i = 0; i < paths.Length; i++)
        {
            string[] temp = Directory.GetFiles(paths[i], "*.json");
            saveCount += temp.Length;
        }

        //existingSaves = Directory.GetFiles(Application.persistentDataPath, "*.json");
        //saveCount = existingSaves.Length;
    }

    public void TimeSkip(TimeSpan skippedTime)
    {
        DateTime currentRefresh = GameController.SaveData.GetRefreshTime();
        DateTime newRefresh = currentRefresh - skippedTime;
        GameController.SaveData.refreshTime = newRefresh.ToString();
        SaveGame();
    }

    /*public void DeleteGame()
    {
        if (File.Exists(existingSaves[0]))
        {
            File.Delete(existingSaves[0]);
            Debug.Log("Save filed deleted");
        }
        else
            Debug.Log("There is no file to delete!");
    }*/

    /*public void UpdateText()
    {
        display.text = GameController.SaveData.cardCount.ToString();
    }

    public void TestIncrement()
    {
        GameController.SaveData.cardCount++;
    }*/
}
