
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpeedPickup : MonoBehaviour
{
    //[SerializeField] private PlayerTag _ForThisPlayer;
    [SerializeField] private float _SpeedAmount = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectablesHandler.Instance.ChangeSpeed(_SpeedAmount);
            Destroy(gameObject);
        }
    }
}
