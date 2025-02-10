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

    [SerializeField] GameObject constructionDoor;
    [SerializeField] GameObject officeConstruction;


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
        //DoorwayUpdate();
    }

    public void DoorwayUpdate()
    {
        //check north doorway
        if (roomData.northRoom > -99)
        {
            
            GameObject newDoor = DoorRetrieve(roomData.northRoom, ConstructionCheck(roomData, 0));
            DoorwayReplace(northWall, newDoor, 0);
        }

        //check east doorway
        if (roomData.eastRoom > -99)
        {
            GameObject newDoor = DoorRetrieve(roomData.eastRoom, ConstructionCheck(roomData, 1));
            DoorwayReplace(eastWall, newDoor, 1);
        }

        //check south doorway
        if (roomData.southRoom > -99)
        {
            GameObject newDoor = DoorRetrieve(roomData.southRoom, ConstructionCheck(roomData, 2));
            DoorwayReplace(southWall, newDoor, 2);
        }

        //check west doorway
        if (roomData.westRoom > -99)
        {
            GameObject newDoor = DoorRetrieve(roomData.westRoom, ConstructionCheck(roomData, 3));
            DoorwayReplace(westWall, newDoor, 3);
        }
    }

    public bool ConstructionCheck(RoomData room, int whichDoor)
    {
        bool construction = false;
        switch (whichDoor)
        {
            case 0:
                if (roomData.roomType == RoomType.CONSTRUCTION || GameController.SaveData.roomData[roomData.northRoom].roomType == RoomType.CONSTRUCTION)
                    construction = true;
                break;
            case 1:
                if (roomData.roomType == RoomType.CONSTRUCTION || GameController.SaveData.roomData[roomData.eastRoom].roomType == RoomType.CONSTRUCTION)
                    construction = true;
                break;
            case 2:
                if (roomData.roomType == RoomType.CONSTRUCTION || GameController.SaveData.roomData[roomData.southRoom].roomType == RoomType.CONSTRUCTION)
                    construction = true;
                break;
            case 3:
                if (roomData.roomType == RoomType.CONSTRUCTION || GameController.SaveData.roomData[roomData.westRoom].roomType == RoomType.CONSTRUCTION)
                    construction = true;
                break;
        }
        return construction;
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
                Debug.Log("North Replaced");
                break;
            //East
            case 1:
                eastWall = newDoorway;
                Debug.Log("East Replaced");
                break;
            //South
            case 2:
                southWall = newDoorway;
                Debug.Log("South Replaced");
                break;
            //West
            case 3:
                westWall = newDoorway;
                Debug.Log("West Replaced");
                break;
        }
    }

    public GameObject DoorRetrieve(int index, bool underConstruction)
    {
        GameObject newDoor;
        if (index == 1)
        {
            if (underConstruction)
            {
                Debug.Log("Double bruh");
                newDoor = Instantiate(officeConstruction);
            }
            else
                newDoor = Instantiate(officeDoor);
        }
        else
        {
            if (underConstruction)
            {
                Debug.Log("Triple bruh");
                newDoor = Instantiate(constructionDoor);
            }
            else
                newDoor = Instantiate(defaultDoor);
        }
        return newDoor;
    }
}
