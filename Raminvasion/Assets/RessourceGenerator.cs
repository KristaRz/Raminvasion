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


    [SerializeField] private DifficultyMode difficultyMode;

    [SerializeField] private List<TileInformation> foodTiles;

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
        MazeGenerator.Instance.OnMazeGenerated += RessourcePlanning;
    }

    private void RessourcePlanning(List<TileInformation> newMazeTiles){

        List<TileInformation> givenList=new(newMazeTiles);
        foodTiles=new();

        List<TileInformation> mainPathTiles=new();
        // List<TileInformation> secondaryPathTiles=new();
        // List<TileInformation> deadEndTiles=new();

        mainPathTiles=givenList.FindAll(tile=> tile.Area==TileArea.MainPath);

        foodTiles=DistributeFood(mainPathTiles);

    }

    public bool CheckIfFood(TileInformation tile){
        if(foodTiles.Contains(tile)){
            return true;
        }
        return false;
    }

    private List<TileInformation> DistributeFood(List<TileInformation> pathTiles){

        List<TileInformation> tilesWithFood=new ();

        //for secondary path it could be flat 60-70 and deadend like a drop of 2-3
        float foodPercentage=GetFoodPercentage(difficultyMode)*0.01f;

        int foodAmount= Mathf.CeilToInt(pathTiles.Count*foodPercentage);


        
        
        while (foodAmount >=0)
        {
            int randomIndex=Mathf.RoundToInt(Random.Range(0,pathTiles.Count-1));
            if(!tilesWithFood.Contains(pathTiles[randomIndex])){
                tilesWithFood.Add(pathTiles[randomIndex]);
                foodAmount--;
            }
            
        }

        return tilesWithFood;


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
    
    

