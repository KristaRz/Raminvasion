// Created by Krista Plagemann //
// Handles selecting a room in the lobby and joining as a certain player //


using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomPlayerSelection : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI _RoomName;
    [SerializeField] private GameObject _ReadyButton;

    private int _playerSelected = 0;
    private bool _ready = false;
    private int _playersReady = 0;

    public void SetRoom(string roomName)
    {
        _RoomName.SetText(roomName);
    }

    public void SelectPlayer(int playerIndex)
    {
        if(_playerSelected != playerIndex)
            _playerSelected = playerIndex;
        else
            _playerSelected = 0;
    }

    public void SetToggleReady()
    {
        _ready = !_ready;

        if (_ready)
        {
            photonView.RPC("SetReady", RpcTarget.Others, +1);
            _playersReady += 1;
        }
        else
        {
            photonView.RPC("SetReady", RpcTarget.Others, -1);
            _playersReady -= 1;
        }

        if (_playersReady == PhotonNetwork.CurrentRoom.Players.Count)
        {
            if(_playerSelected == 1)
                LobbyManager.Instance.StartGame(PlayerTag.Player1);
            else if(_playerSelected == 2)
                LobbyManager.Instance.StartGame(PlayerTag.Player2);

        }
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.CurrentRoom.Players.Count == 2)
            _ReadyButton.SetActive(true);
        base.OnJoinedRoom();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.Players.Count == 2)
            _ReadyButton.SetActive(true);
        base.OnPlayerEnteredRoom(newPlayer);
    }

    [PunRPC]
    private void SetReady(int readyAmount)
    {
        _playersReady += readyAmount;
        Debug.Log(_playerSelected);
        if (_playersReady == PhotonNetwork.CurrentRoom.Players.Count)
        {
            if (_playerSelected == 1)
                LobbyManager.Instance.StartGame(PlayerTag.Player1);
            else if (_playerSelected == 2)
                LobbyManager.Instance.StartGame(PlayerTag.Player2);
        }
    }
}
