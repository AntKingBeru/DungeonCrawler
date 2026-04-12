using UnityEngine;



[CreateAssetMenu(menuName = "Game/Party Data")]
public class PartyData : ScriptableObject
{
    public PartyMemberData player;
    public PartyMemberData[] members = new PartyMemberData[3];

    public FormationType formation;
}