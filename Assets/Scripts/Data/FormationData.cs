using UnityEngine;

[CreateAssetMenu(menuName = "Game/Formation")]
public class FormationData : ScriptableObject
{
    public FormationType type;
    public Sprite icon;
    
    [TextArea] public string description;

    public Vector3[] offsets;
}
