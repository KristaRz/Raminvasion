using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
      
        if (other.CompareTag("Player"))
        {
            CollectFood();
        }
    }

    private void CollectFood()
    {
        
        Debug.Log("Food collected!");
        Destroy(gameObject);
    }
}
