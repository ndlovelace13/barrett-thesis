using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this will be used to place all rooms in the world corresponding to RoomData in the Save File
public class MuseumLoader : MonoBehaviour
{
    [SerializeField] GameObject museumContainer;

    //room types
    [SerializeField] GameObject entrancePrefab;
    [SerializeField] GameObject officePrefab;
    [SerializeField] GameObject defaultPrefab;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //call to place all museum rooms at their assigned grid locations
    public void MuseumRestore()
    {
        Debug.Log("Restoring " + GameController.SaveData.roomData.Count + " Rooms");
        //might cause a problem if passing a ref - CHECK ON THIS
        foreach (RoomData room in GameController.SaveData.roomData)
        {
            GameObject newRoom;
            switch (room.roomType)
            {
                case RoomType.ENTRANCE:
                    newRoom = Instantiate(entrancePrefab);
                    break;
                case RoomType.OFFICE:
                    newRoom = Instantiate(officePrefab);
                    break;
                default:
                    newRoom = Instantiate(defaultPrefab);
                    break;
            }
            newRoom.transform.SetParent(transform, false);
            newRoom.transform.localPosition = new Vector3(room.colId, 0, room.rowId);
            newRoom.transform.localScale = Vector3.one * 0.1f;
            newRoom.GetComponent<RoomControl>().PlaceRoom(room);
        }
    }
}
