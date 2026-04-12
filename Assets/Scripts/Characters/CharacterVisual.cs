using UnityEngine;

public class CharacterVisual : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform modelRoot;

    private GameObject _currentModel;
    private CharacterModelView _currentView;
    
    public Animator animator;

    public void Initialize(CharacterSelectionData data)
    {
        SetClass(data.@class);
        SetColor(data.@class, data.colorIndex);
    }

    private void SetClass(CharacterClassData classData)
    {
        if (_currentModel)
            Destroy(_currentModel);

        _currentView = Instantiate(
            classData.modelPrefab,
            modelRoot
        );

        _currentView.transform.localPosition = Vector3.zero;
        _currentView.transform.localRotation = Quaternion.identity;

        _currentModel = _currentView.gameObject;
        animator = _currentView.Animator;
    }

    private void SetColor(CharacterClassData data, int index)
    {
        if (!_currentModel)
            return;
        
        var renderers = _currentView.GetRenderers();
        var material = data.colorVariants[index];
        
        foreach (var render in renderers)
            render.material = material;
    }
}