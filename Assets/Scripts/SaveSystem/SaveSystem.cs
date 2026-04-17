using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private const int MaxSlots = 3;

    public static void Save(int slot, RunSaveData data)
    {
        var json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(GetKey(slot), json);
        PlayerPrefs.Save();
    }

    public static RunSaveData Load(int slot)
    {
        if (!HasSave(slot))
            return null;
        
        var json = PlayerPrefs.GetString(GetKey(slot));
        return JsonUtility.FromJson<RunSaveData>(json);
    }
    
    public static bool HasSave(int slot)
    {
        return PlayerPrefs.HasKey(GetKey(slot));
    }

    public static void Delete(int slot)
    {
        PlayerPrefs.DeleteKey(GetKey(slot));
    }

    public static int GetFirstEmptySlot()
    {
        for (var i = 0; i < MaxSlots; i++)
            if (!HasSave(i))
                return i;
        
        return -1;
    }

    public static bool HasFreeSlot()
    {
        return GetFirstEmptySlot() != -1;
    }
    
    private static string GetKey(int slot)
    {
        return $"save_{slot}";
    }
}
