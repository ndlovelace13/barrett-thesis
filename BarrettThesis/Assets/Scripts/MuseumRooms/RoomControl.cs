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

    //card stuff
    public List<Flashcard> assignedCards;
    [SerializeField] Transform cardSpawn;
    public LayerMask hitLayers;
    ObjectPool scatteredPool;


    // Start is called before the first frame update
    void Start()
    {
        hitLayers = LayerMask.GetMask("ground") | LayerMask.GetMask("Wall") | LayerMask.GetMask("Ceiling");

        //retrieve the pool object
        scatteredPool = GameObject.FindWithTag("ScatteredPool").GetComponent<ObjectPool>();
}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceRoom(RoomData newData)
    {
        roomData = newData;

        assignedCards = new List<Flashcard>();
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

    public int ConstructionCheck(RoomData room, int whichDoor)
    {
        int constructionIndex = -1;
        switch (whichDoor)
        {
            case 0:
                if (roomData.roomType == RoomType.CONSTRUCTION)
                    constructionIndex = roomData.roomId;
                else if (GameController.SaveData.roomData[roomData.northRoom].roomType == RoomType.CONSTRUCTION)
                    constructionIndex = roomData.northRoom;
                break;
            case 1:
                if (roomData.roomType == RoomType.CONSTRUCTION)
                    constructionIndex = roomData.roomId;
                else if (GameController.SaveData.roomData[roomData.eastRoom].roomType == RoomType.CONSTRUCTION)
                    constructionIndex = roomData.eastRoom;
                break;
            case 2:
                if (roomData.roomType == RoomType.CONSTRUCTION)
                    constructionIndex = roomData.roomId;
                else if (GameController.SaveData.roomData[roomData.southRoom].roomType == RoomType.CONSTRUCTION)
                    constructionIndex = roomData.southRoom;
                break;
            case 3:
                if (roomData.roomType == RoomType.CONSTRUCTION)
                    constructionIndex = roomData.roomId;
                else if (GameController.SaveData.roomData[roomData.westRoom].roomType == RoomType.CONSTRUCTION)
                    constructionIndex = roomData.westRoom;
                break;
        }
        return constructionIndex;
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

    public GameObject DoorRetrieve(int index, int constructionIndex)
    {
        GameObject newDoor;
        if (index == 1 || roomData.roomId == 1)
        {
            Debug.Log("Office Detected");
            if (constructionIndex > -1)
            {
                Debug.Log("Double bruh");
                newDoor = Instantiate(officeConstruction);
                newDoor.GetComponentInChildren<ConstructionTape>().AssignData(constructionIndex);
            }
            else
            {
                newDoor = Instantiate(officeDoor);
                Debug.Log("Office Door Created");
            }
                
        }
        else
        {
            if (constructionIndex > -1)
            {
                Debug.Log("Triple bruh");
                newDoor = Instantiate(constructionDoor);
                newDoor.GetComponentInChildren<ConstructionTape>().AssignData(constructionIndex);
            }
            else
                newDoor = Instantiate(defaultDoor);
        }
        return newDoor;
    }

    //place all the cards assigned into the environment at a random position determined by a raycast from the center
    public void ScatterCards()
    {
        Debug.Log("Room No. " + roomData.roomId + " has " + assignedCards.Count + " cards");
        Debug.Log("Room No. " + roomData.roomId + " at " + transform.position);

        for (int i = 0; i < assignedCards.Count; i++)
        {
            //calculate a random angle

            Vector3 finalAngle = new Vector3(Random.Range(-180f, 180f), Random.Range(-30f, 30f), Random.Range(-180f, 180f));
            finalAngle.Normalize();

            Debug.Log("Stupid fucking angle --> " + finalAngle);

            //calculate magnitude
            //Vector3 ceilingCorner = ceiling.GetComponent<MeshRenderer>().bounds.max;
            float mag = 15f;

            Debug.Log("Raycasting from " + cardSpawn.position + " to " + (cardSpawn.position + (finalAngle * mag)));
            RaycastHit hit;
            if (Physics.Raycast(cardSpawn.position, cardSpawn.position + (finalAngle * mag), out hit, mag, hitLayers))
            {
                Debug.Log(hit.point);
                //GameObject newCard = Instantiate(GameController.GameControl.scatteredCard, hit.point, Quaternion.identity);
                //get new card from pool
                GameObject newCard = scatteredPool.GetPooledObject();
                newCard.SetActive(true);
                newCard.transform.position = hit.point;
                newCard.GetComponent<ScatteredCard>().Place(hit);
                newCard.GetComponent<ScatteredCard>().FillCard(assignedCards[i]);
                //newCard.GetComponent<Rigidbody>().useGravity = false;
            }
            else
                Debug.Log("NO OBJECT HIT");
        }
        
    }
}
