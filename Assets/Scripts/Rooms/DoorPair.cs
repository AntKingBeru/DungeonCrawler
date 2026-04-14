using UnityEngine;
using System.Collections;

public class DoorPair : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject doorframe;
    [SerializeField] private Transform door;
    [SerializeField] private GameObject wall;
    
    [Header("Settings")]
    [SerializeField] private float openAngle = -90f;
    [SerializeField] private float speed = 5f;
    
    public GameObject Doorframe => doorframe;
    public GameObject Wall => wall;

    private bool _isOpen;
    private Coroutine _animRoutine;

    public void SetConnection(bool connection)
    {
        wall.SetActive(!connection);
        
        doorframe.SetActive(connection);
    }

    public void Open()
    {
        if (_isOpen)
            return;

        _isOpen = true;
        StartAnimation(openAngle);
    }
    
    public void Close()
    {
        if (!_isOpen) return;

        _isOpen = false;
        StartAnimation(0f);
    }
    
    private void StartAnimation(float targetAngle)
    {
        if (_animRoutine != null)
            StopCoroutine(_animRoutine);

        _animRoutine = StartCoroutine(RotateDoor(targetAngle));
    }

    private IEnumerator RotateDoor(float target)
    {
        var current = door.localEulerAngles.y;

        if (current > 180f)
            current -= 360f;

        while (Mathf.Abs(current - target) > 0.1f)
        {
            current = Mathf.Lerp(current, target, Time.deltaTime * speed);
            door.localRotation = Quaternion.Euler(0f, current, 0f);
            yield return null;
        }
        
        door.localRotation = Quaternion.Euler(0f, target, 0f);
    }
}