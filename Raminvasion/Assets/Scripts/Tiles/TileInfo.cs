// Created by Julia Podlipensky
//> Stores Information about the Instantiated Tile
//> Functions for Changing Tile-Visuals according to needs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Type of Area this Tile belongs to
public enum TileArea{
    MainPath,
    DeadEnd
}

//Type of Direction of Tile (the directions signal which walls are collapsed)
public enum TileDirection{ 
    Vertical, Horizontal,
    BackRight, BackLeft, LeftFront ,RightFront,
    LeftFrontRight, FrontRightBack, RightBackLeft, BackLeftFront, 
    LeftDead,FrontDead, RightDead, BackDead,
    Empty
}

//Tile Base Type 
public enum TileType{
    Straight,
    Curved,
    Fork,
    DeadEnd
}




public class TileInfo : MonoBehaviour
{   
    
    public TileType tileType;

    public TileDirection tileDirection;

    int tiledirectionCount;
    
    [Header("GameObjects on Tile to be activated/deactivated")]
    [SerializeField] private GameObject wallTop;
    [SerializeField] private GameObject wallRight;
    [SerializeField] private GameObject wallDown;
    [SerializeField] private GameObject wallLeft;

    [SerializeField] private GameObject ramensStall;

    

    //Declares the Direction Visuals of the Tile by picking TileType and rotating it accordingly
    public void DeclareTileDirection(TileDirection type){
        tileDirection=type;
        switch (type)
        {   
            //Straights
            case TileDirection.Vertical:
            DeclareTileType(TileType.Straight);
            this.gameObject.transform.eulerAngles=new Vector3(0,0,0);
            break;

            case TileDirection.Horizontal:
            DeclareTileType(TileType.Straight);
            this.gameObject.transform.eulerAngles=new Vector3(0,90,0);
            break;

            //Curved
            case TileDirection.BackRight:
            DeclareTileType(TileType.Curved);
            this.gameObject.transform.eulerAngles=new Vector3(0,0,0);
            break;

            case TileDirection.BackLeft:
            DeclareTileType(TileType.Curved);
            this.gameObject.transform.eulerAngles=new Vector3(0,90,0);
            break;

            case TileDirection.LeftFront:
            DeclareTileType(TileType.Curved);
            this.gameObject.transform.eulerAngles=new Vector3(0,180,0);
            break;

            case TileDirection.RightFront:
            DeclareTileType(TileType.Curved);
            this.gameObject.transform.eulerAngles=new Vector3(0,270,0);
            break;

            //Forks
            case TileDirection.LeftFrontRight:
            DeclareTileType(TileType.Fork);
            this.gameObject.transform.eulerAngles=new Vector3(0,0,0);
            break;

            case TileDirection.FrontRightBack:
            DeclareTileType(TileType.Fork);
            this.gameObject.transform.eulerAngles=new Vector3(0,90,0);
            break;

            case TileDirection.RightBackLeft:
            DeclareTileType(TileType.Fork);
            this.gameObject.transform.eulerAngles=new Vector3(0,180,0);
            break;

            case TileDirection.BackLeftFront:
            DeclareTileType(TileType.Fork);
            this.gameObject.transform.eulerAngles=new Vector3(0,270,0);
            break;

            //DeadEnds
            case TileDirection.LeftDead:
            DeclareTileType(TileType.DeadEnd);
            this.gameObject.transform.eulerAngles=new Vector3(0,0,0);
            break;

            case TileDirection.FrontDead:
            DeclareTileType(TileType.DeadEnd);
            this.gameObject.transform.eulerAngles=new Vector3(0,90,0);
            break;

            case TileDirection.RightDead:
            DeclareTileType(TileType.DeadEnd);
            this.gameObject.transform.eulerAngles=new Vector3(0,180,0);
            break;

            case TileDirection.BackDead:
            DeclareTileType(TileType.DeadEnd);
            this.gameObject.transform.eulerAngles=new Vector3(0,270,0);
            break;

            // default:
            // Debug.Log("Do nothing");
            // break;
        }
    }

    //Declares Tile BaseType
    public void DeclareTileType(TileType type){
        tileType=type;
        switch(type)
        {   
            //default Vertical
            case TileType.Straight:
            wallTop.SetActive(false);
            wallDown.SetActive(false);
            wallLeft.SetActive(true);
            wallRight.SetActive(true);
            ramensStall.SetActive(false);
            break;

            //default BackRight
            case TileType.Curved:
            wallRight.SetActive(false);
            wallDown.SetActive(false);
            wallTop.SetActive(true);
            wallLeft.SetActive(true);
            ramensStall.SetActive(false);
            break;
            
            //default LeftRightFront
            case TileType.Fork:
            wallRight.SetActive(false);
            wallDown.SetActive(true);
            wallTop.SetActive(false);
            wallLeft.SetActive(false);
            ramensStall.SetActive(false);
            break;

            //default LeftDead
            case TileType.DeadEnd:
            wallDown.SetActive(true);
            wallRight.SetActive(false);
            wallTop.SetActive(true);
            wallLeft.SetActive(true);
            ramensStall.SetActive(true);
            break;
            
        }
    }

}
