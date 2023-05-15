using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueLanes : MonoBehaviour
{
    public int arrayExtension=30;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OneLane oneLane = FindObjectOfType<OneLane>();
            oneLane.arrayDepth = oneLane.arrayDepth + arrayExtension;
            Debug.Log("ExtendTheArray");
        }
    }
}
