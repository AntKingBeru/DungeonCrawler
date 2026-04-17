using System;
using System.Collections.Generic;

[Serializable]
public class RunSaveData
{
    public int slotIndex;

    public string playerName;

    public string roomId;
    public int bossesDefeated;

    public CharacterSaveData player;
    public List<CharacterSaveData> party = new();

    public string timestamp;
}