using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    
    [SerializeField] private TMP_Text sensitivityText;

    private bool _isOpen;

    private void Start()
    {
        sensitivitySlider.value = SettingsManager.Instance.MouseSensitivity;
        UpdateSensitivityText(SettingsManager.Instance.MouseSensitivity);
        sensitivitySlider.onValueChanged.AddListener(value =>
        {
            SettingsManager.Instance.SetMouseSensitivity(value);
            UpdateSensitivityText(value);
        });
        
        masterSlider.value = SettingsManager.Instance.MasterVolume;
        musicSlider.value = SettingsManager.Instance.MusicVolume;
        sfxSlider.value = SettingsManager.Instance.SfxVolume;
        
        masterSlider.onValueChanged.AddListener(SettingsManager.Instance.SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SettingsManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SettingsManager.Instance.SetSfxVolume);
        
        resolutionDropdown.ClearOptions();
        
        var options = SettingsManager.Instance.Resolutions.Select(r => $"{r.width}x{r.height}").ToList();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = SettingsManager.Instance.CurrentResolutionIndex;
        
        fullscreenToggle.isOn = SettingsManager.Instance.Fullscreen;
        
        resolutionDropdown.onValueChanged.AddListener(SettingsManager.Instance.SetResolution);
        fullscreenToggle.onValueChanged.AddListener(SettingsManager.Instance.SetFullscreen);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        _isOpen = true;

        GameStateManager.Instance.SetPaused(_isOpen);
        
        CursorManager.Instance.UpdateCursorState(_isOpen);
    }
    
    public void Close()
    {
        gameObject.SetActive(false);
        _isOpen = false;
        
        GameStateManager.Instance.SetPaused(_isOpen);
        
        CursorManager.Instance.UpdateCursorState(_isOpen);
    }
    
    private void UpdateSensitivityText(float value)
    {
        sensitivityText.text = value.ToString("0.00");
    }
}