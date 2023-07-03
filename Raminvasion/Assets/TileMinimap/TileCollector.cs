using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TileCollector : MonoBehaviour
{
    [SerializeField] private LayerMask LayersToHit;
    [SerializeField] private string TagOfColliders;

    private bool _updateMap = true;


    private async void Start()
    {
        await CollectTiles();
    }
    private async Task CollectTiles()
    {
        while (_updateMap)
        {
            RaycastHit rayHit;
            Ray ray = new(transform.position, transform.forward);
            if (Physics.Raycast(ray, out rayHit, 60f, LayersToHit, QueryTriggerInteraction.Ignore))
            {
                Collider collider = rayHit.collider;
                if (collider.CompareTag(TagOfColliders))
                {
                    if(StoreTile(collider.gameObject.transform.position.x, collider.gameObject.transform.position.z))  // only disable the collider if we can store it
                        collider.enabled = false;
                }
            }
            await Task.Yield();
        }
    }

    private bool StoreTile(float xCoordinate, float zCoordinate)
    {
        bool successful = MinimapGenerator.Instance.SetMazeTile(xCoordinate, zCoordinate);
        return successful;
    }

    private void OnDisable()
    {
        _updateMap = false;
    }
}
