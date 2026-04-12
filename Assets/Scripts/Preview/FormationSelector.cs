using UnityEngine;

public class FormationSelector : MonoBehaviour
{
    [SerializeField] private PartyData partyData;

    public void SelectFormation(int index)
    {
        partyData.formation = (FormationType)index;
    }
}