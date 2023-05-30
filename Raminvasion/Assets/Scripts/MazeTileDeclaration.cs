using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeTileDeclaration : MonoBehaviour
{
    public void ChangePreviousTile(GameObject tile, Vector2Int direction, int adjacentTilesIndex){

        //Only for Curves with previous direction.y>0, Changing in next Maze-Generation step bcs both curve direction are needed
        if(adjacentTilesIndex==0){

            //Going forward and turning right
            if(direction.x>0){
                tile.GetComponent<TileInfo>().DeclareTileDirection(TileDirection.RightBack);
            }
            //Going forward and turning left
            else if(direction.x<0){
                tile.GetComponent<TileInfo>().DeclareTileDirection(TileDirection.BackLeft);
            }
        }
    }

    public void DeclareAdjacentTiles(GameObject tile, Vector2Int direction, int adjacentTilesIndex, int numAdjacentTiles){

        //Curves based on maze-pattern to go Vector2Int.up again after last horizontal adjacent Tile
        if(adjacentTilesIndex==(numAdjacentTiles-1)){

            //Going right turning up
            if(direction.x>0){
                tile.GetComponent<TileInfo>().DeclareTileDirection(TileDirection.LeftFront);
  
            }
            //Going left turning up
            else if(direction.x<0){
                tile.GetComponent<TileInfo>().DeclareTileDirection(TileDirection.FrontRight);
                
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
}
