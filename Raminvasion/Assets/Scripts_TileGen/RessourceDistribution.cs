using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceDistribution : MonoBehaviour
{

    [SerializeField] private GameObject tileGround;

    [SerializeField] private GameObject ramenShop=null;

    [SerializeField] private List<GameObject> foodPrefabs=new();
    [SerializeField] private List<GameObject> obstaclePrefabs=new();



    [SerializeField] private float TileWidth;

    [SerializeField] private List<Vector3> possiblePositions;

    private bool firstCheckPos=false;


    [Header("Values for Placement Positions based on scale of Tile Prefab (TileScale=1)")] 

    [SerializeField] private float foodHeight=0.105f; 
    [SerializeField] private float innerSpace=0.11f; 
    [SerializeField] private float outerSpace=0.33f; 



    private Vector3 GetPossiblePlacementPoint(TileType tileType){

        if(!firstCheckPos){
            possiblePositions=new();
        
            
            //always possible inner Positions

            //first on x-z plane
           possiblePositions=new List<Vector3>()
            {
                new Vector3(0,foodHeight,0),
                new Vector3(-innerSpace,0,0),
                new Vector3(innerSpace,0,0),
                new Vector3(innerSpace,0,innerSpace),
                new Vector3(-innerSpace,0,innerSpace),
                new Vector3(-innerSpace,0,-innerSpace),
                new Vector3(innerSpace,0,-innerSpace),
            }; 

            //outer positions depending on TileType (base for TileDirection)
            if(tileType==TileType.Straight){
                possiblePositions.Add(new Vector3(0,0,-outerSpace));
                possiblePositions.Add(new Vector3(0,0,outerSpace));

            }
            else if(tileType==TileType.Curved){
                possiblePositions.Add(new Vector3(0,0,-outerSpace));
                possiblePositions.Add(new Vector3(outerSpace,0,0));
            }
            else if(tileType==TileType.Fork){
                possiblePositions.Add(new Vector3(0,0,outerSpace));
                possiblePositions.Add(new Vector3(outerSpace,0,0));
                possiblePositions.Add(new Vector3(-outerSpace,0,0));
            }

            firstCheckPos=true;
        }

        
        


        Vector3 randomPosition=GetRandomItem(possiblePositions);

        //prevent double placement
        possiblePositions.Remove(randomPosition);
        
        //height depending on ressource
    	Vector3 addedHeightPos=new Vector3(randomPosition.x,foodHeight,randomPosition.z);
        
        //adjust to TileWidth
        TileWidth=gameObject.transform.localScale.x; 
        Vector3 scaledAdjustedPosition=addedHeightPos*TileWidth+tileGround.transform.position;

        return scaledAdjustedPosition;

    }

    private T GetRandomItem<T>(List<T> list){
        if(list!=null && list.Count!=0){
            int randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }
        return default;
        
    }

    public void PlaceFood() {
        TileType tileType=gameObject.GetComponent<TileInfo>().tileType;

        Vector3 randomPosition=GetPossiblePlacementPoint(tileType);
        GameObject foodPrefab=GetRandomItem(foodPrefabs);

        GameObject food= Instantiate(foodPrefab, randomPosition, Quaternion.identity,this.gameObject.transform);

        if(tileType==TileType.DeadEnd){
            GameObject ramenStall=transform.Find("ramen_stall").gameObject;
            ramenStall.GetComponent<RamenStallInteraction>().foods.Add(food);
        }
    }

    public void PlaceObstacles(){
        TileType tileType=gameObject.GetComponent<TileInfo>().tileType;

        Vector3 randomPosition=GetPossiblePlacementPoint(tileType);
        GameObject obstaclePrefab=GetRandomItem(obstaclePrefabs);

        Instantiate(obstaclePrefab, new Vector3(randomPosition.x, tileGround.transform.position.y, randomPosition.z), Quaternion.identity,this.gameObject.transform);
    }

}
