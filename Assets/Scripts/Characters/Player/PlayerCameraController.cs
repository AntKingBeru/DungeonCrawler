using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private float distance = 5f;
    [SerializeField] private float minY = -30f;
    [SerializeField] private float maxY = 60f;
    [SerializeField] private Vector3 offset;
    
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private LayerMask collisionMask;

    private Transform _target;
    private float _yaw;
    private float _pitch;

    private void OnEnable()
    {
        lookAction.action.Enable();
    }
    
    private void OnDisable()
    {
        lookAction.action.Disable();
    }

    public void Initialize(Transform target)
    {
        _target = target;
    }

    private void LateUpdate()
    {
        if (!_target)
            return;
        
        var input = lookAction.action.ReadValue<Vector2>();
        
        var sensitivity = SettingsManager.Instance.MouseSensitivity;
        
        _yaw += input.x * sensitivity;
        _pitch -= input.y * sensitivity;
        _pitch = Mathf.Clamp(_pitch, minY, maxY);
        
        var rotation = Quaternion.Euler(_pitch, _yaw, 0);
        
        var targetPosition = _target.position + offset;
        var desiredPosition = targetPosition - rotation * Vector3.forward * distance;

        if (Physics.SphereCast(
                targetPosition,
                collisionRadius,
                desiredPosition - targetPosition,
                out var hit,
                distance,
                collisionMask
            ))
        {
            hit.distance = Mathf.Max(hit.distance, 1.5f);
            desiredPosition = targetPosition + (desiredPosition - targetPosition).normalized * hit.distance;
        }
        
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10f);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);
    }
}