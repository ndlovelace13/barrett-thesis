using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    ENTRANCE,
    DEFAULT,
    OFFICE,
    CONSTRUCTION
}


[Serializable]
public class RoomData
{
    public int roomId;

    public int rowId;
    public int colId;

    public RoomType roomType;

    public int northRoom;
    public int eastRoom;
    public int southRoom;
    public int westRoom;

    public bool tempRoom;

    public bool analyzed;

    public int dayFinished;


    public RoomData(Vector2 gridId)
    {
        tempRoom = true;
        analyzed = false;

        dayFinished = 0;

        rowId = (int)gridId.x;
        colId = (int)gridId.y;

        northRoom = -100;
        eastRoom = -100;
        southRoom = -100;
        westRoom = -100;

        TypeAssign();
    }

    public RoomData(int id)
    {
        tempRoom = false;
        analyzed = false;

        dayFinished = 0;

        roomId = id;

        northRoom = -99;
        eastRoom = -99;
        southRoom = -99;
        westRoom = -99;

        TypeAssign();
    }

    public RoomData(int id, int rowId, int colId)
    {
        tempRoom = false;
        analyzed = false;
        roomId = id;

        dayFinished = 0;

        this.rowId = rowId;
        this.colId = colId;

        northRoom = -99;
        eastRoom = -99;
        southRoom = -99;
        westRoom = -99;

        TypeAssign();
    }

    public void RealizeRoom()
    {
        roomId = GameController.SaveData.roomData.Count;
        tempRoom = false;
        analyzed = false;
        GameController.SaveData.roomData.Add(this);

        //set the conditions for fully built
        roomType = RoomType.CONSTRUCTION;
        dayFinished = GameController.SaveData.dayIndex + 1;

        LinkNewRoom();
    }

    public void LinkNewRoom()
    {
        if (northRoom > -100)
        {
            RoomData roomBelow = GameController.SaveData.roomData[northRoom];
            LinkBelow(roomBelow);
        }
        else
        {
            Debug.Log("new edge");
            northRoom = -99;
        }

        if (eastRoom > -100)
        {
            RoomData roomRight = GameController.SaveData.roomData[eastRoom];
            LinkLeft(roomRight);
        }
        else
        {
            Debug.Log("new edge");
            eastRoom = -99;
        }

        if (southRoom > -100)
        {
            RoomData roomAbove = GameController.SaveData.roomData[southRoom];
            LinkAbove(roomAbove);
        }
        else
        {
            Debug.Log("new edge");
            southRoom = -99;
        }

        if (westRoom > -100)
        {
            RoomData roomLeft = GameController.SaveData.roomData[westRoom];
            LinkRight(roomLeft);
        }
        else
        {
            Debug.Log("new edge");
            westRoom = -99;
        }
    }

    private void TypeAssign()
    {
        if (roomId == 0)
            roomType = RoomType.ENTRANCE;
        else if (roomId == 1)
            roomType = RoomType.OFFICE;
        else
            roomType = RoomType.DEFAULT;
    }

    //called when the currentRoom is to the TOP of the otherRoom
    public void LinkAbove(RoomData otherRoom)
    {
        southRoom = otherRoom.roomId;
        otherRoom.northRoom = roomId;

        colId = otherRoom.colId;
        rowId = otherRoom.rowId + 1;

        EdgeCheck();
    }

    //called when the currentRoom is to the BOTTOM of the otherRoom
    public void LinkBelow(RoomData otherRoom)
    {
        northRoom = otherRoom.roomId;
        otherRoom.southRoom = roomId;

        colId = otherRoom.colId;
        rowId = otherRoom.rowId - 1;

        EdgeCheck();
    }

    //called when the currentRoom is to the LEFT of the otherRoom
    public void LinkLeft(RoomData otherRoom)
    {
        eastRoom = otherRoom.roomId;
        otherRoom.westRoom = roomId;

        colId = otherRoom.colId - 1;
        rowId = otherRoom.rowId;

        EdgeCheck();
    }

    //called when the currentRoom is to the RIGHT of the otherRoom
    public void LinkRight(RoomData otherRoom)
    {
        westRoom = otherRoom.roomId;
        otherRoom.eastRoom = roomId;

        colId = otherRoom.colId + 1;
        rowId = otherRoom.rowId;

        EdgeCheck();
    }

    public void EdgeCheck()
    {
        if (rowId == 0)
        {
            southRoom = -100;
        }
    }

    public void ResetAnalyzed()
    {
        analyzed = false;
    }

    public void PotentialNorth(int id)
    {
        northRoom = id;
    }

    public void PotentialEast(int id)
    {
        eastRoom = id;
    }

    public void PotentialSouth(int id)
    {
        southRoom = id;
    }

    public void PotentialWest(int id)
    {
        westRoom = id;
    }
}
