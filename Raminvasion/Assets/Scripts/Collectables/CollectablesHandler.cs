// Created by Krista Plagemann //
// Reacts to collected things and sends values over the network.
// Onion > speed for self
// Carrot > speed for other

// Also send distance values over network bcs there is a photonView basically (not very clean code sry)


using Photon.Pun;
using System;

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
    private PlayerTag otherPlayer;

    // private Dictionary<int, CollectableType> collectedThingsInTime=new();
    public event Action<CollectableType> OnCollected = delegate { };


    private void Start()
    {
        GameHandler.Instance.OnPlayerChange += SetPlayer;
        // this.OnCollected+=CountCollectables;
    }

    // Saves which player this and the other is. Also subscribes to this player's distance event.
    private void SetPlayer(PlayerTag player)
    {
        currentPlayer = player;

        if (player == PlayerTag.Player1)
        {
            GameHandler.Instance.OnPlayer1Distance += UpdateDistanceForOtherPlayer;
            otherPlayer = PlayerTag.Player2;
        }
        else
        {
            GameHandler.Instance.OnPlayer2Distance += UpdateDistanceForOtherPlayer;
            otherPlayer = PlayerTag.Player1;
        }
    }

    /// <summary>
    /// Records a collectable as collected and saves the values associated with it.
    /// </summary>
    /// <param name="speed">How much speed does this change (use minus values to slow down)</param>
    /// <param name="typeCollected">Which type of collectable is this? (onion for this player, carrot for other player)</param>
    public void ChangeSpeed(float speed, CollectableType typeCollected)
    {
        OnCollected?.Invoke(typeCollected);
        if (typeCollected == CollectableType.Onion)
        {
            SendSpeedValues(currentPlayer, speed);
            SpreadSpeeed(currentPlayer, speed);
        }
        else if(typeCollected == CollectableType.Carrot)
        {
            SendSpeedValues(otherPlayer, speed);
            SpreadSpeeed(otherPlayer, speed);
        }
    }

    //Julia didnt finish
    private void CountCollectables(CollectableType item){
        
        // collectedThingsInTime.Add(Time.time, item);

        //should remove all saved items with timestamp< currentTime-intervall
        // float intervall=15f;
        // foreach (var item in collectedThingsInTime){
        //     if(item.Key<=Time.time)
        // }
        
        // Debug.Log(collectedThingsInTime.Count);
    }

    // Records the speed change in the GameHandler
    private void SpreadSpeeed(PlayerTag player, float speed)
    {
        GameHandler.Instance.UpdateSpeed(player, speed);
    }

    // Sends the own collected speed to other player
    public void SendSpeedValues(PlayerTag player, float speed)
    {
        photonView.RPC("ReceiveSpeed", RpcTarget.Others, player, speed);
    }

    // Receives the speed change from other player
    [PunRPC]
    private void ReceiveSpeed(PlayerTag player, float speed)
    {
        SpreadSpeeed(player, speed);
    }

    #region UpdateFullRamenSpeed

    public void UpdateRamenSpeed(PlayerTag playerTag, int speedChange)
    {
        photonView.RPC("RamenSpeedTransfer", RpcTarget.Others, playerTag, (float)speedChange);
    }

    [PunRPC]
    private void RamenSpeedTransfer(PlayerTag player, float speed)
    {
        GameHandler.Instance.UpdateSpeed(player, speed);
    }

    #endregion

    #region DistanceUpdate

    // Updates the distance of this player recorded in the other player.//

    public void UpdateDistanceForOtherPlayer(float distance)
    {
        photonView.RPC("UpdateDistanceOfOtherPlayer", RpcTarget.Others, currentPlayer, distance);
    }

    // Updates the distance of other player recorded in this player.//
    [PunRPC]
    private void UpdateDistanceOfOtherPlayer(PlayerTag player, float distance)
    {
        GameHandler.Instance.UpdateDistance(player, distance);
    }

    #endregion
}
