using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArtifactType
{
    TOPMASTERY,
    DECKPERCENT,
    STUDYSTREAK,
    PLACEABLES,
    EXPANSIONS
}

[CreateAssetMenu(fileName = "artifactData", menuName = "ScriptableObjects/Artifacts")]
public class ArtifactData: ScriptableObject
{
    [Header("Core Data")]
    public int id;
    public bool unlocked = false;
    public ArtifactType type;
    public int level;
    public float unlockThreshold;
    public Material mat;

    [Header("Attributes")]
    public string artifactName;
    public string effectDescription;

    public virtual void UnlockCheck()
    {
        //only call this base if the condition is met for the particular artifact
        Unlock();
    }

    public void Unlock()
    {
        //in game notification???

        Debug.Log("ARTIFACT UNLOCK: " + artifactName);
        unlocked = true;
        GameController.SaveData.unlockedArtifacts++;
    }
}
