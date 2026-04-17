using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Globalization;

public class GameSession : MonoBehaviour
{
    private const string StartRoomId = "StartRoom";
    
    public static GameSession Instance { get; private set; }
    
    [Header("Player")]
    public CharacterSelectionData Player { get; private set; }
    
    [Header("Party")]
    public CharacterSelectionData[] Party { get; private set; } = new CharacterSelectionData[3];
    
    [Header("Save Data")]
    public int CurrentSlot { get; private set; } = -1;
    public string CurrentRoomId { get; set; }
    
    public FormationData Formation { get; set; }
    public int BossesDefeated { get; private set; }
    public int TotalBosses { get; set; } = 3;

    public UnityEvent onDataUpdated;
    public UnityEvent onBossProgressUpdated;
    public UnityEvent<int, int> onGameLost;
    public UnityEvent onGameWon;

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

    public void StartNewRun(int slot)
    {
        CurrentSlot = slot;
        BossesDefeated = 0;
        CurrentRoomId = StartRoomId;
        
        onDataUpdated?.Invoke();
    }
    
    public void SetPlayer(CharacterSelectionData data)
    {
        Player = data;
        Player.SetHealth(data.MaxHealth);
        Player.SetMana(data.MaxMana);
        onDataUpdated?.Invoke();
    }

    public void SetPartyMember(int index, CharacterSelectionData data)
    {
        Party[index] = data;
        Party[index].SetHealth(data.MaxHealth);
        Party[index].SetMana(data.MaxMana);
        onDataUpdated?.Invoke();
    }

    public void SetFormation(FormationData data)
    {
        Formation = data;
        onDataUpdated?.Invoke();
    }

    public void RegisterBossKill()
    {
        BossesDefeated++;
        onBossProgressUpdated?.Invoke();
    }

    public void HandlePlayerDeath()
    {
        Time.timeScale = 0f;
        
        Cursor.visible = true;
        
        onGameLost?.Invoke(BossesDefeated, TotalBosses);
    }

    public RunSaveData CreateSaveData()
    {
        var data = new RunSaveData
        {
            slotIndex = CurrentSlot,
            playerName = Player.name,
            roomId = CurrentRoomId,
            bossesDefeated = BossesDefeated,
            timestamp = System.DateTime.Now.ToString(CultureInfo.InvariantCulture),
            player = ConvertCharacter(Player)
        };

        foreach (var member in Party)
            data.party.Add(ConvertCharacter(member));
        
        return data;
    }

    private CharacterSaveData ConvertCharacter(CharacterSelectionData source)
    {
        var save = new CharacterSaveData
        {
            classId = source.@class.id,
            currentHealth = source.currentHealth,
            currentMana = source.currentMana,
            bonusMaxHealth = source.bonusMaxHealth,
            bonusMaxMana = source.bonusMaxMana,
            bonusDamage = source.bonusDamage
        };
        
        foreach (var skill in source.unlockedSkills)
            save.unlockedSkills.Add(skill.id);

        return save;
    }

    public void LoadFromSave(RunSaveData data, int slot)
    {
        CurrentSlot = slot;
        
        BossesDefeated = data.bossesDefeated;
        CurrentRoomId = data.roomId;
        
        Player = BuildCharacter(data.player);
        
        Party = new CharacterSelectionData[data.party.Count];

        for (var i = 0; i < Party.Length; i++)
            Party[i] = BuildCharacter(data.party[i]);
        
        onDataUpdated?.Invoke();
    }

    private CharacterSelectionData BuildCharacter(CharacterSaveData save)
    {
        var data = new CharacterSelectionData
        {
            @class = Database.GetClassById(save.classId),
            bonusMaxHealth = save.bonusMaxHealth,
            bonusMaxMana = save.bonusMaxMana,
            bonusDamage = save.bonusDamage
        };

        data.SetHealth(save.currentHealth);
        data.SetMana(save.currentMana);
        
        data.unlockedSkills.Clear();

        foreach (var skill in save.unlockedSkills.Select(Database.GetSkillById).Where(skill => skill))
        {
            data.unlockedSkills.Add(skill);
        }

        return data;
    }
}