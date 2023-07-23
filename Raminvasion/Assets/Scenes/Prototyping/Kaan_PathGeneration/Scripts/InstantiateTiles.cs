using System.Collections;
using UnityEngine;

public class InstantiateTiles : MonoBehaviour
{
    public GameObject leftRightBackFront;
    public GameObject backFrontPrefab;
    public GameObject backLeftPrefab;
    public GameObject backRightPrefab;
    public GameObject leftFrontPrefab;
    public GameObject leftRightPrefab;
    public GameObject rightFrontPrefab;
    public GameObject backFrontRight;
    public GameObject backLeftFront;
    public GameObject backLeftRight;
    public GameObject leftFrontRight;
    public GameObject left;
    public GameObject right;
    public GameObject front;
    public GameObject back;

    public void GenerateTiles(Transform parent, int[,] mazeGrid, int gridSizeX, int gridSizeZ, float tileSize, float delayBetweenSteps)
    { StartCoroutine(InstantiateTilesCoroutine(parent, mazeGrid, gridSizeX, gridSizeZ, tileSize, delayBetweenSteps)); }

    private IEnumerator InstantiateTilesCoroutine(Transform parent, int[,] mazeGrid, int gridSizeX, int gridSizeZ, float tileSize, float delayBetweenSteps)
    {
        for (int row = 0; row < gridSizeZ; row++)
        {
            for (int column = 0; column < gridSizeX; column++)
            {
                if (mazeGrid[column, row] > 0)
                {
                    // Determine the presence of neighboring tiles
                    bool hasBack = row < gridSizeZ - 1 && mazeGrid[column, row + 1] > 0;
                    bool hasFront = row > 0 && mazeGrid[column, row - 1] > 0;
                    bool hasLeft = column > 0 && mazeGrid[column - 1, row] > 0;
                    bool hasRight = column < gridSizeX - 1 && mazeGrid[column + 1, row] > 0;

                    Vector3 tilePosition = new Vector3(column * tileSize, 0f, row * tileSize); // Calculate the position of the tile

                    // Determine the appropriate tile prefab based on the neighboring tiles
                    if (row == 0 || row == gridSizeZ - 1)
                        Instantiate(backFrontPrefab, tilePosition, Quaternion.identity, parent);
                    else if (hasRight && hasLeft && hasFront && hasBack)
                        Instantiate(leftRightBackFront, tilePosition, Quaternion.identity, parent);
                    else if (hasRight && hasLeft && hasFront)
                        Instantiate(leftFrontRight, tilePosition, Quaternion.identity, parent);
                    else if (hasRight && hasLeft && hasBack)
                        Instantiate(backLeftRight, tilePosition, Quaternion.identity, parent);
                    else if (hasFront && hasLeft && hasBack)
                        Instantiate(backFrontRight, tilePosition, Quaternion.identity, parent);
                    else if (hasRight && hasFront && hasBack)
                        Instantiate(backLeftFront, tilePosition, Quaternion.identity, parent);
                    else if (hasBack && hasFront)
                        Instantiate(backFrontPrefab, tilePosition, Quaternion.identity, parent);
                    else if (hasBack && hasLeft)
                        Instantiate(backRightPrefab, tilePosition, Quaternion.identity, parent);
                    else if (hasBack && hasRight)
                        Instantiate(backLeftPrefab, tilePosition, Quaternion.identity, parent);
                    else if (hasLeft && hasFront)
                        Instantiate(rightFrontPrefab, tilePosition, Quaternion.identity, parent);
                    else if (hasLeft && hasRight)
                        Instantiate(leftRightPrefab, tilePosition, Quaternion.identity, parent);
                    else if (hasRight && hasFront)
                        Instantiate(leftFrontPrefab, tilePosition, Quaternion.identity, parent);
                    else if (hasFront)
                        Instantiate(front, tilePosition, Quaternion.identity, parent);
                    else if (hasBack)
                        Instantiate(back, tilePosition, Quaternion.identity, parent);
                    else if (hasLeft)
                        Instantiate(right, tilePosition, Quaternion.identity, parent);
                    else if (hasRight)
                        Instantiate(left, tilePosition, Quaternion.identity, parent);
                }
            }

            yield return new WaitForSeconds(delayBetweenSteps); // Wait for the specified delay between each step
        }
    }
}
