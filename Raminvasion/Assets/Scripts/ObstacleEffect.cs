using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;


public class ObstacleEffect : MonoBehaviour
{
    
    //[SerializeField] private PlayerTag _ForThisPlayer;
    [SerializeField] private float _DecreaseSpeedAmount = 3;
    [SerializeField] private float _EffectTime = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {   ThirdPersonController playerController =other.GetComponent<ThirdPersonController>();
            StartCoroutine(ChangeMoveSpeed(playerController));
        }

    }

    private IEnumerator ChangeMoveSpeed(ThirdPersonController playerController)
    {
        if (gameObject.name == "BoxObstacle")
        {
            //Doesnt work :(
            playerController.MoveSpeed -= _DecreaseSpeedAmount;
        }
        else if (gameObject.name == "SauceObstacle")
        {
            playerController.MoveSpeed = 0;
        }

        // Debug.Log(playerController.MoveSpeed);

        yield return new WaitForSeconds(_EffectTime);

        playerController.MoveSpeed = 10;
        // Debug.Log(playerController.MoveSpeed);
    }

    

}

