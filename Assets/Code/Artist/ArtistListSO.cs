using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtistListSO", menuName = "Scriptable Objects/ArtistListSO")]
public class ArtistListSO : ScriptableObject
{
    [SerializeField] private ArtistSO[] _artists;
    [SerializeField] private List<int> _unusedIndexes;

    public void InitializeIndexes()
    {
        // Initialize the indexes
        _unusedIndexes = new List<int>();
        for (int i = 0; i < _artists.Length; i++)
        {
            _unusedIndexes.Add(i);
        }

        // Shuffle the indexes
        int length = _unusedIndexes.Count;
        for (int i = 0; i < length - 1; i++)
        {
            int randomIndex = Random.Range(i, length);
            int temp = _unusedIndexes[i];
            _unusedIndexes[i] = _unusedIndexes[randomIndex];
            _unusedIndexes[randomIndex] = temp;
        }
    }

    public ArtistSO GetRandomArtist()
    {
        if(_unusedIndexes.Count == 0)
        {
            InitializeIndexes();
        }

        int index = _unusedIndexes[_unusedIndexes.Count - 1];
        _unusedIndexes.RemoveAt(_unusedIndexes.Count - 1);

        return _artists[index];
    }
}
