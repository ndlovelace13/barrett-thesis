using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardFill : MonoBehaviour
{
    Flashcard currentCard;

    [SerializeField] TMP_Text cardFront;
    [SerializeField] TMP_Text cardBack;

    bool cardAssigned;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CardAssign(Flashcard newCard)
    {
        //assign the card
        currentCard = newCard;
        cardAssigned = true;

        //retrieve the important fields from the note
        string noteType = currentCard.noteId;
        NoteType noteInfo = GameController.SaveData.currentDeck.dictRetrieve(noteType);
        currentCard.FieldPrint();
        cardFront.text = currentCard.fields[noteInfo.front[0]];
        cardBack.text = currentCard.fields[noteInfo.back[0]];
    }
}
