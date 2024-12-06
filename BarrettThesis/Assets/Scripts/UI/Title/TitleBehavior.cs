using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleBehavior : MonoBehaviour
{
    [SerializeField] TMP_Text promptText;

    [SerializeField] GameObject initialButtons;
    [SerializeField] GameObject saveSlotHolder;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Continue()
    {
        SaveHandler.SaveSystem.loading = true;
        promptText.text = "Select a Save Slot";
        initialButtons.SetActive(false);
        saveSlotHolder.SetActive(true);
    }

    public void Cancel()
    {
        promptText.text = "Anki Curation Game";
        initialButtons.SetActive(true);
        saveSlotHolder.SetActive(false);
    }

    public void NewDeck()
    {
        promptText.text = "Choose a Slot";
        SaveHandler.SaveSystem.gameObject.GetComponentInChildren<MainParser>().FileSearch();
    }

    public void ChooseSave()
    {
        SaveHandler.SaveSystem.loading = false;
        initialButtons.SetActive(false);
        saveSlotHolder.SetActive(true);
    }
}
