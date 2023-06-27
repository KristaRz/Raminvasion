using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//should be somwhere else in a levelmanager with checks. for now its ok here
public enum DifficultyMode{
    Easy,
    Medium,
    Hard
}


public class RessourceGenerator : MonoBehaviour
{
    public static RessourceGenerator Instance { get; private set; }

    [SerializeField] private List<TileInformation> toPopulateTiles;

    [SerializeField] private DifficultyMode difficultyMode;

    [SerializeField] private Dictionary<TileInformation, int> foodTiles;

    [SerializeField] public Queue<TileInformation> tileQueue;

    [SerializeField] private int toPopulateTilesAmount=5;

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

        List<TileInformation> mainPathTiles=new();
        // List<TileInformation> secondaryPathTiles=new();
        List<TileInformation> deadEndTiles=new();

        mainPathTiles=givenList.FindAll(tile=> tile.Area==TileArea.MainPath);
        //secondaryPathTiles=givenList.FindAll(tile=> tile.Area==TileArea.SecondaryPath);
        deadEndTiles=givenList.FindAll(tile=> tile.Area==TileArea.DeadEnd);

        if(mainPathTiles.Count>0){
          DistributeFood(mainPathTiles);  
        }
        // if(secondaryPathTiles.Count>0){
        //   DistributeFood(secondaryPathTiles);  
        // }
        if(deadEndTiles.Count>0){
          DistributeFood(deadEndTiles);  
        }
        
        InstantiateOnTiles(foodTiles);

    }


    private void InstantiateOnTiles(Dictionary<TileInformation,int> foodTiles){
        foreach (var item in foodTiles)
        {
            for (int i = 0; i < item.Value; i++)
            {
                item.Key.TileObject.GetComponent<FoodDistribution>().PlaceFood();
            }
        }
    }

    private void DistributeFood(List<TileInformation> pathTiles){
        if(pathTiles!=null){

            //there should be extern function as rulebook for different cases

            float foodPercentage=GetFoodPercentage(difficultyMode)*0.01f;

            // if(pathTiles[0].Area==TileArea.SecondaryPath){
            //     //for secondary path it could be flat 60-70%
            //     foodPercentage=0.65f;
            // }

            int foodAmount= Mathf.CeilToInt(pathTiles.Count*foodPercentage);

            if(pathTiles[0].Area==TileArea.DeadEnd){
                //for deadEnd drop 2-3
                foodAmount=3;
            }

            //building Dictionary for tiles and food
            Dictionary<TileInformation,int> foodTilesAmount=new();

            foreach(TileInformation item in pathTiles){
                foodTilesAmount.Add(item,0);
            }
            
            int index = 0;
            while (foodAmount >=0 && index < 1000)
            {
                int randomIndex=Mathf.RoundToInt(Random.Range(0,pathTiles.Count-1));
                
                foodTilesAmount[pathTiles[randomIndex]]++;

                foodAmount--;
    
            }

            foreach (var item in foodTilesAmount)
            {
                foodTiles.Add(item.Key,item.Value);
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
                    return 10; 
                default:
                    return 0; 
            }
    }
}
    
    

