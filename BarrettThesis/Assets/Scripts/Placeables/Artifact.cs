using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : Rearrangeable, IInteractable, IVisitable
{
    protected int level;
    float minVisit;
    float maxVisit;

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
        base.Interact();
        return true;
    }

    public override void CancelInteract()
    {
        base.CancelInteract();
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
        GameObject currentPillar = hit.collider.gameObject;
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
}
