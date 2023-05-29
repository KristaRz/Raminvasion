using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGrid : MonoBehaviour
{   
    public int gridWidth=12;
    public int gridLength=12;

    public GameObject block;
    
    
   private void Awake() {
    
    //--------< Grid in Grid >--
    for (int i = 0; i < gridWidth; i++)
    {
        for (int j = 0; j < gridLength; j++)
        {
            Instantiate(block, new Vector3(i*block.transform.localScale.x,-0.1f,j*block.transform.localScale.z), Quaternion.identity,this.gameObject.transform);
            block.name="EmptyBlock R"+i+"C"+j;
            
        }     
    } 
   }


}
