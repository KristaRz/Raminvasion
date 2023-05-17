// Derived from CharacterControllerSmooth https://assetstore.unity.com/packages/tools/physics/character-controller-smooth-173259

using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private PhysicalCC physicalCC;

    [SerializeField] private Transform _CameraFollow;
    [SerializeField] private float _MaxVerticalAngle;
    [SerializeField] private float _Sensitivity;

    void Update()
	{
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            speed = 20;
        if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            speed = 10;

        if (physicalCC.isGround)
		{
			physicalCC.moveInput = Vector3.ClampMagnitude(transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal"), 1f) * speed;

			if (Input.GetKeyDown(KeyCode.Space))
			{
				physicalCC.inertiaVelocity.y = 0f;
				physicalCC.inertiaVelocity.y += jumpHeight;
			}
		}

        mouseVerticalValue = Input.GetAxis("Mouse Y");
        mouseHorizontalValue = Input.GetAxis("Mouse X");

        Quaternion lookRotation = Quaternion.Euler(0, mouseHorizontalValue * _Sensitivity, 0);
        Quaternion CameraHeightRotation = Quaternion.Euler( -mouseVerticalValue * _Sensitivity, 0, 0);

        transform.localRotation = lookRotation;
        _CameraFollow.localRotation = CameraHeightRotation;

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }



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

}
