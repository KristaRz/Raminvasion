// Created by Julia Podlipensky
// > calculates amount of resources to distribute for dequeued instantiated Tiles
//> distributes resource amount randomly across those Tiles in a Dictionary
//> max. 2 foods and 3 obstacles per Tile
//> functions for Food and Obstacles are almost duplicates :O They can be compromised for cleaner code, the only difference should be an accesible rule books


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class RessourceGenerator : MonoBehaviour
{
    public static RessourceGenerator Instance { get; private set; }

    [Header("Lists and Dictionaries for RessourcePlanning")] 
    [SerializeField] private Dictionary<TileInformation, int> foodTiles;
    [SerializeField] private Dictionary<TileInformation, int> obstacleTiles;
    [SerializeField] public Queue<TileInformation> tileQueue;
    [SerializeField] private List<TileInformation> toPopulateTiles;

    [Header("RessourceGenerator Stats")] 
    [SerializeField] private int toPopulateTilesAmount=5;
    [SerializeField] private int maxObstacleOnTile=3;
    [SerializeField] private int maxFoodOnTile=2;

    //just for updating those values on this script
    [SerializeField] private int currentRowSpawnedIndex=0;
    [SerializeField] private DifficultyMode difficultyMode;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start() {
        tileQueue=new();
    }



    
    // Building a Tile Queue of Instantiated Tiles. 
    // After certain number is reached, these number of tiles are dequeued for the Resource Planning
    /// <param name="tile">TileInformation of instantiated Tile that should be queued.</param>
    public void HandleTileQueue(TileInformation tile, int rowsGeneratedIndex){
        tileQueue.Enqueue(tile);
        if(tileQueue.Count>=toPopulateTilesAmount){

            //updating value for this script
            currentRowSpawnedIndex=rowsGeneratedIndex;

            toPopulateTiles=new();
                for (int i = 0; i < toPopulateTilesAmount ; i++)
                {
                    TileInformation tileToPopulate= tileQueue.Dequeue();
                    toPopulateTiles.Add(tileToPopulate);
                }
            RessourceAreaPlanning(toPopulateTiles);
        }
    }

    //Building Tile-List with sorted Tiles by Area (because different amounts and rules for areas)
    //Sending those Lists of to distribute Ressources and add those to foodTiles & obstacleTiles Dictionary
    //Sending Dictionaries for Instantiation
    private void RessourceAreaPlanning(List<TileInformation> newMazeTiles){

        List<TileInformation> givenList=new(newMazeTiles);
        foodTiles=new Dictionary<TileInformation, int>();
        obstacleTiles=new Dictionary<TileInformation, int>();

        List<TileInformation> mainPathTiles=new();
        List<TileInformation> deadEndTiles=new();

        mainPathTiles=givenList.FindAll(tile=> tile.Area==TileArea.MainPath);
        deadEndTiles=givenList.FindAll(tile=> tile.Area==TileArea.DeadEnd);

        if(mainPathTiles.Count>0){
          DistributeFood(mainPathTiles);  
          DistributeObstacles(mainPathTiles);  
        }
        if(deadEndTiles.Count>0){
          DistributeFood(deadEndTiles); 
          //no obstacles on DeadEnd-Tiles 
        }
        
        InstantiateFoodOnTiles(foodTiles);
        InstantiateObstacleOnTiles(obstacleTiles);

    }

    #region Instantiators
    //Goes through dictionary and calls Instantion-Function in RessourceDistribution.cs for each Tile n-amount times

        //For Food
        private void InstantiateFoodOnTiles(Dictionary<TileInformation,int> foodTiles){

            foreach (var item in foodTiles)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    //sending over currentRowSpawnedIndex for rules on Tile-level
                    item.Key.TileObject.GetComponent<RessourceDistribution>().PlaceFood(currentRowSpawnedIndex);
                }
            }
        }

        //For Obstacles (its a duplicate from quick extension)
        private void InstantiateObstacleOnTiles(Dictionary<TileInformation,int> foodTiles){
            foreach (var item in foodTiles)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    item.Key.TileObject.GetComponent<RessourceDistribution>().PlaceObstacles(currentRowSpawnedIndex);
                }
            }
        }

    #endregion

    #region ResourceAmount
    //Calculates Resource-Amount based on GameDifficulty to distribute across Tile-Lists

        //For Food. Difference between Areas. Amount based on densityPercentage & amount of pathTiles to populate
        private int GetFoodAmount(List<TileInformation> pathTiles){
                int foodAmount=0;

                if(pathTiles[0].Area==TileArea.DeadEnd){
                    //for deadEnd drop 3 flat;
                    foodAmount=3;
                }
                else{
                    difficultyMode=GameHandler.Instance.difficultyMode;
                    float foodPercentage=GetFoodPercentage(difficultyMode)*0.01f;

                foodAmount= Mathf.CeilToInt(pathTiles.Count*foodPercentage);
                }
                return foodAmount;
        }

        //For Obstacles. Amount based on densityPercentage, amount of pathTiles to populate & max. placements on Tile
        private int GetObstacleAmount(List<TileInformation> pathTiles){

                int obstacleAmount=0;

                difficultyMode=GameHandler.Instance.difficultyMode;
                
                float obstaclePercentage=GetFoodPercentage(difficultyMode)*0.01f;

                obstacleAmount= Mathf.CeilToInt(maxObstacleOnTile*pathTiles.Count*obstaclePercentage);
                
                return obstacleAmount;
        }

    #endregion

    #region DictionaryDistribution
    //Distributes ResourceAmount across Dictionary of Area-Tiles to populate. Random Tiles increase values for resources (if not capped). Food or Obstacle Dictionary is then extended with those new entries
        
        //For Food
        private void DistributeFood(List<TileInformation> pathTiles){
            if(pathTiles!=null){

                int foodAmount=GetFoodAmount(pathTiles);

                //building Dictionary for tiles and food
                Dictionary<TileInformation,int> foodTilesAmount=new();

                foreach(TileInformation item in pathTiles){
                    foodTilesAmount.Add(item,0);
                    }
                
                int index = 0;
                while (foodAmount >=0 && index < 1000)
                    {   
                        
                        int randomIndex=Mathf.RoundToInt(Random.Range(0,pathTiles.Count-1));
                        
                        //not more than 2 foods on a tile rn
                        if(foodTilesAmount[pathTiles[randomIndex]]<maxFoodOnTile){
                        
                            foodTilesAmount[pathTiles[randomIndex]]++;

                            foodAmount--; 
                        }
                        else if(pathTiles[0].Area==TileArea.DeadEnd){
                            foodTilesAmount[pathTiles[randomIndex]]++;

                            foodAmount--; 
                        }
                        

                        index++;
            
                    }

                foreach (var item in foodTilesAmount)
                    {
                        foodTiles.Add(item.Key,item.Value);
                    }
                }
            }

        //For Obstacles (its basically the same)
        private void DistributeObstacles(List<TileInformation> pathTiles){
            if(pathTiles!=null){

                int obstacleAmount=GetObstacleAmount(pathTiles);

                //building Dictionary for tiles and obstacles
                Dictionary<TileInformation,int> obstacleTilesAmount=new();

                foreach(TileInformation item in pathTiles){
                    obstacleTilesAmount.Add(item,0);
                }
                
                int index = 0;
                while (obstacleAmount >=0 && index < 1000)
                {   
                    
                    int randomIndex=Mathf.RoundToInt(Random.Range(0,pathTiles.Count-1));
                    
                    if(obstacleTilesAmount[pathTiles[randomIndex]]<maxObstacleOnTile){
                        obstacleTilesAmount[pathTiles[randomIndex]]++;

                        obstacleAmount--;
                    }

                    index++;
                }

                foreach (var item in obstacleTilesAmount)
                {
                    obstacleTiles.Add(item.Key,item.Value);
                }
                }
            }

        #endregion


        //Food DensityPercentage for dequeued Tiles based on DifficultyMode
        private int GetFoodPercentage(DifficultyMode difficulty)
        {
            switch (difficulty)
            {
                case DifficultyMode.Easy:
                    return 40; 
                case DifficultyMode.Medium:
                    return 20; 
                case DifficultyMode.Hard:
                    return 5; 
                default:
                    return 0; 
            }
        }

        //Obstacle DensityPercentage for dequeued Tiles based on DifficultyMode
        private int GetObstaclePercentage(DifficultyMode difficulty)
        {
            switch (difficulty)
            {
                case DifficultyMode.Easy:
                    return 35; 
                case DifficultyMode.Medium:
                    return 65; 
                case DifficultyMode.Hard:
                    return 95; 
                default:
                    return 0; 
            }
        }

        
}
    
