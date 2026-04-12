using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    [SerializeField] private RectTransform root;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statsText;
    
    [SerializeField] private Vector2 offset;
    
    [Header("Input")]
    [SerializeField] private InputActionReference pointAction;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Hide();
    }
    
    private void OnEnable()
    {
        pointAction.action.Enable();
    }

    private void OnDisable()
    {
        pointAction.action.Disable();
    }

    private void Update()
    {
        var screenPos = pointAction.action.ReadValue<Vector2>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.parent as RectTransform,
            screenPos,
            null,
            out var localPos
        );

        root.anchoredPosition = localPos + offset;
    }

    public void Show(SkillData data)
    {
        nameText.text = data.skillName;
        descriptionText.text = data.description;

        statsText.text =
            $"Cooldown: {data.cooldown}s\n" +
            $"Mana: {data.manaCost}";
        
        root.gameObject.SetActive(true);
    }

    public void Hide()
    {
        root.gameObject.SetActive(false);
    }
}