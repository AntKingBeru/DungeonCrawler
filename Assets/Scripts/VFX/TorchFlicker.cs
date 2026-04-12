using UnityEngine;

public class TorchFlicker : MonoBehaviour
{
    [SerializeField] private Light torchlight;
    
    [Header("Flicker Settings")]
    [SerializeField] private float minIntensity = 2.5f;
    [SerializeField] private float maxIntensity = 3.5f;
    [SerializeField] private float flickerSpeed = 8f;

    private float _noiseOffset;

    private void Start()
    {
        _noiseOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        var noise = Mathf.PerlinNoise(Time.time * flickerSpeed, _noiseOffset);
        torchlight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}