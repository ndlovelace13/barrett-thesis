using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : Rearrangeable, IInteractable
{
    public bool donation;

    public GameObject displayedObj;
    [SerializeField] Transform displayedLoc;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        surface = LayerMask.GetMask("ground");

        //deprecated now that Donation has its own type
        /*if (donation && GameController.SaveData.placeables.Count == 0)
            DonationCheck(saveData);*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override Vector3 PlaceOffset(GameObject wall)
    {
        Vector3 baseVector = wall.transform.up;
        float offset = transform.localScale.y / 2f;
        return baseVector * offset;
    }

    public override void RestoreData(Placeable placedData)
    {
        base.RestoreData(placedData);
        /*if (placedData.donationPillar)
        {
            DonationCheck(placedData);
        }*/
    }

    /*private void DonationCheck(Placeable placedData)
    {
        GameObject donationJar = GameObject.FindWithTag("donation");
        //first donation pillar can stay
        if (donationJar.transform.parent == null)
        {
            displayedObj = GameObject.FindWithTag("donation");
            donationJar.transform.SetParent(transform);
            donationJar.transform.localPosition = displayedLoc.localPosition;
            donation = true;
        }
        else
        {
            //duplicate donationPillar
            GameController.SaveData.placeables.Remove(placedData);
            Destroy(gameObject);
        }
    }*/
}
