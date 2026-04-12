using UnityEngine;

public class VFX_ShakeTrigger : MonoBehaviour
{
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private float magnitude = 0.2f;

    private void Start()
    {
        CameraShake.Instance.Shake(duration, magnitude);
    }
}