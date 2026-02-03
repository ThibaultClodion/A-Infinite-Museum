using TMPro;
using UnityEngine;

public class Lectern : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _artistDescriptionText; 
    [SerializeField] private TextMeshProUGUI _artistNameText;
    
    public void DefineArtistDescription(string artistDescription, string artistName)
    {
        _artistDescriptionText.text = artistDescription;
        _artistNameText.text = artistName;
    }
}
