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


    public RoomData(Vector2 gridId)
    {
        tempRoom = true;
        analyzed = false;

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

        this.rowId = rowId;
        this.colId = colId;

        northRoom = -99;
        eastRoom = -99;
        southRoom = -99;
        westRoom = -99;

        TypeAssign();
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

    public void LinkAbove(RoomData otherRoom)
    {
        southRoom = otherRoom.roomId;
        otherRoom.northRoom = roomId;

        colId = otherRoom.colId;
        rowId = otherRoom.rowId + 1;

        EdgeCheck();
    }

    public void LinkBelow(RoomData otherRoom)
    {
        northRoom = otherRoom.roomId;
        otherRoom.southRoom = roomId;

        colId = otherRoom.colId;
        rowId = otherRoom.rowId - 1;

        EdgeCheck();
    }

    public void LinkLeft(RoomData otherRoom)
    {
        eastRoom = otherRoom.roomId;
        otherRoom.westRoom = roomId;

        colId = otherRoom.colId - 1;
        rowId = otherRoom.rowId;

        EdgeCheck();
    }

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
}
