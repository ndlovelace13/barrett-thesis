using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableHandler : MonoBehaviour
{
    [SerializeField] GameObject paintingPrefab;
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
        foreach (var placeable in GameController.SaveData.placeables)
        {
            Debug.Log("painting restored");
            GameObject newPainting = Instantiate(paintingPrefab);
            newPainting.GetComponent<Painting>().RestoreData(placeable);
        }
    }
}
