// Defines the tile directions 

using System.Collections.Generic;


public static class MazeTileDeclaration
{

    public static List<TileInformation> PositionTiles(int[,] mazeGrid, int gridSizeX, int gridSizeZ, int tileSize)
    {
        List<TileInformation> continuousMazeDirections = new();

        for (int row = 0; row < gridSizeZ; row++)
        {
            for (int column = 0; column < gridSizeX; column++)
            {
                if (mazeGrid[column, row] > 0)
                {
                    bool hasBack = row < gridSizeZ - 1 && mazeGrid[column, row + 1] > 0;
                    bool hasFront = row > 0 && mazeGrid[column, row - 1] > 0;
                    bool hasLeft = column > 0 && mazeGrid[column - 1, row] > 0;
                    bool hasRight = column < gridSizeX - 1 && mazeGrid[column + 1, row] > 0;

                    TileInformation newTile = new TileInformation(column, row , 0, TileDirection.Vertical);  // create a tile 

                    /*
                    Vector3 tilePosition = new Vector3(column * tileSize, 0f, row * tileSize);

                    GameObject tile = ObjectPool.Instance.GetTile();
                    tile.SetActive(true);
                    tile.transform.position = tilePosition;
                    tile.transform.rotation = Quaternion.identity;

                    TileInfo TileInfo = tile.GetComponent<TileInfo>();
                    */

                    if (row == 0 || row == gridSizeZ - 1)
                    {
                        newTile.Direction = TileDirection.Vertical;
                    }
                    //else if (hasRight && hasLeft && hasFront && hasBack)
                    //{
                    //    TileInfo.DeclareTileDirection(TileDirection.LeftFrontRight);
                    //}
                    else if (hasRight && hasLeft && hasFront)
                    {
                        newTile.Direction = TileDirection.RightBackLeft;//
                    }
                    else if (hasRight && hasLeft && hasBack)
                    {
                        newTile.Direction = TileDirection.LeftFrontRight;
                    }
                    else if (hasFront && hasLeft && hasBack)
                    {
                        newTile.Direction = TileDirection.BackLeftFront;//
                    }
                    else if (hasRight && hasFront && hasBack)
                    {
                        newTile.Direction = TileDirection.FrontRightBack;
                    }
                    else if (hasBack && hasFront)
                    {
                        newTile.Direction = TileDirection.Vertical;
                    }
                    else if (hasBack && hasLeft)
                    {
                        newTile.Direction = TileDirection.LeftFront;
                    }
                    else if (hasBack && hasRight)
                    {
                        newTile.Direction = TileDirection.RightFront;
                    }
                    else if (hasLeft && hasFront)
                    {
                        newTile.Direction = TileDirection.BackLeft;
                    }
                    else if (hasLeft && hasRight)
                    {
                        newTile.Direction = TileDirection.Horizontal;
                    }
                    else if (hasRight && hasFront)
                    {
                        newTile.Direction = TileDirection.BackRight;
                    }
                    else if (hasFront)
                    {
                        newTile.Direction = TileDirection.FrontDead;
                        newTile.Area=TileArea.DeadEnd;
                    }
                    else if (hasBack)
                    {   
                        //just added this for completion
                        newTile.Direction = TileDirection.BackDead;
                        newTile.Area=TileArea.DeadEnd;  
                    }
                    else if (hasLeft)
                    {
                        newTile.Direction = TileDirection.RightDead; // Swap LeftDead with RightDead
                        newTile.Area=TileArea.DeadEnd;
                    }
                    else if (hasRight)
                    {
                        newTile.Direction = TileDirection.LeftDead; // Swap RightDead with LeftDead
                        newTile.Area=TileArea.DeadEnd;
                    }

                    continuousMazeDirections.Add(newTile);
                }
            }
        }

        return continuousMazeDirections;
    }


    #region Jules declaration

    /*

    public static void ChangePreviousTile(GameObject tile, Vector2Int direction, int adjacentTilesIndex){

        //Only for Curves with previous direction.y>0, Changing in next Maze-Generation step bcs both curve direction are needed
        if(adjacentTilesIndex==0){

            //Going forward and turning right
            if(direction.x>0){
                tile.GetComponent<TileInfo>().DeclareTileDirection(TileDirection.BackRight);
            }
            //Going forward and turning left
            else if(direction.x<0){
                tile.GetComponent<TileInfo>().DeclareTileDirection(TileDirection.BackLeft);
            }
        }
    }

    public static void DeclareAdjacentTiles(GameObject tile, Vector2Int direction, int adjacentTilesIndex, int numAdjacentTiles){

        //Curves based on maze-pattern to go Vector2Int.up again after last horizontal adjacent Tile
        if(adjacentTilesIndex==(numAdjacentTiles-1)){

            //Going right turning up
            if(direction.x>0){
                tile.GetComponent<TileInfo>().DeclareTileDirection(TileDirection.LeftFront);
  
            }
            //Going left turning up
            else if(direction.x<0){
                tile.GetComponent<TileInfo>().DeclareTileDirection(TileDirection.RightFront);
                
            }
        }

        //Straight Tiles
        else {
            if(direction.x!=0){
            tile.GetComponent<TileInfo>().DeclareTileDirection(TileDirection.Horizontal);
            }
            else if(direction.y>0){
            tile.GetComponent<TileInfo>().DeclareTileDirection(TileDirection.Vertical);
            }
            
        } 
    }
    */

    #endregion


    #region Krista merge

    /*

    /////////////////////////////////////////////////////////////
    //////////// Tile declaration for MazeGenerator /////////////
    /////////////////////////////////////////////////////////////

    // 1 Vertical, 2 Horizontal,
    // BackRight, BackLeft, LeftFront, RightFront,
    // LeftFrontRight, FrontRightBack, RightBackLeft, BackLeftFront, 
    // LeftDead, FrontDead, RightDead

    // Declares the directions(.Direction) of the tiles back to front based on the general direction(1 or 2 in .TileDirectionIndex / .Vertical or .Horizontal in .Direction)

    public static List<TileInformation> DeclareTileTypes(List<TileInformation> continuousMazeDirections, TileInformation lastTile, bool firstMazeGenerated)
    {
        List<TileInformation> continousMaze = continuousMazeDirections;

        for (int i = continousMaze.Count - 1; i >= 0; i--)
        {
            TileInformation currentTile = continousMaze[i];
            TileInformation previousTile;
            if (i != 0) // for all normal tiles we use the previous tile to calculate
                previousTile = continousMaze[i - 1];
            else if (lastTile != continousMaze[continousMaze.Count - 1])
            {
                // NOTE: actually i'm not sure what i'm doing here or with the next if, this might be wrong lol
                // I think I was trying to make sure that the subsequent mazes have a current first tile, which can be a bit weird because i copy it from maze to maze
                previousTile = lastTile;
            }
            else
                previousTile = new TileInformation(currentTile.IndexX, currentTile.IndexZ - 1, currentTile.TileDirectionIndex, currentTile.Direction, currentTile.Area); // i think this is for subsequent mazes

            // The last tile gets calculated and then returned
            if (i == continousMaze.Count - 1)
            {
                if (previousTile.Direction != TileDirection.Vertical) // IN is not Vertical
                {
                    if (currentTile.IndexX < previousTile.IndexX)    // if IN is right
                        currentTile.Direction = TileDirection.BackRight;
                    else   // if IN is left
                        currentTile.Direction = TileDirection.BackLeft;
                }
                else // the tile is already this, but basically we leave it at vertical
                    currentTile.Direction = TileDirection.Vertical;
            }
            else if (i == 0)
            {
                if (firstMazeGenerated)
                    return continousMaze;
                if (currentTile.Direction == TileDirection.Horizontal)
                {
                    if (continousMaze[i + 1].IndexX < currentTile.IndexX)    // if OUT is left
                        currentTile.Direction = TileDirection.BackLeft;
                    else   // if OUT is right
                        currentTile.Direction = TileDirection.BackRight;
                }
                else
                    currentTile.Direction = TileDirection.Vertical;
            }
            else
            {
                switch (previousTile.Direction)
                {
                    case TileDirection.Vertical: // IN tile was vertical
                        if (currentTile.Direction == TileDirection.Horizontal) // IN is not the same as OUT
                        {
                            if (continousMaze[i + 1].IndexX < currentTile.IndexX)    // if OUT is left
                                currentTile.Direction = TileDirection.BackLeft;
                            else   // if OUT is right
                                currentTile.Direction = TileDirection.BackRight;
                        }
                        else
                            currentTile.Direction = TileDirection.Vertical;
                        break;
                    case TileDirection.Horizontal: // IN tile was horizontal
                        if (currentTile.Direction == TileDirection.Vertical)    // IN is not the same as OUT
                        {
                            if (previousTile.IndexX < currentTile.IndexX) // IN is from left
                                currentTile.Direction = TileDirection.LeftFront;
                            else   // else IN is from right
                                currentTile.Direction = TileDirection.RightFront;
                        }
                        else
                            currentTile.Direction = TileDirection.Horizontal;
                        break;
                }
            }
        }

        return continousMaze;
    }



    // 1 Vertical, 2 Horizontal,
    // BackRight, BackLeft, LeftFront, RightFront,
    // LeftFrontRight, FrontRightBack, RightBackLeft, BackLeftFront, 
    // LeftDead, FrontDead, RightDead

    // Declares the tile directions for the deadends for each single street to deadend. 
    public static List<TileInformation> DeclareDeadEndTypes(List<TileInformation> deadendTiles, List<TileInformation> continuousMazeDirections)
    {
        List<TileInformation> continousMaze = continuousMazeDirections;
        // For loops goes until 1 because we leave out the last one in the list which is the tile belonging to the maze
        for (int i = deadendTiles.Count - 1; i >= 1; i--)
        {
            TileInformation currentTile = deadendTiles[i];
            TileInformation previousTile;
            if (i == 1)
                previousTile = deadendTiles[0];
            else
                previousTile = deadendTiles[i - 1];

            // First do the deadend itself
            if (i == deadendTiles.Count - 1)
            {
                if (previousTile.IndexX < currentTile.IndexX)    // if IN is left
                    currentTile.Direction = TileDirection.RightDead;
                else if (previousTile.IndexX > currentTile.IndexX)    // if IN is right
                    currentTile.Direction = TileDirection.LeftDead;
                else if (previousTile.IndexZ > currentTile.IndexZ)    // if IN is below
                    currentTile.Direction = TileDirection.FrontDead;
                
                //better for it to be in MazeGenerator i think
                currentTile.Area=TileArea.DeadEnd;
            }
            else // then the rest of the tiles
            {
                switch (previousTile.Direction)
                {
                    case TileDirection.Vertical: // IN tile was vertical
                        if (currentTile.Direction == TileDirection.Horizontal) // IN is not the same as OUT
                        {
                            if (continousMaze[i + 1].IndexX < currentTile.IndexX)    // if OUT is left
                                currentTile.Direction = TileDirection.BackLeft;
                            else   // if OUT is right
                                currentTile.Direction = TileDirection.BackRight;
                        }
                        else
                            currentTile.Direction = TileDirection.Vertical;
                        break;
                    case TileDirection.Horizontal: // IN tile was horizontal
                        if (currentTile.Direction == TileDirection.Vertical)    // IN is not the same as OUT
                        {
                            if (previousTile.IndexX < currentTile.IndexX) // IN is from left
                                currentTile.Direction = TileDirection.LeftFront;
                            else   // else IN is from right
                                currentTile.Direction = TileDirection.RightFront;
                        }
                        else
                            currentTile.Direction = TileDirection.Horizontal;
                        break;
                }
            }
            continousMaze.Add(currentTile);
        }

        // Then we do the tile that belongs to the maze

        TileInformation currentLastTile = deadendTiles[0];
        TileInformation afterTile = deadendTiles[1];
        switch (currentLastTile.Direction)
        {
            // Current Vertical / Horizontal // 
            case TileDirection.Vertical:
                if (currentLastTile.IndexX < afterTile.IndexX)  // OUT is right
                    currentLastTile.Direction = TileDirection.FrontRightBack;
                else  // OUT is left
                    currentLastTile.Direction = TileDirection.BackLeftFront;
                break;

            case TileDirection.Horizontal: // Horizontal can only go up
                currentLastTile.Direction = TileDirection.LeftFrontRight;
                break;
            // Current Curved // 
            case TileDirection.BackRight:
                if (currentLastTile.IndexX > afterTile.IndexX)  // OUT is left
                    currentLastTile.Direction = TileDirection.RightBackLeft;
                else if (currentLastTile.IndexZ < afterTile.IndexZ)  // OUT is front
                    currentLastTile.Direction = TileDirection.FrontRightBack;
                break;

            case TileDirection.BackLeft:
                if (currentLastTile.IndexX < afterTile.IndexX)  // OUT is right
                    currentLastTile.Direction = TileDirection.RightBackLeft;
                else if (currentLastTile.IndexZ < afterTile.IndexZ)  // OUT is front
                    currentLastTile.Direction = TileDirection.FrontRightBack;
                break;

            case TileDirection.LeftFront:
                if (currentLastTile.IndexX < afterTile.IndexX)  // OUT is right
                    currentLastTile.Direction = TileDirection.LeftFrontRight;
                break;

            case TileDirection.RightFront:
                if (currentLastTile.IndexX > afterTile.IndexX)  // OUT is left
                    currentLastTile.Direction = TileDirection.LeftFrontRight;
                break;

            default:
                Debug.Log("Do nothing"); ;
                break;
        }
        // In the end we need to remove it from the list bcs we don't want to instantiate it twice.
        deadendTiles.RemoveAt(0);
        return continousMaze;
    }

    */

    #endregion

}
