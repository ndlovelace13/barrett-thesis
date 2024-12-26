using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlaceableControl
{
    public PlaceableType type;
    public bool unlocked;
    public int currentPlaced;
    public int currentMax;
    public int absoluteMax;

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

        title = ogSpecs.title;
        description = ogSpecs.description;
    }
}

public class PlaceableHandler : MonoBehaviour
{
    [SerializeField] List<PlaceableDefault> defaultControls;
    Dictionary<PlaceableType, PlaceableControl> controlDict;

    [SerializeField] GameObject paintingPrefab;
    [SerializeField] GameObject seatingPrefab;
    [SerializeField] GameObject pillarPrefab;
    [SerializeField] GameObject donationPrefab;

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
        //setup control objects for each if doesn't exist
            ControlSetup();

        Delivery();

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
                    break;
                case PlaceableType.Donation:
                    Debug.Log("donation pillar restored");
                    newObj = Instantiate(donationPrefab);
                    break;
            }
            if (newObj != null)
            {
                newObj.GetComponent<Rearrangeable>().RestoreData(GameController.SaveData.placeables[i]);
            }
        }
        //SaveHandler.SaveSystem.SaveGame();
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

    public void Delivery()
    {
        //enable delivery boxes if there are any items in orderedPlaceables
        if (GameController.SaveData.orderedPlaceables.Count > 0)
            deliveryBox.SetActive(true);
        else
            deliveryBox.SetActive(false);
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
            default:
                break;

        }
        return returnedObj;
    }

    public GameObject RetrievePainting()
    {
        return Instantiate(paintingPrefab);
    }
}
