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

    Dictionary<Vector2, RoomData> tempRooms;

    // Start is called before the first frame update
    void Start()
    {
        rowList = new List<GameObject>();
        tempRooms = new Dictionary<Vector2, RoomData>();
        RoomReset();
        //InitRoom();
        RoomFill(GameController.SaveData.roomData[0]);
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

        //reset tempList
        tempRooms.Clear();

        //reset rowList
        for (int i = rowList.Count - 1; i >= 0; i--)
        {
            Destroy(rowList[i]);
        }
        rowList.Clear();

        //add the first row
        RowAddition();
    }

    public void RoomFill(RoomData room)
    {
        if (!room.analyzed)
        {
            room.analyzed = true;
            Debug.Log(GameController.SaveData.roomData.Count + " rooms exist");
            Debug.Log(tempRooms.Count + " temp rooms exist");

            GameObject newPanel = Instantiate(roomPanelPrefab);
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
        RoomFill(GameController.SaveData.roomData[0]);

        //may need to completely rebuild the mapping here, in case a tempRoom gets added that is adjacent to a preexisting room

    }
}
