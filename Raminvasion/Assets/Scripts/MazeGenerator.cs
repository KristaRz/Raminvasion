/*Kaan Ko�ak              Procedural Level Generation Elective                 May 2023/
/*
 * MazeGenerator.cs
 * 
 * This script generates a maze using a randomized algorithm. It creates a grid of tiles
 * and connects them to form a maze structure.
 * 
 * Functionality:
 * - Regenerates the maze upon key press or at the start of the game.
 * - Uses a coroutine to generate the maze step by step with a visual delay.
 * - Allows customization of grid size, tile parameters, and delay between steps.
 * - Stores the generated maze in a 2D array.
 * 
 * Usage:
 * - Attach this script to a GameObject in the scene.
 * - Assign the required variables such as tilePrefab, tileSize, gridSizeX, gridSizeZ, etc.
 * - Press the designated key (default: R) to regenerate the maze.
 */

//Note vector2int.up represents the front direction.




using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance { get; private set; }
    private void Awake()
    {
        transform.parent = null;
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }


    public event Action<List<TileInformation>> OnMazeGenerated = delegate { };
    private List<TileInformation> continuousMazeDirections = new();

    private int _minAdjacentTiles = 3;
    private int _maxAdjacentTiles = 5;

    private int[,] mazeGrid;

    // Dead ends
    //public int maxNumDeadEnds = 20;
    private int _numDeadEnds = 10;
    private int _numExpansionTiles = 1;

    private int gridSizeX = 20;
    private int gridSizeZ = 20;
    private int _tileSize = 20;

    private int _lastTileX;
    private bool _firstMazeGenerated = false;

    private int _crossWaysAmount = 0;

    public void GenerateMazeBlueprint(int columns, int rows, int tileSize, int minAdjacentTiles, int maxAdjacentTiles, int numDeadEnds, int numExpansionTiles)
    {       
        _tileSize = tileSize;
        gridSizeX = columns;
        gridSizeZ = rows;
        _minAdjacentTiles = minAdjacentTiles;
        _maxAdjacentTiles = maxAdjacentTiles;
        _numDeadEnds = numDeadEnds;
        _numExpansionTiles = numExpansionTiles;

        mazeGrid = new int[gridSizeX, gridSizeZ];

        // Deciding the first column position based on previous maze or in the center if first maze

        int startColumn;
        if (!_firstMazeGenerated)   // if this is first maze
        {
            startColumn = gridSizeX / 2;
            _firstMazeGenerated = true;
        }
        else
            startColumn = _lastTileX;

        mazeGrid[startColumn, 0] = 1;

        Vector2Int direction = Vector2Int.up;
        int currentRow = 0;
        int currentColumn = startColumn;

        bool firstPathGenerated = false;

        while (currentRow < gridSizeZ - 1)
        {
            int numAdjacentTiles = UnityEngine.Random.Range(_minAdjacentTiles, _maxAdjacentTiles + 1);

            if (!firstPathGenerated)
            {
                // Set the direction to front for the first path
                direction = Vector2Int.up;
                firstPathGenerated = true;
            }
            else
            {
                if (direction == Vector2Int.up)
                {
                    if (currentColumn - numAdjacentTiles < 0)
                        direction = Vector2Int.right;
                    else if (currentColumn + numAdjacentTiles >= gridSizeX)
                        direction = Vector2Int.left;
                    else
                        direction = UnityEngine.Random.value < 0.5f ? Vector2Int.left : Vector2Int.right;
                }
                else
                {
                    direction = Vector2Int.up;
                }
            }

            for (int i = 0; i < numAdjacentTiles; i++)
            {
                int newRow = currentRow + direction.y;
                int newColumn = currentColumn + direction.x;

                if (newRow >= gridSizeZ - 1)
                {
                    int remainingRows = gridSizeZ - 1 - currentRow;
                    numAdjacentTiles = Mathf.Min(numAdjacentTiles, remainingRows);
                }

                mazeGrid[newColumn, newRow] = 1;

                currentRow = newRow;
                currentColumn = newColumn;
            }
        }

        // Go through the last row and save the column position of the filled tile for the next maze
        for(int i = 0; i < gridSizeX; i++)
        {
            if (mazeGrid[i, gridSizeZ - 1] == 1)
                _lastTileX = i;        
        }

        // Add dead ends
        ExpandMaze();
    }

    private void ExpandMaze()
    {
        int deadEndCount = 0;
        int tryCount = 0;
        int failCount = 0;

        int excludeRows = Mathf.CeilToInt(gridSizeZ * 0.05f);
        int excludeRange = gridSizeZ - excludeRows * 2;

        int[,] firstPath = (int[,])mazeGrid.Clone(); // Create a copy of the mazeGrid for the first path

        while (deadEndCount < _numDeadEnds)
        {
            int randomRow = UnityEngine.Random.Range(excludeRows, excludeRows + excludeRange);
            int randomColumn = UnityEngine.Random.Range(0, gridSizeX);

            if (firstPath[randomColumn, randomRow] == 1)
            {
                // Expand the maze from the selected tile
                int expansionTilesCount = UnityEngine.Random.Range(1, _numExpansionTiles + 1);
                int expansionCount = 0;

                while (expansionCount < expansionTilesCount)
                {
                    Vector2Int expansionDirection = GetRandomExpansionDirection();
                    int expansionRow = randomRow + expansionDirection.y;
                    int expansionColumn = randomColumn + expansionDirection.x;

                    if (IsExpansionValid(expansionRow, expansionColumn))
                    {
                        mazeGrid[randomColumn, randomRow] = 2; // Mark the selected tile as 2
                        firstPath[randomColumn, randomRow] = 2; // Mark the selected tile as 2

                        mazeGrid[expansionColumn, expansionRow] = 2;

                        randomRow = expansionRow;
                        randomColumn = expansionColumn;
                        expansionCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (expansionCount > 0)
                {
                    deadEndCount++;
                }

            }
        }
        AddCrossWays( firstPath);


        // Declare the directions of the maze tiles and receive back a list of TileInformation to hand to the TileGenerator
        continuousMazeDirections = MazeTileDeclaration.PositionTiles(mazeGrid, gridSizeX, gridSizeZ, _tileSize);
        OnMazeGenerated(continuousMazeDirections);


    }

    private void AddCrossWays( int[,] firstPath)
    {
        int crossWayCount = 0;

        while (crossWayCount < _crossWaysAmount)
        {
            for (int row = 0; row < gridSizeZ; row++)
            {
                for (int column = 0; column < gridSizeX; column++)
                {
                    if (firstPath[column, row] == 2)
                    {
                        if (TryExpandFromPosition(column, row))
                        {
                            crossWayCount++;
                            if (crossWayCount >= _crossWaysAmount)
                                return;
                        }
                    }
                }
            }
        }
    }

    private bool TryExpandFromPosition(int column, int row)
    {

        int expansionCount = 0;

        while (expansionCount < _numExpansionTiles)
        {
            Vector2Int expansionDirection = GetRandomExpansionDirection();
            int expansionRow = row + expansionDirection.y;
            int expansionColumn = column + expansionDirection.x;

            if (IsExpansionValid(expansionRow, expansionColumn))
            {
                mazeGrid[column, row] = 2; // Mark the selected tile as 2
                mazeGrid[expansionColumn, expansionRow] = 2;

                column = expansionColumn;
                row = expansionRow;
                expansionCount++;
            }
            else
            {
                break;
            }
        }

        if (expansionCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    
    }

    private Vector2Int GetRandomExpansionDirection()
    {
        int randomDirection = UnityEngine.Random.Range(0, 4);

        switch (randomDirection)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.down;
            case 2: return Vector2Int.left;
            case 3: return Vector2Int.right;
            default: return Vector2Int.zero;
        }
    }

    private bool IsExpansionValid(int row, int column)
    {
        if (row < 1 || row >= gridSizeZ - 1 || column < 0 || column >= gridSizeX)
        {
            return false;
        }

        if (mazeGrid[column, row] != 0) // Not an empty tile
        {
            return false;
        }

        int adjacentTiles = 0;

        if (row > 0 && mazeGrid[column, row - 1] > 0) // Check top neighbor
        {
            adjacentTiles++;
        }

        if (row < gridSizeZ - 1 && mazeGrid[column, row + 1] > 0) // Check bottom neighbor
        {
            adjacentTiles++;
        }

        if (column > 0 && mazeGrid[column - 1, row] > 0) // Check left neighbor
        {
            adjacentTiles++;
        }

        if (column < gridSizeX - 1 && mazeGrid[column + 1, row] > 0) // Check right neighbor
        {
            adjacentTiles++;
        }

        return adjacentTiles == 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Vector3 gridSize = new Vector3(gridSizeX * _tileSize, 0f, gridSizeZ * _tileSize);
        Gizmos.DrawWireCube(gridSize /2f, gridSize);
    }


    /*

    public int minAdjacentTiles = 3;
    public int maxAdjacentTiles = 5;
    public GameObject tilePrefab;
    public float tileSize = 1f;
    public int gridSizeX;
    public int gridSizeZ;
    public float delayBetweenSteps;

    private GameObject[,] mazeGrid;

    public KeyCode regenerateKey = KeyCode.R;
    private GameObject mazeObject;

    private void OnEnable()
    {
        //RegenerateMaze();
    }

    private void Update()
    {
        if (Input.GetKeyDown(regenerateKey))
        {
            RegenerateMaze();
        }
    }

    private void RegenerateMaze()
    {
        if (mazeObject != null)
        {
            //Deactivate Tile Objects in Objectpool
            List<GameObject> toDisableObjects = new();
            foreach (Transform child in mazeObject.transform)
            {
                toDisableObjects.Add(child.gameObject);
            }
            ObjectPool.Instance.DisableObjects(toDisableObjects);

            // Destroy(mazeObject);

        }

        mazeObject = new GameObject("Maze");

        //StartCoroutine(GenerateMaze(mazeObject.transform));
    }
    */

    #region Kaan 1stMaze

    /*
        private IEnumerator GenerateMaze(Transform parent)
        {
            mazeGrid = new GameObject[gridSizeX, gridSizeZ];

            // Randomly create a tile on the first row [EDIT: Changed it always start in the middle]
            //int startColumn = Random.Range(0, gridSizeX);
            int startColumn = gridSizeX / 2;

            // mazeGrid[startColumn, 0] = Instantiate(tilePrefab, new Vector3(startColumn * tileSize, 0f, 0f), Quaternion.identity, parent);

            //Instead of Instantiating, activating Tile in ObjectPool
            GameObject newMazeTile = ObjectPool.Instance.GetTile(); // Instead of always going into the list to find it, we can just work with a direct access to the object       
            newMazeTile.transform.position = new Vector3(startColumn * tileSize - (gridSizeX * tileSize) / 2, 0f, 0f);
            newMazeTile.transform.rotation = Quaternion.identity;
            newMazeTile.transform.localScale = Vector3.one * 20;
            newMazeTile.transform.parent = parent;

            mazeGrid[startColumn, 0] = newMazeTile;

            // Set the initial direction
            Vector2Int direction = Vector2Int.up;

            int currentRow = 0;
            int currentColumn = startColumn;

            bool firstPathGenerated = false; // Flag to track if the first path has been generated

            while (currentRow < gridSizeZ - 1)
            {
                // Decide the number of adjacent tiles
                int numAdjacentTiles = UnityEngine.Random.Range(minAdjacentTiles, maxAdjacentTiles + 1);

                // Change direction based on the current direction
                if (direction == Vector2Int.up)
                {
                    // Next direction must be left or right
                    if (currentColumn - numAdjacentTiles < 0)
                    {
                        // If the decision falls outside the left boundary, set the direction to right
                        direction = Vector2Int.right;
                    }
                    else if (currentColumn + numAdjacentTiles >= gridSizeX)
                    {
                        // If the decision falls outside the right boundary, set the direction to left
                        direction = Vector2Int.left;
                    }
                    else
                    {
                        // Randomly decide between left and right
                        direction = UnityEngine.Random.value < 0.5f ? Vector2Int.left : Vector2Int.right;
                    }
                }
                else
                {
                    // Next direction must be up
                    direction = Vector2Int.up;
                }

                // Instantiate adjacent tiles based on the current direction
                for (int i = 0; i < numAdjacentTiles; i++)
                {
                    int newRow = currentRow + direction.y;
                    int newColumn = currentColumn + direction.x;

                    //Changing previous adjacent Tile with direction=Vector2Int.up to Curve
                    MazeTileDeclaration.ChangePreviousTile(mazeGrid[currentColumn, currentRow], direction, i);

                    // Check if the next position goes beyond the array boundaries on the front direction
                    if (newRow >= gridSizeZ - 1)
                    {
                        // Adjust the direction to fit within the boundaries
                        direction = (currentColumn == 0) ? Vector2Int.right : Vector2Int.left;
                    }

                    // //Instantiate the tile at the new position
                    // GameObject newTile = Instantiate(tilePrefab, new Vector3(newColumn * tileSize, 0f, newRow * tileSize), Quaternion.identity, parent);

                    //Instead of Instantiating, activating Tile in ObjectPool
                    GameObject newTile = ObjectPool.Instance.GetTile();
                    newTile.transform.position = new Vector3(newColumn * tileSize - (gridSizeX * tileSize) / 2, 0f, newRow * tileSize);
                    newTile.transform.rotation = Quaternion.identity;
                    newTile.transform.localScale = Vector3.one * 20;
                    newTile.transform.parent = parent;

                    //Declaring Tile Direction & Curves for adjacent Tiles
                    MazeTileDeclaration.DeclareAdjacentTiles(newTile, direction, i, numAdjacentTiles);

                    // Store the tile in the mazeGrid array and mark it as 1
                    mazeGrid[newColumn, newRow] = newTile;

                    currentRow = newRow;
                    currentColumn = newColumn;
                }

                if (!firstPathGenerated && currentRow == gridSizeZ - 1)
                {
                    // First path generated
                    firstPathGenerated = true;
                }

                yield return new WaitForSeconds(delayBetweenSteps);
            }

            //if (firstPathGenerated)

        }

    */

    #endregion

    #region Krista Maze

    /*

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Vector3 gridSize = new Vector3(gridSizeX * tileSize, 0f, gridSizeZ * tileSize);
        Gizmos.DrawWireCube(new Vector3(0f, 0f, gridSize.z / 2), gridSize);
    }

    [SerializeField] private bool _TurnOnBuggyDeadends = false;
    [SerializeField] private int _DeadendIterations = 20;
    [SerializeField] private int _DeadendLength = 3;

    public event Action<List<TileInformation>> OnMazeGenerated = delegate { };


    private List<TileInformation> continuousMazeDirections = new();
    private int maxColumns = 20;
    private int _adjacentTilesLeft = 0;
    private TileInformation lastTile = null;
    private bool _firstMazeGenerated = false;

    // 1 Vertical, 2 Horizontal,

    public void GenerateMazeBlueprint(int columns, int rows, int tileSize)
    {
        maxColumns = columns;
        int randomAdjacentAmount = 0;

        // Starts the maze in the middle
        int startColumn = columns / 2;

        // Deciding the first tile direction based on previous maze or new directions if first maze
        int newTileDirection = 0;

        if (lastTile == null)   // if this is first maze
        {

            newTileDirection = 1;
            lastTile = new TileInformation(startColumn, 0, newTileDirection, TileDirection.Vertical,TileArea.MainPath);  // create a tile at our current standpoint
        }
        else
        {
            randomAdjacentAmount = _adjacentTilesLeft;
            if(_adjacentTilesLeft == 0)
                newTileDirection = UnityEngine.Random.value < 0.5f ? 1 : 2;
        }
        continuousMazeDirections.Clear();
        continuousMazeDirections = new() { lastTile };  // make a new list with only the last tile as starting point
        //Debug.Log("Starting the maze");


        for (int i = 0; i < rows-1 ; i++)
        {
            if (randomAdjacentAmount <= 0)
            {
                randomAdjacentAmount = UnityEngine.Random.Range(minAdjacentTiles - 1, maxAdjacentTiles);
                _adjacentTilesLeft = randomAdjacentAmount;
            }
           
            //Debug.Log("repeating with more");

            // If last was vertical we just make a new vertical one and bye haha
            if (lastTile.TileDirectionIndex == 1)
            {
                // Reduce the adjacent tile count. If we are done with them, change direction
                randomAdjacentAmount--;
                TileInformation newTileInfo;
                if (randomAdjacentAmount <= 0)
                    newTileInfo = new TileInformation(lastTile.IndexX, lastTile.IndexZ + 1, 2, TileDirection.Horizontal, TileArea.MainPath); // add horizontal if end
                else
                    newTileInfo = new TileInformation(lastTile.IndexX, lastTile.IndexZ + 1, 1, TileDirection.Vertical,TileArea.MainPath); // else add vertical again
                continuousMazeDirections.Add(newTileInfo);
            }
            else  // if last was horizontal
            {
                int randomDirection = UnityEngine.Random.value < 0.5f ? -1 : 1;    // -1 = Left, 1 = Right
                GenerateHorizontal(randomAdjacentAmount, randomDirection);  // Generates the continuous lane

                // And then adds one vertical
                randomAdjacentAmount = UnityEngine.Random.Range(minAdjacentTiles - 1, maxAdjacentTiles);
                _adjacentTilesLeft = randomAdjacentAmount-1;

                TileInformation newTileInfo = new TileInformation(lastTile.IndexX, lastTile.IndexZ + 1, 1, TileDirection.Vertical,TileArea.MainPath); // else add vertical again
                continuousMazeDirections.Add(newTileInfo);
            }

            lastTile = continuousMazeDirections[^1];
        }

        lastTile = continuousMazeDirections[^1];
        _adjacentTilesLeft = randomAdjacentAmount;

        List<TileInformation> declaredTile = MazeTileDeclaration.DeclareTileTypes(continuousMazeDirections, lastTile, _firstMazeGenerated);
        continuousMazeDirections = declaredTile;

        if (_firstMazeGenerated)
            continuousMazeDirections.RemoveAt(0);   // if we make subsequent mazes, we need to get rid of the 0th tile bcs its a double for tile types
        _firstMazeGenerated = true;

        //Debug.Log(continuousMazeDirections.Count);
        if(_TurnOnBuggyDeadends)
            SpawnDeadEnds(_DeadendIterations, _DeadendLength);

        OnMazeGenerated?.Invoke(continuousMazeDirections);
    }



    private void GenerateHorizontal(int numAdjacentTiles, int randomTileDirection)
    {
        //Debug.Log("Running a horizontal adjacent tiles task.");
        int newTileDirection;
        TileDirection newTileDirectionEnum;

        // Spawn tiles according to numAdjacentTiles
        for (int i = 1; i <= numAdjacentTiles; i++)
        {
            if (lastTile.IndexX + randomTileDirection <= 0 || lastTile.IndexX + randomTileDirection >= maxColumns // when we hit left or right
                || i == numAdjacentTiles)     // or this is the last tile
            {
                // next Tile is a vertical one again
                newTileDirection = 1;
                newTileDirectionEnum = TileDirection.Vertical;
            }
            else // if previous was horizontal tile
            {
                // next tile is a horizontal one again
                newTileDirection = 2;
                newTileDirectionEnum = TileDirection.Horizontal;
            }
            TileInformation newTileInfo = new TileInformation
                (lastTile.IndexX + randomTileDirection, lastTile.IndexZ, newTileDirection, newTileDirectionEnum,TileArea.MainPath);

            continuousMazeDirections.Add(newTileInfo);
            lastTile = newTileInfo;
        }
    }


    ///////////////////////////////////
    //////////// DEADENDS /////////////
    ///////////////////////////////////

    private void SpawnDeadEnds(int amount, int deadEndLength)
    {
        for (int i = 0; i < amount; i++)
        {
            List<TileInformation> singleDeadend = new();

            TileInformation randomStartpoint = continuousMazeDirections[UnityEngine.Random.Range(continuousMazeDirections.Count-1, 0)];
            singleDeadend.Add(randomStartpoint);

            // Try to find a position around that is free (tries twice)
            TileInformation firstTilePos = TryFindStartPoint(randomStartpoint, false);
            if (firstTilePos.TileDirectionIndex != 0) // kind of a null check 
            {
                // Check if all the tiles around are free (except for the one we came from)
                bool firstTileSurround = CheckIfTileFreeSurround(firstTilePos.IndexX, firstTilePos.IndexZ, true, new Vector2(randomStartpoint.IndexX, randomStartpoint.IndexZ));
                if (firstTileSurround)
                {
                    singleDeadend.Add(firstTilePos);

                    int amountAccepted = 0;
                    TileInformation previousTilePos = firstTilePos;

                    for (int k = 0; k < deadEndLength - 1; k++)
                    {
                        TileInformation newTilePosition = TryFindStartPoint(previousTilePos, true);
                        if (newTilePosition.TileDirectionIndex != 0) // kind of a null check
                        {
                            bool tileSurroundingFree = CheckIfTileFreeSurround(newTilePosition.IndexX, newTilePosition.IndexZ, true, new Vector2(previousTilePos.IndexX, previousTilePos.IndexZ));
                            if (tileSurroundingFree)
                            {
                                amountAccepted++;
                                previousTilePos = newTilePosition;
                                singleDeadend.Add(newTilePosition);
                            }
                        }
                    }
                    if (amountAccepted >= deadEndLength - 1)
                    {
                        List<TileInformation> declaredTiles = MazeTileDeclaration.DeclareDeadEndTypes(singleDeadend, continuousMazeDirections);
                        continuousMazeDirections = declaredTiles;
                    }
                }
            }         
        }       
    }

    private TileInformation TryFindStartPoint(TileInformation randomStartpoint, bool includeAll)
    {
        int randomChecked = 0;
        int maxDirections = 3;
        // Check twice if we can find a free direction
        for (int j = 0; j < 2; j++)
        {
            if (includeAll)
                maxDirections = 4;
            int randomDirection = UnityEngine.Random.Range(1, maxDirections);
            while (randomDirection == randomChecked)
                randomDirection = UnityEngine.Random.Range(1, maxDirections);

            switch (randomDirection)
            {
                /// Left ///
                case 1:
                    if (CheckIfTileFree(randomStartpoint.IndexX - 1, randomStartpoint.IndexZ))
                    {
                        randomStartpoint.TileDirectionIndex = 2;
                        return new TileInformation(randomStartpoint.IndexX - 1, randomStartpoint.IndexZ, 2, TileDirection.Horizontal,TileArea.SecondaryPath);
                    }
                    break;
                /// Foward ///
                case 2:
                    if (CheckIfTileFree(randomStartpoint.IndexX, randomStartpoint.IndexZ + 1))
                    {
                        randomStartpoint.TileDirectionIndex = 1;
                        return new TileInformation(randomStartpoint.IndexX, randomStartpoint.IndexZ + 1, 1, TileDirection.Vertical,TileArea.SecondaryPath);
                    }
                    break;
                /// Right ///
                case 3:
                    if (CheckIfTileFree(randomStartpoint.IndexX + 1, randomStartpoint.IndexZ))
                    {
                        randomStartpoint.TileDirectionIndex = 2;
                        return new TileInformation(randomStartpoint.IndexX + 1, randomStartpoint.IndexZ, 2, TileDirection.Horizontal,TileArea.SecondaryPath);
                    }
                    break;
                /// Back ///
                case 4:
                    if (CheckIfTileFree(randomStartpoint.IndexX, randomStartpoint.IndexZ - 1))
                    {
                        randomStartpoint.TileDirectionIndex = 1;
                        return new TileInformation(randomStartpoint.IndexX, randomStartpoint.IndexZ - 1, 1, TileDirection.Vertical,TileArea.SecondaryPath);
                    }
                    break;

            }
        }
        return new TileInformation(-1, -1, 0, TileDirection.Vertical,TileArea.MainPath);
    }

    private bool CheckIfTileFree(float indexX, float indexY)
    {
        foreach (var tile in continuousMazeDirections)
        {
            if (tile.IndexX == indexX && tile.IndexZ == indexY)
                return false;
        }
        return true;
    }

    private bool CheckIfTileFreeSurround(float indexX, float indexY, bool excludeOriginal, Vector2 originalTile)
    {
        bool leftFree = CheckIfTileFree(indexX - 1, indexY);
        bool rightFree = CheckIfTileFree(indexX + 1, indexY);
        bool forwardFree = CheckIfTileFree(indexX, indexY + 1);
        bool backFree = CheckIfTileFree(indexX, indexY - 1);

        if (excludeOriginal)
        {
            Vector2 offset = originalTile - new Vector2(indexX, indexY);
            
            if (offset.x > 0) // Old tile right
                rightFree = true;
            else if (offset.x < 0) // Old tile left
                leftFree = true;
            else if(offset.y < 0) // Old tile back
                backFree = true;
        }

        if (leftFree && forwardFree && backFree && rightFree)
            return true;
        else
            return false;
    }

        */

    #endregion

}

[Serializable]
public class TileInformation
{
    /// <summary>
    /// Index of the tile on the x axis (from 0 to maximum amount of columns)
    /// </summary>
    public int IndexX;

    /// <summary>
    /// Index of the tile on the z axis (from 0 to maximum amount of rows)
    /// </summary>
    public int IndexZ;
    /// <summary>
    /// A simple int 1 or 2 to declare Vertical(1) or Horizontal(2) direction while generating the maze
    /// </summary>
    public int TileDirectionIndex;
    /// <summary>
    /// The precise direction or type of the tile.
    /// </summary>
    public TileDirection Direction;
    /// <summary>
    /// The GameObject when instantiated.
    /// </summary>
    public GameObject TileObject;
    /// <summary>
    /// Area of the Maze the Tile is part of
    /// </summary>
    public TileArea Area;


    public TileInformation(int indexX, int indexZ, int tileDirectionIndex, TileDirection direction, TileArea area)
    {
        IndexX = indexX;
        IndexZ = indexZ;
        TileDirectionIndex = tileDirectionIndex;
        Direction = direction; 
        Area=area;
    }
}