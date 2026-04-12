using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClassButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject selectionBorder;
    [SerializeField] private Button button;
    [SerializeField] private Image background;
    [SerializeField] private Color normalColor = new Color(0.16f,0.16f,0.16f);
    [SerializeField] private Color hoverColor = new Color(0.3f,0.3f,0.3f);

    private int _index;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        background.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.color = normalColor;
    }

    public void Initialize(int index, Sprite sprite, UnityAction<int> callback)
    {
        _index = index;
        icon.sprite = sprite;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback(_index));
    }

    public void SetSelected(bool selected)
    {
        selectionBorder.SetActive(selected);
    }
}