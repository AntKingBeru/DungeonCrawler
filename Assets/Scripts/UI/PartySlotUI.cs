using UnityEngine;

public class PartySlotUI : MonoBehaviour
{
    [Header("Preview")]
    [SerializeField] private CharacterPreview preview;
    
    [Header("Class Buttons")]
    [SerializeField] private Transform classButtonParent;
    
    [Header("Color Buttons")]
    [SerializeField] private ColorButtonUI[] colorButtons;
    
    public CharacterPreview Preview => preview;
    public Transform ClassButtonParent => classButtonParent;
    public ColorButtonUI[] ColorButtons => colorButtons;
}