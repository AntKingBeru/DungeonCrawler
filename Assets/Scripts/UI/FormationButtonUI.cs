using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FormationButtonUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedFrame;
    
    private int _index;
    private UnityAction<int> _callBack;

    public void Initialize(int i, FormationData data, UnityAction<int> onClicked)
    {
        _index = i;
        _callBack = onClicked;
        
        icon.sprite = data.icon;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        _callBack?.Invoke(_index);
    }
    
    public void SetSelected(bool selected)
    {
        if (selectedFrame)
            selectedFrame.SetActive(selected);
    }
}