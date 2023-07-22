using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContRotation : MonoBehaviour
{
    public float rotationSpeed = 0.5f;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
