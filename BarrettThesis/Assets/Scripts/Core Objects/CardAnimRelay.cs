using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//necessary because unity be like
public class CardAnimRelay : MonoBehaviour
{
    [SerializeField] ScatteredCard baseCard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyMastery()
    {
        baseCard.ApplyMastery();
    }

    public void SpinComplete()
    {
        baseCard.SpinComplete();
    }

    public void ShakeComplete()
    {
        baseCard.ShakeComplete();
    }
}
