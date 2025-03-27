using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MatchScanner : ObjectMotion
{
    LayerMask layer;

    bool disabled = false;

    [SerializeField] Transform startingPos;
    [SerializeField] Transform offScreen;
    [SerializeField] Transform cardDestination;

    [SerializeField] MeshRenderer scannedPainting;
    [SerializeField] Image imageDisplay;
    [SerializeField] TMP_Text scannedPrompt;

    [SerializeField] List<TMP_Text> answers;
    [SerializeField] GameObject answerHolder;
    [SerializeField] Animator screenAnimator;
    string correctText;

    //Correct Visual Objects
    [Header("Correct Elements")]
    [SerializeField] GameObject correctMain;
    [SerializeField] TMP_Text correctAnswerText;
    [SerializeField] TMP_Text masteryUpdate;

    //Incorrect Visual Objects
    [Header("Incorrect Elements")]
    [SerializeField] GameObject incorrectMain;
    [SerializeField] TMP_Text incorrectAnswerText;
    [SerializeField] TMP_Text masteryDrop;

    Sprite placeholder;

    ScatteredCard cardControl;
    Flashcard scannedCard;
    SprenBehavior heldSpren;
    Flashcard prevCard;
    bool masteryUp;

    bool answerFilled = false;

    Dictionary<int, int> answerMap;

    // Start is called before the first frame update
    void Start()
    {
        layer = LayerMask.NameToLayer("cardLayer");
        prevCard = null;
        //Debug.Log("CHew on this you filthy animal: " + layer);
        placeholder = imageDisplay.sprite;
    }

    public override void Update()
    {
        if (answerFilled && scannedCard != null)
        {
            StartCoroutine(AnswerCheck());
        }
        if (Input.GetKey(KeyCode.Mouse1) && GameController.GameControl.gameMode == GameMode.MATCHING && heldSpren != null)
        {
            MatchCancel();
        }
        base.Update();
    }

    void FixedUpdate()
    {
        //if (!disabled)
            //StartCoroutine(ScanCheck());

        if (scannedCard == null)
        {
            answerFilled = false;
        }
        else
        {
            if (!answerFilled)
                StartCoroutine(FillAnswers());
            
        }

        DisableHandler();
    }

    public void MatchCancel()
    {
        StartCoroutine(ScannerReset());
        screenAnimator.SetBool("answerMode", false);
        heldSpren.SprenDeselect();
        heldSpren = null;
    }

    public void CardSelect(GameObject selectedSpren, Flashcard selectedCard)
    {
        //begin the animation
        screenAnimator.SetBool("answerMode", true);

        Debug.Log("Card Select Request Received for Card #" + selectedCard.cardId);
        cardControl = selectedSpren.GetComponentInChildren<ScatteredCard>();
        scannedCard = selectedCard;
        heldSpren = selectedSpren.GetComponent<SprenBehavior>();

        Texture2D tex = SaveHandler.SaveSystem.GetPainting(scannedCard.customArt);
        imageDisplay.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);

        NoteType noteInfo = GameController.SaveData.currentDeck.dictRetrieve(scannedCard.noteId);
        scannedPrompt.text = scannedCard.fields[noteInfo.matchPromptField];

        //execute spren lerp
        selectedSpren.GetComponent<SprenBehavior>().SprenSelect();

        //StartCoroutine(FillAnswers());

        //do some lerping here
    }

    public void AnswerFill()
    {
        StartCoroutine(FillAnswers());
    }

    IEnumerator FillAnswers()
    {
        answerFilled = true;
        answerHolder.SetActive(true);

        //get the cards answers
        List<int> answerIndexes = new List<int>(scannedCard.RetrieveAnswers());
        NoteType noteType = GameController.SaveData.currentDeck.dictRetrieve(scannedCard.noteId);

        //create a list of all the answer text objs
        //List<TMP_Text> remainingAnswers = new List<TMP_Text>(answers);
        answerMap = new Dictionary<int, int>();

        for (int i = 0; i < answers.Count; i++)
        {
            int selectedText = Random.Range(0, answerIndexes.Count);

            //retrieve the correct value from the 
            string tempText = (i + 1) + ". " + GameController.SaveData.currentDeck.cards[answerIndexes[selectedText]].fields[noteType.matchAnswerField];

            //store to correctText if its the correctAnswer
            if (answerIndexes[selectedText] == scannedCard.cardId)
                correctText = tempText;
            answers[i].text = tempText;
            
            //add to the answer map
            answerMap.Add(i, answerIndexes[selectedText]);

            //remove from the possible answers
            answerIndexes.RemoveAt(selectedText);

        }
        yield return null;
    }

    IEnumerator AnswerCheck()
    {
        int chosenAns;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            chosenAns = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            chosenAns = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            chosenAns = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            chosenAns = 3;
        }
        else
        {
            chosenAns = -1;
        }

        if (chosenAns != -1)
        {
            Debug.Log("Chosen Ans Recieved: " + chosenAns);

            //check whether the answer is correct or not
            int answerIndex = answerMap[chosenAns];

            if (scannedCard.cardId == answerIndex)
            {
                masteryUp = scannedCard.Correct();

                //Correct Answer Routines Here (remove the scannedCard)
                StartCoroutine(CorrectCoroutine());
            }
            else
            {
                scannedCard.Missed(answerIndex);

                //Incorrect Answer Routines Here (display incorrect answer)
                StartCoroutine(IncorrectCoroutine());
            }
        }

        yield return null;
    }

    IEnumerator CorrectCoroutine()
    {
        //start anim
        screenAnimator.SetTrigger("correct");

        //Lerp the card to the user's viewpoint
        cardControl.CorrectBehavior(cardDestination);
        Debug.Log("Correct Coroutine Initiated in the MatchScanner");

        //if mastery upgraded, do special animation

        //else do basic animation - reveal mastery color regardless

        //meanwhile, show correct on match scanner - time until next check
        answerHolder.SetActive(false);
        correctMain.SetActive(true);

        //fill text with correct text and mastery progress
        correctAnswerText.text = correctText;
        if (masteryUp)
        {
            //check whether maxMastery has been reached or not
            if (scannedCard.masteryLevel == DeckManager.DeckManage.masteryDays.Length)
            {
                masteryUpdate.text = "Max Mastery reached! | Next Review in " + scannedCard.daysTilNext + " Days";
            }
            else
            {
                masteryUpdate.text = "Mastery Tier " + scannedCard.masteryLevel + " reached! | Next Review in " + scannedCard.daysTilNext + " Days";
            }
        }
        else
        {
            //check whether maxMastery has been reached or not
            if (scannedCard.masteryLevel == DeckManager.DeckManage.masteryDays.Length)
            {
                masteryUpdate.text = "Max Mastery Retained | Next Review in " + scannedCard.daysTilNext + " Days";
            }
            else
            {
                masteryUpdate.text = "Mastery Tier " + scannedCard.masteryLevel + " " + (int)(scannedCard.MasteryPercent() * 100f) + "% Complete | Next Review in " + scannedCard.daysTilNext + " Days";
            }

        }

        //maybe a bar showing progress towards next mastery tier

        //update checklist
        GameObject.FindWithTag("Checklist").GetComponent<ChecklistDisplay>().TaskUpdate();

        //card fly over shoulder lerp, back to archive?
        float currentTime = 0f;
        while (currentTime < 1f)
        {
            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        //disable the correct menu and reenable the base answer holder
        answerHolder.SetActive(true);
        correctMain.SetActive(false);

        //reset answers
        answerFilled = false;

        screenAnimator.SetBool("answerMode", false);
        StartCoroutine(ScannerReset());

        yield return null;
    }

    IEnumerator IncorrectCoroutine()
    {
        //start anim
        screenAnimator.SetTrigger("incorrect");

        //Leave card there
        Debug.Log("Incorrect Coroutine Initiated in the MatchScanner");
        cardControl.IncorrectBehavior();

        //show incorrect on match scanner - new time until next check
        answerHolder.SetActive(false);
        incorrectMain.SetActive(true);

        //reveal the correct answer
        correctAnswerText.text = correctText;

        //bar showing reset mastery progress on match scanner
        masteryUpdate.text = "Mastery Tier " + scannedCard.masteryLevel + " Reset | Additional Review Required";

        //lerp to a new location?
        float currentTime = 0f;
        while (currentTime < 1f)
        {
            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        //disable the correct menu and reenable the base answer holder
        answerHolder.SetActive(true);
        incorrectMain.SetActive(false);

        //refresh answers
        answerFilled = false;

        screenAnimator.SetBool("answerMode", false);
        StartCoroutine(ScannerReset());

        yield return null;
    }

    IEnumerator ScannerReset()
    {
        imageDisplay.sprite = placeholder;
        scannedPrompt.text = (GameController.SaveData.cardCount - GameController.SaveData.cardQueue.Count) + " of " + GameController.SaveData.cardCount + " spren captured";
        yield return null;
    }


    IEnumerator ScanCheck()
    {
        //Debug.Log("I'm tryingggg");
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f, layer))
        {
            //if a card is detected, display it to the screen of the scanner
            //Debug.Log("card detected");
            cardControl = hit.collider.transform.root.GetComponentInChildren<ScatteredCard>();
            scannedCard = cardControl.ReportCard();
            
        }
        else
        {
            cardControl = null;
            scannedCard = null;
        }
            

        //apply to the scanner
        if (scannedCard != null && scannedCard != prevCard)
        {
            if (scannedCard.useCustom)
            {
                Texture2D tex = SaveHandler.SaveSystem.GetPainting(scannedCard.customArt);
                imageDisplay.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            }
                
            NoteType noteInfo = GameController.SaveData.currentDeck.dictRetrieve(scannedCard.noteId);
            scannedPrompt.text = scannedCard.fields[noteInfo.matchPromptField];
        }
        else if (scannedCard == null)
        {
            imageDisplay.sprite = placeholder;
            scannedPrompt.text = "No Archive Detected";
            answerHolder.SetActive(false);
        }
        prevCard = scannedCard;
        yield return null;
    }

    private void DisableHandler()
    {
        if (GameController.GameControl.gameMode == GameMode.MATCHING)
        {
            //lerp checklist back to its normal place
            if (disabled)
            {
                disabled = false;
                StartCoroutine(Enable());
                Debug.Log("Lerping Scanner Up");
            }
        }
        else if (GameController.GameControl.gameMode != GameMode.MATCHING)
        {
            //lerp checklist down and don't render it
            if (!disabled)
            {
                Debug.Log("Lerping Scanner Down");
                StopAllCoroutines();
                disabled = true;
                StartCoroutine(Disable());
            }
        }


    }

    IEnumerator Disable()
    {
        Vector3 startLoc = transform.localPosition;
        float timer = 0f;
        while (timer < 1f)
        {
            transform.localPosition = Vector3.Lerp(startLoc, offScreen.localPosition, timer);
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
            if (!disabled)
                yield break;
        }
        transform.localPosition = offScreen.localPosition;
        //GetComponent<MeshRenderer>().enabled = false;
        held = false;
        yield return null;

    }

    IEnumerator Enable()
    {
        Debug.Log("Enabling Scanner");

        //fill with default
        StartCoroutine(ScannerReset());

        //GetComponent<ChecklistDisplay>().TaskUpdate();
        Vector3 startLoc = transform.localPosition;
        //GetComponent<MeshRenderer>().enabled = true;
        float timer = 0f;
        while (timer < .25f)
        {
            transform.localPosition = Vector3.Lerp(startLoc, startingPos.localPosition, timer / .25f);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            if (disabled)
                yield break;
        }
        transform.localPosition = startingPos.localPosition;
        held = true;
        yield return null;
        Debug.Log("Done Enabling Scanner");
    }
}
