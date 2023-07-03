
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
    public event Action<GameObject, GameObject> OnPlayerDefined = delegate { };

    private GameObject _playerObject;
    private GameObject _ramenObject;

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

    public void PlayerSpawned()
    {
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _ramenObject = GameObject.FindGameObjectWithTag("Ramen");
        OnPlayerDefined(_playerObject, _ramenObject);
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
            if(Player1Speed < 0f) Player1Speed = 0f;
            OnPlayer1Speed(Player1Speed);
        }
        else
        {
            Player2Speed += speed;
            if (Player2Speed < 0f) Player2Speed = 0f;
            OnPlayer2Speed(Player2Speed);
        }
    }

    #endregion

}
