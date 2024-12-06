using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardArrange : MonoBehaviour
{
    [SerializeField] float padding;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpaceCards()
    {
        CardFill[] cards = GetComponentsInChildren<CardFill>();
        GameObject[] currentCards = new GameObject[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            currentCards[i] = cards[i].gameObject;
        }

        float xSize = currentCards[0].GetComponentInChildren<Collider>().bounds.size.x;
        padding = xSize * 0.2f;

        float totalX = xSize * currentCards.Length + padding * (currentCards.Length + 1);
        float startX = -totalX / 2;
        float currentX = startX + padding + xSize / 2;

        for (int i = 0; i < currentCards.Length; i++)
        {
            //currentX += xSize / 2;
            currentCards[i].transform.localPosition = Vector3.right * currentX;
            currentX += xSize + padding;
        }
    }

    
}
