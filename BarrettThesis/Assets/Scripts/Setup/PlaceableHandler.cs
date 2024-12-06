using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableHandler : MonoBehaviour
{
    [SerializeField] GameObject paintingPrefab;
    [SerializeField] GameObject seatingPrefab;
    [SerializeField] GameObject pillarPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceableRestore()
    {
        Debug.Log("Placeables Detected: " + GameController.SaveData.placeables.Count);
        for (int i = 0; i < GameController.SaveData.placeables.Count; i++)
        {
            GameObject newObj = null;
            switch (GameController.SaveData.placeables[i].type)
            {
                case PlaceableType.Painting:
                    Debug.Log("painting restored");
                    newObj = Instantiate(paintingPrefab);
                    break;
                case PlaceableType.Seating:
                    Debug.Log("seating restored");
                    newObj = Instantiate(seatingPrefab);
                    break;
                case PlaceableType.Pillar:
                    newObj = Instantiate(pillarPrefab);
                    break;
            }
            if (newObj != null)
            {
                newObj.GetComponent<Painting>().RestoreData(GameController.SaveData.placeables[i]);
            }
        }
        SaveHandler.SaveSystem.SaveGame();
    }
}
