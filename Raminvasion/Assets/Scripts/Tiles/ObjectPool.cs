
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    [SerializeField] private GameObject _SampleTile;
    [SerializeField] private int _StartingAmount;

    private List<GameObject> _Tiles = new();

    private void Start()
    {
        for (int i = 0; i < _StartingAmount; i++)
        {
            GameObject newTile = Instantiate(_SampleTile);
            newTile.SetActive(false);
            _Tiles.Add(newTile);
        }
    }

    public GameObject GetTile()
    {
        foreach(GameObject tile in _Tiles)
        {
            if (!tile.gameObject.activeInHierarchy)
            {
                tile.SetActive(true);
                return tile;
            }
        }
        GameObject newTile = Instantiate(_SampleTile);
        _Tiles.Add(newTile);
        return newTile;
    }

    public void DisableObjects(List<GameObject> objects)
    {
        foreach(GameObject tile in objects)
        {
            if(_Tiles.Contains(tile))
                tile.SetActive(false);
        }
    }
}
