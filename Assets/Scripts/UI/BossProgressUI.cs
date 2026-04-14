using UnityEngine;
using TMPro;

public class BossProgressUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private void Start()
    {
        GameSession.Instance.onBossProgressUpdated.AddListener(UpdateUI);
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (GameSession.Instance)
            GameSession.Instance.onBossProgressUpdated.RemoveListener(UpdateUI);
    }

    private void UpdateUI()
    {
        text.text = $"Bosses: {GameSession.Instance.BossesDefeated} / {GameSession.Instance.TotalBosses}";
    }
}