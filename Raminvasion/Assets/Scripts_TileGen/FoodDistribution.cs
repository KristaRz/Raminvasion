using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDistribution : MonoBehaviour
{

    public GameObject tileGround;

    public GameObject foodPrefab;

    public float foodRadius=0.5f;


    public void PlaceFood() {
        
            //not Random Position but should be one of fixed SpawnPoints
            Vector3 randomPosition = tileGround.transform.position + new Vector3(Random.Range(-foodRadius, foodRadius), 2f, Random.Range(-foodRadius, foodRadius));
            Instantiate(foodPrefab, randomPosition, Quaternion.identity,this.gameObject.transform);
    

        
        

    }

}
