using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class SaveHandler : MonoBehaviour
{
    public static SaveHandler SaveSystem;

    string saveFilePath;
    string deckFilePath;

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
        saveFilePath = Application.persistentDataPath + "/PlayerData.json";
    }
    
    public void SaveGame()
    {
        try
        {
            if (GameController.SaveData == null)
                GameController.SaveData = new SaveData();
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
            GameController.SaveData = JsonUtility.FromJson<SaveData>(playerData);
            Debug.Log("Save data successfully loaded!");
        }
        else
        {
            Debug.Log("There is no data to load!");
        }

    }

    public void DeleteGame()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save filed deleted");
        }
        else
            Debug.Log("There is no file to delete!");
    }

    public void UpdateText()
    {
        display.text = GameController.SaveData.cardCount.ToString();
    }

    public void TestIncrement()
    {
        GameController.SaveData.cardCount++;
    }
}
