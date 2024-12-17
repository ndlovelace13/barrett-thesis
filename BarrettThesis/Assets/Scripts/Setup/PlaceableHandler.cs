using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableHandler : MonoBehaviour
{
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
        SaveHandler.SaveSystem.SaveGame();
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
