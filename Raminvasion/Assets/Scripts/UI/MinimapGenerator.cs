// Created 25.06.2023 by Krista Plagemann //
// Generates a minimap based on tiles "collected" in TileCollector via "gaze tracking". Displays the player and enemy on the map as well.
// Use "E" key to open or close the map.

// In the future should probably adjust the canvas size according to the maze size (so get row and column count of TileGenerator)
// Also needs to somehow preserve some info backwards, probably according to enemy position plus half the map (bcs you would die if you meet in the middle)

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MinimapGenerator : MonoBehaviour
{
    public static MinimapGenerator Instance { get; private set; }

    private void Awake() => Instance = this;

    [SerializeField] private GameObject _MapVisuals;
    [SerializeField] private RectTransform _MazeParent;
    [SerializeField] private GameObject _spriteTilePrefab;
    [SerializeField] private Sprite _MazeSprite;
    [SerializeField] private Sprite _BlankSprite;

    private List<List<GameObject>> _tileGrid = new();
    private int _tileSize;    // size of the displayed tiles
    private int _offset;            // half size of the minimap
    private float _MapOffset;       // Anchored position of the map parent
    private int _rowsWalked = 0;


    [SerializeField] private RectTransform _mapPlayer;  // canvas display of player
    [SerializeField] private RectTransform _mapEnemy; // canvas display of ramen
    private Transform _player; // player in game
    private Transform _enemy; // ramen in game


    private void Start()
    {
        GameHandler.Instance.OnPlayerDefined += SetPlayer;

        // Fills the canvas with rows and columns of square sprites.

        _tileSize = 20;
        _offset = (_tileSize * 20)/2;
        _MapOffset = _MazeParent.anchoredPosition.y;
        for (int i = 0; i < 20; i++)
        {
            GameObject rowParent = new GameObject();
            RectTransform trans = rowParent.AddComponent<RectTransform>();
            trans.sizeDelta = new Vector2(20*20, 20);
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
        _tileGrid.Reverse();    // gotta turn around the list so it's bottom to top.
    }

    // As soon as the player is set we can start updating the position of the player and ramen we get.
    private async void SetPlayer(GameObject player, GameObject ramen)
    {
        _player = player.transform;
        _enemy = ramen.transform;
        await UpdatePlayerPos();
    }

    private bool _recalculating = false; // ensures we don't add tiles while we are shifting the map.

    /// <summary>
    /// Collects a tile when we see it and adds it to the map.
    /// </summary>
    /// <param name="xCoordinate"></param>
    /// <param name="zCoordinate"></param>
    /// <returns>If the setting was successful</returns>
    public bool SetMazeTile(float xCoordinate, float zCoordinate)
    {
        if (_recalculating)
            return false;
        if (_tileGrid.Count <= _tileSize/2 + (((int)zCoordinate - (_rowsWalked * _tileSize)) / _tileSize))
            return false;
        List<GameObject> mazeRow = _tileGrid[_tileSize/2 + (((int)zCoordinate - (_rowsWalked * _tileSize)) / _tileSize)];

        if (mazeRow.Count <= ((int)xCoordinate + _offset) / _tileSize)
            return false;
        mazeRow[((int)xCoordinate + _offset) / _tileSize].GetComponent<Image>().sprite = _MazeSprite;
        return true;
    }

    private bool _updatePosition = true;

    /// <summary>
    /// Updates the player and ramen position continuously.
    /// </summary>
    private async Task UpdatePlayerPos()
    {
        while (_updatePosition)
        {
            if(Input.GetKeyDown(KeyCode.E))
                _MapVisuals.SetActive(!_MapVisuals.activeSelf);

            // Player position by normal horizontal x position while vertical y position is at half map.
            _mapPlayer.anchoredPosition = new Vector2(_player.position.x + _tileSize/2, _tileSize/2);

            float playerToRamenDistance = _player.position.z - _enemy.position.z;

            // Ramen position is again normal horizontal x in relation to tiles and vertical y calculated by distance to player
            _mapEnemy.anchoredPosition = new Vector2(_enemy.position.x + _tileSize/2, _tileSize/2 - playerToRamenDistance);
            // Shifts the map position up and down within one tilesize range according to player position within the tile he is in.
            _MazeParent.anchoredPosition = new Vector2(_MazeParent.anchoredPosition.x, _MapOffset + _tileSize + _tileSize/2 - (_player.position.z -(_rowsWalked * _tileSize)));

            // If we enter a new row that we haven't walked on before, we shift the sprite tiles info one row down.
            if (_player.position.z >= (_rowsWalked * _tileSize) + _tileSize)
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
                // makes the top row of the list blank sprites
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
