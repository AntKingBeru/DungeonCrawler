using UnityEngine;

public class CharacterModelView : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Animator animator;
    
    public Animator Animator => animator;
    
    public Renderer[] GetRenderers() => renderers;

    public void ApplyMaterial(Material material)
    {
        foreach (var render in renderers)
            render.material = material;
    }
}