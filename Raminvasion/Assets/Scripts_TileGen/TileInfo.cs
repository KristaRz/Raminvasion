using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum TileDirection{ 
    Vertical, Horizontal,
    RightBack, BackLeft, LeftFront ,FrontRight,
    LeftFrontRight, FrontRightBack, RightBackLeft, BackLeftFront, 
    LeftDead,FrontDead, RightDead,
    Empty
}

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
    
    [SerializeField] private GameObject wallTop;
    [SerializeField] private GameObject wallRight;
    [SerializeField] private GameObject wallDown;
    [SerializeField] private GameObject wallLeft;

    public void DeclareTileDirection(TileDirection type){
        switch (type)
        {   
            //Straights
            case TileDirection.Vertical:
            DeclareTileType(TileType.Straight);
            break;

            case TileDirection.Horizontal:
            DeclareTileType(TileType.Straight);
            this.gameObject.transform.Rotate(0,90,0);
            break;

            //Curved
            case TileDirection.RightBack:
            DeclareTileType(TileType.Curved);
            break;

            case TileDirection.BackLeft:
            DeclareTileType(TileType.Curved);
            this.gameObject.transform.Rotate(0,90,0);
            break;

            case TileDirection.LeftFront:
            DeclareTileType(TileType.Curved);
            this.gameObject.transform.Rotate(0,180,0);
            break;

            case TileDirection.FrontRight:
            DeclareTileType(TileType.Curved);
            this.gameObject.transform.Rotate(0,270,0);
            break;

            //Forks
            case TileDirection.LeftFrontRight:
            DeclareTileType(TileType.Fork);
            break;

            case TileDirection.FrontRightBack:
            DeclareTileType(TileType.Fork);
            this.gameObject.transform.Rotate(0,90,0);
            break;

            case TileDirection.RightBackLeft:
            DeclareTileType(TileType.Fork);
            this.gameObject.transform.Rotate(0,180,0);
            break;

            case TileDirection.BackLeftFront:
            DeclareTileType(TileType.Fork);
            this.gameObject.transform.Rotate(0,270,0);
            break;

            //DeadEnds
            case TileDirection.LeftDead:
            DeclareTileType(TileType.DeadEnd);
            break;

            case TileDirection.FrontDead:
            DeclareTileType(TileType.DeadEnd);
            this.gameObject.transform.Rotate(0,90,0);
            break;

            case TileDirection.RightDead:
            DeclareTileType(TileType.DeadEnd);
            this.gameObject.transform.Rotate(0,180,0);
            break;
            
            case TileDirection.Empty:
            Destroy(this.gameObject);
            break;

            default:
            Debug.Log("Do nothing");;
            break;
        }
    }

    public void DeclareTileType(TileType type){
        switch(type)
        {   
            //default Vertical
            case TileType.Straight:
            wallTop.SetActive(false);
            wallDown.SetActive(false);
            wallLeft.SetActive(true);
            wallRight.SetActive(true);
            break;

            //default RightBack
            case TileType.Curved:
            wallRight.SetActive(false);
            wallDown.SetActive(false);
            wallTop.SetActive(true);
            wallLeft.SetActive(true);
            break;
            
            //default LeftFrontRight
            case TileType.Fork:
            wallRight.SetActive(false);
            wallDown.SetActive(true);
            wallTop.SetActive(false);
            wallLeft.SetActive(false);
            break;

            //default LeftDead
            case TileType.DeadEnd:
            wallDown.SetActive(true);
            wallRight.SetActive(false);
            wallTop.SetActive(true);
            wallLeft.SetActive(true);
            break;
            
        }
    }

    private void Start() {
        DeclareTileDirection(tileDirection);  
        tiledirectionCount=System.Enum.GetNames(typeof(TileDirection)).Length;
    }

    private void OnMouseDown() {
        
        if((int)tileDirection==tiledirectionCount-1){
            tileDirection=0;
        }
        else{
            tileDirection++;
        }
        

        DeclareTileDirection(tileDirection);  

        Debug.Log("Change type to: "+tileDirection);
        
    }

}