// Created 15.05.2023 by Krista Plagemann //
// Makes the enemy ramen follow the player using navmesh //

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public enum PlayerTag { Player1, Player2 }

public class RamenAI : MonoBehaviour
{
    [SerializeField] private PlayerTag _RamenEnemy;

    [SerializeField] private Transform _Player;
    [SerializeField] private float _StartingDelay;
    [SerializeField, Tooltip("Rate at which the ramen gets faster.")] private float SpeedupRate = 1;

    [SerializeField] private float RamenFollowSpeed = 8;


    public bool _active = false;

    private NavMeshAgent _agent;

    private IEnumerator Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        SpeedupRate = SpeedupRate /2;
        _agent.speed = RamenFollowSpeed;

        if (_RamenEnemy == PlayerTag.Player1)
            CollectablesHandler.Instance.OnPlayer1SpeedChange += ChangeSpeed;
        else if (_RamenEnemy == PlayerTag.Player2)
            CollectablesHandler.Instance.OnPlayer2SpeedChange += ChangeSpeed;

        yield return new WaitForSeconds(_StartingDelay);        
        _agent.SetDestination(_Player.position);

        _active = true;
    }

    private void ChangeSpeed(float amount)
    {
        _agent.speed += amount;
    }

    private void LateUpdate()
    {
        if (_active)
        {
            _agent.speed += SpeedupRate * Time.deltaTime;
            Debug.Log(_agent.speed);
            _agent.SetDestination(_Player.position);
        }
    }
}
