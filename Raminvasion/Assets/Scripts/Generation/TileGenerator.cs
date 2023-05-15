
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;


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

    [SerializeField] private GameObject _Collider;

    [SerializeField] private int _LaneColumns = 4;
    [SerializeField] private int _LaneRows = 8;
    [SerializeField] private float _TileWidth = 10;

    private float _offset;
    private int _rowsGeneratedIndex = -1;
    [SerializeField] private List<GameObject> _activeSortedTiles = new();
    private int _tilesActive = 0;

    private void Start()
    {
        _offset = (_LaneColumns / 2) * _TileWidth - _TileWidth / 2;
        GenerateLane();
    }

    public void GenerateNewLane()
    {
        GenerateLane();
    }

    [SerializeField] private NavMeshSurface _navMeshSurface;

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


}
