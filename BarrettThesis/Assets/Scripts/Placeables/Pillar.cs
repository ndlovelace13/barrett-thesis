using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : Rearrangeable, IInteractable
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        surface = LayerMask.GetMask("ground");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
