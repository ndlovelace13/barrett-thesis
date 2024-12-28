using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderPanel : MonoBehaviour
{
    //UI Elements
    [SerializeField] TMP_Text title;
    [SerializeField] Image image;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text ownedAvailable;
    [SerializeField] TMP_Text price;
    [SerializeField] Button orderButton;

    //Variables
    public PlaceableControl associatedControl;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    //called by order game mode when the user needs to see all available placeables
    public void AssignPlaceable(PlaceableControl newControl)
    {
        associatedControl = newControl;

        title.text = associatedControl.title;
        description.text = associatedControl.description;

        UpdatePanel();
    }

    public void UpdatePanel()
    {
        ownedAvailable.text = associatedControl.currentPlaced + " Owned | " + associatedControl.currentMax + " Available";
        if (associatedControl.currentPlaced < associatedControl.currentMax)
        {
            price.text = "Purchase now for " + ((float)(associatedControl.currentCost / 100f)).ToString("C2");
            if (associatedControl.currentCost <= GameController.SaveData.balance)
            {
                orderButton.GetComponentInChildren<TMP_Text>().text = "Place Order";
                orderButton.interactable = true;
            }
            else
            {
                orderButton.GetComponentInChildren<TMP_Text>().text = "Insufficient Funds";
                orderButton.interactable = false;
            }
        }
        else
        {
            orderButton.GetComponentInChildren<TMP_Text>().text = "Out of Stock";
            price.text = "Not Currently Available";
            orderButton.interactable = false;
        }
    }

    public void OrderObject()
    {
        associatedControl.currentPlaced++;
        UpdatePrice();
        UpdatePanel();


        Placeable newPlaceable = new Placeable(associatedControl.type);
        GameController.SaveData.newOrders.Add(newPlaceable);
        SaveHandler.SaveSystem.SaveGame();
    }

    public void UpdatePrice()
    {
        GameController.SaveData.balance -= associatedControl.currentCost;
        associatedControl.UpdateCost();
    }
}
