// Created 15.05.2023 by Krista Plagemann //
// Edit: Generates batches of 4x8 tiles(or takes them) and instantiates rows of tiles according to player position //
// Earlier:Generates a single line of tiles 4x8 upon entering trigger collider.//


using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public enum TileDirection
{ 
    Horizontal, Vertical, 
    LeftFront, FrontRight, RightBack, BackLeft, 
    BackLeftFront, LeftFrontRight, FrontRightBack, RightBackLeft, 
    LeftDead, FrontDead, RightDead
};

public class TileGenerator : MonoBehaviour
{

    #region Singleton

    public static TileGenerator Instance { get; private set; }

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

    #endregion

    [SerializeField] private Transform _PlayerToTrack;
    [SerializeField] private Transform _VacuumRamenToTrack;
    [SerializeField] private int _TilesVisibleForward => _LaneRows;
    //[SerializeField] private GameObject _Collider;

    [SerializeField] private int _LaneColumns = 4;
    [SerializeField] private int _LaneRows = 8;
    [SerializeField] private float _TileWidth = 10;

    private float _offset;
    private int _rowsGeneratedIndex = 0;
    private int _lastRowActive = 0;
    private List<GameObject> _activeSortedTiles = new();
    //private int _tilesActive = 0;

    [SerializeField] private NavMeshSurface _navMeshSurface;
    private void Start()
    {
        _offset = (_LaneColumns / 2) * _TileWidth - _TileWidth / 2;
        GenerateFirstLanes();
    }

    public void GenerateNewLane()
    {
        //GenerateLane();
    }

    /*
    private void GenerateLane()
    {
        for (int i = 0; i < _LaneRows; i++)
        {
            _rowsGeneratedIndex++;
            for (int j = 0; j < _LaneColumns; j++)
            {
                GameObject newTile = ObjectPool.Instance.GetTile();
                newTile.transform.position = new Vector3(j * _TileWidth - _offset, newTile.transform.position.y, (_rowsGeneratedIndex * _TileWidth) - (_TileWidth/2));
                //Debug.Log($"Row: {_rowsGeneratedIndex}, Column: {j}, Position: {newTile.transform.position}");
                _activeSortedTiles.Add(newTile);
                _tilesActive++;

                if (_tilesActive >= _LaneRows * _LaneColumns * 3)
                {
                    List<GameObject> toDisableObjects = new();
                    int oneBatch = _LaneRows * _LaneColumns;
                    for (int k = 0; k < oneBatch; k++)
                    {
                        toDisableObjects.Add(_activeSortedTiles[0]);
                        _activeSortedTiles.RemoveAt(0);
                        
                    }
                    ObjectPool.Instance.DisableObjects(toDisableObjects);
                    _tilesActive -= oneBatch;
                }
            }
        }
        _Collider.transform.position = new Vector3( 0, 0, (_rowsGeneratedIndex * _TileWidth) - (_LaneRows* _TileWidth) + (2 * _TileWidth));

        _navMeshSurface.BuildNavMesh();
    }
    */

    private List<List<int>> _nextBatch = new();
    
    private void GenerateFirstLanes()
    {
        for (int i = 0; i < _TilesVisibleForward; i++)
            GenerateSingleLine();
    }

    private void Update()
    {
        if (_PlayerToTrack.position.z >= (_rowsGeneratedIndex - _TilesVisibleForward)* _TileWidth)
            GenerateSingleLine();
        if(_VacuumRamenToTrack.position.z >= (_lastRowActive +2) * _TileWidth)
            RemoveSingleLine();
    }

    private void GenerateSingleLine()
    {
        List<int> lines = GetNewLine();
        _rowsGeneratedIndex++;
        for (int j = 0; j < lines.Count; j++)
        {
            // if(_lines[j] == 0)
            // make a straight line or make a corner etc

            GameObject newTile = ObjectPool.Instance.GetTile();
            newTile.transform.position = new Vector3(j * _TileWidth - _offset, newTile.transform.position.y, (_rowsGeneratedIndex * _TileWidth) - (_TileWidth / 2));
            //Debug.Log($"Row: {_rowsGeneratedIndex}, Column: {j}, Position: {newTile.transform.position}");
            _activeSortedTiles.Add(newTile);
        }

        _navMeshSurface.BuildNavMesh();
    }

    private void RemoveSingleLine()
    {
        List<GameObject> toDisableObjects = new();
        for (int k = 0; k < _LaneColumns; k++)
        {
            toDisableObjects.Add(_activeSortedTiles[0]);
            _activeSortedTiles.RemoveAt(0);
        }
        ObjectPool.Instance.DisableObjects(toDisableObjects);
        _lastRowActive++;
    }

    private List<int> GetNewLine()
    {
        if (_nextBatch.Count <= 0)
        {
            _nextBatch = GenerateNewTiles();
            List<int> toReturn = _nextBatch[0];
            _nextBatch.RemoveAt(0);
            return toReturn;
        }
        else
        {
            List<int> toReturn = _nextBatch[0];
            _nextBatch.RemoveAt(0);
            return toReturn;
        }       
    }

    public List<List<int>> GenerateNewTiles()
    {
        List<List<int>> returnList = new();
        for (int i = 0; i < _LaneRows; i++)
        {
            List<int> listToFill = new();
            for (int j = 0; j < _LaneColumns; j++)
            {
                listToFill.Add(0);
            }
            returnList.Add(listToFill);
        }
        return returnList;
    }
}
