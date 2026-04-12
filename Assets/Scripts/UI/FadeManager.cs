using UnityEngine;
using System;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [SerializeField] private CanvasGroup group;
    [SerializeField] private float fadeSpeed = 2f;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeOut()
    {
        while (group.alpha < 1f)
        {
            group.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }
        
        group.alpha = 1f;
    }

    public IEnumerator FadeIn()
    {
        while (group.alpha > 0f)
        {
            group.alpha -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }
        
        group.alpha = 0f;
    }

    public IEnumerator FadeRoutine(Action midAction)
    {
        yield return FadeOut();
        
        midAction?.Invoke();
        yield return new WaitForSecondsRealtime(0.1f);
        
        yield return FadeIn();       
    }
}