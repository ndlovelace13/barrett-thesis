using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanelControl : MonoBehaviour
{
    //housing parent
    [SerializeField] GameObject housingPanel;

    [Header("Room Panel Types")]
    [SerializeField] GameObject roomPanelPrefab;
    [SerializeField] GameObject officePanelPrefab;
    [SerializeField] GameObject entrancePanelPrefab;
    [SerializeField] GameObject fillerPanelPrefab;

    [SerializeField] GameObject roomRowPrefab;

    List<GameObject> rowList;

    Dictionary<Vector2, RoomData> tempRooms;
    Dictionary<GameObject, Vector3> fillerData;

    int numPerRow;
    int rowCount;

    // Start is called before the first frame update
    void Start()
    {
        rowList = new List<GameObject>();
        tempRooms = new Dictionary<Vector2, RoomData>();
        fillerData = new Dictionary<GameObject, Vector3>();
        RoomReset();
        //InitRoom();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RoomReset()
    {
        //reset analysis in each roomData object
        for (int i = 0; i < GameController.SaveData.roomData.Count; i++)
        {
            GameController.SaveData.roomData[i].analyzed = false;
        }

        //reset tempList & fillerList
        tempRooms.Clear();
        fillerData.Clear();

        //reset rowList
        for (int i = rowList.Count - 1; i >= 0; i--)
        {
            Destroy(rowList[i]);
        }
        rowList.Clear();

        //add the first row
        RowAddition();

        //begin filling the rooms
        RoomFill(GameController.SaveData.roomData[0]);

        //add filler rooms once all rooms have been instantiated
        SpacingFix();
    }

    public void RoomFill(RoomData room)
    {
        if (!room.analyzed)
        {
            room.analyzed = true;
            Debug.Log(GameController.SaveData.roomData.Count + " rooms exist");
            Debug.Log(tempRooms.Count + " temp rooms exist");

            GameObject newPanel = ChoosePanel(room);
            newPanel.GetComponent<RoomPanel>().AssignRoom(room);

            //add another row if necessary
            if (room.rowId > rowList.Count - 1)
                RowAddition();

            //Debug.Log(room.rowId + " may have broke things");
            //add the current roomPanel to the associated row
            newPanel.transform.SetParent(rowList[room.rowId].transform, false);

            //move to front if colId is less than current head
            if (room.colId < rowList[room.rowId].transform.GetChild(0).GetComponent<RoomPanel>().currentRoom.colId)
                newPanel.transform.SetSiblingIndex(0);

            //check all other edges
            if (!room.tempRoom)
                AdjacentRooms(room);
        }
        else
        {
            Debug.Log("room already analyzed, ignoring");
        }
        
    }

    public GameObject ChoosePanel(RoomData room)
    {
        if (room.roomId == 0)
            return Instantiate(entrancePanelPrefab);
        else if (room.roomId == 1)
            return Instantiate(officePanelPrefab);
        else
            return Instantiate(roomPanelPrefab);
    }

    public void AdjacentRooms(RoomData room)
    {
        //-100 means the edge is inaccessible
        //-99 means the edge is unassigned but accessible

        //check north
        if (room.northRoom > -100)
        {
            if (room.northRoom > -99)
                RoomFill(GameController.SaveData.roomData[room.northRoom]);
            else
            {
                //identify the row/col of the tempRoom
                Vector2 tempLoc = new Vector2(room.rowId + 1, room.colId);

                //check if a temp room has already been created at that location
                if (!tempRooms.ContainsKey(tempLoc))
                {
                    //create a new tempRoom
                    RoomData northTemp = new RoomData(tempLoc);
                    tempRooms.Add(new Vector2(northTemp.rowId, northTemp.colId), northTemp);
                    RoomFill(northTemp);
                }
                tempRooms[tempLoc].PotentialSouth(room.roomId);
            }
        }

        //check east
        if (room.eastRoom > -100)
        {
            if (room.eastRoom > -99)
                RoomFill(GameController.SaveData.roomData[room.eastRoom]);
            else
            {
                //identify the row/col of the tempRoom
                Vector2 tempLoc = new Vector2(room.rowId, room.colId + 1);

                //check if a temp room has already been created at that location
                if (!tempRooms.ContainsKey(tempLoc))
                {
                    //create a new tempRoom
                    RoomData eastTemp = new RoomData(tempLoc);
                    tempRooms.Add(new Vector2(eastTemp.rowId, eastTemp.colId), eastTemp);
                    RoomFill(eastTemp);
                }
                tempRooms[tempLoc].PotentialWest(room.roomId);
            }
        }

        //check south
        if (room.southRoom > -100)
        {
            if (room.southRoom > -99)
                RoomFill(GameController.SaveData.roomData[room.southRoom]);
            else
            {
                //identify the row/col of the tempRoom
                Vector2 tempLoc = new Vector2(room.rowId -1, room.colId);

                //check if a temp room has already been created at that location
                if (!tempRooms.ContainsKey(tempLoc) && tempLoc.x > -1)
                {
                    //create a new tempRoom
                    RoomData southTemp = new RoomData(tempLoc);
                    tempRooms.Add(new Vector2(southTemp.rowId, southTemp.colId), southTemp);
                    RoomFill(southTemp);
                }
                if (tempLoc.x > -1)
                {
                    tempRooms[tempLoc].PotentialNorth(room.roomId);
                }
            }
        }

        //check west
        if (room.westRoom > -100)
        {
            if (room.westRoom > -99)
                RoomFill(GameController.SaveData.roomData[room.westRoom]);
            else
            {
                //identify the row/col of the tempRoom
                Vector2 tempLoc = new Vector2(room.rowId, room.colId - 1);

                //check if a temp room has already been created at that location
                if (!tempRooms.ContainsKey(tempLoc))
                {
                    //create a new tempRoom
                    RoomData westTemp = new RoomData(tempLoc);
                    tempRooms.Add(new Vector2(westTemp.rowId, westTemp.colId), westTemp);
                    RoomFill(westTemp);
                }
                tempRooms[tempLoc].PotentialEast(room.roomId);
            }
        }

    }

    //handles adding an additional row and orienting it in the housingPanel
    public void RowAddition()
    {
        GameObject newRow = Instantiate(roomRowPrefab);
        newRow.transform.SetParent(housingPanel.transform, false);
        newRow.transform.SetSiblingIndex(0);
        rowList.Add(newRow);
        Debug.Log(rowList.Count + " rows now available");
    }

    //called whenever a new room is added
    public void RoomAddition(Vector2 coords)
    {
        RoomData addedRoom;
        tempRooms.Remove(coords, out addedRoom);
        RoomReset();
        //may need to completely rebuild the mapping here, in case a tempRoom gets added that is adjacent to a preexisting room

    }

    //called to add filler rooms to any grids that are misaligned
    private void SpacingFix()
    {
        numPerRow = 0;
        int lowestIndex = 1000;
        int highestIndex = -1000;

        //create a fillerData entry for each row in the rowList
        //Vector3(numOfPanels, lowestInd, highestInd)
        for (int i = 0; i < rowList.Count; i++)
        {
            RoomPanel[] rooms = rowList[i].GetComponentsInChildren<RoomPanel>();
            int lowIndex = 1000;
            int highIndex = -1000;

            //grab current low and high indexes first
            foreach (RoomPanel panel in rooms)
            {
                if (panel.currentRoom.colId < lowIndex)
                    lowIndex = panel.currentRoom.colId;
                if (panel.currentRoom.colId > highIndex)
                    highIndex = panel.currentRoom.colId;
            }

            //store to the vector and into the dict
            Vector3 rowData = new Vector3(rooms.Length, lowIndex, highIndex);
            fillerData.Add(rowList[i], rowData);

            //check if they are greater than the current max
            if (rooms.Length > numPerRow)
                numPerRow = rooms.Length;
            if (lowIndex < lowestIndex)
                lowestIndex = lowIndex;
            if (highIndex > highestIndex)
                highestIndex = highIndex;
        }

        Debug.Log("Rows should all have " + numPerRow + " panels");
        Debug.Log("Low: " + lowestIndex + " | High: " + highestIndex);

        int rowCounter = 0;

        foreach (var data in fillerData)
        {
            GameObject currentRow = data.Key;
            Vector3 stats = data.Value;
            Debug.Log("Row " + rowCounter + ": Current Panels - " + stats.x + " | Low Index - " + stats.y + " | High Index - " + stats.z);
            

            /*
            if (stats.x < numPerRow)
            {
                Debug.Log(numPerRow - stats.x + " panels should be added");
            }
            else
                Debug.Log("Row already has enough panels");*/

            //add fillers to the front
            for (int i = lowestIndex; i < stats.y; i++)
            {
                GameObject lowFiller = Instantiate(fillerPanelPrefab, currentRow.transform);
                lowFiller.transform.SetAsFirstSibling();

                Debug.Log("New filler added to front of row " + rowCounter);
            }

            //add fillers to the back
            for (int i = highestIndex; i > stats.z; i--)
            {
                GameObject highFiller = Instantiate(fillerPanelPrefab, currentRow.transform);
                highFiller.transform.SetAsLastSibling();

                Debug.Log("New filler added to back of row " + rowCounter);
            }

            rowCounter++;
        }

        //
    }

    /* Abandon the grid, you're in too deep with rows and columns
    public void GridReset()
    {
        GridLayout grid = housingPanel.GetComponent<GridLayout>();

        //set the row and columns of the grid
       

        //set the spacing of each cell

    }*/
}
