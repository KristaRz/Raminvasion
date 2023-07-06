using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class RessourceGenerator : MonoBehaviour
{
    public static RessourceGenerator Instance { get; private set; }

    [SerializeField] private List<TileInformation> toPopulateTiles;

    [SerializeField] private DifficultyMode difficultyMode;

    [SerializeField] private Dictionary<TileInformation, int> foodTiles;
    [SerializeField] private Dictionary<TileInformation, int> obstacleTiles;

    [SerializeField] public Queue<TileInformation> tileQueue;

    [SerializeField] private int toPopulateTilesAmount=5;

    // [SerializeField] private int PlacementsOnTile=6;
    [SerializeField] private int maxObstacleOnTile=3;
    [SerializeField] private int maxFoodOnTile=2;

    private float currentDistance;

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

    

    

    public void HandleTileQueue(TileInformation tile){
        
        tileQueue.Enqueue(tile);
        if(tileQueue.Count>=toPopulateTilesAmount){
            toPopulateTiles=new();
                for (int i = 0; i < toPopulateTilesAmount ; i++)
                {
                    TileInformation tileToPopulate= tileQueue.Dequeue();
                    toPopulateTiles.Add(tileToPopulate);
                }
            RessourceAreaPlanning(toPopulateTiles);

        }
    }

    private void RessourceAreaPlanning(List<TileInformation> newMazeTiles){

        List<TileInformation> givenList=new(newMazeTiles);
        foodTiles=new Dictionary<TileInformation, int>();
        obstacleTiles=new Dictionary<TileInformation, int>();

        List<TileInformation> mainPathTiles=new();
        // List<TileInformation> secondaryPathTiles=new();
        List<TileInformation> deadEndTiles=new();

        mainPathTiles=givenList.FindAll(tile=> tile.Area==TileArea.MainPath);
        deadEndTiles=givenList.FindAll(tile=> tile.Area==TileArea.DeadEnd);

        if(mainPathTiles.Count>0){
          DistributeFood(mainPathTiles);  
          DistributeObstacles(mainPathTiles);  
        }
        if(deadEndTiles.Count>0){
          DistributeFood(deadEndTiles);  
        }
        
        InstantiateFoodOnTiles(foodTiles);
        InstantiateObstacleOnTiles(obstacleTiles);

    }


    private void InstantiateFoodOnTiles(Dictionary<TileInformation,int> foodTiles){
        foreach (var item in foodTiles)
        {
            for (int i = 0; i < item.Value; i++)
            {
                item.Key.TileObject.GetComponent<RessourceDistribution>().PlaceFood();
            }
        }
    }

    private void InstantiateObstacleOnTiles(Dictionary<TileInformation,int> foodTiles){
        foreach (var item in foodTiles)
        {
            for (int i = 0; i < item.Value; i++)
            {
                item.Key.TileObject.GetComponent<RessourceDistribution>().PlaceObstacles();
            }
        }
    }

    private int GetFoodAmount(List<TileInformation> pathTiles){
       //rulebook to fill
            int foodAmount=0;

            if(pathTiles[0].Area==TileArea.DeadEnd){
                //for deadEnd drop 2-3
                foodAmount=3;
            }
            else{
                difficultyMode=GameHandler.Instance.difficultyMode;
                float foodPercentage=GetFoodPercentage(difficultyMode)*0.01f;

              foodAmount= Mathf.CeilToInt(pathTiles.Count*foodPercentage);
            }
            return foodAmount;
    }

    private int GetObstacleAmount(List<TileInformation> pathTiles){
       //rulebook to fill
            int obstacleAmount=0;

            difficultyMode=GameHandler.Instance.difficultyMode;
            
            float obstaclePercentage=GetFoodPercentage(difficultyMode)*0.01f;

            obstacleAmount= Mathf.CeilToInt(maxObstacleOnTile*pathTiles.Count*obstaclePercentage);
            
            return obstacleAmount;
    }

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
                    
                    //not more than 2 foods on a tile
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

        private void DistributeObstacles(List<TileInformation> pathTiles){
        if(pathTiles!=null){

            int obstacleAmount=GetObstacleAmount(pathTiles);

            //building Dictionary for tiles and food
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
    
    

