// Created 15.05.2023 by Krista Plagemann //
// Makes the enemy ramen follow the player using navmesh //

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class RamenAI : MonoBehaviour
{
    [SerializeField] private PlayerTag _RamenEnemy;

    [SerializeField] private Transform _Player;
    [SerializeField] private float _StartingDelay;
    [SerializeField, Tooltip("Rate at which the ramen gets faster.")] private float SpeedupRate = 1;

    [SerializeField] private float RamenFollowSpeed = 8;


    public bool _active = false;

    private NavMeshAgent _agent;

    private void Start()
    {
        GameHandler.Instance.OnStartGame.AddListener(StartGame);


        if (!GameHandler.Instance.PlayerSet)
            GameHandler.Instance.OnPlayerChange += InitializeRamen;
        else
            InitializeRamen(_RamenEnemy);  
    }

    private void StartGame()
    {
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
            _agent.speed += SpeedupRate * Time.deltaTime;
            _agent.SetDestination(_Player.position);
        }
    }
}
