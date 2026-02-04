using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private ArtistSO _artistData;

    [Header("Room Fondations")]
    [SerializeField] private List<Renderer> _wallsRenderer;
    [SerializeField] private Renderer _floorRenderer;
    [SerializeField] private Renderer _roofRenderer;

    [Header("Paintings")]
    [SerializeField] private List<Painting> _paintings;

    [Header("Lectern")]
    [SerializeField] private Lectern _lectern;

    public void AdaptRoomToNewArtist(ArtistSO artistData)
    {
        _artistData = artistData;

        // Change walls color
        Color wallColor = ArrayToColor(_artistData.ArtistModel.WallsColor);
        foreach (Renderer renderer in _wallsRenderer)
        {
            renderer.material.color = wallColor;
        }

        _floorRenderer.material.color = ArrayToColor(_artistData.ArtistModel.FloorColor);
        _roofRenderer.material.color = ArrayToColor(_artistData.ArtistModel.RoofColor);

        UpdatePaintings(_artistData.Paintings);
        UpdateLectern(_artistData.ArtistModel.Description, _artistData.ArtistModel.Name);
    }

    private void UpdatePaintings(List<Sprite> sprites)
    {
        Color frameColor = ArrayToColor(_artistData.ArtistModel.FrameColor);
        Color informationFrameColor = ArrayToColor(_artistData.ArtistModel.FrameInformationColor);

        for(int i = 0; i < sprites.Count && i < _paintings.Count; i++)
        {
            _paintings[i].ChangeSpriteAndFrame(sprites[i], frameColor);
            _paintings[i].ChangeInformation(informationFrameColor, _artistData.ArtistModel.PaintingPrompts[i], _artistData.ArtistModel.Name);
        }
    }

    private void UpdateLectern(string artistDescription, string artistName)
    {
        _lectern.DefineArtistDescription(artistDescription, artistName);
    }

    private Color ArrayToColor(int[] colorArray)
    {
        // Normalize RGB values from 0-255 to 0-1 range for Unity Color
        return new Color(colorArray[0] / 255f, colorArray[1] / 255f, colorArray[2] / 255f);
    }
}
