using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    
    [SerializeField] private Transform camTransform;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        var originalPos = camTransform.localPosition;
        var timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            camTransform.localPosition = originalPos + Random.insideUnitSphere * magnitude;
            yield return null;
        }
        
        camTransform.localPosition = originalPos;
    }
}