using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerTag { Player1, Player2 }

[DefaultExecutionOrder(-10)]
public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);

        if (FindObjectOfType<NetworkManager>() == null)
            SetPlayer(PlayerTag.Player1);
    }

    #region Player 

    public bool PlayerSet { get; private set; } = false ;
    public PlayerTag currentPlayer { get; private set; }
    public event Action<PlayerTag> OnPlayerChange = delegate { };

    public UnityEvent OnStartGame;

    public void SetPlayer(PlayerTag player)
    {
        currentPlayer = player;
        OnPlayerChange(currentPlayer);
        PlayerSet = true;
    }

    public void StartGame()
    {
        OnStartGame?.Invoke();
    }

    #endregion

    #region Speed

    public float Player1Speed { get; private set; } = 0f;
    public float Player2Speed { get; private set; } = 0f;

    public event Action<float> OnPlayer1Speed = delegate { };
    public event Action<float> OnPlayer2Speed = delegate { };

    public void UpdateSpeed(PlayerTag player, float speed)
    {
        if(player == PlayerTag.Player1)
        {
            Player1Speed += speed;
            OnPlayer1Speed(Player1Speed);
        }
        else
        {
            Player2Speed += speed;
            OnPlayer2Speed(Player2Speed);
        }
    }

    #endregion

}
