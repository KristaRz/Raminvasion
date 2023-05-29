/*Kaan Koçak              Procedural Level Generation Elective                 May 2023/
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



using System.Collections;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
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

    private void RegenerateMaze()
    {
        if (mazeObject != null)
        {
            Destroy(mazeObject);
        }

        mazeObject = new GameObject("Maze");

        StartCoroutine(GenerateMaze(mazeObject.transform));
    }
    private IEnumerator GenerateMaze(Transform parent)
    {
        mazeGrid = new GameObject[gridSizeX, gridSizeZ];

        // Randomly create a tile on the first row
        int startColumn = Random.Range(0, gridSizeX);
        mazeGrid[startColumn, 0] = Instantiate(tilePrefab, new Vector3(startColumn * tileSize, 0f, 0f), Quaternion.identity, parent);

        // Set the initial direction
        Vector2Int direction = Vector2Int.up;

        int currentRow = 0;
        int currentColumn = startColumn;

        bool firstPathGenerated = false; // Flag to track if the first path has been generated

        while (currentRow < gridSizeZ - 1)
        {
            // Decide the number of adjacent tiles
            int numAdjacentTiles = Random.Range(minAdjacentTiles, maxAdjacentTiles + 1);

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
                    direction = Random.value < 0.5f ? Vector2Int.left : Vector2Int.right;
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

                // Check if the next position goes beyond the array boundaries on the front direction
                if (newRow >= gridSizeZ - 1)
                {
                    // Adjust the direction to fit within the boundaries
                    direction = (currentColumn == 0) ? Vector2Int.right : Vector2Int.left;
                }

                // Instantiate the tile at the new position
                GameObject newTile = Instantiate(tilePrefab, new Vector3(newColumn * tileSize, 0f, newRow * tileSize), Quaternion.identity, parent);

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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Vector3 gridSize = new Vector3(gridSizeX * tileSize, 0f, gridSizeZ * tileSize);
        Gizmos.DrawWireCube(gridSize / 2f, gridSize);
    }




}