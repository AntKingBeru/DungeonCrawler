using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class SceneEntry
{
    public SceneId id;

    [SerializeField] private string sceneName;
    
    #if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;
    
    public void OnValidate()
    {
        if (sceneAsset)
            sceneName = sceneAsset.name;
    }
    #endif
    
    public string SceneName => sceneName;
}