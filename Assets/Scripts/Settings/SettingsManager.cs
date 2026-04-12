using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    
    private const string InitializedKey = "SettingsInitialized";
    private const string SensitivityKey = "MouseSensitivity";
    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SFXVolume";
    private const string ResolutionIndexKey = "ResolutionIndex";
    private const string FullscreenKey = "Fullscreen";
    
    [Header("Input")]
    public float MouseSensitivity { get; private set; } = 1f;

    [Header("Audio")]
    public float MasterVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float SfxVolume { get; private set; }
    [SerializeField] private AudioMixer mixer;
    
    [Header("Resolution")]
    public Resolution[] Resolutions { get; private set; }
    public int CurrentResolutionIndex { get; private set; }
    public bool Fullscreen { get; private set; }
    
    public UnityEvent onSettingsUpdated;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Resolutions = Screen.resolutions;
        Load();
    }
    
    public void SetMouseSensitivity(float value)
    {
        MouseSensitivity = value;
        PlayerPrefs.SetFloat(SensitivityKey, value);
    }
    
    private float LinearToDb(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
        mixer.SetFloat(MasterVolumeKey, LinearToDb(value));
        Save();
    }
    
    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        mixer.SetFloat(MusicVolumeKey, LinearToDb(value));
        Save();
    }

    public void SetSfxVolume(float value)
    {
        SfxVolume = value;
        mixer.SetFloat(SfxVolumeKey, LinearToDb(value));
        Save();
    }

    public void SetResolution(int index)
    {
        CurrentResolutionIndex = Mathf.Clamp(index, 0, Resolutions.Length - 1);
        
        ApplyResolution();
        
        PlayerPrefs.SetInt(ResolutionIndexKey, CurrentResolutionIndex);
    }

    public void SetFullscreen(bool value)
    {
        Fullscreen = value;
        
        ApplyResolution();
        
        PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);
    }

    private void Save()
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
        PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
        
        onSettingsUpdated?.Invoke();
    }

    private void Load()
    {
        var initialized = PlayerPrefs.GetInt(InitializedKey, 0) == 1;

        if (!initialized)
        {
            MouseSensitivity = 1f;
            
            MasterVolume = 1f;
            MusicVolume = 1f;
            SfxVolume = 1f;
            
            CurrentResolutionIndex = Resolutions.Length - 1;
            Fullscreen = true;
            
            Save();
            
            PlayerPrefs.SetInt(InitializedKey, 1);
        }
        else
        {
            MouseSensitivity = PlayerPrefs.GetFloat(SensitivityKey, 1f);
            
            MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
            
            var savedIndex = PlayerPrefs.GetInt(ResolutionIndexKey, Resolutions.Length - 1);
            CurrentResolutionIndex = Mathf.Clamp(savedIndex, 0, Resolutions.Length - 1);
            
            Fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
        }

        ApplyAllSettings();
    }

    private void ApplyVolumes()
    {
        mixer.SetFloat(MasterVolumeKey, LinearToDb(MasterVolume));
        mixer.SetFloat(MusicVolumeKey, LinearToDb(MusicVolume));
        mixer.SetFloat(SfxVolumeKey, LinearToDb(SfxVolume));
    }
    
    private void ApplyResolution()
    {
        var res = Resolutions[CurrentResolutionIndex];
        Screen.SetResolution(res.width, res.height, Fullscreen);
    }
    
    private void ApplyAllSettings()
    {
        ApplyVolumes();
        ApplyResolution();
    }
}