//---------------------------------------------------------------------------------------------
// Prof. Dr. Frank Gabler, University of Applied Sciences Darmstadt, Unity3D Elective WS17/18
// Created: 20-11-17
// Last Update: 20-05-21
//---------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmothCameraFollowV2 : MonoBehaviour {
	// The target we are following
	public Transform target;
	// The distance in the x-z plane to the target
	public float distance = 10.0f;
	// the height we want the camera to be above the target
	public float height = 5.0f;
	// How much we 
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	public bool ToogleView = false;


	void LateUpdate () {

		// Early out if we don't have a target
		if (!target) return;

		// Calculate the current rotation angles
		float wantedRotationAngle = target.eulerAngles.y;
		float wantedHeight = target.position.y + height;

		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

		// Damp the rotation around the y-axis
		if (ToogleView)
		{
			// look from the front
			currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle + 180.0f, rotationDamping * Time.deltaTime);
		}
		else
		{
			// look from the back
			currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		}
		
		// Damp the height
		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		// Convert the angle into a rotation. Quaternions are used to represent rotations.
		// They are based on complex numbers!
		// A feature of quaternions is that multiplication of two quaternions is noncommutative. 
		// Hamilton defined a quaternion as the quotient of two directed lines in a three-dimensional space[3] or equivalently as the quotient of two vectors.[4]
		// Quaternions are generally represented in the form: a + bi + cj + dk
		// where a, b, c, and d are real numbers, and i, j, and k are the fundamental quaternion units.
		// Quaternion.Euler() returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).
		var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;

		// Set the height of the camera
		transform.position = new Vector3(transform.position.x,currentHeight,transform.position.z);

		// Always look at the target
		transform.LookAt(target);
	}
}
