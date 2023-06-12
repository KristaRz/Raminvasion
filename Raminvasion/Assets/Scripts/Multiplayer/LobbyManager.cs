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
    [SerializeField] private GameObject _SelectionCanvas;
    [SerializeField] private GameObject _CreateCanvas;
    [SerializeField] private GameObject _JoinCanvas;
    [SerializeField] private GameObject _RoomLobbyCanvas;

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
        NetworkManager.Instance.SetRoomName(roomName);

        // We make a new set of room options for the room we want to open. You can assign these with a 
        RoomOptions roomOptions = new()
        {
            MaxPlayers = 2,    // hover over names to see explanations
            IsVisible = true,
            IsOpen = true
        };
        // When we connect to the server we create or join "Room 1".
        PhotonNetwork.CreateRoom(_roomName, roomOptions);

        _SelectionCanvas.SetActive(false);
        _CreateCanvas.SetActive(false);

        _RoomLobbyCanvas.SetActive(true);
        _RoomLobbyCanvas.GetComponent<RoomPlayerSelection>().SetRoom(roomName);
    }

    public void JoinExistingRoom(string roomName)
    {
        Debug.Log(roomName);
        _roomName = roomName;
        NetworkManager.Instance.SetRoomName(roomName);
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        // Join the room with the name _roomName.
        PhotonNetwork.JoinRoom(_roomName);

        _SelectionCanvas.SetActive(false);
        _JoinCanvas.SetActive(false);

        _RoomLobbyCanvas.SetActive(true);
        _RoomLobbyCanvas.GetComponent<RoomPlayerSelection>().SetRoom(roomName);
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

    public void StartGame(PlayerTag playerTag)
    {
        GameHandler.Instance.SetPlayer(playerTag);
        if (playerTag == PlayerTag.Player1)
        {
            LogEventMessage.Instance.LogText("Joined as Player 1.");
        }
        else
        {
            LogEventMessage.Instance.LogText("Joined as Player 2.");
        }
        _RoomLobbyCanvas.SetActive(false);
        _MenuCanvas.SetActive(false);
        GameHandler.Instance.StartGame();
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


    ///////////////////////////////////
    //////////////DEBUG////////////////
    ///////////////////////////////////

    public void StartDebugGame()
    {
        RoomOptions roomOptions = new()
        {
            MaxPlayers = 2,    // hover over names to see explanations
            IsVisible = true,
            IsOpen = true
        };
        // When we connect to the server we create or join "Room 1".
        PhotonNetwork.CreateRoom("DebugRoom", roomOptions);

        _MenuCanvas.SetActive(false);
        GameHandler.Instance.SetPlayer(PlayerTag.Player1);
        GameHandler.Instance.StartGame();
    }
}
