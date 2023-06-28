using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDistribution : MonoBehaviour
{

    [SerializeField] private GameObject tileGround;

    [SerializeField] private GameObject foodPrefab;

    [SerializeField] private float TileWidth;

    [SerializeField] private List<Vector3> possiblePositions;


    [Header("Values for Placement Positions based on scale of Tile Prefab (TileScale=1)")] 

    [SerializeField] private float foodHeight=0.105f; 
    [SerializeField] private float innerSpace=0.11f; 
    [SerializeField] private float outerSpace=0.33f; 



    private Vector3 GetPossiblePlacementPoint(){

        possiblePositions=new();
        
        TileType tileType=gameObject.GetComponent<TileInfo>().tileType;
        //always possible inner Positions
        if(tileType!=TileType.DeadEnd){
           possiblePositions=new List<Vector3>()
            {
                new Vector3(0,foodHeight,0),
                new Vector3(-innerSpace,foodHeight,0),
                new Vector3(innerSpace,foodHeight,0),
                new Vector3(innerSpace,foodHeight,innerSpace),
                new Vector3(-innerSpace,foodHeight,innerSpace),
                new Vector3(-innerSpace,foodHeight,-innerSpace),
                new Vector3(innerSpace,foodHeight,-innerSpace),
            }; 

            //outer positions depending on TileType (base for TileDirection)
            if(tileType==TileType.Straight){
                possiblePositions.Add(new Vector3(0,foodHeight,-outerSpace));
                possiblePositions.Add(new Vector3(0,foodHeight,outerSpace));

            }
            else if(tileType==TileType.Curved){
                possiblePositions.Add(new Vector3(0,foodHeight,-outerSpace));
                possiblePositions.Add(new Vector3(outerSpace,foodHeight,0));
            }
            else if(tileType==TileType.Fork){
                possiblePositions.Add(new Vector3(0,foodHeight,outerSpace));
                possiblePositions.Add(new Vector3(outerSpace,foodHeight,0));
                possiblePositions.Add(new Vector3(-outerSpace,foodHeight,0));
            }
        }


        Vector3 randomPosition;

        if(tileType==TileType.DeadEnd){
            //one place to loot foodAmount
            randomPosition=new Vector3(innerSpace,tileGround.transform.position.y,0);
        }
        else{
            randomPosition=GetRandomItem(possiblePositions);
        }
        
        //adjust to TileWidth
        TileWidth=gameObject.transform.localScale.x; 
        Vector3 scaledAdjustedPosition=randomPosition*TileWidth+tileGround.transform.position;

        return scaledAdjustedPosition;

    }

    private T GetRandomItem<T>(List<T> list){
        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }

    public void PlaceFood() {

        Vector3 randomPosition=GetPossiblePlacementPoint();

        Instantiate(foodPrefab, randomPosition, Quaternion.identity,this.gameObject.transform);
    }

}
