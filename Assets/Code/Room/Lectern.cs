using TMPro;
using UnityEngine;

public class Lectern : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI artistDescriptionText; 
    [SerializeField] private TextMeshProUGUI artistNameText;
    
    public void DefineArtistDescription(string artistDescription, string artistName)
    {
        artistDescriptionText.text = artistDescription;
        artistNameText.text = artistName;
    }
}
