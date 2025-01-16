using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : Rearrangeable, IInteractable, IVisitable
{
    protected int level;
    float minVisit;
    float maxVisit;

    GameObject currentPillar;

    // Start is called before the first frame update
    protected override void Awake()
    {
        VisitCalc();
        base.Awake();
        surface = LayerMask.GetMask("Interactable");
    }

    //interact to move it - snap to predefined locations
    public override bool Interact()
    {
        Debug.Log("Artifact Interact Reached");
        //reset the parent behavior
        if (currentPillar != null)
        {
            currentPillar.GetComponent<Pillar>().displayedObj = null;
            transform.SetParent(null, true);
            currentPillar = null;
        }

        base.Interact();

        return true;
    }

    public override bool CancelInteract()
    {
        if (base.CancelInteract())
        {
            PillarPlace();
            return true;
        }
        return false;
    }

    private void PillarPlace()
    {
        //saveData.Print();
        currentPillar.GetComponent<Pillar>().displayedObj = gameObject;
        transform.SetParent(currentPillar.transform, true);
        //saveData.Print();
        Debug.Log("Artifact Placed");
    }

    public override void PlacementCheck()
    {
        RaycastHit hit = new RaycastHit();
        //must be a pillar for artifacts specifically
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f, surface)
            && hit.collider.transform.GetComponent<Pillar>() != null)
            Place(hit);
        else
            Hold();
    }

    public override Vector3 PlaceOffset(GameObject pillar)
    {
        Vector3 baseVector = pillar.transform.up;
        float offset = transform.localScale.y / 2f;
        return baseVector * offset;
    }

    public override void Place(RaycastHit hit)
    {
        inPlace = true;
        currentPillar = hit.collider.gameObject;
        transform.rotation = Quaternion.Euler(currentPillar.transform.rotation.eulerAngles + new Vector3(0, 90, 0));
        transform.position = currentPillar.GetComponent<Pillar>().displayedLoc.position + PlaceOffset(currentPillar);
    }

    public override string GetPrompt()
    {
        GameObject obj = GameObject.FindWithTag("ObjectSlot");
        if (obj != null)
        {
            if (transform.parent == obj)
            {
                return "Press E to Place Artifact";
            }
            else
            {
                return "Press E to Replace";
            }
        }
        else
        {
            return base.GetPrompt();
        }

    }

    public Placeable GetPillarData()
    {
        Placeable returnData = null;
        if (currentPillar != null)
        {
            returnData = currentPillar.GetComponent<Rearrangeable>().saveData;
        }
        return returnData;
    }

    private void VisitCalc()
    {
        minVisit = 7 + (level * 1.5f);
        maxVisit = 12 + (level * 1.5f);
    }

    public float VisitTime()
    {
        float visitTime = Random.Range(minVisit, maxVisit);
        Debug.Log(minVisit + " " + maxVisit + "= Range | Decided Time: " + visitTime);
        return visitTime;
    }

    public float RetrieveHappiness()
    {
        return 2 + (level * 0.75f);
    }

    public float AvgVisitTime()
    {
        return (minVisit + maxVisit) / 2f;
    }

    public override void RestoreData(Placeable placedData)
    {
        base.RestoreData(placedData);
        Debug.Log("We got here");
        if (placedData.pillarIndex != -1)
        {
            List<Pillar> allPillars = GameObject.FindWithTag("PlaceableHandler").GetComponent<PlaceableHandler>().pillarStorage;
            Debug.Log("no. of pillars: " + allPillars.Count);
            for (int i = 0; i < allPillars.Count; i++)
            {
                if (allPillars[i].saveData.pillarIndex == placedData.pillarIndex)
                {
                    currentPillar = allPillars[i].gameObject;
                    //CancelInteract();
                    Debug.Log("Pillar Parent Found");
                    PillarPlace();
                    break;
                }
            }
        }
        Debug.Log("This pillar index: " + placedData.pillarIndex);
    }
}
