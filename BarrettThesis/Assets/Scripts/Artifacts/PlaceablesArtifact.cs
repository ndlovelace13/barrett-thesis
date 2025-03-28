using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "artifactData", menuName = "ScriptableObjects/Artifacts/PlaceablesArtifact")]
public class PlaceablesArtifact : ArtifactData
{
    public override void UnlockCheck()
    {
        if (GameController.SaveData.placeableCount > unlockThreshold)
            base.UnlockCheck();
        else
            Debug.Log(artifactName + " checked");
    }
}
