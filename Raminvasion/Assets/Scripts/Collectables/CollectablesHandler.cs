
using Photon.Pun;
using System;
using UnityEngine;

public enum CollectableType { Onion, Carrot }

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
    public event Action<CollectableType> OnCollected = delegate { };


    private void Start()
    {
        GameHandler.Instance.OnPlayerChange += SetPlayer;
    }

    private void SetPlayer(PlayerTag player)
    {
        Debug.Log("PlayerSet");
        currentPlayer = player;
    }

    public void ChangeSpeed(float speed, CollectableType typeCollected)
    {
        OnCollected?.Invoke(typeCollected);
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
        photonView.RPC("RamenSpeedTransfer", RpcTarget.Others, playerTag, (float)speedChange);
    }

    [PunRPC]
    private void RamenSpeedTransfer(PlayerTag player, float speed)
    {
        GameHandler.Instance.UpdateSpeed(player, speed);
    }

}
