using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Fondations")]
    [SerializeField] private List<Renderer> wallsRenderer;
    [SerializeField] private Renderer floorRenderer;
    [SerializeField] private Renderer roofRenderer;

    [Header("Paintings")]
    [SerializeField] private List<Painting> paintings;

    [Header("Lectern")]
    [SerializeField] private Lectern lectern;

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
