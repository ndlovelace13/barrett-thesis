using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Localization.LocalizationTableCollection;

public class Order : CoreGameMode, IInteractable
{

    [SerializeField] Canvas orderMenu;
    [SerializeField] GameObject orderHolderRow;
    [SerializeField] GameObject orderHolderCol;
    [SerializeField] GameObject currentOrderHolder;
    [SerializeField] GameObject orderPanel;

    [SerializeField] PlaceableHandler objectHandler;

    [SerializeField] TMP_Text budgetDisplay;

    private List<OrderPanel> orderPanels;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        gameMode = GameMode.ORDERING;
        orderMenu.enabled = false;
        orderPanels = new List<OrderPanel>();
    }

    // Update is called once per frame
    void Update()
    {
        budgetDisplay.text = "Budget: " + ((float)(GameController.SaveData.balance / 100f)).ToString("C2");
    }
    
    public override void CancelInteract()
    {
        base.CancelInteract();
        orderMenu.enabled = false;
    }

    protected override void PostCameraShift()
    {
        base.PostCameraShift();

        orderMenu.enabled = true;
        if (orderPanels.Count > 0)
        {
            PanelUpdate();
        }
        else
            PanelFill();
    }

    private void PanelFill()
    {
        //orderMenu.enabled = true;
        foreach (PlaceableControl control in GameController.SaveData.placeableControls)
        {
            GameObject newPanel = Instantiate(orderPanel);
            //newPanel.transform.SetParent(currentOrderHolder.transform);
            newPanel.GetComponent<OrderPanel>().AssignPlaceable(control);
            orderPanels.Add(newPanel.GetComponent<OrderPanel>());
        }
        PanelUpdate();
    }

    private void PanelUpdate()
    {
        Debug.Log("Sorting " + orderPanels.Count + " panels");
        int counter = 0;
        int rowNum = 0;
        List<HorizontalLayoutGroup> rows = orderHolderCol.GetComponentsInChildren<HorizontalLayoutGroup>().ToList();
        if (rows.Count > 0)
            currentOrderHolder = rows[rowNum].gameObject;
        else
            NewRow(rows);

        foreach (OrderPanel panel in orderPanels)
        {
            if (counter == 4)
            {
                counter = 0;
                rowNum++;

                //add a new row if there is not enough space with the current rows
                if (rowNum >= rows.Count)
                {
                    NewRow(rows);
                }
                else
                {
                    currentOrderHolder = rows[rowNum].gameObject;
                }
            }
            
            //assign the panel to the current row if unlocked
            if (panel.associatedControl.unlocked)
            {
                panel.enabled = true;
                panel.transform.SetParent(currentOrderHolder.transform);
                panel.transform.localScale = Vector3.one;
                counter++;
            }
            else
            {
                panel.enabled = false;
            }
                
        }
    }

    //create a new row and add to the list
    private void NewRow(List<HorizontalLayoutGroup> rows)
    {
        GameObject newRow = Instantiate(orderHolderRow);
        newRow.transform.SetParent(orderHolderCol.transform);
        newRow.transform.localScale = Vector3.one;
        rows.Add(newRow.GetComponent<HorizontalLayoutGroup>());
        currentOrderHolder = newRow;
    }


    private void TutorialOrder()
    {
        Placeable donationJar = new Placeable(PlaceableType.Donation);
        GameController.SaveData.newOrders.Add(donationJar);
        Debug.Log("Tutorial Order Placed");
        SaveHandler.SaveSystem.SaveGame();
    }

    public override string GetPrompt()
    {
        return "Press E to Place an Order";
    }
}
