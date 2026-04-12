using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private CharacterController controller;
    [SerializeField] private CharacterVisual visual;
    
    [Header("Animation")]
    private Animator _animator;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    
    [SerializeField] private SkillController skillController;
    
    private Transform _cameraTransform;

    private void OnEnable()
    {
        moveAction.action.Enable();
    }
    
    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    public void Initialize(CharacterSelectionData data, Transform camTransform)
    {
        _cameraTransform = camTransform;
        visual.Initialize(data);
        _animator = visual.animator;
        skillController.Initialize(data, _animator);
    }
    
    private void Update()
    {
        Move();
        UpdateAnimation();
    }

    private void Move()
    {
        var input = moveAction.action.ReadValue<Vector2>();

        if (input.sqrMagnitude < 0.01f)
            return;
        
        var camForward = _cameraTransform.forward;
        var camRight = _cameraTransform.right;
        
        camForward.y = 0;
        camRight.y = 0;
        
        var moveDir = camForward.normalized * input.y + camRight.normalized * input.x;
        
        controller.Move(moveDir * moveSpeed * Time.deltaTime);
        
        var targetRotation = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void UpdateAnimation()
    {
        if (!_animator)
            return;
        
        var input = moveAction.action.ReadValue<Vector2>();
        _animator.SetFloat(SpeedHash, input.magnitude);
    }
}