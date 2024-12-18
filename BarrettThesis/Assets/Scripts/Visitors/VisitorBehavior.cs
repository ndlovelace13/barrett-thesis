using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class VisitorBehavior : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    GameObject finalDest;
    List<GameObject> destinations;
    GameObject currentDest;

    //calculations
    float happiness = 0f;
    bool looking = false;

    public void BeginVisit()
    {
        Debug.Log("A New Visitor has Arrived!");
        
        //reset vars
        happiness = 0f;
        looking = false;

        //acquire destinations
        destinations = GameObject.FindGameObjectsWithTag("VisitorDestination").ToList<GameObject>();

        finalDest = GameObject.FindWithTag("donation");

        //assign the first destination
        UpdateDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if ((!agent.hasPath || agent.velocity.sqrMagnitude == 0f) && !looking)
                {
                    
                    //initiate the happiness calculations
                    BeginLooking();
                }
            }
        }
    }

    private void BeginLooking()
    {
        looking = true;
        if (currentDest != finalDest)
        {
            StartCoroutine(Looking());
        }
        else
            MakeDonation();
    }

    IEnumerator Looking()
    {
        Debug.Log("Now Looking");
        float lookTime = 5f;
        float currentTime = 0f;
        while (currentTime < lookTime)
        {
            happiness += HappinessCalc();
            currentTime += 1f;
            yield return new WaitForSeconds(1f);
        }
        UpdateDestination();
    }

    //do the happiness calculations here
    private float HappinessCalc()
    {
        float deltaHappiness = 1f;
        Debug.Log("Happiness Increased by " + deltaHappiness);
        return deltaHappiness;
    }

    private void MakeDonation()
    {
        int totalDonation = HappyToDollar();
        Debug.Log("Donated " + totalDonation + " for " + happiness + " seconds of happiness");
        GameController.SaveData.jarBalance += totalDonation;
        gameObject.SetActive(false);
    }

    private int HappyToDollar()
    {
        int donation = Mathf.CeilToInt(happiness / 60 * 100);
        return donation;
    }

    //set a new destination
    private void UpdateDestination()
    {
        //if there are still destinations to visit, select the next and remove from the array
        if (destinations.Count > 0)
        {
            currentDest = destinations.First();
            destinations.RemoveAt(0);
        }
        //otherwise go to the donation terminal
        else
        {
            currentDest = finalDest;
        }
        agent.SetDestination(currentDest.transform.position);
        looking = false;
    }
    
}
