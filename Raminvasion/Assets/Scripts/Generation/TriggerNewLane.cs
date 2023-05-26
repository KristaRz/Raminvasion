
using UnityEngine;

public class TriggerNewLane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TileGenerator.Instance.GenerateNewLane();
        }
    }

}
