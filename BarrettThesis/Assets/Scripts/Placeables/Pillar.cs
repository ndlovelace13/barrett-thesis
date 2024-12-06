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

    public override Vector3 PlaceOffset(GameObject wall)
    {
        Vector3 baseVector = wall.transform.up;
        float offset = transform.localScale.y / 2f;
        return baseVector * offset;
    }
}
