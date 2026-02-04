using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Artists Parameters")]
    [SerializeField] private ArtistListSO _artistList;

    [Header("Room Parameters")]
    [SerializeField] private Room[] _rooms;
    private int _actualRoomIndex = 0;

    private void Awake()
    {
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
        ArtistSO randomArtist = _artistList.GetRandomArtist();

        _rooms[index].AdaptRoomToNewArtist(randomArtist);
    }
}
