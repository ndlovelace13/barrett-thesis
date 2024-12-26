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
        ownedAvailable.text = associatedControl.currentPlaced + " Owned | " + associatedControl.currentMax + " Available";
        if (associatedControl.currentPlaced < associatedControl.currentMax)
        {
            price.text = "Purchase now for " + ((float)(0)).ToString("C2");
            orderButton.interactable = true;
        }
        else
        {
            price.text = "Not Currently Available";
            orderButton.interactable = false;
        }

    }
}
