using UnityEngine;

public class DungeonVisualizer : MonoBehaviour
{
    [SerializeField] private DungeonGenerator generator;

    private void Start()
    {
        generator.Generate();
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!generator || generator.Rooms == null)
            return;

        foreach (var room in generator.Rooms.Values)
        {
            Gizmos.color = GetColor(room.Type);

            var pos = generator.GetWorldPosition(room.Position);

            Gizmos.DrawCube(pos, Vector3.one);
        }
    }
    
    private Color GetColor(RoomType type)
    {
        return type switch
        {
            RoomType.Start => Color.green,
            RoomType.Normal => Color.white,
            RoomType.Boss => Color.red,
            RoomType.Reward => Color.yellow,
            _ => Color.gray
        };
    }
    #endif
}