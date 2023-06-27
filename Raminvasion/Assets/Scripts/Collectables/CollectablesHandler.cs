
using Photon.Pun;
using System;
using UnityEngine;

public class CollectablesHandler : MonoBehaviourPunCallbacks
{
    #region Singleton

    public static CollectablesHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    #endregion

    private PlayerTag currentPlayer;

    private void Start()
    {
        GameHandler.Instance.OnPlayerChange += SetPlayer;
    }

    private void SetPlayer(PlayerTag player)
    {
        Debug.Log("PlayerSet");
        currentPlayer = player;
    }

    public void ChangeSpeed(float speed)
    {
        SendSpeedValues(currentPlayer, speed);
        SpreadSpeeed(currentPlayer, speed);
    }

    private void SpreadSpeeed(PlayerTag player, float speed)
    {
        GameHandler.Instance.UpdateSpeed(player, speed);
    }

    public void SendSpeedValues(PlayerTag player, float speed)
    {
        photonView.RPC("ReceiveSpeed", RpcTarget.Others, player, speed);
    }

    [PunRPC]
    private void ReceiveSpeed(PlayerTag player, float speed)
    {
        SpreadSpeeed(player, speed);
    }


    public void UpdateRamenSpeed(PlayerTag playerTag, int speedChange)
    {
        photonView.RPC("RamenSpeedTransfer", RpcTarget.Others, playerTag, speedChange);
    }

    [PunRPC]
    private void RamenSpeedTransfer(PlayerTag player, float speed)
    {
        GameHandler.Instance.UpdateSpeed(player, speed);
    }

}
