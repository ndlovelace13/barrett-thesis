using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatteredHitDetection : MonoBehaviour
{

    [SerializeField] ScatteredCard baseObj;
    // Start is called before the first frame update


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        baseObj.OnMouseDown();
    }

    private void OnMouseEnter()
    {
        baseObj.OnMouseEnter();
    }

    private void OnMouseExit()
    {
        baseObj.OnMouseExit();
    }
}