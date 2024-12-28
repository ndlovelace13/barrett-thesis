using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlaceableDefault")]
public class PlaceableDefault : ScriptableObject
{
    public PlaceableType type;
    public bool unlocked;
    public int startingMax;
    public int absoluteMax;

    //money stuff here?
    public int startingCost;
    public float costMod;

    //UI stuff
    public string title;
    public string description;
}
