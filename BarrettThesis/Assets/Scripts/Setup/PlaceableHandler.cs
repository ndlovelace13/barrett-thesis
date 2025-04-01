using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlaceableControl
{
    public PlaceableType type;
    public bool unlocked;
    public int currentPlaced;
    public int currentMax;
    public int absoluteMax;
    public int extraPerRoom;

    //cost
    public int currentCost;
    public float costMod;

    //UI stuff
    public string title;
    public string description;

    public PlaceableControl(PlaceableDefault ogSpecs)
    {
        type = ogSpecs.type;
        unlocked = ogSpecs.unlocked;
        currentPlaced = 0;
        currentMax = ogSpecs.startingMax;
        absoluteMax = ogSpecs.absoluteMax;
        extraPerRoom = ogSpecs.extraPerRoom;

        currentCost = ogSpecs.startingCost;
        costMod = ogSpecs.costMod;

        title = ogSpecs.title;
        description = ogSpecs.description;
    }

    public void UpdateCost()
    {
        currentCost = Mathf.CeilToInt(currentCost * costMod);
        Debug.Log("Price increased to " + ((float)(currentCost / 100f)).ToString("C2"));
    }

    public void IncreaseMax()
    {
        currentMax += extraPerRoom;
    }
}

public class PlaceableHandler : MonoBehaviour
{
    [SerializeField] List<PlaceableDefault> defaultControls;
    public Dictionary<PlaceableType, PlaceableControl> controlDict;

    [SerializeField] GameObject paintingPrefab;
    [SerializeField] GameObject seatingPrefab;
    [SerializeField] GameObject pillarPrefab;
    [SerializeField] GameObject donationPrefab;
    [SerializeField] GameObject artifactPrefab;

    public List<Pillar> pillarStorage;

    [SerializeField] GameObject deliveryBox;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaceableRestore()
    {
        deliveryBox = FindObjectOfType<Deliveries>().gameObject;

        //setup control objects for each if doesn't exist
        ControlSetup();

        StorageSetup();

        Debug.Log("Placeables Detected: " + GameController.SaveData.placeables.Count);
        for (int i = 0; i < GameController.SaveData.placeables.Count; i++)
        {
            GameObject newObj = null;
            switch (GameController.SaveData.placeables[i].type)
            {
                case PlaceableType.Painting:
                    Debug.Log("painting restored");
                    newObj = Instantiate(paintingPrefab);
                    break;
                case PlaceableType.Seating:
                    Debug.Log("seating restored");
                    newObj = Instantiate(seatingPrefab);
                    break;
                case PlaceableType.Pillar:
                    Debug.Log("pillar restored");
                    newObj = Instantiate(pillarPrefab);
                    pillarStorage.Add(newObj.GetComponent<Pillar>());
                    break;
                case PlaceableType.Donation:
                    Debug.Log("donation pillar restored");
                    newObj = Instantiate(donationPrefab);
                    break;
                case PlaceableType.Artifact:
                    Debug.Log("artifact restored");
                    newObj = Instantiate(artifactPrefab);
                    break;
            }
            if (newObj != null)
            {
                newObj.GetComponent<Rearrangeable>().RestoreData(GameController.SaveData.placeables[i]);
            }
        }

        //add additional objects to the ordered placeables if something got fucked up
        MismatchCheck();

        Delivery();
        //SaveHandler.SaveSystem.SaveGame();
    }

    public void StorageSetup()
    {
        pillarStorage = new List<Pillar>();   
    }

    public void ControlSetup()
    {
        Debug.Log("Setting Up Placeable Controllers");
        //fill in the control list with defaults if it is not already filled
        if (GameController.SaveData.placeableControls.Count == 0)
        {
            foreach (PlaceableDefault control in defaultControls)
            {
                PlaceableControl newControl = new PlaceableControl(control);
                GameController.SaveData.placeableControls.Add(newControl);
            }
            Debug.Log(GameController.SaveData.placeableControls.Count + " defaults have been added to the list");
        }
        
        controlDict = new Dictionary<PlaceableType, PlaceableControl>();
        //create a dictionary entry for each item in the list - more easy to retrieve based on type
        foreach (PlaceableControl control in GameController.SaveData.placeableControls)
        {
            controlDict.Add(control.type, control);
        }
        Debug.Log(controlDict.Count + " placeable controllers are in the dictionary");
    }

    public void SpecialDelivery()
    {
        GameController.SaveData.orderedPlaceables.AddRange(GameController.SaveData.newOrders);
        GameController.SaveData.newOrders.Clear();
        Delivery();
    }

    public void Delivery()
    {

        //enable delivery boxes if there are any items in orderedPlaceables
        if (GameController.SaveData.orderedPlaceables.Count > 0)
        {
            deliveryBox.SetActive(true);
            Debug.Log("Delivery Box correctly enabled");

            /*if (mismatch)
            {
                deliveryBox.GetComponent<Deliveries>().Mismatch();
            }*/
        }
            
        else
        {
            deliveryBox.SetActive(false);
            Debug.Log("Why the fuck would this work as intended");
        }
    }

    public void MismatchCheck()
    {
        Debug.Log("Orders before mismatch: " + GameController.SaveData.orderedPlaceables.Count);

        //check paintings
        Painting[] paintings = GameObject.FindObjectsOfType<Painting>();
        int newCount = controlDict[PlaceableType.Painting].currentPlaced - paintings.Length - CountOrdered(PlaceableType.Painting);
        Debug.Log("Painting Count: " + newCount);
        for (int i = 0; i < newCount; i++)
        {
            Placeable newPlaceable = new Placeable(PlaceableType.Painting);
            GameController.SaveData.orderedPlaceables.Add(newPlaceable);
        }

        //check pillar
        Pillar[] pillars = GameObject.FindObjectsOfType<Pillar>();
        newCount = controlDict[PlaceableType.Pillar].currentPlaced - pillars.Length - CountOrdered(PlaceableType.Pillar);
        Debug.Log("Pillar Count: " + newCount);
        for (int i = 0; i < newCount; i++)
        {
            Placeable newPlaceable = new Placeable(PlaceableType.Pillar);
            GameController.SaveData.orderedPlaceables.Add(newPlaceable);
        }

        //check donation
        DonationJar[] donations = GameObject.FindObjectsOfType<DonationJar>();
        newCount = controlDict[PlaceableType.Donation].currentPlaced - donations.Length - CountOrdered(PlaceableType.Donation);
        Debug.Log("Donation Count: " + newCount);
        for (int i = 0; i < newCount; i++)
        {
            Placeable newPlaceable = new Placeable(PlaceableType.Donation);
            GameController.SaveData.orderedPlaceables.Add(newPlaceable);
        }

        Debug.Log("Orders after mismatch: " + GameController.SaveData.orderedPlaceables.Count);
    }

    public int CountOrdered(PlaceableType type)
    {
        int count = 0;
        for (int i = 0; i < GameController.SaveData.orderedPlaceables.Count; i++)
            if (GameController.SaveData.orderedPlaceables[i].type == type)
                count++;

        return count;
    }

    //DEBUG
    public GameObject RetrieveOrder(Placeable order)
    {
        GameObject returnedObj = null;
        switch (order.type)
        {
            case PlaceableType.Painting:
                returnedObj = Instantiate(paintingPrefab);
                break;
            case PlaceableType.Seating:
                returnedObj = Instantiate(seatingPrefab);
                break;
            case PlaceableType.Pillar:
                returnedObj = Instantiate(pillarPrefab);
                break;
            case PlaceableType.Donation:
                returnedObj = Instantiate(donationPrefab);
                break;
            case PlaceableType.Artifact:
                returnedObj = Instantiate(artifactPrefab);
                break;
            default:
                Debug.Log("you serve zero purpose");
                break;

        }
        return returnedObj;
    }

    public GameObject RetrievePainting()
    {
        return Instantiate(paintingPrefab);
    }

    public GameObject RetrieveArtifact()
    {
        return Instantiate(artifactPrefab);
    }

    public void RoomGrowth()
    {
        //increase the max visitors as well
        GameController.SaveData.maxVisitors += 2;

        foreach (var controller in controlDict.Values)
        {
            controller.IncreaseMax();
        }
    }
}
