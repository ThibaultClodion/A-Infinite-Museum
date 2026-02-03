using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Fondations")]
    [SerializeField] private List<Renderer> _wallsRenderer;
    [SerializeField] private Renderer _floorRenderer;
    [SerializeField] private Renderer _roofRenderer;

    [Header("Paintings")]
    [SerializeField] private List<Painting> _paintings;

    [Header("Lectern")]
    [SerializeField] private Lectern _lectern;

    void Start()
    {
        /*foreach (var wallRenderer in wallsRenderer)
        {
            wallRenderer.material.color = Color.blue;
        }
        floorRenderer.material.color = Color.green;
        roofRenderer.material.color = Color.gray;*/
    }
}
