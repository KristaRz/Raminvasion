// Created on base of youtube tutorial by Valem //
// Instantiates a prefab as the player avatar when we join a room and destroys it again when we leave a room. //


using Photon.Pun;
using UnityEngine;

public class SpawnNetworkPlayer : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _MenuCamera;

    [SerializeField] private GameObject _Player1Prefab;
    [SerializeField] private GameObject _Player2Prefab;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // When we join a room, we spawn an Avatar for ourselves(the NetworkPlayer). This will be visible to others then since we instantiate it in the network.


        if (PhotonNetwork.CurrentRoom.Players.Count == 1)
        {
            //_Player1Prefab = PhotonNetwork.Instantiate("NetPlayer1", transform.position, transform.rotation);
            Instantiate(_Player1Prefab, transform.position, transform.rotation);
            LogEventMessage.Instance.LogText("Joined as Player 1.");
            GameHandler.Instance.SetPlayer(PlayerTag.Player1);
        }
        else
        {
            //_Player2Prefab = PhotonNetwork.Instantiate("NetPlayer2", transform.position, transform.rotation);
            Instantiate(_Player2Prefab, transform.position, transform.rotation);
            LogEventMessage.Instance.LogText("Joined as Player 2.");
            GameHandler.Instance.SetPlayer(PlayerTag.Player2);
        }

        _MenuCamera.SetActive(false);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        // When we leave the room again, we also destroy our avatar on the network
        //PhotonNetwork.Destroy(_spawnedPlayerPrefab);
        _MenuCamera.SetActive(true);
    }
}
