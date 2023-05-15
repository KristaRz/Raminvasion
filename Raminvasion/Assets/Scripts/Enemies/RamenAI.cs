using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RamenAI : MonoBehaviour
{
    [SerializeField] private Transform _Player;
    [SerializeField] private float _StartingDelay;
    [SerializeField] public float RamenFollowSpeed = 10;

    public bool _active = false;

    private NavMeshAgent _agent;

    private IEnumerator Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        yield return new WaitForSeconds(_StartingDelay);
        _agent.speed = RamenFollowSpeed;
        _agent.SetDestination(_Player.position);
        _active = true;
    }

    private void Update()
    {
        if (_active)
        {
            _agent.SetDestination(_Player.position);
        }
    }
}
