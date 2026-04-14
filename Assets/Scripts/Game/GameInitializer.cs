using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Transform playerSpawnPoint;
    
    [Header("Prefabs")]
    [SerializeField] private PlayerMovement playerPrefab;
    [SerializeField] private PartyFollower partyPrefab;
    
    [Header("Camera")]
    [SerializeField] private PlayerCameraController cam;
    
    [SerializeField] private HotbarUI hotbarUI;

    public static PlayerMovement PlayerInstance;
    public static PartyFollower[] PartyMembers = new PartyFollower[3];

    private void Start()
    {
        CursorManager.Instance.UpdateCursorState(false);
        Time.timeScale = 1f;
        
        SpawnPlayerAndParty();
    }

    private void SpawnPlayerAndParty()
    {
        var player = Instantiate(
            playerPrefab,
            playerSpawnPoint.position,
            playerSpawnPoint.rotation
        );
        
        PlayerInstance = player;
        
        player.Initialize(GameSession.Instance.Player, cam.transform, hotbarUI);
        cam.Initialize(player.transform);

        for (var i = 0; i < GameSession.Instance.Party.Length; i++)
        {
            var memberData = GameSession.Instance.Party[i];

            var member = Instantiate(
                partyPrefab,
                playerSpawnPoint.position,
                playerSpawnPoint.rotation
            );

            member.Initialize(memberData, player.transform, i);
            
            PartyMembers[i] = member;
        }
    }
}