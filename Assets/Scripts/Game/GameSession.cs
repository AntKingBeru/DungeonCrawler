using UnityEngine;
using UnityEngine.Events;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }
    
    [Header("Player")]
    public CharacterSelectionData Player { get; private set; }
    
    [Header("Party")]
    public CharacterSelectionData[] Party { get; private set; } = new CharacterSelectionData[3];
    
    public FormationData Formation { get; set; }

    public UnityEvent onDataUpdated;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Player = new CharacterSelectionData();
        Party = new CharacterSelectionData[3];

        for (var i = 0; i < Party.Length; i++)
            Party[i] = new CharacterSelectionData();
    }
    
    public void SetPlayer(CharacterSelectionData data)
    {
        Player = data;
        onDataUpdated?.Invoke();
    }

    public void SetPartyMember(int index, CharacterSelectionData data)
    {
        Party[index] = data;
        onDataUpdated?.Invoke();
    }

    public void SetFormation(FormationData data)
    {
        Formation = data;
        onDataUpdated?.Invoke();
    }
}