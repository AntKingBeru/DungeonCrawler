using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DungeonEntrance : MonoBehaviour
{
    private const string PlayerTag = "Player";
    
    [SerializeField] private Transform dungeonSpawnPoint;
    [SerializeField] private InputActionReference interactAction;

    [SerializeField] private DungeonSpawner spawner;

    private bool _playerInside;
    
    private void OnEnable()
    {
        interactAction.action.Enable();
    }
    
    private void OnDisable()
    {
        interactAction.action.Disable();
    }

    private void Update()
    {
        if (_playerInside && interactAction.action.triggered)
            EnterDungeon();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            _playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            _playerInside = false;
        }
    }

    private void EnterDungeon()
    {
        StartCoroutine(EnterDungeonRoutine());
    }

    private IEnumerator EnterDungeonRoutine()
    {
        yield return FadeManager.Instance.FadeRoutine(TeleportToDungeon);
    }

    private void TeleportToDungeon()
    {
        spawner.SpawnDungeon();
        
        var dungeonStart = dungeonSpawnPoint.position;
        
        GameInitializer.PlayerInstance.transform.position = dungeonStart;
        
        foreach (var member in GameInitializer.PartyMembers)
            member.transform.position = dungeonStart;
    }
}