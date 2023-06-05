using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDistribution : MonoBehaviour
{

    public GameObject tileGround;

    public GameObject foodPrefab;

    public float foodRadius=3.5f;
    public int foodAmount =5;

    private void Start() {
        for (int i = 0; i < foodAmount; i++)
        {
            Vector3 randomPosition = tileGround.transform.position + new Vector3(Random.Range(-foodRadius, foodRadius), 2f, Random.Range(-foodRadius, foodRadius));
            Instantiate(foodPrefab, randomPosition, Quaternion.identity,this.gameObject.transform);
        }
    }
    
}
