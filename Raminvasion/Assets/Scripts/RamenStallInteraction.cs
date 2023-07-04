using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamenStallInteraction : MonoBehaviour
{
    [SerializeField] public List<GameObject> foods=new();

    public void AddFoodToStall(GameObject item) {
        
        item.transform.position=gameObject.transform.position;
        foods.Add(item);
        item.SetActive(false);
        
        
    }

    private void OnTriggerEnter(Collider other) {
        // if(other.CompareTag("Player")){
        //     //idk if we introduce coins to exchange for better food or not
            
        //         foreach (var item in foods)
        //         {
        //         item.SetActive(true);
        //         foods.Remove(item);
                
        //         }
            
            
        // }
    }
}
