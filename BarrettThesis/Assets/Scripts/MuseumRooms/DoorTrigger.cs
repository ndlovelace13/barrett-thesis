using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{

    [SerializeField] Animator animControl;
    //[SerializeField] OfficeDoor doorControl;
    public bool entrance;

    BoxCollider trigger;
    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.GameControl.gameMode == GameMode.MATCHING || GameController.SaveData.museumOpen)
        {
            animControl.SetBool("enter", entrance);
            animControl.SetBool("open", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameController.GameControl.gameMode != GameMode.MATCHING && !GameController.SaveData.museumOpen && other.transform.root.tag == "Player")
        {
            Debug.Log("Opening Door");
            animControl.SetBool("enter", entrance);
            animControl.SetBool("open", true);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (GameController.GameControl.gameMode != GameMode.MATCHING && !GameController.SaveData.museumOpen && other.transform.root.tag == "Player")
        {
            Debug.Log("Closing Door");
            animControl.SetBool("enter", entrance);
            animControl.SetBool("open", false);
        }
        
    }
}
