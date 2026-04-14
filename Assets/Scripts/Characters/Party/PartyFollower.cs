using UnityEngine;

[RequireComponent( typeof(CharacterController))]
public class PartyFollower : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private CharacterVisual visual;
    
    [Header("Movement")]
    [SerializeField] private float followSpeed = 4f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundForce = -2f;
    
    [Header("Animation")]
    private Animator _animator;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private Transform _leader;
    private Vector3 _offset;
    private Vector3 _lastPosition;
    private float _verticalVelocity;

    public void Initialize(CharacterSelectionData data, Transform leader, int index)
    {
        _leader = leader;
        visual.Initialize(data);
        _animator = visual.animator;
        
        var formation = GameSession.Instance.Formation;
        
        if (formation && formation.offsets.Length > index)
            _offset = formation.offsets[index];
        else
            _offset = Vector3.back * (index + 1);
    }

    private void Update()
    {
        Follow();
        UpdateAnimation();
    }

    private void Follow()
    {
        if (!_leader)
            return;
        
        var desiredPosition =
            _leader.position +
            _leader.forward * _offset.z +
            _leader.right * _offset.x + 
            Vector3.up * _offset.y;
        
        var direction = desiredPosition - transform.position;
        var distance = direction.magnitude;
        
        if (controller.isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = groundForce;
        
        _verticalVelocity += gravity * Time.deltaTime;

        if (distance < 0.1f)
        {
            controller.Move(Vector3.up * _verticalVelocity * Time.deltaTime);
            return;
        }
        
        var moveDir = direction.normalized;
        var speedMultiplier = Mathf.Clamp01(distance);
        var moveSpeedScaled = followSpeed * speedMultiplier;
        
        var velocity = moveDir * moveSpeedScaled;
        velocity.y = _verticalVelocity;
        
        controller.Move(velocity * Time.deltaTime);
        
        if (moveDir.sqrMagnitude > 0.01f)
        {
            var targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    private void UpdateAnimation()
    {
        if (!_animator)
            return;
        
        var delta = transform.position - _lastPosition;
        var speed = delta.magnitude / Time.deltaTime;

        _lastPosition = transform.position;

        if (speed < 0.05f)
            speed = 0f;

        _animator.SetFloat(SpeedHash, speed, 0.1f, Time.deltaTime);
    }
}