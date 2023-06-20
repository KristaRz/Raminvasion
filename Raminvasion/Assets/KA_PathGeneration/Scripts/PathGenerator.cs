using System.Collections;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public int minAdjacentTiles = 3;
    public int maxAdjacentTiles = 5;
    public GameObject tilePrefab;
    public float tileSize = 1f;
    public int gridSizeX;
    public int gridSizeZ;
    public float delayBetweenSteps;

    private int[,] mazeGrid;

    // Dead Ends
    public int numDeadEnds = 10;
    //public int maxNumDeadEnds = 20;
    public int numExpansionTiles = 1;


    private void Start()
    {
        GenerateMaze();
    }

    private void GenerateMaze()
    {
        mazeGrid = new int[gridSizeX, gridSizeZ];

        int startColumn = Random.Range(0, gridSizeX);
        mazeGrid[startColumn, 0] = 1;
        Instantiate(tilePrefab, new Vector3(startColumn * tileSize, 0f, 0f), Quaternion.identity);

        Vector2Int direction = Vector2Int.up;
        int currentRow = 0;
        int currentColumn = startColumn;

        bool firstPathGenerated = false;

        while (currentRow < gridSizeZ - 1)
        {
            int numAdjacentTiles = Random.Range(minAdjacentTiles, maxAdjacentTiles + 1);

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
                    {
                        direction = Vector2Int.right;
                    }
                    else if (currentColumn + numAdjacentTiles >= gridSizeX)
                    {
                        direction = Vector2Int.left;
                    }
                    else
                    {
                        direction = Random.value < 0.5f ? Vector2Int.left : Vector2Int.right;
                    }
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
                //Instantiate(tilePrefab, new Vector3(newColumn * tileSize, 0f, newRow * tileSize), Quaternion.identity);

                currentRow = newRow;
                currentColumn = newColumn;
            }

        }

        ExpandMaze();
    }

    private void ExpandMaze()
    {
        //int numDeadEnds = numDeadEnds;
        //int numExpansionTiles = numExpansionTiles;

        int deadEndCount = 0;

        int excludeRows = Mathf.CeilToInt(gridSizeZ * 0.05f);
        int excludeRange = gridSizeZ - excludeRows * 2;

        while (deadEndCount < numDeadEnds)
        {
            bool tileExpanded = false;

            while (!tileExpanded)
            {
                int randomRow = Random.Range(excludeRows, excludeRows + excludeRange);
                int randomColumn = Random.Range(0, gridSizeX);

                if (mazeGrid[randomColumn, randomRow] == 1)
                {
                    // Scale the selected tile on the Y axis
                    Vector3 tilePosition = new Vector3(randomColumn * tileSize, 0f, randomRow * tileSize);
                    //GameObject selectedTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                    //selectedTile.transform.localScale = new Vector3(1f, 3f, 1f);

                    // Expand the maze from the selected tile
                    int expansionTilesCount = Random.Range(1, numExpansionTiles + 1);
                    bool validExpansion = true;
                    int expansionCount = 0;

                    while (validExpansion && expansionCount < expansionTilesCount)
                    {
                        Vector2Int expansionDirection = GetRandomExpansionDirection();
                        int expansionRow = randomRow + expansionDirection.y;
                        int expansionColumn = randomColumn + expansionDirection.x;

                        if (IsExpansionValid(expansionRow, expansionColumn))
                        {
                            mazeGrid[expansionColumn, expansionRow] = 1;
                            Vector3 expansionPosition = new Vector3(expansionColumn * tileSize, 0f, expansionRow * tileSize);
                            //Instantiate(tilePrefab, expansionPosition, Quaternion.identity);
                            randomRow = expansionRow;
                            randomColumn = expansionColumn;
                            expansionCount++;
                        }
                        else
                        {
                            validExpansion = false;
                        }
                    }

                    if (expansionCount > 0)
                    {
                        tileExpanded = true;
                        deadEndCount++;
                    }
                    //else
                    //{
                    //    Destroy(selectedTile);
                    //}
                }
            }
        }

        DefineTiles defineTiles = GetComponent<DefineTiles>();
        if (defineTiles != null)
        {
            defineTiles.GenerateTiles( mazeGrid, gridSizeX, gridSizeZ, tileSize);
        }

    }

    private Vector2Int GetRandomExpansionDirection()
    {
        int randomDirection = Random.Range(0, 4);

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

        if (mazeGrid[column, row] == 1)
        {
            return false;
        }

        int adjacentTiles = 0;

        if (row > 0 && mazeGrid[column, row - 1] == 1)
        {
            adjacentTiles++;
        }

        if (row < gridSizeZ - 1 && mazeGrid[column, row + 1] == 1)
        {
            adjacentTiles++;
        }

        if (column > 0 && mazeGrid[column - 1, row] == 1)
        {
            adjacentTiles++;
        }

        if (column < gridSizeX - 1 && mazeGrid[column + 1, row] == 1)
        {
            adjacentTiles++;
        }

        return adjacentTiles <= 1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Vector3 gridSize = new Vector3(gridSizeX * tileSize, 0f, gridSizeZ * tileSize);
        Gizmos.DrawWireCube(gridSize / 2f, gridSize);
    }
}
