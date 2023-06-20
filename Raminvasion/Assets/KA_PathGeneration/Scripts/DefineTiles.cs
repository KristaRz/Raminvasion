using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;

public class DefineTiles : MonoBehaviour
{

    public void GenerateTiles(int[,] mazeGrid, int gridSizeX, int gridSizeZ, float tileSize)
    {
        PositionTiles(mazeGrid, gridSizeX, gridSizeZ, tileSize);
    }

    private void PositionTiles(int[,] mazeGrid, int gridSizeX, int gridSizeZ, float tileSize)
    {
        for (int row = 0; row < gridSizeZ; row++)
        {
            for (int column = 0; column < gridSizeX; column++)
            {
                if (mazeGrid[column, row] == 1)
                {
                    bool hasBack = row < gridSizeZ - 1 && mazeGrid[column, row + 1] == 1;
                    bool hasFront = row > 0 && mazeGrid[column, row - 1] == 1;
                    bool hasLeft = column > 0 && mazeGrid[column - 1, row] == 1;
                    bool hasRight = column < gridSizeX - 1 && mazeGrid[column + 1, row] == 1;

                    Vector3 tilePosition = new Vector3(column * tileSize, 0f, row * tileSize);

                    GameObject tile = ObjectPool.Instance.GetTile();
                    tile.SetActive(true);
                    tile.transform.position = tilePosition;
                    tile.transform.rotation = Quaternion.identity;

                    TileInfo TileInfo = tile.GetComponent<TileInfo>();

                    if (row == 0 || row == gridSizeZ - 1)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.Vertical);
                    }
                    //else if (hasRight && hasLeft && hasFront && hasBack)
                    //{
                    //    TileInfo.DeclareTileDirection(TileDirection.LeftFrontRight);
                    //}
                    else if (hasRight && hasLeft && hasFront)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.RightBackLeft);//
                    }
                    else if (hasRight && hasLeft && hasBack)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.LeftFrontRight  );
                    }
                    else if (hasFront && hasLeft && hasBack)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.BackLeftFront);//
                    }
                    else if (hasRight && hasFront && hasBack)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.FrontRightBack); 
                    }
                    else if (hasBack && hasFront)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.Vertical);
                    }
                    else if (hasBack && hasLeft)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.LeftFront);
                    }
                    else if (hasBack && hasRight)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.RightFront);
                    }
                    else if (hasLeft && hasFront)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.BackLeft);
                    }
                    else if (hasLeft && hasRight)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.Horizontal);
                    }
                    else if (hasRight && hasFront)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.BackRight);
                    }
                    else if (hasFront)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.FrontDead);
                    }
                    else if (hasBack)
                    {
                        //TileInfo.DeclareTileDirection(TileDirection.BackDead);
                    }
                    else if (hasLeft)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.RightDead); // Swap LeftDead with RightDead
                    }
                    else if (hasRight)
                    {
                        TileInfo.DeclareTileDirection(TileDirection.LeftDead); // Swap RightDead with LeftDead
                    }
                }
            }
        }
    }
}
