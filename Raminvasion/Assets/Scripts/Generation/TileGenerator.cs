// Created 15.05.2023 by Krista Plagemann //
// Edit: Generates batches of 4x8 tiles(or takes them) and instantiates rows of tiles according to player position //
// Earlier:Generates a single line of tiles 4x8 upon entering trigger collider.//


using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField] private int _LaneColumns = 20;
    [SerializeField] private int _LaneRows = 20;
    [SerializeField] private int _TileWidth = 20;

    // Dead Ends
    [SerializeField] private int _numDeadEnds = 10;
    [SerializeField] private int _numExpansionTiles = 1;

    private float _offset;
    private int _rowsGeneratedIndex = 0;
    private int _lastRowActive = 0;
    private List<TileInformation> _activeSortedTiles = new();

    private bool firstMazeReady = false;

    private GameObject _mazeParent;
    
    [SerializeField] private NavMeshSurface _navMeshSurface;
    private void Start()
    {
        _offset = ((_LaneColumns / 2) * _TileWidth);
        _mazeParent = new GameObject();
        MazeGenerator.Instance.OnMazeGenerated += FinishedMazeGeneration;
        OnSingleLineReady += GenerateSingleRow;

        OnFirstLaneGenerated.AddListener(GenerateRestLanes);    // to generate the rest of the beginning lines that are started below
        GetNewLine();   // Spawn one line at the beginning
    }

    private List<List<TileInformation>> _nextBatch = new();

    public UnityEvent OnFirstLaneGenerated;
    private event Action<List<TileInformation>> OnSingleLineReady;

    private void GenerateRestLanes()
    {
        OnFirstLaneGenerated.RemoveListener(GenerateRestLanes);

        for (int i = 0; i < _TilesVisibleForward - 1; i++)
            GetNewLine();
        firstMazeReady = true;
    }

    // We check the Player position to spawn new tiles in the front and
    // check the Ramen position to remove lines behind it.
    private void Update()
    {
        if (firstMazeReady)
        {
            if (_PlayerToTrack.position.z >= (_rowsGeneratedIndex - _TilesVisibleForward) * _TileWidth)
                GetNewLine();

            if (_VacuumRamenToTrack.position.z >= (_lastRowActive + 2) * _TileWidth)
                RemoveSingleLine();
        }
    }

    // Instantiates a row of tiles from the object pool
    private void GenerateSingleRow(List<TileInformation> lines)
    {
        _rowsGeneratedIndex++;
        //Debug.Log(_rowsGeneratedIndex);
        for (int j = 0; j < lines.Count; j++)
        {
            GameObject newTile = ObjectPool.Instance.GetTile();
            newTile.transform.position = new Vector3(lines[j].IndexX * _TileWidth - _offset, newTile.transform.position.y, (_rowsGeneratedIndex * _TileWidth) - (_TileWidth / 2));
            newTile.transform.localScale = Vector3.one * _TileWidth;
            newTile.transform.parent = _mazeParent.transform;
            newTile.GetComponent<TileInfo>().DeclareTileDirection(lines[j].Direction);
            lines[j].TileObject = newTile;
            _activeSortedTiles.Add(lines[j]);
        }

        OnFirstLaneGenerated?.Invoke();  // this is empty after the first one so maybe remove later with bool or smth
        _navMeshSurface.BuildNavMesh(); // every time we make a row, we build the navmesh new with all active ones
    }

    // Receive one new row. If there is none left, generate a new maze
    private void GetNewLine()
    {
        if (_nextBatch.Count <= 0)
        {
            //Debug.Log("Getting new maze");
            MazeGenerator.Instance.GenerateMazeBlueprint(_LaneColumns, _LaneRows, _TileWidth, _numDeadEnds, _numExpansionTiles);
        }
        else
        {
            List<TileInformation> toReturn = new(_nextBatch[0]);
            _nextBatch.RemoveAt(0);
            //Debug.Log("Lines list to spawn length: " + toReturn.Count);
            OnSingleLineReady?.Invoke(toReturn);
        }
    }

    // Makes a line behind ramen disappear
    private void RemoveSingleLine()
    {
        List<GameObject> toDisableObjects = new(GetLine());

        if (toDisableObjects != null)
        {
            for (int k = 0; k < toDisableObjects.Count; k++)
            {
                _activeSortedTiles.RemoveAt(0);
            }
            ObjectPool.Instance.DisableObjects(toDisableObjects);
            _lastRowActive++;
        }

        // This is called in the first line of this function, i just didn't want to put it outside lol (it's like its own void function)
        List<GameObject> GetLine()
        {
            List<GameObject> toDisableObjects = new();
            int lowestIndex = _activeSortedTiles[0].IndexZ;
            for (int k = 0; k < _activeSortedTiles.Count; k++)
            {
                if (_activeSortedTiles[k].IndexZ != lowestIndex)
                    return toDisableObjects;
                else
                    toDisableObjects.Add(_activeSortedTiles[k].TileObject);
            }
            return null;
        }
    }

    // Hacks the continous list into lists that contain all tiles of a single row (row lists are stored in the outer list :D)
    private List<List<TileInformation>> ChopTileListIntoRows(List<TileInformation> tileBatch)
    {
        //Debug.Log(tileBatch[0].IndexZ + " while we are in " + _rowsGeneratedIndex);
        List<TileInformation> givenTiles = new(tileBatch);
        List<List<TileInformation>> returnList = new();
        for(int i  = 0; i < _LaneRows; i++)
        {
            List<TileInformation> listToFill = new();
            foreach(var tile in givenTiles)
                if(tile.IndexZ == i)
                    listToFill.Add(tile);
            foreach(var removeTile in listToFill)
                givenTiles.Remove(removeTile);

            returnList.Add(listToFill);
        }
        //Debug.Log("Chopped tiles. " + returnList.Count);
        return returnList;
    }

    private void FinishedMazeGeneration(List<TileInformation> newMazeTiles)
    {
        //Debug.Log("Finished maze generation in tile spawner"+ newMazeTiles.Count);

        _nextBatch = new();
        _nextBatch = ChopTileListIntoRows(newMazeTiles);

        List<TileInformation> toReturn = _nextBatch[0];
        _nextBatch.RemoveAt(0);

        OnSingleLineReady?.Invoke(toReturn);
    }
/*

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
    }*/
}
