
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public enum PlayerTag { Player1, Player2 }

public enum DifficultyMode{
    Easy,
    Medium,
    Hard
}

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
        StartCoroutine(CheckLevelDifficulty(_playerObject,_ramenObject));

    }

    #endregion

    #region Speed

    public float Player1Speed { get; private set; } = 0f;
    public float Player2Speed { get; private set; } = 0f;

    public event Action<float> OnPlayer1Speed = delegate { };
    public event Action<float> OnPlayer2Speed = delegate { };

    public void UpdateSpeed(PlayerTag player, float speed)
    {
        //Debug.Log($"Speed update for: {player} by {speed}");
        if (player == PlayerTag.Player1)
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

    public float Player1Distance { get; private set; } = 0f;
    public float Player2Distance { get; private set; } = 0f;

    public event Action<float> OnPlayer1Distance = delegate { };
    public event Action<float> OnPlayer2Distance = delegate { };

    public void UpdateDistance(PlayerTag player, float distance)
    {
        
        if (player == PlayerTag.Player1)
        {
            Player1Distance = distance;
            if (Player1Distance < 0f) Player1Distance = 0f;
            OnPlayer1Distance(Player1Distance);
        }
        else
        {
            Player2Distance = distance;
            if (Player2Distance < 0f) Player2Distance = 0f;
            OnPlayer2Distance(Player2Distance);
        }
    }

    #endregion



    #region Difficulty

    public DifficultyMode difficultyMode;
    public event Action<DifficultyMode> OnDifficultySwitched = delegate { };

    [SerializeField] private float difficultyCheckTime=5f;

    [SerializeField] private const float difficultyStep=10f;

    private float CheckDistance(GameObject playerObj, GameObject ramenObj){
        float currentDistance = Vector3.Distance(playerObj.transform.position, ramenObj.transform.position);

        return currentDistance;
    }

    IEnumerator CheckLevelDifficulty(GameObject playerObj, GameObject ramenObj){
        while(true){
            
            float distance = CheckDistance(playerObj,ramenObj);
            
            DifficultyMode difficulty = GetDifficulty(distance);

            if(difficulty!=difficultyMode){
                difficultyMode=difficulty;

                //if an event for difficulty change is needed
                OnDifficultySwitched?.Invoke(difficultyMode);
            }
            yield return new WaitForSeconds(difficultyCheckTime); 
        }
        
    }
    
    //lets start simple here
    public DifficultyMode GetDifficulty(float distancePlayerRamen){
            switch(distancePlayerRamen){
                case >=0 and < difficultyStep :
                    return DifficultyMode.Easy;
                case >=difficultyStep and <difficultyStep*2: 
                    return DifficultyMode.Medium;
                case >=difficultyStep*2 :
                    return DifficultyMode.Hard;
                default:
                    return DifficultyMode.Medium;
            }
    }

    #endregion


    

}
