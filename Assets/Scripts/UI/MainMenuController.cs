using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void OnNewGame()
    {
        SceneLoader.Instance.LoadScene(SceneId.CharacterCreationPlayer);
    }

    public void OnContinue()
    {
        // TODO: Implement Continue
    }

    public void OnSettings()
    {
        GlobalUIManager.Instance.ToggleSettings();
    }

    public void OnQuit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}