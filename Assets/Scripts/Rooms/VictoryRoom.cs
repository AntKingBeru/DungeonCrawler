using UnityEngine;
using System.Collections;

public class VictoryRoom : Room
{
    [Header("Victory")]
    [SerializeField] private GameObject victoryVFX;
    [SerializeField] private Transform centerPoint;

    protected override IEnumerator EncounterRoutine()
    {
        CloseAllDoors();
        
        yield return new WaitForSecondsRealtime(0.2f);

        TriggerVictory();
        
        yield return new WaitForSecondsRealtime(1f);
        
        OpenAllDoors();
    }

    private void TriggerVictory()
    {
        if (victoryVFX)
            Instantiate(victoryVFX, centerPoint.position, Quaternion.identity);
        
        GameSession.Instance.onGameWon?.Invoke();
    }
}