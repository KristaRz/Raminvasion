using UnityEngine;

public class OneLane : MonoBehaviour
{
    public GameObject box;
    public GameObject col;
    public int arrayWidth = 6;
    public int arrayDepth = 30;
    public float cubeSpacing = 1.0f; // new variable for cube spacing

<<<<<<< HEAD:Raminvasion/Assets/OneLane.cs
=======

>>>>>>> maze:Raminvasion/Assets/Scripts/LaneExtension/OneLane.cs
    private void Start()
    {
        GenerateLane();
    }

    private void GenerateLane()
    {
        for (int i = 0; i < arrayWidth; i++)
        {
            for (int j = 0; j < arrayDepth; j++)
            {
                if (j < arrayDepth)
                {
                    Instantiate(box, new Vector3(i * cubeSpacing, 0, j * cubeSpacing), Quaternion.identity);
                    col.transform.position = new Vector3(0, 1, j * cubeSpacing);
                }
            }
        }
    }

    public void ExtendLane(int extensionAmount)
    {
        arrayDepth += extensionAmount;
        GenerateLane();
<<<<<<< HEAD:Raminvasion/Assets/OneLane.cs

        Debug.Log("Lane extended by " + extensionAmount);
    }
}
=======
>>>>>>> maze:Raminvasion/Assets/Scripts/LaneExtension/OneLane.cs

        Debug.Log("Lane extended by " + extensionAmount);
    }
}
