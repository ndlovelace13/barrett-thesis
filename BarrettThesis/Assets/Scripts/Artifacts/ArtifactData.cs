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
    public int unlockDate;
    public bool onDisplay = false;
    public ArtifactType type;
    public int level;
    public float unlockThreshold;
    public Material mat;
    public Sprite texture;

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
        //in game notification - add to the queue, artifact control will handle the rest
        GameObject.FindFirstObjectByType<ArtifactControl>().unlockQueue.Add(this);
        GameObject.FindFirstObjectByType<TutorialControl>().CheckTutorial("firstArtifact");

        Debug.Log("ARTIFACT UNLOCK: " + artifactName);
        unlocked = true;
        unlockDate = GameController.SaveData.dayIndex;
        GameController.SaveData.unlockedArtifacts++;
    }
}
