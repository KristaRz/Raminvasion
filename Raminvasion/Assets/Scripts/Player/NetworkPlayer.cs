// Created on base of youtube tutorial by Valem, extended by Krista Plagemann //
// Syncs the position of head and hands of our XRPlayer with the visuals for the network //

using Photon.Pun;
using UnityEngine;


public class NetworkPlayer : MonoBehaviour
{
    [SerializeField] private Transform _Player;           // Our head seen by others in the network
    private Transform _LocalPlayer;

    private PhotonView _photonView;     // Photon view component which handles syncing this obejct in the network

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();

        // If this Player is my own, then I hide the visuals for myself
        if (_photonView.IsMine)
        {
            foreach (var renderer in GetComponentsInChildren<Renderer>())
                renderer.enabled = false;
            _LocalPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
    }

    private void Update()
    {
        if (_photonView.IsMine)
        {
            _Player.position = _LocalPlayer.transform.position;
            _Player.rotation = _LocalPlayer.transform.rotation;
        }
    }
}
