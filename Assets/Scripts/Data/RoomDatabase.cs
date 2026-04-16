using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Room Database")]
public class RoomDatabase : ScriptableObject
{
    public Room startRoom;
    public Room normalRoom;
    public Room bossRoom;
    public Room rewardRoom;
    public Room victoryRoom;
    
    public Room GetRoom(RoomType type) => type switch
    {
        RoomType.Start => startRoom,
        RoomType.Normal => normalRoom,
        RoomType.Boss => bossRoom,
        RoomType.Reward => rewardRoom,
        RoomType.Victory => victoryRoom,
        _ => normalRoom
    };
}