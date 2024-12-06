using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMotion : MonoBehaviour
{
    [SerializeField] protected PlayerMovement player;
    [SerializeField] public Vector3 heldRot;

    float currentY = 0f;
    float maxY = 0.15f;
    float minY = -0.15f;
    bool up = true;
    public bool inspecting = false;
    public bool held = false;
    

    // Start is called before the first frame update
    public virtual void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        held = false;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (player.moving && !inspecting && held)
        {
            if (up)
            {
                if (currentY < maxY)
                {
                    currentY += 1 * Time.deltaTime;
                    transform.localPosition = transform.localPosition + Vector3.up * Time.deltaTime * 0.1f;
                }
                else
                {
                    currentY = maxY;
                    up = false;
                }
            }
            else
            {
                if (currentY > minY)
                {
                    currentY -= 1 * Time.deltaTime;
                    transform.localPosition = transform.localPosition + Vector3.down * Time.deltaTime * 0.1f;
                }
                else
                {
                    currentY = minY;
                    up = true;
                }
            }
        }
    }

    
}
