using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;


public class ObstacleEffect : MonoBehaviour
{
    
    //[SerializeField] private PlayerTag _ForThisPlayer;
    [SerializeField] private float _DecreaseSpeedAmount = 3;
    [SerializeField] private float _EffectTime = 3;

    [SerializeField] private ParticleSystem particleEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {   ThirdPersonController playerController =other.GetComponent<ThirdPersonController>();

            Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);          
                

            ParticleSystem newParticleEffect = Instantiate(particleEffect, hitPoint, Quaternion.identity);
            newParticleEffect.Play();
            
            StartCoroutine(ChangeMoveSpeed(playerController));
        }

    }

    private IEnumerator ChangeMoveSpeed(ThirdPersonController playerController)
    {   
        if (gameObject.CompareTag("BoxObstacle"))
        {
            playerController.MoveSpeed = playerController.MoveSpeed-_DecreaseSpeedAmount;
        }
        else if (gameObject.CompareTag("SauceObstacle"))
        {   
            playerController.MoveSpeed = 0;
        }

        yield return new WaitForSeconds(_EffectTime);

        playerController.MoveSpeed = 10;
    }

    

}

