using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TilePlacement : MonoBehaviour
{   
    public GameObject block;
    
    private void OnMouseDown() {
        Instantiate(block, new Vector3(gameObject.transform.position.x,5,gameObject.transform.position.z), Quaternion.identity);
        
    }
    
}
