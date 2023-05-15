using UnityEngine;

public class OneLane : MonoBehaviour
{
    public GameObject box;
    public GameObject col;
    public int arrayWidth = 6;
    public int arrayDepth = 30;
    public float cubeSpacing = 1.0f; // new variable for cube spacing

    private void Start()
    {
        // Add ContinueLanes component to col object
        ContinueLanes continueLanes = col.AddComponent<ContinueLanes>();
    }
    void Update()
    {
        for (int i = 0; i < arrayWidth; i++)
            for (int j = 0; j < arrayDepth; j++)
            {
                Instantiate(box, new Vector3(i * cubeSpacing,0, j * cubeSpacing), Quaternion.identity);
                if(i == arrayWidth)
                    Instantiate(col, new Vector3(i * cubeSpacing, 0, j * cubeSpacing), Quaternion.identity);

            }
    }

    public class ContinueLanes : MonoBehaviour
    {
        private int addArrayDepth=5;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                int oneLane = GameObject.FindObjectOfType<OneLane>().arrayDepth;
                oneLane = +addArrayDepth;


            }
        }
    }
}

