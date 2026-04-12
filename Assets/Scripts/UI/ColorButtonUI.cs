using UnityEngine;
using UnityEngine.UI;

public class ColorButtonUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;

    private int _index;
    private int _slot;
    private PartyCreationController _controller;

    public void Initialize(int index, int slot, PartyCreationController controller, Color color)
    {
        _index = index;
        _slot = slot;
        _controller = controller;
        
        image.color = color;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }
    
    private void OnClicked()
    {
        _controller.OnColorSelected(_slot, _index);
    }
    
    public void SetInteractable(bool value)
    {
        button.interactable = value;
    }
    
    public void SetSelected(bool selected)
    {
        image.transform.localScale = selected ? Vector3.one * 1.2f : Vector3.one;
    }
}