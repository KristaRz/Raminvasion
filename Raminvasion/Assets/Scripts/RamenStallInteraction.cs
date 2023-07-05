using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamenStallInteraction : MonoBehaviour
{   
    [SerializeField] public List<GameObject> foods=new();

    [SerializeField] private ParticleSystem PositiveParticle;
    [SerializeField] private ParticleSystem NegativeParticle;


    public void AddFoodToStall(GameObject item) {
        
        item.transform.position=transform.position;
        foods.Add(item);
        
    }

    private void OnTriggerEnter(Collider other) {
       
        if(other.CompareTag("Player")){
            ParticleSystem newParticleEffect;
            if(foods.Count!=0){
                newParticleEffect = Instantiate(PositiveParticle, transform.position+new Vector3(0,2,0), Quaternion.identity);
                
                StartCoroutine(ActivateFoods(other));
                
                
            }
            else{
                newParticleEffect = Instantiate(NegativeParticle, transform.position+new Vector3(0,2,0), Quaternion.identity);
                
            } 
            newParticleEffect.Play(); 
            
        }
    }

    IEnumerator ActivateFoods(Collider other){
        foreach (var item in foods)
                {
                // Debug.Log($"Food: {item.name}, Pos: {item.transform.position}, RamenStalPos:{transform.position}");
                
                item.GetComponent<SpeedPickup>().player=other.gameObject;
                item.GetComponent<SpeedPickup>().triggerd=true;

                yield return new WaitForSeconds(0.2f);
                }
                foods=new();
    }
}
