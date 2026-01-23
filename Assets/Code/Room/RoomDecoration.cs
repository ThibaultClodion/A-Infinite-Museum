using System.Collections.Generic;
using UnityEngine;

public class RoomDecoration : MonoBehaviour
{
    [Header("Room Fondations")]
    [SerializeField] private List<Renderer> wallsRenderer;
    [SerializeField] private Renderer floorRenderer;
    [SerializeField] private Renderer roofRenderer;

    [Header("Paintings")]
    [SerializeField] private List<Painting> paintings;

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
