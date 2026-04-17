using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class VictoryScreenUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject root;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform content;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float scaleDuration = 0.4f;
    [SerializeField] private float moveDistance = 40f;
    
    private Vector3 _startPosition;

    private void Awake()
    {
        root.SetActive(false);
        
        canvasGroup.alpha = 0f;
        
        _startPosition = content.anchoredPosition;
        content.anchoredPosition = _startPosition - Vector3.up * moveDistance;
        content.localScale = Vector3.one * 0.9f;
        
        mainMenuButton.onClick.AddListener(ReturnToMenu);

        GameSession.Instance.onGameWon.AddListener(Show);
    }

    private void Show()
    {
        root.SetActive(true);

        var playerName = GameSession.Instance.Player.name;
        messageText.text = $"Victory is yours, {playerName}!\n" +
                           $"The dungeon lies silent.\n\n" +
                           $"For now...";
        
        Time.timeScale = 0f;
        InputSystem.DisableAllEnabledActions();
        
        StopAllCoroutines();
        StartCoroutine(AnimateIn());
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

            content.anchoredPosition = Vector3.Lerp(
                _startPosition - Vector3.up * moveDistance,
                _startPosition,
                EaseOutCubic(t)
            );

            content.localScale = Vector3.Lerp(
                Vector3.one * 0.9f,
                Vector3.one,
                EaseOutBack(t)
            );
            
            yield return null;
        }
    }

    private float EaseOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }

    private float EaseOutBack(float x)
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