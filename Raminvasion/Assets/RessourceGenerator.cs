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
        // MazeGenerator.Instance.OnMazeGenerated += RessourcePlanning;
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
        List<TileInformation> secondaryPathTiles=new();
        List<TileInformation> deadEndTiles=new();

        mainPathTiles=givenList.FindAll(tile=> tile.Area==TileArea.MainPath);
        secondaryPathTiles=givenList.FindAll(tile=> tile.Area==TileArea.SecondaryPath);
        deadEndTiles=givenList.FindAll(tile=> tile.Area==TileArea.DeadEnd);

        
        foodTiles=DistributeFood(mainPathTiles);
        InstantiateOnTiles(foodTiles);

    }

    // public bool CheckIfFood(TileInformation tile){
    //     //needs to be rewritten
    //     if(foodTiles.Contains(tile)){
    //         return true;
    //     }
    //     return false;
    // }

    private void InstantiateOnTiles(Dictionary<TileInformation,int> foodTiles){
        foreach (var item in foodTiles)
        {
            for (int i = 0; i < item.Value; i++)
            {
                item.Key.TileObject.GetComponent<FoodDistribution>().PlaceFood();
            }
        }
    }

    private Dictionary<TileInformation,int> DistributeFood(List<TileInformation> pathTiles){

        List<TileInformation> tilesWithFood=new ();
        Dictionary<TileInformation,int> foodTilesAmount=new();

        //for secondary path it could be flat 60-70 and deadend like a drop of 2-3
        float foodPercentage=GetFoodPercentage(difficultyMode)*0.01f;

        int foodAmount= Mathf.CeilToInt(pathTiles.Count*foodPercentage);

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

        Debug.Log(foodTilesAmount);

        return foodTilesAmount;
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
    
    

