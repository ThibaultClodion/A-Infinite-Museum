using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Room Parameters")]
    [SerializeField] private Room[] _rooms;
    private int _actualRoomIndex = 0;

    [Header("Artist Parameters")]
    [SerializeField] private ArtistListSO _artistList;
    [SerializeField] private Artist _artist;

    private void Awake()
    {
        _artistList.InitializeIndexes();
        
        for (int i = 0; i < _rooms.Length; i++)
        {
            _rooms[i].OnEnterSurveyRoom.AddListener(EnterNextRoom);
        }

        GenerateARoom(0);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _rooms.Length; i++)
        {
            _rooms[i].OnEnterSurveyRoom.RemoveListener(EnterNextRoom);
        }
    }

    public void EnterNextRoom()
    {
        _actualRoomIndex = (_actualRoomIndex + 1) % _rooms.Length;
        GenerateARoom(_actualRoomIndex);
    }

    private void GenerateARoom(int index)
    {
        ArtistSO randomArtist = _artistList.GetNextArtist();

        _rooms[index].AdaptRoomToNewArtist(randomArtist);
        _artist.InitializeArtist(randomArtist, _rooms[index].GetRandomArtistPosition());
    }
}
