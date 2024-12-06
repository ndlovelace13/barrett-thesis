using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveSlot : MonoBehaviour
{
    //Fields that will be filled
    [SerializeField] TMP_Text deckTitle;
    [SerializeField] TMP_Text cardCount;
    [SerializeField] TMP_Text museumLvl;
    [SerializeField] TMP_Text streak;
    [SerializeField] Button curateButton;

    [SerializeField] int saveIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //disable the button if you save is not filled/not able to overwrite
    }

    private void OnEnable()
    {
        Debug.Log("Filling Now");
        SaveSlotFill();
    }

    private void SaveSlotFill()
    {
        //fill in the known data if its found
        if (SaveHandler.SaveSystem.loading)
        {
            if (SaveHandler.SaveSystem.FindGame(saveIndex))
            {
                deckTitle.text = "Save File Found!";
                curateButton.GetComponentInChildren<TMP_Text>().text = "Curate!";
                curateButton.interactable = true;
            }
            else
            {
                deckTitle.text = "NEW SAVE";
                curateButton.GetComponentInChildren<TMP_Text>().text = "Select Slot!";
                curateButton.interactable = false;
            }
        }
        else
        {
            if (SaveHandler.SaveSystem.FindGame(saveIndex))
            {
                deckTitle.text = "Save File Exists!";
                curateButton.GetComponentInChildren<TMP_Text>().text = "Overwrite";
                curateButton.interactable = true;
            }
            else
            {
                deckTitle.text = "Unused Slot";
                curateButton.GetComponentInChildren<TMP_Text>().text = "Select Slot!";
                curateButton.interactable = true;
            }
        }
        
    }

    public void SaveSelect()
    {
        PlayerPrefs.SetInt("recentSave", saveIndex);
        SaveHandler.SaveSystem.SetSaveFile();
        if (SaveHandler.SaveSystem.loading)
        {
            SaveHandler.SaveSystem.LoadGame();
            SceneManager.LoadScene("Museum");
        }
        else
        {
            SaveHandler.SaveSystem.SaveGame();
            SceneManager.LoadScene("Museum");
        }
    }


}
