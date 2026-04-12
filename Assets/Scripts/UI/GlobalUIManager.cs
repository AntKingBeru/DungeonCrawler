using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager Instance { get; private set; }
    
    [SerializeField] private InputActionReference settingsAction;
    [SerializeField] private SettingsMenuUI settingsMenu;

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
    
    private void OnEnable()
    {
        settingsAction.action.Enable();
        settingsAction.action.performed += OnToggleSettings;
    }
    
    private void OnDisable()
    {
        settingsAction.action.performed -= OnToggleSettings;
        settingsAction.action.Disable();
    }

    private void OnToggleSettings(InputAction.CallbackContext ctx)
    {
        ToggleSettings();
    }

    public void ToggleSettings()
    {
        if (settingsMenu.gameObject.activeSelf)
            settingsMenu.Close();
        else
            settingsMenu.Open();
    }
}