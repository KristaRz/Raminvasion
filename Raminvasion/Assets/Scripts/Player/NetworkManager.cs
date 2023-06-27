// Created on base of youtube tutorial by Valem //
// Initializes the connection to the server and gives status updates via Debug.Log. Also creates a room "Room 1" once connected or joins said room if exciting already. //


using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }

    private void Awake() => Instance = this;

    private string _roomName = "DefaultRoom";
    [SerializeField] private TextMeshProUGUI _DebugText;

    public void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        ConnectToServer();
    }

    // Connect to the Photon server on start using the settings you set in the inspector when setting up the server.
    private void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        _DebugText.SetText("Trying to connect...");
        Debug.Log("Trying to connect to server...");
    }

    // We are getting the following functions from the MonoBehaviourPunCallbacks that we derive this class form
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        _DebugText.SetText("Connected.");
        Debug.Log("Connected to server.");
        base.OnConnectedToMaster();
    }

    public void SetRoomName(string roomName)
    {
        _roomName = roomName;
    }

}
