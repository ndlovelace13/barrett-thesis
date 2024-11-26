using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchingPrompt : MonoBehaviour
{
    [SerializeField] GameObject cardFront;

    Flashcard associatedCard;

    [SerializeField] GameObject textObj;
    //[SerializeField] GameObject imgPlane;
    //[SerializeField] GameObject audioCue;

    bool cardFilled = false;
    string prevNoteId;

    //AudioSource audioPlayer;
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
    public void ActivatePrompt(Flashcard card)
    {
        associatedCard = card;
        if (cardFilled && prevNoteId.Equals(card.noteId))
            StartCoroutine(RefillCard());
        else
            StartCoroutine(FillCard());
    }

    //fill the prompt card with the necessary fields as specified in the deck object
    IEnumerator FillCard()
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
    }
}
