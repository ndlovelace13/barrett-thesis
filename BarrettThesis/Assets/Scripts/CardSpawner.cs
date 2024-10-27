using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardSpawner : MonoBehaviour
{
    public float cardCooldown;
    [SerializeField] GameObject cardObject;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CardSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CardSpawn()
    {
        foreach (Flashcard card in GameController.SaveData.currentDeck.cards)
        {
            GameObject newCard = Instantiate(cardObject, transform.position, Quaternion.identity);
            newCard.GetComponent<CardFill>().CardAssign(card);
            yield return new WaitForSeconds(cardCooldown);
        }
    }
}
