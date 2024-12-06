using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : Rearrangeable, IInteractable
{
    // Start is called before the first frame update
    void Start()
    {
        surface = LayerMask.GetMask("ground");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
