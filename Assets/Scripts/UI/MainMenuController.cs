using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void OnNewGame()
    {
        if (SaveSystem.HasFreeSlot())
        {
            var slot = SaveSystem.GetFirstEmptySlot();
            
            GameSession.Instance.StartNewRun(slot);
            
            SceneLoader.Instance.LoadScene(SceneId.CharacterCreationPlayer);
        }
        else
        {
            LoadSaveUI.ReplaceMode = true;
            
            SceneLoader.Instance.LoadScene(SceneId.LoadSave);
        }
    }

    public void OnContinue()
    {
        SceneLoader.Instance.LoadScene(SceneId.LoadSave);
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