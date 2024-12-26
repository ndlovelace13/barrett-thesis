using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VisitorHandler : MonoBehaviour
{
    [SerializeField] ObjectPool visitorPool;
    float visitorCooldown;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void VisitorSpawn()
    {
        VisitorCalc();
        StartCoroutine(VisitorSpawning());
    }

    //Calculate the visitors and donations while the user was away from the game
    public void VisitorsAway(bool open)
    {
        Debug.Log("Visitors While Away Calculations Beginning");
        //call VisitorCalc first
        VisitorCalc();

        DateTime lastSave = GameController.SaveData.GetSaveTime();
        TimeSpan timeAway;
        //retrieve the time since last save
        if (open)
        {
            DateTime nowTime = DateTime.UtcNow;

            timeAway = nowTime - lastSave;
        }
        //retrieve the amount of time the museum was open until it closed
        else
        {
            DateTime closeTime = GameController.SaveData.GetRefreshTime();
            timeAway = closeTime - lastSave;
        }
        float secondsAway = (float)timeAway.TotalSeconds;

        //retrieve all visitables and their average visit time
        GameObject[] visitables = GameObject.FindGameObjectsWithTag("VisitorDestination");
        float avgHappiness = 0f;
        float avgTime = 0f;
        foreach (GameObject dest in visitables)
        {
            //add the average happiness per visitor to the total
            float visitTime = dest.GetComponent<IVisitable>().AvgVisitTime();
            avgTime += visitTime;
            float happiness = dest.GetComponent<IVisitable>().RetrieveHappiness();
            avgHappiness += visitTime * happiness;
        }

        //TODO - avgTime needs to take into properly account for travel time - estimation now
        avgTime *= 1.5f;
        Debug.Log("Average Visit Time = " + avgTime);

        //total visitor calculation - maybe accurate?
        int totalVisits = Mathf.FloorToInt(GameController.SaveData.maxVisitors * (secondsAway / visitorCooldown) / (avgTime / visitorCooldown));
        Debug.Log(totalVisits + " visitors over the course of " + timeAway.TotalHours + " hours");
        int totalEarnings = totalVisits * Mathf.CeilToInt(avgHappiness / 60f * 100f);
        
        //update the jar balance
        GameController.SaveData.jarBalance += totalEarnings;

        //save the game
        SaveHandler.SaveSystem.SaveGame();

        //resume visitor spawning if still open
        if (open)
            StartCoroutine(VisitorSpawning());
    }

    //set the variables here for visitor calculation - TODO!!!
    private void VisitorCalc()
    {
        GameObject[] dests = GameObject.FindGameObjectsWithTag("VisitorDestination");
        visitorCooldown = 20 - dests.Length;
        if (visitorCooldown < 1)
            visitorCooldown = 1;
        GameController.SaveData.maxVisitors = 2;
    }

    IEnumerator VisitorSpawning()
    {
        float currentCooldown = 0f;
        while (GameController.SaveData.museumOpen)
        {
            //spawn a visitor
            if (currentCooldown > visitorCooldown)
            {
                if (FindObjectsOfType<VisitorBehavior>().Length < GameController.SaveData.maxVisitors)
                {
                    GameObject newVisitor = visitorPool.GetPooledObject();
                    newVisitor.SetActive(true);
                    newVisitor.transform.position = transform.position;
                    newVisitor.GetComponent<VisitorBehavior>().BeginVisit();
                }
                else
                {
                    Debug.Log("Too many visitors");
                }
                currentCooldown = 0f;
            }
            else
                currentCooldown += 1f;
            yield return new WaitForSeconds(1f);
        }
    }
}
