using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[System.Serializable]
public class Flashcard
{
    public int cardId;
    public List<string> fields;
    public List<int> imgFields;
    public List<int> audioFields;
    public string noteId;

    public string customArt;
    public bool useCustom = false;

    //progression elements
    public bool discovered;
    public int masteryLevel;
    //public int masteryPoints;
    public int daysTilNext;
    public int prevInterval;
    public int correctCount;
    public int missedCount;

    //tracker for frequently confused
    public List<int> confusedIndexes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Flashcard(JSONNode noteNode, int currentCount)
    {
        cardId = currentCount;
        fields = new List<string>();
        imgFields = new List<int>();
        audioFields = new List<int>();
        noteId = noteNode["note_model_uuid"].Value;

        int i = 0;
        foreach (var field in noteNode["fields"].Values)
        {
            string finalField = field;
            //if field contains img, isolate the img address and store index to imgFields
            if (finalField.Contains("<img"))
            {
                string[] splitPath = finalField.Split("\"");
                finalField = splitPath[1];
                imgFields.Add(i);
            }
            //if field contains sound, isolate the sound address and store index to audioFields
            else if (finalField.Contains("[sound:"))
            {
                string[] splitPath = finalField.Split(":");
                finalField = splitPath[1].Substring(0, splitPath[1].Length - 1);
                audioFields.Add(i);
            }

            fields.Add(finalField);
            i++;
        }

        //set initial progression elements
        masteryLevel = 0;
        //masteryPoints = 0;
        discovered = false;
        prevInterval = 0;
        daysTilNext = 0;
        correctCount = 0;
        missedCount = 0;
        confusedIndexes = new List<int>();
        
        Debug.Log("Card created: " + imgFields.Count + " " + audioFields.Count);
    }

    public void FieldPrint()
    {
        foreach (var field in fields)
        {
            Debug.Log(field);
        }
    }

    public void Correct()
    {
        correctCount++;
        //increment the mastery points
        if (!discovered)
        {
            discovered = true;
            daysTilNext++;
            
        }
        else if (daysTilNext == 0)
        {
            daysTilNext = 1;
        }
        else
        {
            //increase the days til next 
            daysTilNext = Mathf.CeilToInt(prevInterval * 1.5f);
            prevInterval = daysTilNext;
        }

        MasteryCheck();
        
        Debug.Log(cardId + " was Correct | Days til Next Review: " + daysTilNext);
    }

    public void Missed(int index)
    {
        missedCount++;
        //decrement the mastery points

        //reset the days til next
        daysTilNext = 0;

        //add the mistaken answer to the confused indexes list
        confusedIndexes.Add(index);

        Debug.Log(cardId + " was confused with" + index);
    }

    //Mastery only updated on Card Correct
    private void MasteryCheck()
    {
        int newMastery = 0;
        if (daysTilNext >= 30)
            newMastery = 6;
        else if (daysTilNext >= 21)
            newMastery = 5;
        else if (daysTilNext >= 14)
            newMastery = 4;
        else if (daysTilNext >= 7)
            newMastery = 3;
        else if (daysTilNext >= 3)
            newMastery = 2;
        else if (daysTilNext >= 1)
            newMastery = 1;
   
        //trigger an event to reward the player for new mastery level here
        //mastery can never dip below its previous level
        if (newMastery > masteryLevel)
            masteryLevel = newMastery;
    }
}
