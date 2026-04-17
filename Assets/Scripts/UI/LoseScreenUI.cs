using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoseScreenUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject root;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform content;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float scaleDuration = 0.3f;

    private void Awake()
    {
        root.SetActive(false);
        canvasGroup.alpha = 0f;
        content.localScale = Vector3.one * 0.8f;
        
        mainMenuButton.onClick.AddListener(ReturnToMenu);
        
        GameSession.Instance.onGameLost.AddListener(Show);
    }

    private void Show(int bossesKilled, int totalBosses)
    {
        root.SetActive(true);
        
        progressText.text = $"Bosses Defeated: {bossesKilled} / {totalBosses}";
        
        StopAllCoroutines();
        StartCoroutine(AnimateIn());
        
        Cursor.visible = true;
    }

    private IEnumerator AnimateIn()
    {
        var t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeDuration;
            
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            
            yield return null;
        }

        t = 0f;
        
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / scaleDuration;
            
            var scale = Mathf.Lerp(0.8f, 1f, EaseOuyBack(t));
            content.localScale = Vector3.one * scale;
            
            yield return null;
        }
    }

    private float EaseOuyBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        
        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1f, 2);
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadScene(SceneId.MainMenu);
    }
}