using UnityEngine;
using System.Collections.Generic;

public static class Database
{
    private static Dictionary<string, CharacterClassData> _classes;
    private static Dictionary<string, SkillData> _skills;
    
    private const string ClassesPath = "Data/Classes";
    private const string SkillsPath = "Data/Skills";
    
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized)
            return;
        
        _classes = new Dictionary<string, CharacterClassData>();
        _skills = new Dictionary<string, SkillData>();
        
        var classes = Resources.LoadAll<CharacterClassData>(ClassesPath);
        var skills = Resources.LoadAll<SkillData>(SkillsPath);

        foreach (var c in classes)
            _classes.TryAdd(c.id, c);
        
        foreach (var s in skills)
            _skills.TryAdd(s.id, s);
        
        _initialized = true;
    }

    public static CharacterClassData GetClassById(string id)
    {
        Initialize();

        return _classes.GetValueOrDefault(id);
    }

    public static SkillData GetSkillById(string id)
    {
        Initialize();
        
        return _skills.GetValueOrDefault(id);
    }
}