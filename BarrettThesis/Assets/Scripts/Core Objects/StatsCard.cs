using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsCard : MonoBehaviour
{
    Flashcard displayedCard;
    [SerializeField] MeshRenderer masteryMat;

    [SerializeField] TMP_Text cardNum;
    [SerializeField] TMP_Text currentMastery;
    [SerializeField] TMP_Text nextMastery;
    [SerializeField] TMP_Text daysCounter;
    [SerializeField] TMP_Text discoveryDate;
    [SerializeField] TMP_Text timesCorrect;
    [SerializeField] TMP_Text timesIncorrect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignCard(Flashcard newCard)
    {
        displayedCard = newCard;

        StartCoroutine(FillFields());

        //assign mastery material here
        masteryMat.material = DeckManager.DeckManage.masteryMaterials[displayedCard.masteryLevel];
    }

    IEnumerator FillFields()
    {
        cardNum.text = "#" + (displayedCard.cardId + 1);
        currentMastery.text = "Mastery Tier " + displayedCard.masteryLevel;
        nextMastery.text = displayedCard.MasteryPercent() + "% of way to next tier";
        daysCounter.text = "Next Review in " + displayedCard.daysTilNext + " Days\nMost Days Between Reviews: " + displayedCard.highestDays;
        timesCorrect.text = "Correct Review Count: " + displayedCard.correctCount;
        timesIncorrect.text = "Mistake Count: " + displayedCard.missedCount;
        discoveryDate.text = "Discovered on Day #" + displayedCard.dayDiscovered;


        yield return null;
    }
}
