using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "artifactData", menuName = "ScriptableObjects/Artifacts/DeckPercentArtifact")]
public class DeckPercentArtifact : ArtifactData
{
    public override void UnlockCheck()
    {
        float currentPercent = (float)GameController.SaveData.unlockedCardCount / (float)GameController.SaveData.currentDeck.cards.Count;
        if (currentPercent >= unlockThreshold)
            base.UnlockCheck();
        else
            Debug.Log(artifactName + " checked");
    }
}
