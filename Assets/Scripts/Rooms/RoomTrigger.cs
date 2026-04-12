using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private const string PlayerTag = "Player";
    
    [SerializeField] private Room room;

    private bool _triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered)
            return;

        if (!other.CompareTag(PlayerTag))
            return;

        _triggered = true;
        
        room.ActivateRoom();
    }
}