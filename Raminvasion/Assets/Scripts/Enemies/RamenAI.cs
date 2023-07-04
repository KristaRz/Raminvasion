// Created 15.05.2023 by Krista Plagemann //
// Makes the enemy ramen follow the player using navmesh //

using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class RamenAI : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerTag _RamenEnemy;

    [SerializeField] private Transform _Player;
    [SerializeField] private float _StartingDelay;
    [SerializeField, Tooltip("Rate at which the ramen gets faster.")] private float SpeedupRate = 1;
    [SerializeField] private float RamenFollowSpeed = 8;

    [SerializeField] private float _PlayerUpdateRate = 2f;
    private float _speedCollected = 0;

    public bool _active = false;

    private NavMeshAgent _agent;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_StartingDelay);

        if (GameHandler.Instance.PlayerSet)
            InitializeRamen(GameHandler.Instance.currentPlayer);      
        else
            GameHandler.Instance.OnPlayerChange += InitializeRamen;

        _agent.SetDestination(_Player.position);
        _active = true;
    }

    private void InitializeRamen(PlayerTag player)
    {
        _RamenEnemy = player;
        _agent = GetComponent<NavMeshAgent>();
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        SpeedupRate = SpeedupRate / 2;
        _agent.speed = RamenFollowSpeed;
        GameHandler.Instance.UpdateSpeed(_RamenEnemy, RamenFollowSpeed);

        if (_RamenEnemy == PlayerTag.Player1)
            GameHandler.Instance.OnPlayer1Speed += ChangeSpeed;
        else if (_RamenEnemy == PlayerTag.Player2)
            GameHandler.Instance.OnPlayer2Speed += ChangeSpeed;
    }

    private void ChangeSpeed(float amount)
    {
        _agent.speed = amount;
    }

    private void LateUpdate()
    {
        if (_active)
        {
            float speedUP = SpeedupRate * Time.deltaTime;
            //_agent.speed += speedUP;
            _agent.SetDestination(_Player.position);
            _speedCollected += speedUP;
            if(_speedCollected >= _PlayerUpdateRate)
            {
                CollectablesHandler.Instance.UpdateRamenSpeed(_RamenEnemy, (int)_speedCollected);
                GameHandler.Instance.UpdateSpeed(_RamenEnemy, (int)_speedCollected);
                _speedCollected = 0;
            }
        }
    }

}
