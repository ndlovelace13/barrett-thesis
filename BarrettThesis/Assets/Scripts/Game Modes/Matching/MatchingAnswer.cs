using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchingAnswer : CardFill
{
    //float camDist;
    // Start is called before the first frame update
    public bool selected = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void CardAssign(Flashcard newCard)
    {
        base.CardAssign(newCard);
        //camDist = Mathf.Abs(transform.position.z - transform.root.position.z);
    }

    protected override void FillDecision(NoteType noteInfo)
    {
        List<int> indices = new List<int>();
        indices.Add(noteInfo.matchAnswerField);
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
            foreach (var child in transform.parent.GetComponentsInChildren<MatchingAnswer>())
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

    /*private void OnMouseDrag()
    {
        Debug.Log("Mouse Down");
        Vector3 ScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDist);
        Vector3 newPos = Camera.main.ScreenToWorldPoint(ScreenPos);
        transform.position = newPos;
    }*/

    /*IEnumerator FillCard()
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
    }*/
}
