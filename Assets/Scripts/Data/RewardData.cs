using UnityEngine;

[CreateAssetMenu(menuName = "Game/Reward")]
public class RewardData : ScriptableObject
{
    public RewardType type;

    public int skillIndex;

    public float healthBonus;
    public float manaBonus;
    public float damageBonus;
}