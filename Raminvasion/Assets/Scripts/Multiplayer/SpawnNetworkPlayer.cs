// Created on base of youtube tutorial by Valem, completely modified by Krista Plagemann lol //
// Instantiates a prefab of the player when we join a room. //


using Photon.Pun;
using UnityEngine;

public class SpawnNetworkPlayer : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _MenuCamera;

    [SerializeField] private GameObject _Player1Prefab;
    [SerializeField] private GameObject _Player2Prefab;

    private void Start()
    {
        GameHandler.Instance.OnStartGame.AddListener(OnGameStarted);
    }

    public void OnGameStarted()
    {
        // When we join a room, we spawn our player for ourselves according to which player we are

        if (GameHandler.Instance.currentPlayer == PlayerTag.Player1)
        {
            //_Player1Prefab = PhotonNetwork.Instantiate("NetPlayer1", transform.position, transform.rotation);
            Instantiate(_Player1Prefab, transform.position, transform.rotation);
        }
        else
        {
            //_Player2Prefab = PhotonNetwork.Instantiate("NetPlayer2", transform.position, transform.rotation);
            Instantiate(_Player2Prefab, transform.position, transform.rotation);
        }
        GameHandler.Instance.PlayerSpawned();

        _MenuCamera.SetActive(false);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        // When we leave the room again, we also destroy our avatar on the network
        //PhotonNetwork.Destroy(_spawnedPlayerPrefab);
        //_MenuCamera.SetActive(true);
    }
}
