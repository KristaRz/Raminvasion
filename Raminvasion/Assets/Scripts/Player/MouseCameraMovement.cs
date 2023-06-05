using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _CameraLookAt;
    [SerializeField] private float _MaxVerticalAngle;
    [SerializeField] private float _Sensitivity;

    private float _mouseVerticalValue;
    private float mouseVerticalValue
    {
        get => _mouseVerticalValue;
        set
        {
            if (value == 0) return;

            float verticalAngle = _mouseVerticalValue + value;
            verticalAngle = Mathf.Clamp(verticalAngle, -_MaxVerticalAngle, _MaxVerticalAngle);
            _mouseVerticalValue = verticalAngle;
        }
    }

    private float _mouseHorizontalValue;
    private float mouseHorizontalValue
    {
        get => _mouseHorizontalValue;
        set
        {
            if (value == 0) return;

            float horizontalValue = _mouseHorizontalValue + value;
            _mouseHorizontalValue = horizontalValue;
        }
    }

    private void Update()
    {
        mouseVerticalValue = Input.GetAxis("Mouse Y");
        mouseHorizontalValue = Input.GetAxis("Mouse X");

        transform.localRotation = Quaternion.Euler(-mouseVerticalValue * _Sensitivity, mouseHorizontalValue * _Sensitivity, 0); ;
       // _CameraFollow.localRotation = Quaternion.Euler(-mouseVerticalValue * _Sensitivity, 0, 0);
    }
}
