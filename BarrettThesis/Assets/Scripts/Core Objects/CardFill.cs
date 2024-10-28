using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System;

public class CardFill : MonoBehaviour
{
    Flashcard currentCard;

    [SerializeField] GameObject cardFront;
    [SerializeField] GameObject cardBack;

    [SerializeField] GameObject textObj;
    [SerializeField] GameObject imgPlane;
    [SerializeField] GameObject audioCue;

    AudioSource audioPlayer;
    List<AudioClip> clipList;

    bool cardAssigned;
    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        clipList = new List<AudioClip>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            if (clipList.Count > 0)
            {
                audioPlayer.clip = clipList[0];
                audioPlayer.Play();
            }
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            if (clipList.Count > 1)
            {
                audioPlayer.clip = clipList[1];
                audioPlayer.Play();
            }
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            if (clipList.Count > 2)
            {
                audioPlayer.clip = clipList[2];
                audioPlayer.Play();
            }
        }
    }

    //for the full card information
    public void CardAssign(Flashcard newCard)
    {
        //assign the card
        currentCard = newCard;
        cardAssigned = true;

        //retrieve the important fields from the note
        string noteType = currentCard.noteId;
        NoteType noteInfo = GameController.SaveData.currentDeck.dictRetrieve(noteType);
        currentCard.FieldPrint();
        //cardFront.text = currentCard.fields[noteInfo.front[0]];
        //cardBack.text = currentCard.fields[noteInfo.back[0]];

        //fill the front
        FieldFill(cardFront, noteInfo.front);

        //fill the back
        FieldFill(cardBack, noteInfo.back);
    }

    private void FieldFill(GameObject parent, List<int> indexes)
    {
        foreach (int index in indexes)
        {
            //create an image plane
            if (currentCard.imgFields.Contains(index))
            {
                GameObject newImg = Instantiate(imgPlane, parent.transform);
                //load the image into a texture
                string filePath = GameController.SaveData.currentDeck.mediaPath + currentCard.fields[index];
                Debug.Log(filePath);
                byte[] fileData = File.ReadAllBytes(filePath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                newImg.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", tex);
            }
            //create an audio button
            else if (currentCard.audioFields.Contains(index))
            {
                //TODO - placeholder text for now
                GameObject newAudio = Instantiate(audioCue, parent.transform);
                newAudio.GetComponent<TMP_Text>().text = currentCard.fields[index];

                //load audio
                string filePath = GameController.SaveData.currentDeck.mediaPath + currentCard.fields[index];
                Debug.Log(filePath);
                StartCoroutine(LoadSongCoroutine(filePath));
            }
            //create text field
            else
            {
                GameObject newText = Instantiate(textObj, parent.transform);
                newText.GetComponent<TMP_Text>().text = currentCard.fields[index];
            }

        }
    }
    IEnumerator LoadSongCoroutine(string path)
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
                    Debug.Log(path);
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    clipList.Add(clip);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
                
                
            }
        }
    }
}
