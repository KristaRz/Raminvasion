using System.Collections;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI; // Required when Using UI elements.

public class PathGenerator : MonoBehaviour
{
    //public Slider slidee;

    public int minAdjacentTiles = 3;
    public int maxAdjacentTiles = 5;
    public GameObject tilePrefab;
    public float tileSize = 1f;
    public int gridSizeX;
    public int gridSizeZ;
    public float delayBetweenSteps;

    // Dead Ends
    public int numDeadEnds = 10;
    public int numExpansionTiles = 1;

    public int crossWaysAmount = 0;

    public KeyCode regenerateKey = KeyCode.R;

    private int[,] mazeGrid;
    private GameObject mazeObject;



    private void Start()
    {
        RegenerateMaze();

    }

    private void Update()
    {
        if (Input.GetKeyDown(regenerateKey))
        {
            RegenerateMaze();
        }
    }

    public void RegenerateMaze()
    {
        if (mazeObject != null)
        {
            Destroy(mazeObject);
        }

        mazeObject = new GameObject("Maze");

        // Reset mazeGrid
        mazeGrid = new int[gridSizeX, gridSizeZ];

        StartCoroutine(GenerateMaze(mazeObject.transform));
    }


    private IEnumerator GenerateMaze(Transform parent)
    {
        mazeGrid = new int[gridSizeX, gridSizeZ]; // Create a 2D array to represent the maze grid

        int startColumn = Random.Range(0, gridSizeX); // Choose a random starting column for the maze path
        mazeGrid[startColumn, 0] = 1; // Mark the starting position as part of the maze path
        Instantiate(tilePrefab, new Vector3(startColumn * tileSize, 0f, 0f), Quaternion.identity, parent); // Instantiate a tile at the starting position

        Vector2Int direction = Vector2Int.up; // Set the initial direction as up
        int currentRow = 0; // Initialize the current row as 0
        int currentColumn = startColumn; // Set the current column as the starting column

        bool firstPathGenerated = false; // Flag to check if the first path has been generated

        while (currentRow < gridSizeZ - 1) // Continue until reaching the last row of the maze
        {
            int numAdjacentTiles = Random.Range(minAdjacentTiles, maxAdjacentTiles + 1); // Determine the number of adjacent tiles to generate

            if (!firstPathGenerated) //Make the first decision to the front
            {
                // Set the direction to up for the first path
                direction = Vector2Int.up;
                firstPathGenerated = true;
            }
            else
            {
                if (direction == Vector2Int.up) // Making Left and Right Decisions
                {
                    if (currentColumn - numAdjacentTiles < 0)
                        direction = Vector2Int.right; // If moving left is not possible, change direction to right

                    else if (currentColumn + numAdjacentTiles >= gridSizeX)
                        direction = Vector2Int.left; // If moving right is not possible, change direction to left

                    else
                        direction = Random.value < 0.5f ? Vector2Int.left : Vector2Int.right; // Randomly choose to move left or right
                }
                else // Making front decisions
                    direction = Vector2Int.up; // If the current direction is not up, change it to up
            }

            for (int i = 0; i < numAdjacentTiles; i++)  // Generate the adjacent tiles in the maze based on the current direction
            {
                int newRow = currentRow + direction.y; // Calculate the new row based on the current direction
                int newColumn = currentColumn + direction.x; // Calculate the new column based on the current direction

                if (newRow >= gridSizeZ - 1)  //If the decision is out of array boundaries on Z axis
                {
                    int remainingRows = gridSizeZ - 1 - currentRow;
                    numAdjacentTiles = Mathf.Min(numAdjacentTiles, remainingRows); // Reduce the number of adjacent tiles if reaching the last rows
                }

                mazeGrid[newColumn, newRow] = 1; // Mark the new position as part of the maze path
                Instantiate(tilePrefab, new Vector3(newColumn * tileSize, 0f, newRow * tileSize), Quaternion.identity, parent); // Instantiate a tile at the new position

                currentRow = newRow; // Update the current row
                currentColumn = newColumn; // Update the current column
            }
            yield return new WaitForSeconds(delayBetweenSteps); // Wait for a delay between generating each step of the maze
        }
        ExpandMaze(parent); // Expand the generated maze to create dead ends and crossways
    }
    private void ExpandMaze(Transform parent)
    {
        int deadEndCount = 0; // Counter for the number of dead ends created

        int excludeRows = Mathf.CeilToInt(gridSizeZ * 0.05f); // Number of rows to exclude from expansion at the top and bottom of the maze
        int excludeRange = gridSizeZ - excludeRows * 2; // Range of rows available for expansion

        int[,] firstPath = (int[,])mazeGrid.Clone(); // Create a copy of the mazeGrid for the first path

        while (deadEndCount < numDeadEnds)
        {
            int randomRow = Random.Range(excludeRows, excludeRows + excludeRange); // Choose a random row within the expansion range
            int randomColumn = Random.Range(0, gridSizeX); // Choose a random column

            if (firstPath[randomColumn, randomRow] == 1)
            {
                // Scale the selected tile on the Y axis
                Vector3 tilePosition = new Vector3(randomColumn * tileSize, 0f, randomRow * tileSize);
                GameObject selectedTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, parent);
                selectedTile.transform.localScale = new Vector3(1f, 3f, 1f);

                // Expand the maze from the selected tile
                int expansionTilesCount = Random.Range(1, numExpansionTiles + 1); // Determine the number of expansion tiles to generate from the selected tile
                int expansionCount = 0; // Counter for the number of expansions made

                while (expansionCount < expansionTilesCount)
                {
                    Vector2Int expansionDirection = GetRandomExpansionDirection(); // Get a random expansion direction
                    int expansionRow = randomRow + expansionDirection.y; // Calculate the new row based on the expansion direction
                    int expansionColumn = randomColumn + expansionDirection.x; // Calculate the new column based on the expansion direction

                    if (IsExpansionValid(expansionRow, expansionColumn))
                    {
                        mazeGrid[randomColumn, randomRow] = 2; // Mark the selected tile as 2
                        firstPath[randomColumn, randomRow] = 2; // Mark the selected tile as 2

                        mazeGrid[expansionColumn, expansionRow] = 2; // Mark the expansion tile as 2
                        Vector3 expansionPosition = new Vector3(expansionColumn * tileSize, 0f, expansionRow * tileSize);
                        Instantiate(tilePrefab, expansionPosition, Quaternion.identity, parent); // Instantiate a tile at the expansion position

                        randomRow = expansionRow; // Update the random row
                        randomColumn = expansionColumn; // Update the random column
                        expansionCount++; // Increment the expansion count
                    }
                    else
                        break; // Stop expansion if the expansion is not valid
                }

                if (expansionCount > 0)
                    deadEndCount++; // Increment the dead end count if at least one expansion was made
                else
                    Destroy(selectedTile); // Destroy the selected tile if no expansions were made
            }
        }

        AddCrossWays(parent, firstPath); // Add crossways to the maze based on the first path

        InstantiateTiles instantiateTiles = GetComponent<InstantiateTiles>();
        if (instantiateTiles != null)
        {
            instantiateTiles.GenerateTiles(parent, mazeGrid, gridSizeX, gridSizeZ, tileSize, delayBetweenSteps); // Generate the tiles for the expanded maze
        }
    }

    private void AddCrossWays(Transform parent, int[,] firstPath)
    {
        int crossWayCount = 0; // Counter for the number of crossways added

        while (crossWayCount < crossWaysAmount)
        {
            for (int row = 0; row < gridSizeZ; row++) // Iterate through each row of the maze
            {
                for (int column = 0; column < gridSizeX; column++) // Iterate through each column of the maze
                {
                    if (firstPath[column, row] == 2) // Check if the tile at the current position is part of the first path
                    {
                        if (TryExpandFromPosition(column, row, parent)) // Try expanding from the current position
                        {
                            crossWayCount++; // Increment the crossway count
                            if (crossWayCount >= crossWaysAmount)
                                return; // Return if the desired number of crossways has been added
                        }
                    }
                }
            }
        }
    }
    private bool TryExpandFromPosition(int column, int row, Transform parent)
    {
        Vector3 tilePosition = new Vector3(column * tileSize, 0f, row * tileSize); // Calculate the position of the selected tile
        GameObject selectedTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, parent); // Instantiate the selected tile
        selectedTile.transform.localScale = new Vector3(1f, 3f, 1f); // Scale the selected tile on the Y axis

        int expansionCount = 0; // Counter for the number of expansions made

        while (expansionCount < numExpansionTiles)
        {
            Vector2Int expansionDirection = GetRandomExpansionDirection(); // Get a random expansion direction
            int expansionRow = row + expansionDirection.y; // Calculate the new row based on the expansion direction
            int expansionColumn = column + expansionDirection.x; // Calculate the new column based on the expansion direction

            if (IsExpansionValid(expansionRow, expansionColumn))
            {
                mazeGrid[column, row] = 2; // Mark the selected tile as 2
                mazeGrid[expansionColumn, expansionRow] = 2; // Mark the expansion tile as 2

                Vector3 expansionPosition = new Vector3(expansionColumn * tileSize, 0f, expansionRow * tileSize); // Calculate the position of the expansion tile
                Instantiate(tilePrefab, expansionPosition, Quaternion.identity, parent); // Instantiate the expansion tile

                column = expansionColumn; // Update the column to the new expansion column
                row = expansionRow; // Update the row to the new expansion row
                expansionCount++; // Increment the expansion count
            }
            else
                break; // Stop expansion if the expansion is not valid
        }

        if (expansionCount > 0)
            return true; // Return true if at least one expansion was made
        else
        {
            Destroy(selectedTile); // Destroy the selected tile if no expansions were made
            return false; // Return false
        }
    }
    private Vector2Int GetRandomExpansionDirection()
    {
        int randomDirection = Random.Range(0, 4); // Generate a random number between 0 and 3 to represent the four possible directions

        switch (randomDirection)
        {
            case 0: return Vector2Int.up; // Return up direction if randomDirection is 0
            case 1: return Vector2Int.down; // Return down direction if randomDirection is 1
            case 2: return Vector2Int.left; // Return left direction if randomDirection is 2
            case 3: return Vector2Int.right; // Return right direction if randomDirection is 3
            default: return Vector2Int.zero; // Return zero vector if randomDirection is invalid
        }
    }

    private bool IsExpansionValid(int row, int column)
    {
        if (row < 1 || row >= gridSizeZ - 1 || column < 0 || column >= gridSizeX)
            return false; // Return false if the expansion is outside the valid maze bounds or in the top or bottom row

        if (mazeGrid[column, row] != 0) // Return false if the expansion position is already occupied in the maze
            return false;

        int adjacentTiles = 0; // Counter for the number of adjacent tiles to the expansion position

        if (row > 0 && mazeGrid[column, row - 1] > 0) // Check top neighbor
            adjacentTiles++;

        if (row < gridSizeZ - 1 && mazeGrid[column, row + 1] > 0) // Check bottom neighbor
            adjacentTiles++;

        if (column > 0 && mazeGrid[column - 1, row] > 0) // Check left neighbor
            adjacentTiles++;

        if (column < gridSizeX - 1 && mazeGrid[column + 1, row] > 0) // Check right neighbor
            adjacentTiles++;

        return adjacentTiles == 1; // Return true if there is exactly one adjacent tile to the expansion position, indicating a valid expansion
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Vector3 gridSize = new Vector3(gridSizeX * tileSize, 0f, gridSizeZ * tileSize);
        Gizmos.DrawWireCube(gridSize / 2f, gridSize);
    }

}

