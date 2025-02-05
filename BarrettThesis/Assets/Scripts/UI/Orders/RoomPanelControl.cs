using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanelControl : MonoBehaviour
{
    [SerializeField] GameObject housingPanel;
    [SerializeField] GameObject roomPanelPrefab;

    [SerializeField] GameObject roomRowPrefab;

    List<GameObject> rowList;

    // Start is called before the first frame update
    void Start()
    {
        rowList = new List<GameObject>();
        rowList.Add(housingPanel.GetComponentInChildren<HorizontalLayoutGroup>().gameObject);
        initRoom();
        roomFill();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initRoom()
    {
        if (GameController.SaveData.roomData.Count == 0)
        {
            //instantiate the starting room
            RoomData firstRoom = new RoomData(0, 0, 0);
            GameController.SaveData.roomData.Add(firstRoom);

            //instantiate the office
            RoomData office = new RoomData(1, 0, 1);
            GameController.SaveData.roomData.Add(office);

            //connect the two
            firstRoom.LinkRight(office);

            //save game
            SaveHandler.SaveSystem.SaveGame();
        }
    }

    public void roomFill()
    {
        Debug.Log(GameController.SaveData.roomData.Count + " rooms exist");

        foreach (RoomData room in GameController.SaveData.roomData)
        {
            GameObject newPanel = Instantiate(roomPanelPrefab);
            newPanel.GetComponent<RoomPanel>().currentRoom = room;

            //add another row if necessary
            if (room.rowId - 1 > rowList.Count)
                RowAddition();

            //add the current roomPanel to the associated row
            newPanel.transform.SetParent(rowList[room.rowId].transform, false);
        }
    }

    //handles adding an additional row and orienting it in the housingPanel
    public void RowAddition()
    {
        GameObject newRow = Instantiate(roomRowPrefab);
        newRow.transform.SetParent(housingPanel.transform);
        newRow.transform.SetSiblingIndex(0);
        rowList.Add(newRow);
    }
}
