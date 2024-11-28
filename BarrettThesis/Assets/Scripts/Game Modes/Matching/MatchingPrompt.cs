using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchingPrompt : CardFill
{
    public bool selected = false;
    // Start is called before the first frame update
    void Start()
    {
        //audioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //called when a card is selected to be displayed to the user
    protected override void FillDecision(NoteType noteInfo)
    {
        List<int> indices = new List<int>();
        indices.Add(noteInfo.matchPromptField);
        if (cardAssigned && prevNoteId.Equals(currentCard.noteId))
            FieldReplace(cardFront, indices);
        else
            FieldFill(cardFront, indices);
    }

    private void OnMouseOver()
    {
        GetComponent<Outline>().enabled = true;
    }

    private void OnMouseExit()
    {
        if (!selected)
            GetComponent<Outline>().enabled = false;
    }

    private void OnMouseDown()
    {
        if (!selected)
        {
            selected = true;
            GetComponent<Outline>().OutlineColor = Color.green;
            foreach (var child in transform.parent.GetComponentsInChildren<MatchingPrompt>())
            {
                if (child != this)
                    child.Deselect();
            }
        }
        else
        {
            Deselect();
        }
        
    }

    public void Deselect()
    {
        selected = false;
        GetComponent<Outline>().OutlineColor = Color.white;
        GetComponent<Outline>().enabled = false;
    }

    public void Incorrect(int wrongAnswer)
    {
        currentCard.Missed(wrongAnswer);
    }

    public void Correct()
    {
        currentCard.Correct();
    }

    //fill the prompt card with the necessary fields as specified in the deck object
    /*IEnumerator FillCard()
    {
        //destroy existing children
        if (cardFront.transform.childCount > 0)
        {
            //fix this to destroy instead of detach
            cardFront.transform.DetachChildren();
        }

        //get the field to display
        int field = GameController.SaveData.currentDeck.dictRetrieve(associatedCard.noteId).matchPromptField;
        GameObject newText = Instantiate(textObj, cardFront.transform);
        newText.GetComponent<TMP_Text>().text = associatedCard.fields[field];
        prevNoteId = associatedCard.noteId;
        yield return null;
    }

    IEnumerator RefillCard()
    {
        int field = GameController.SaveData.currentDeck.dictRetrieve(associatedCard.noteId).matchPromptField;
        GameObject newText = cardFront.transform.GetChild(1).gameObject;
        newText.GetComponent<TMP_Text>().text = associatedCard.fields[field];
        prevNoteId = associatedCard.noteId;
        yield return null;
    }*/
}
