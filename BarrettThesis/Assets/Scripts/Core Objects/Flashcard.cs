using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Linq;
using Unity.VisualScripting;

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
    public int dayDiscovered;
    public int masteryLevel;
    //public int masteryPoints;
    public int daysTilNext;
    public int highestDays;
    public int prevInterval;
    public int correctCount;
    public int missedCount;

    //tracker for frequently confused
    public List<int> confusedIndexes;
    
    //storage for generated answers
    public List<int> returnedIndexes;

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
        highestDays = 0;
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

    public bool Correct()
    {
        Debug.Log("Before Correct Application " + daysTilNext);
        correctCount++;
        //increment the mastery points
        if (daysTilNext == 0)
        {
            daysTilNext = 1;
            prevInterval = daysTilNext;
        }
        else
        {
            //increase the days til next 
            daysTilNext = Mathf.CeilToInt(prevInterval * 1.5f);
            prevInterval = daysTilNext;
        }

        bool masteryUp = MasteryCheck();
        GameController.SaveData.cardQueue.Remove(this);

        //highest days check
        if (daysTilNext > highestDays)
        {
            highestDays = daysTilNext;
        }
        
        Debug.Log(cardId + " was Correct | Days til Next Review: " + daysTilNext);
        return masteryUp;
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

        //generate new indices
        GenerateAnswers();
    }

    //Mastery only updated on Card Correct
    private bool MasteryCheck()
    {
        //declaration of vars
        int newMastery = 0;
        bool masteryUp = false;

        //check masteryTier
        if (daysTilNext >= DeckManager.DeckManage.masteryDays[5])
            newMastery = 6;
        else if (daysTilNext >= DeckManager.DeckManage.masteryDays[4])
            newMastery = 5;
        else if (daysTilNext >= DeckManager.DeckManage.masteryDays[3])
            newMastery = 4;
        else if (daysTilNext >= DeckManager.DeckManage.masteryDays[2])
            newMastery = 3;
        else if (daysTilNext >= DeckManager.DeckManage.masteryDays[1])
            newMastery = 2;
        else if (daysTilNext >= DeckManager.DeckManage.masteryDays[0])
            newMastery = 1;
   
        //trigger an event to reward the player for new mastery level here
        //mastery can never dip below its previous level
        if (newMastery > masteryLevel)
        {
            masteryLevel = newMastery;
            Debug.Log("Mastery upgraded on card + " + cardId + ": Level " + masteryLevel);
            masteryUp = true;

            //check against global best
            if (masteryLevel > GameController.SaveData.highestMastery)
                GameController.SaveData.highestMastery = masteryLevel;
        }
        else
        {
            Debug.Log("Mastery retained on card " + cardId + ": Level " + masteryLevel);
        }
        return masteryUp;
    }

    public float MasteryPercent()
    {
        return (float)daysTilNext / (float)DeckManager.DeckManage.masteryDays[masteryLevel + 1];
    }

    //will be called upon at the beginning of the matching phase, or if the wrong answer was chosen
    public void GenerateAnswers()
    {
        returnedIndexes = new List<int>();

        //add four random answers to the list, one of them is correct
        returnedIndexes.Add(cardId);

        //if there are confusedIndexes, roll the dice to incorporate them instead of random pulls
        for (int i = 0; i < 3; i++)
        {
            if (confusedIndexes.Count > 0 && Random.Range(0f, 1f) < (0.1f * masteryLevel))
            {
                int newConf = confusedIndexes[Random.Range(0, confusedIndexes.Count)];
                if (returnedIndexes.Contains(newConf))
                    AnyIndex();
                else
                    returnedIndexes.Add(newConf);
            }
            else
                AnyIndex();
        }

        Debug.Log("returned Indexes contains: " + returnedIndexes.Count);
        //returnedIndexes = returnedIndexes.OrderBy(_ => Guid.NewGuid()).ToList();
    }

    private void AnyIndex()
    {
        int newIndex;
        Debug.Log("Unlocked card count: " + GameController.SaveData.unlockedCardCount);
        do
        {
            newIndex = Random.Range(0, GameController.SaveData.unlockedCardCount);
        } while (returnedIndexes.Contains(newIndex));
        returnedIndexes.Add(newIndex);
    }

    //called to return the answers generated earlier
    public List<int> RetrieveAnswers()
    {
        return returnedIndexes;
    }
}
