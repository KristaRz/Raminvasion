using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    public static LobbyManager Instance { get; private set; }

    private void Awake() => Instance = this;

    private string _roomName = "DefaultRoom";

    [SerializeField] private GameObject _RoomItemPrefab;
    [SerializeField] private GameObject _ContentPlaceholder;
    [SerializeField] private GameObject _MenuCanvas;

    private List<RoomListItem> _currentRooms = new List<RoomListItem>();

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

    }
    public void SetNextRoom(string roomName)
    {
        _roomName = roomName;
    }

    public void SetNewRoomAndJoin(string roomName)
    {
        _roomName = roomName;

        // We make a new set of room options for the room we want to open. You can assign these with a 
        RoomOptions roomOptions = new()
        {
            MaxPlayers = 2,    // hover over names to see explanations
            IsVisible = true,
            IsOpen = true
        };
        // When we connect to the server we create or join "Room 1".
        PhotonNetwork.CreateRoom(_roomName, roomOptions);

        _MenuCanvas.SetActive(false);
    }

    public void JoinExistingRoom(string roomName)
    {
        Debug.Log(roomName);
        _roomName = roomName;
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        // Join the room with the name _roomName.
        PhotonNetwork.JoinRoom(_roomName);

        _MenuCanvas.SetActive(false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Create room failed bcs: " + message);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.LogWarning("Join room failed bcs: " + message);
    }



    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room.");
        if (PhotonNetwork.CurrentRoom.Players.Count == 1)
        {
            LogEventMessage.Instance.LogText("Joined as Player 1.");
            GameHandler.Instance.SetPlayer(PlayerTag.Player1);
        }
        else
        {
            LogEventMessage.Instance.LogText("Joined as Player 2.");
            GameHandler.Instance.SetPlayer(PlayerTag.Player2);
        }
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Someone joined a room.");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList) { Debug.Log(room.Name); }
        UpdateRoomList(roomList);
    }

    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (var room in _currentRooms)
            Destroy(room.gameObject);
        _currentRooms.Clear();

        foreach (var room in roomList)
        {
            RoomListItem newRoom = Instantiate(_RoomItemPrefab, _ContentPlaceholder.transform).GetComponent<RoomListItem>();
            newRoom.SetRoomName(room.Name);
            _currentRooms.Add(newRoom);
        }
    }
}
