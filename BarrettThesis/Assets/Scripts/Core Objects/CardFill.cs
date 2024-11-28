using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Reflection;

public class CardFill : MonoBehaviour
{
    protected Flashcard currentCard;

    [SerializeField] protected GameObject cardFront;
    [SerializeField] protected GameObject cardBack;

    [SerializeField] protected GameObject textObj;
    [SerializeField] protected GameObject imgPlane;
    [SerializeField] protected GameObject audioCue;

    protected bool cardAssigned = false;
    protected string prevNoteId;

    // Start is called before the first frame update
    void Start()
    {
        //audioPlayer = GetComponent<AudioSource>();
        //clipList = new List<AudioClip>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetCardID()
    {
        return currentCard.cardId;
    }

    //for the full card information
    public virtual void CardAssign(Flashcard newCard)
    {
        //assign the card
        currentCard = newCard;

        //retrieve the important fields from the note
        string noteType = currentCard.noteId;
        NoteType noteInfo = GameController.SaveData.currentDeck.dictRetrieve(noteType);
        currentCard.FieldPrint();
        //cardFront.text = currentCard.fields[noteInfo.front[0]];
        //cardBack.text = currentCard.fields[noteInfo.back[0]];

        //call the object's FillDecision
        FillDecision(noteInfo);

        cardAssigned = true;
        prevNoteId = currentCard.noteId;
    }

    protected virtual void FillDecision(NoteType noteInfo)
    {
        if (cardAssigned && prevNoteId.Equals(currentCard.noteId))
        {
            //fields already set, just need to fill them in
            FieldReplace(cardFront, noteInfo.front);
            FieldReplace(cardBack, noteInfo.back);
        }
        else
        {
            //fill the front
            FieldFill(cardFront, noteInfo.front);
            //fill the back
            FieldFill(cardBack, noteInfo.back);
        }
    }

    //use this to fill initially
    protected void FieldFill(GameObject parent, List<int> indexes)
    {
        foreach (int index in indexes)
        {
            //create an image plane
            if (currentCard.imgFields.Contains(index))
            {
                GameObject newImg = Instantiate(imgPlane, parent.transform);
                if (currentCard.fields[index] != "")
                {
                    //load the image into a texture
                    string filePath = GameController.SaveData.currentDeck.mediaPath + currentCard.fields[index];
                    //Debug.Log(filePath);
                    byte[] fileData = File.ReadAllBytes(filePath);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(fileData);
                    newImg.GetComponentInChildren<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                }
                else
                    newImg.SetActive(false);
            }
            //create an audio button
            else if (currentCard.audioFields.Contains(index))
            {
                //TODO - placeholder text for now
                GameObject newAudio = Instantiate(audioCue, parent.transform);
                //newAudio.GetComponentInChildren<TMP_Text>().text = currentCard.fields[index];

                //load audio
                string filePath = GameController.SaveData.currentDeck.mediaPath + currentCard.fields[index];
                Debug.Log(filePath);
                StartCoroutine(LoadSongCoroutine(filePath, newAudio));
            }
            //create text field
            else
            {
                GameObject newText = Instantiate(textObj, parent.transform);
                newText.GetComponentInChildren<TMP_Text>().text = currentCard.fields[index];
            }

        }
    }

    protected void FieldReplace(GameObject parent, List<int> indexes)
    {
        //Debug.Log("child 0: " + parent.transform.GetChild(0).gameObject.name);
        for (int i = 0; i < indexes.Count; i++)
        {
            //create an image plane
            if (currentCard.imgFields.Contains(indexes[i]))
            {
                GameObject newImg = parent.transform.GetChild(i).gameObject;
                if (currentCard.fields[indexes[i]] != "")
                {
                    //load the image into a texture
                    string filePath = GameController.SaveData.currentDeck.mediaPath + currentCard.fields[indexes[i]];
                    Debug.Log(filePath);
                    byte[] fileData = File.ReadAllBytes(filePath);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(fileData);
                    newImg.GetComponentInChildren<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                }
                else
                    newImg.SetActive(false);
            }
            //create an audio button
            else if (currentCard.audioFields.Contains(indexes[i]))
            {
                //TODO - placeholder text for now
                GameObject newAudio = parent.transform.GetChild(i).gameObject;
                //newAudio.GetComponent<TMP_Text>().text = currentCard.fields[i];

                //load audio
                string filePath = GameController.SaveData.currentDeck.mediaPath + currentCard.fields[indexes[i]];
                //Debug.Log(filePath);
                StartCoroutine(LoadSongCoroutine(filePath, newAudio));
            }
            //create text field
            else
            {
                Debug.Log(indexes[i] + " " + currentCard.fields[indexes[i]]);
                GameObject newText = parent.transform.GetChild(i).gameObject;
                newText.GetComponentInChildren<TMP_Text>().text = currentCard.fields[indexes[i]];
            }

        }
    }

    //need to add the audioclip to the button, event on click
    IEnumerator LoadSongCoroutine(string path, GameObject button)
    {
        string uri = "file://" + path;
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                try
                {
                    //Debug.Log(path);
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    button.GetComponentInChildren<AudioSource>().clip = clip;
                    //clipList.Add(clip);
                    Debug.Log("audio button properly instantiated");
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
                
                
            }
        }
    }
}
