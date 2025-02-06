using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used for controlling the room
public class RoomControl : MonoBehaviour
{
    public RoomData roomData;

    [SerializeField] GameObject northWall;
    [SerializeField] GameObject southWall;
    [SerializeField] GameObject eastWall;
    [SerializeField] GameObject westWall;
    [SerializeField] GameObject ceiling;

    [SerializeField] GameObject defaultDoor;
    [SerializeField] GameObject officeDoor;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceRoom(RoomData newData)
    {
        roomData = newData;
        DoorwayUpdate();
    }

    public void DoorwayUpdate()
    {
        //check north doorway
        if (roomData.northRoom > -99)
        {
            GameObject newDoor = DoorRetrieve(roomData.northRoom);
            DoorwayReplace(northWall, newDoor, 0);
        }

        //check east doorway
        if (roomData.eastRoom > -99)
        {
            GameObject newDoor = DoorRetrieve(roomData.eastRoom);
            DoorwayReplace(eastWall, newDoor, 1);
        }

        //check south doorway
        if (roomData.southRoom > -99)
        {
            GameObject newDoor = DoorRetrieve(roomData.southRoom);
            DoorwayReplace(southWall, newDoor, 2);
        }

        //check west doorway
        if (roomData.westRoom > -99)
        {
            GameObject newDoor = DoorRetrieve(roomData.westRoom);
            DoorwayReplace(westWall, newDoor, 3);
        }
    }

    public void DoorwayReplace(GameObject oldDoorway, GameObject newDoorway, int whichWall)
    {
        //set parent
        newDoorway.transform.SetParent(transform, false);

        //exchange the wall and take the transform
        newDoorway.transform.localPosition = oldDoorway.transform.localPosition;
        newDoorway.transform.localRotation = oldDoorway.transform.localRotation;
        newDoorway.transform.localScale = oldDoorway.transform.localScale;
        
        //delete the old doorway
        Destroy(oldDoorway);

        switch (whichWall)
        {
            //North
            case 0:
                northWall = newDoorway;
                break;
            //East
            case 1:
                eastWall = newDoorway;
                break;
            //South
            case 2:
                southWall = newDoorway;
                break;
            //West
            case 3:
                westWall = newDoorway;
                break;
        }
    }

    public GameObject DoorRetrieve(int index)
    {
        GameObject newDoor;
        if (index == 1)
        {
            newDoor = Instantiate(officeDoor);
        }
        else
        {
            newDoor = Instantiate(defaultDoor);
        }
        return newDoor;
    }
}
