using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterPreview : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform modelRoot;
    
    [Header("Input")]
    [SerializeField] private InputActionReference rotateAction;
    
    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 100f;
    
    [Header("Start Values")]
    [SerializeField] private CharacterClassData startClass;
    
    [SerializeField] private PreviewDragHandler dragHandler;

    private CharacterClassData _currentClass;
    private int _currentColorIndex;
    
    private CharacterModelView _currentView;
    
    private void OnEnable()
    {
        rotateAction.action.Enable();
    }
    
    private void OnDisable()
    {
        rotateAction.action.Disable();
    }
    
    private void Start()
    {
        SetClass(startClass);
        SetColor(0);
    }

    private void Update()
    {
        if (!dragHandler.IsDragging) return;

        var input = rotateAction.action.ReadValue<float>();

        if (Mathf.Abs(input) > 0.01f)
        {
            modelRoot.Rotate(Vector3.up, -input * rotationSpeed * Time.deltaTime);
        }
    }

    public void SetClass(CharacterClassData data)
    {
        _currentClass = data;

        SpawnModel();
        ApplyMaterial();
    }

    public void SetColor(int index)
    {
        _currentColorIndex = index;
        ApplyMaterial();
    }

    private void SpawnModel()
    {
        for (var i = modelRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(modelRoot.GetChild(i).gameObject);
        }

        _currentView = Instantiate(_currentClass.modelPrefab, modelRoot);
    }

    private void ApplyMaterial()
    {
        if (!_currentClass || !_currentView)
            return;

        var mat = _currentClass.colorVariants[_currentColorIndex];
        _currentView.ApplyMaterial(mat);
    }
}