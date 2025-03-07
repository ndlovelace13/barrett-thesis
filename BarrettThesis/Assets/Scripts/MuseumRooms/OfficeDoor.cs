using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeDoor : MonoBehaviour
{
    [SerializeField] Animator animControl;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            animControl.SetBool("inDoorway", true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            animControl.SetBool("inDoorway", false);
        }
    }
}
