using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DescriptionConversionTable", menuName = "Scriptable Objects/DescriptionConversionTable")]
public class DescriptionConversionTable : ScriptableObject
{
    public List<DescriptionMeshPair> descriptionToMesh;
}

[System.Serializable]
public class DescriptionMeshPair
{
    public string description;
    public GameObject meshPrefab;
}