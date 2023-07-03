using UnityEngine;

public class Tiles : MonoBehaviour
{
    public GameObject leftRightBackFront;
    public GameObject backFront;
    public GameObject backLeft;
    public GameObject backRight;
    public GameObject leftFront;
    public GameObject leftRight;
    public GameObject rightFront;
    public GameObject backFrontRight;
    public GameObject backLeftFront;
    public GameObject backLeftRight;
    public GameObject leftFrontRight;
    public GameObject left;
    public GameObject right;
    public GameObject front;
    public GameObject back;

    private void OnDisable()
    {
        ActivateChildObjects(false);
    }

    private void ActivateChildObjects(bool active)
    {
        GameObject[] childObjects = new GameObject[]
        {
            leftRightBackFront,
            backFront,
            backLeft,
            backRight,
            leftFront,
            leftRight,
            rightFront,
            backFrontRight,
            backLeftFront,
            backLeftRight,
            leftFrontRight,
            left,
            right,
            front,
            back
        };

        foreach (GameObject childObject in childObjects)
        {
            if (childObject != null)
                childObject.SetActive(active);
        }
    }
}
