// Created 02.07.2023 by Krista Plagemann //
// Handles the UI according to player like spawning the right prefab and calculating the color values from received speed.//


using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedDisplay : MonoBehaviour
{
    [SerializeField] private Color[] _DangerColors;
    [SerializeField] private GameObject _Player1Prefab;
    [SerializeField] private GameObject _Player2Prefab;

    private PlayerUI _playerUIComponent;
    private Image _EnemyStateImage;
    private Image _OtherPlayerStateImage;
    private TextMeshProUGUI _DistanceText;
    private TextMeshProUGUI _DistanceOtherPlayer;

    private PlayerTag _thisPlayer;
    private Transform _player;
    private Transform _ramen;
    private float _thisPlayerSpeed;
    private float _otherPlayerSpeed;

    private bool _updateValues = false;


    // Updates image colors for both player according to speed and distance of the enemy ramens
    private async Task UpdateValues()
    {
        while (_updateValues)
        {
            float distanceBtwPlayerAndRamen = Vector3.Distance(_player.position, _ramen.position);
            // under 0 is bad, over 5 is good?
            //Debug.Log($"Distance: { distanceBtwPlayerAndRamen}, speed: {_player1speed}, Division: {distanceBtwPlayerAndRamen/_player1speed}");

            // THIS PLAYER //

            float relativeColor = distanceBtwPlayerAndRamen / _thisPlayerSpeed / (_DangerColors.Length) * (_DangerColors.Length-1);
            if(relativeColor >= _DangerColors.Length-1 )
                relativeColor = _DangerColors.Length-1;
            else if (relativeColor <= 0)
                relativeColor = 0;
            Color oldColor = _DangerColors[Mathf.FloorToInt(relativeColor)];
            Color newColor = _DangerColors[Mathf.CeilToInt(relativeColor)];
            float newT = relativeColor - Mathf.FloorToInt(relativeColor);

            _EnemyStateImage.color = Color.Lerp(oldColor, newColor, newT);
            _DistanceText.SetText(Mathf.FloorToInt(distanceBtwPlayerAndRamen).ToString());


            // OTHER PLAYER //

            float otherRelativeColor =  _otherPlayerSpeed/ 5 / (_DangerColors.Length) * (_DangerColors.Length - 1);
            if (otherRelativeColor >= _DangerColors.Length - 1)
                otherRelativeColor = _DangerColors.Length - 1;
            else if (otherRelativeColor <= 0)
                otherRelativeColor = 0;
            Color oldColor2 = _DangerColors[Mathf.FloorToInt(otherRelativeColor)];
            Color newColor2 = _DangerColors[Mathf.CeilToInt(otherRelativeColor)];
            float newT2 = otherRelativeColor - Mathf.FloorToInt(otherRelativeColor);

            _OtherPlayerStateImage.color = Color.Lerp(oldColor2, newColor2, newT2);
            //_DistanceOtherPlayer.SetText(Mathf.FloorToInt(distanceBtwPlayerAndRamen).ToString());
            
            await Task.Yield();
        }    
    }

    private void Start()
    {
        GameHandler.Instance.OnPlayerChange += SpawnPlayerUI;
        GameHandler.Instance.OnPlayerDefined += SetPlayer;        
        GameHandler.Instance.OnPlayer1Speed += Update1Speed;
        GameHandler.Instance.OnPlayer2Speed += Update2Speed;
    }

    // Spawns UI elements according to players and get some references as well as storing which player we are
    private void SpawnPlayerUI(PlayerTag player)
    {
        _thisPlayer = player;

        GameObject playerUI;
        if (player == PlayerTag.Player1)
            playerUI = Instantiate(_Player1Prefab, transform);
        else
            playerUI = Instantiate(_Player2Prefab, transform);

        _playerUIComponent = playerUI.GetComponent<PlayerUI>();

        _EnemyStateImage = _playerUIComponent.EnemyStateImage;
        _OtherPlayerStateImage = _playerUIComponent.OtherPlayerStateImage;
        _DistanceText = _playerUIComponent.DistanceText;
        _DistanceOtherPlayer = _playerUIComponent.DistanceOtherPlayer;

        GameHandler.Instance.OnPlayerChange -= SpawnPlayerUI;
    }

    // Sets the gameObjects for player and ramen to calculate distance
    private async void SetPlayer(GameObject player, GameObject ramen)
    {
        _player = player.transform;
        _ramen = ramen.transform;
        _updateValues = true;

        GameHandler.Instance.OnPlayerDefined -= SetPlayer;

        await UpdateValues();
    }

    // Updates player 1 speed on received values from gamehandler
    private void Update1Speed(float speed)
    {
        if (_thisPlayer == PlayerTag.Player1)
            _thisPlayerSpeed = speed;
        else
            _otherPlayerSpeed = speed;    
    }

    // Updates player 2 speed on received values from gamehandler
    private void Update2Speed(float speed)
    {
        if (_thisPlayer == PlayerTag.Player2)
            _thisPlayerSpeed = speed;
        else
            _otherPlayerSpeed = speed;
        _DistanceOtherPlayer.text = speed.ToString();
    }

    private void OnDisable()
    {
        GameHandler.Instance.OnPlayer1Speed -= Update1Speed;
        GameHandler.Instance.OnPlayer2Speed -= Update2Speed;

        _updateValues = false;
    }
}
