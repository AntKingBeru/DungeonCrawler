using UnityEngine;
using UnityEngine.Events;

public class CharacterCreationController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CharacterClassData[] classes;
    
    [Header("Preview")]
    [SerializeField] private CharacterPreview preview;
    
    [Header("Events")]
    public UnityEvent<CharacterClassData> onClassChanged;
    public UnityEvent<int> onColorChanged;
    public UnityEvent<string> onNameChanged;
    
    [Header("Button")]
    [SerializeField] private Transform classButtonParent;
    [SerializeField] private ClassButtonUI classButtonPrefab;
    
    private CharacterClassData _currentClass;
    private int _currentColorIndex;
    private string _currentName;
    private ClassButtonUI[] _buttons;

    private void Start()
    {
        GenerateButtons();
        
        LoadFromSessionOrDefault();
    }
    
    private void GenerateButtons()
    {
        _buttons = new ClassButtonUI[classes.Length];

        for (var i = 0; i < classes.Length; i++)
        {
            var btn = Instantiate(classButtonPrefab, classButtonParent);

            btn.Initialize(i, classes[i].icon, OnClassSelected);

            _buttons[i] = btn;
        }
    }
    
    private void OnClassSelected(int index)
    {
        SelectClass(index);

        for (var i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].SetSelected(i == index);
        }
    }

    public void SelectClass(int index)
    {
        _currentClass = classes[index];
        preview.SetClass(_currentClass);
        onClassChanged?.Invoke(_currentClass);
    }
    
    public void SelectColor(int index)
    {
        _currentColorIndex = index;
        preview.SetColor(_currentColorIndex);
        onColorChanged?.Invoke(_currentColorIndex);
    }

    public void SetName(string newName)
    {
        _currentName = newName;
        onNameChanged?.Invoke(_currentName);
    }

    public void Confirm()
    {
        var data = new CharacterSelectionData
        {
            @class = _currentClass,
            colorIndex = _currentColorIndex,
            name = _currentName
        };
        
        GameSession.Instance.SetPlayer(data);

        SceneLoader.Instance.LoadScene(SceneId.CharacterCreationParty);
    }

    private void LoadFromSessionOrDefault()
    {
        var player = GameSession.Instance.Player;

        if (player.@class)
        {
            var classIndex = 0;

            for (var i = 0; i < classes.Length; i++)
            {
                if (classes[i] == player.@class)
                {
                    classIndex = i;
                    break;
                }
            }
            
            SelectClass(classIndex);
            SelectColor(player.colorIndex);

            SetName(player.name);
        }
        else
        {
            SelectClass(0);
            SelectColor(0);
        }
    }
}