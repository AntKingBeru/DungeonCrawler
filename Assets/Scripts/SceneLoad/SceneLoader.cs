using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    
    [SerializeField] private SceneEntry[] scenes;
    [SerializeField] private GameObject loadingOverlay;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text percentText;
    
    private Dictionary<SceneId, string> _sceneLookup;
    
    public SceneId CurrentScene { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        BuildLookup();
    }
    
    private void BuildLookup()
    {
        _sceneLookup = new Dictionary<SceneId, string>();

        foreach (var entry in scenes)
        {
            if (!_sceneLookup.ContainsKey(entry.id))
            {
                _sceneLookup.Add(entry.id, entry.SceneName);
            }
            else
            {
                Debug.LogWarning($"Duplicate SceneId: {entry.id}");
            }
        }
    }

    public void LoadScene(SceneId id)
    {
        if (!_sceneLookup.TryGetValue(id, out var sceneName))
        {
            Debug.LogError($"Scene not found: {id}");
            return;
        }
        
        CurrentScene = id;
        
        loadingOverlay.SetActive(true);
        
        progressSlider.value = 0f;
        percentText.text = "0%";
        
        StartCoroutine(LoadRoutine(sceneName));
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        var op = SceneManager.LoadSceneAsync(sceneName);
        if (op != null)
        {
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                var targetProgress = Mathf.Clamp01(op.progress / 0.9f);

                progressSlider.value = Mathf.Lerp(progressSlider.value, targetProgress, Time.deltaTime * 10f);
                percentText.text = $"{(int)(targetProgress * 100f)}%";

                yield return null;
            }

            progressSlider.value = 1f;
            percentText.text = "100%";

            yield return new WaitForSeconds(0.2f);

            op.allowSceneActivation = true;

            while (!op.isDone)
                yield return null;
        }

        loadingOverlay.SetActive(false);
    }
    
    public bool IsGameplayScene()
    {
        return CurrentScene == SceneId.GameScene;
    }
}