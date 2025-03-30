using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TutorialControl : MonoBehaviour
{
    public Dictionary<string, TutorialItem> remainingTutorials;

    [Header("Canvas Components")]
    [SerializeField] Canvas dialogueCanvas;
    [SerializeField] TMP_Text characterName;
    [SerializeField] TMP_Text currentDialogue;

    TutorialItem currentTutorial;

    bool dialogueReady = false;
    bool dialoguePrinting = false;
    bool dialogueSkip = false;

    public void Awake()
    {
        dialogueCanvas.enabled = false;
    }

    public void TutorialInit()
    {
        remainingTutorials = new Dictionary<string, TutorialItem>();
        List<TutorialItem> allTutorials = Resources.LoadAll<TutorialItem>("Tutorials/").ToList<TutorialItem>();

        //sort through each, add to the dict if they haven't been displayed before
        foreach (var item in allTutorials)
        {
            if (GameController.SaveData.completedTutorials.Contains(item.key))
            {
                Debug.Log("Tutorial " + item.key + " already complete");
            }
            else
            {
                remainingTutorials.Add(item.key, item);
            }
        }
    }

    //call this from any other script when a tutorial should be executed
    public void CheckTutorial(string reqKey)
    {
        if (remainingTutorials.ContainsKey(reqKey) && !GameController.SaveData.completedTutorials.Contains(reqKey))
        {
            //add to the blacklist, remove from the dictionary
            GameController.SaveData.completedTutorials.Add(reqKey);
            currentTutorial = remainingTutorials[reqKey];
            remainingTutorials.Remove(reqKey);

            //begin the execution of the tutorial itself
            StartCoroutine(TutorialExecute());
        }
        else
        {
            Debug.Log("this tutorial has either already been executed or does not exist");
        }
    }

    IEnumerator TutorialExecute()
    {
        GameController.GameControl.gameMode = GameMode.TUTORIAL;
        dialogueCanvas.enabled = true;
        GameController.GameControl.lockPlayer = true;

        //instantiate the variables necessary for parsing through dialogue
        int dialogueIndex = 0;
        dialogueReady = false;
        dialoguePrinting = false;
        dialogueSkip = false;

        //set the charactername
        characterName.text = currentTutorial.charName;

        //turn on the tutorial obj outline
        Outline tutorialOutline = currentTutorial.RetrieveOutline();
        tutorialOutline.enabled = true;
        tutorialOutline.OutlineColor = Color.yellow;

        while (dialogueIndex < currentTutorial.dialogue.Length)
        {
            //only begin printing the next dialogue if it exists
            if (!dialoguePrinting && !dialogueReady)
            {
                //start the printing process
                dialoguePrinting = true;
                StartCoroutine(DialoguePrint(currentTutorial.dialogue[dialogueIndex]));
            }
            //move on to the next dialogue 
            if (dialogueReady && dialogueSkip)
            {
                //reset the variables and increment the dialogueIndex
                dialogueReady = false;
                dialoguePrinting = false;
                dialogueSkip = false;
                dialogueIndex++;
            }
            
            yield return new WaitForEndOfFrame();
        }

        dialogueCanvas.enabled = false;


        //go back to normal programming
        GameController.GameControl.lockPlayer = false;
        GameController.GameControl.gameMode = GameMode.TUTORIAL;
        yield return null;
    }

    //handles the actual printing of the dialogue to the screen
    IEnumerator DialoguePrint(string currentLine)
    {
        int charIndex = 0;
        while (charIndex < currentLine.Length)
        {
            string startLine = currentLine.Substring(0, charIndex);
            currentDialogue.text = startLine + "<alpha=#00>" + currentLine.Substring(charIndex, currentLine.Length - startLine.Length);
            charIndex++;
            if (dialogueSkip)
            {
                dialogueSkip = false;
                currentDialogue.text = currentLine;
                break;
            }
            yield return new WaitForFixedUpdate();
        }

        dialogueReady = true;
        dialoguePrinting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.GameControl.gameMode == GameMode.TUTORIAL && Input.GetKeyDown(KeyCode.Space))
        {
            dialogueSkip = true;
        }
    }
}
