
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MinimapGenerator : MonoBehaviour
{
    public static MinimapGenerator Instance { get; private set; }

    private void Awake() => Instance = this;

    [SerializeField] private GameObject _MazeParent;
    [SerializeField] private GameObject _spriteTilePrefab;
    [SerializeField] private Sprite _MazeSprite;
    [SerializeField] private Sprite _BlankSprite;

    private List<List<GameObject>> _tileGrid = new();
    private int _sizeConversion;
    private int _offset;
    private int _playerRow = 10;
    private int _rowsWalked = 0;


    // Player
    [SerializeField] private RectTransform _mapPlayer;
    [SerializeField] private RectTransform _mapEnemy;
    private Transform _player;
    private Transform _enemy;
    

    private void Start()
    {
        GameHandler.Instance.OnPlayerSpawned.AddListener(SetPlayer);
        _sizeConversion = 20;
        _offset = (20 * 20)/2;
        for (int i = 0; i < 20; i++)
        {
            GameObject rowParent = new GameObject();
            RectTransform trans = rowParent.AddComponent<RectTransform>();
            trans.sizeDelta = new Vector2((20*20) *2, 40);
            rowParent.transform.SetParent(_MazeParent.transform);
            rowParent.AddComponent<HorizontalLayoutGroup>();

            List<GameObject> newList = new();
            for (int j = 0; j < 20; j++)
            {
                GameObject newSprite = Instantiate(_spriteTilePrefab);
                newSprite.transform.SetParent(rowParent.transform);
                newList.Add(newSprite);
            }
            _tileGrid.Add(newList);
        }
        _tileGrid.Reverse();
    }

    private async void SetPlayer()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _enemy = GameObject.FindGameObjectWithTag("Ramen").transform;
        await UpdatePlayerPos();
    }

    private bool _recalculating = false;
    public bool SetMazeTile(float xCoordinate, float zCoordinate)
    {
        if (_recalculating)
            return false;
        if (_tileGrid.Count <= _playerRow + (((int)zCoordinate - (_rowsWalked * _sizeConversion)) / _sizeConversion))
            return false;
        List<GameObject> mazeRow = _tileGrid[_playerRow  + (((int)zCoordinate - (_rowsWalked * _sizeConversion)) / _sizeConversion)];

        if (mazeRow.Count <= ((int)xCoordinate + _offset) / _sizeConversion)
            return false;
        mazeRow[((int)xCoordinate + _offset) / _sizeConversion].GetComponent<Image>().sprite = _MazeSprite;
        return true;
    }

    private bool _updatePosition = true;
    private async Task UpdatePlayerPos()
    {
        while (_updatePosition)
        {
            _mapPlayer.anchoredPosition = new Vector2(_player.position.x * 2, 10);
            _mapEnemy.anchoredPosition = new Vector2(_enemy.position.x * 2, _enemy.position.z * 2 - (_rowsWalked * 40));

            if (_player.position.z >= (_rowsWalked * 20) + 11)
            {
                _recalculating = true;
                _rowsWalked++;
                List<List<GameObject>> tempList = new();
                for(int i = 1; i < _tileGrid.Count; i++)
                {
                    List<GameObject> tileList = new();
                  
                    for (int j = 0; j < 20; j++)
                    {
                        tileList.Add(_tileGrid[i-1][j]);
                        Sprite lastTileSprite = _tileGrid[i][j].GetComponent<Image>().sprite;
                        tileList[j].GetComponent<Image>().sprite = lastTileSprite;
                    }
                    tempList.Add(tileList);
                }

                List<GameObject> lastTileList = new();
                
                for (int j = 0; j < 20; j++)
                {
                    lastTileList.Add(_tileGrid[^1][j]);
                    Image tileSprite = lastTileList[j].GetComponent<Image>();
                    tileSprite.sprite = _BlankSprite;
                }

                tempList.Add(lastTileList);

                _tileGrid.Clear();
                _tileGrid.AddRange(tempList);
            }
            _recalculating = false;

            await Task.Yield();
        }
    }


    private void OnDisable()
    {
        _updatePosition = false;
    }
}
