using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchingAnswer : MonoBehaviour
{
    [SerializeField] GameObject cardFront;

    Flashcard associatedCard;

    [SerializeField] GameObject textObj;
    bool cardFilled = false;
    string prevNoteId;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateAnswer(Flashcard card)
    {
        associatedCard = card;
        if (cardFilled && prevNoteId.Equals(card.noteId))
            StartCoroutine(RefillCard());
        else
            StartCoroutine(FillCard());
    }

    IEnumerator FillCard()
    {
        //destroy existing children
        if (cardFront.transform.childCount > 0)
        {
            //fix this to destroy instead of detach
            cardFront.transform.DetachChildren();
        }

        //get the field to display
        int field = GameController.SaveData.currentDeck.dictRetrieve(associatedCard.noteId).matchAnswerField;
        GameObject newText = Instantiate(textObj, cardFront.transform);
        newText.GetComponent<TMP_Text>().text = associatedCard.fields[field];
        prevNoteId = associatedCard.noteId;
        yield return null;
    }

    IEnumerator RefillCard()
    {
        int field = GameController.SaveData.currentDeck.dictRetrieve(associatedCard.noteId).matchAnswerField;
        GameObject newText = cardFront.transform.GetChild(1).gameObject;
        newText.GetComponent<TMP_Text>().text = associatedCard.fields[field];
        prevNoteId = associatedCard.noteId;
        yield return null;
    }
}
