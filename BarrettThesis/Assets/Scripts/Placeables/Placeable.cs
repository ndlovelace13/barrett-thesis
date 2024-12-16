using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public enum PlaceableType
{
    Painting,
    Seating,
    Pillar,
    Donation,
    Other
}

[Serializable]
public class Placeable
{
    public PlaceableType type;
    public int cardIndex;
    public bool donationPillar;

    public JsonVector location;
    public JsonVector rotation;
    public JsonVector scale;

    public Placeable(GameObject obj)
    {
        type = GetType(obj);
    }

    public Placeable(PlaceableType givenType)
    {
        type = givenType;
    }

    public virtual void SavePlacement(GameObject placeableObj)
    {
        type = GetType(placeableObj);
        location = new JsonVector(placeableObj.transform.position);
        rotation = new JsonVector(placeableObj.transform.rotation.eulerAngles);
        scale = new JsonVector(placeableObj.transform.localScale);

        if (!GameController.SaveData.placeables.Contains(this))
            GameController.SaveData.placeables.Add(this);
    }

    public virtual void Restore(GameObject placeableObj)
    {
        placeableObj.transform.position = location.ToVector();
        placeableObj.transform.rotation = Quaternion.Euler(rotation.ToVector());
        placeableObj.transform.localScale = scale.ToVector();
    }



    public virtual void Print()
    {
        Debug.Log(location.ToString());
        Debug.Log(rotation.ToString());
        Debug.Log("Type: " + type.ToString());
        Debug.Log("Index: " + cardIndex);
    }

    private PlaceableType GetType(GameObject obj)
    {
        if (obj.GetComponent<Painting>() != null)
        {
            cardIndex = obj.GetComponent<Painting>().associatedCard.cardId;
            return PlaceableType.Painting;
        }
        else if (obj.GetComponent<Seating>() != null)
        {
            return PlaceableType.Seating;
        }
        else if (obj.GetComponent<Pillar>() != null)
        {
            donationPillar = obj.GetComponent<Pillar>().donation;
            if (donationPillar)
                return PlaceableType.Donation;
            else
                return PlaceableType.Pillar;
        }
        else
            return PlaceableType.Other;
    }
}

[Serializable]
public class JsonVector
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector()
    {
        return new Vector3 (x, y, z);
    }

    public JsonVector(Vector3 input)
    {
        x = input.x; y = input.y; z = input.z;
    }

    public override string ToString()
    {
        return x + "," + y + "," + z;
    }
}
