using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomData
{
    public int roomId;

    public int rowId;
    public int colId;

    public int northRoom;
    public int eastRoom;
    public int southRoom;
    public int westRoom;

    public RoomData(int id, int rowId, int colId)
    {
        roomId = id;

        //this.rowId = rowId;
        //this.colId = colId;

        northRoom = -1;
        eastRoom = -1;
        southRoom = -1;
        westRoom = -1;

    }

    public void LinkAbove(RoomData otherRoom)
    {
        southRoom = otherRoom.roomId;
        otherRoom.northRoom = roomId;

        colId = otherRoom.colId;
        rowId = otherRoom.rowId + 1;
    }

    public void LinkBelow(RoomData otherRoom)
    {
        northRoom = otherRoom.roomId;
        otherRoom.southRoom = roomId;

        colId = otherRoom.colId;
        rowId = otherRoom.rowId - 1;
    }

    public void LinkLeft(RoomData otherRoom)
    {
        eastRoom = otherRoom.roomId;
        otherRoom.westRoom = roomId;

        colId = otherRoom.colId - 1;
        rowId = otherRoom.rowId;
    }

    public void LinkRight(RoomData otherRoom)
    {
        westRoom = otherRoom.roomId;
        otherRoom.eastRoom = roomId;

        colId = otherRoom.colId + 1;
        rowId = otherRoom.rowId;
    }
}
