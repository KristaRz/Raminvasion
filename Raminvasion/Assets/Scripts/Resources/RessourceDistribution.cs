// Created by Julia Podlipensky
//> handles placement on Tile level
//> distributes resources from RessourceGenerator-Dictionary across possible spawn positions on tile
//> different resource types based on player progress



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceDistribution : MonoBehaviour
{
    [Header("GameObjects and Prefabs")] 
    [SerializeField] private GameObject tileGround;
    [SerializeField] private GameObject ramenShop=null;
    [SerializeField] private List<GameObject> foodPrefabs=new();
    [SerializeField] private List<GameObject> obstaclePrefabs=new();


    [Header("Tile Stats")] 
    [SerializeField] private float TileWidth;
    [SerializeField] private List<Vector3> possiblePositions;
    private bool firstCheckPos=false;


    [Header("Values for Placement Positions based on scale of Tile Prefab (TileScale=1)")] 

    [SerializeField] private float foodHeight=0.105f; 
    [SerializeField] private float innerSpace=0.11f; 
    [SerializeField] private float outerSpace=0.33f; 

    


    //returns a random possible placement position on this Tile
    private Vector3 GetPossiblePlacementPoint(TileType tileType){
        //check if tile positions are already established
        if(!firstCheckPos){
            possiblePositions=new();
        
            //always possible inner Positions
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
        
        //height for food
    	Vector3 addedHeightPos=new Vector3(randomPosition.x,foodHeight,randomPosition.z);
        
        //adjust to TileWidth
        TileWidth=gameObject.transform.localScale.x; 
        Vector3 scaledAdjustedPosition=addedHeightPos*TileWidth+tileGround.transform.position;

        return scaledAdjustedPosition;
    }

    //Returns random Item from a List
    private T GetRandomItem<T>(List<T> list){
        if(list!=null && list.Count!=0){
            int randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }
        return default;
        
    }

    #region Placers
    //Places Ressource on picked PlacementPosition on Tile. Called from RessourceGenerator.cs

        //For Food. Picks Random Food and Instantiates on picked position. If its a dead End Tile, further work is done on the RamenStall
        public void PlaceFood(int currentRowIndex) {
            TileType tileType=gameObject.GetComponent<TileInfo>().tileType;

            Vector3 randomPosition=GetPossiblePlacementPoint(tileType);

            //could add food after certain currentRowIndex

            GameObject foodPrefab=GetRandomItem(foodPrefabs);

            GameObject food= Instantiate(foodPrefab, randomPosition, Quaternion.identity,this.gameObject.transform);

            if(tileType==TileType.DeadEnd){
                GameObject ramenStall=transform.Find("ramen_stall").gameObject;
                //for work on RamenStall
                ramenStall.GetComponent<RamenStallInteraction>().AddFoodToStall(food);
            }
        }

        //For Obstacles. Picks Random Obstacle and Instantiates on picked position. With increasing currentRowIndex harder obstacle is added to random pick.
        public void PlaceObstacles(int currentRowIndex){
            TileType tileType=gameObject.GetComponent<TileInfo>().tileType;

            Vector3 randomPosition=GetPossiblePlacementPoint(tileType);
            
            GameObject obstaclePrefab;
            if(currentRowIndex<=20){
                //[0] is the box
                obstaclePrefab=obstaclePrefabs[0];
            }
            else{
                obstaclePrefab=GetRandomItem(obstaclePrefabs);
            }

            //random rotation of obstacles for some variety
            float randomAngle = Random.Range(0f, 360f);
            Quaternion randomRotation=Quaternion.Euler(0,randomAngle,0);

            Instantiate(obstaclePrefab, new Vector3(randomPosition.x, tileGround.transform.position.y, randomPosition.z), randomRotation,this.gameObject.transform);
        }
    #endregion
}
