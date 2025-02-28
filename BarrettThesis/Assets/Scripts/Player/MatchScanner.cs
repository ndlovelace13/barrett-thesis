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

    [SerializeField] MeshRenderer scannedPainting;
    [SerializeField] Image imageDisplay;
    [SerializeField] TMP_Text scannedPrompt;

    [SerializeField] List<TMP_Text> answers;
    [SerializeField] GameObject answerHolder;

    Sprite placeholder;

    Flashcard scannedCard;
    Flashcard prevCard;

    bool answerFilled = false;

    // Start is called before the first frame update
    void Start()
    {
        layer = LayerMask.NameToLayer("cardLayer");
        prevCard = null;
        //Debug.Log("CHew on this you filthy animal: " + layer);
        placeholder = imageDisplay.sprite;
    }

    public void FixedUpdate()
    {
        if (!disabled)
            StartCoroutine(ScanCheck());

        if (scannedCard == null)
        {
            answerFilled = false;
        }
        else
        {
            if (!answerFilled)
                FillAnswers();
            StartCoroutine(AnswerCheck());
        }

        DisableHandler();
    }

    private void FillAnswers()
    {
        answerFilled = true;
        answerHolder.SetActive(true);

        //get the cards answers
        List<int> answerIndexes = new List<int>(scannedCard.RetrieveAnswers());
        NoteType noteType = GameController.SaveData.currentDeck.dictRetrieve(scannedCard.noteId);

        //create a list of all the answer text objs
        //List<TMP_Text> remainingAnswers = new List<TMP_Text>(answers);

        for (int i = 0; i < answers.Count; i++)
        {
            int selectedText = Random.Range(0, answerIndexes.Count);

            //retrieve the correct value from the 
            answers[i].text = (i+1) + ". " + GameController.SaveData.currentDeck.cards[answerIndexes[selectedText]].fields[noteType.matchAnswerField];
            answerIndexes.RemoveAt(selectedText);

        }
    }

    IEnumerator AnswerCheck()
    {
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
            scannedCard = hit.collider.transform.root.GetComponent<ScatteredCard>().ReportCard();
        }
        else
            scannedCard = null;

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
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
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
