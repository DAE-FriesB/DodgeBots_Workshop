using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField]
	private CharacterController _characterControl;

	[Header("Input")]
	[SerializeField]
	private InputActionReference _moveAction = null;
	[SerializeField]
	private InputActionReference _jumpAction = null;

	//Movement
	[Header("Movement")]
	[SerializeField]
	private float _moveSpeed = 3f;
	[SerializeField]
	private float _jumpSpeed = 10f;
	[SerializeField]
	private float _rotationSpeed = 360f;


	//Private fields
	private float _verticalSpeed = 0f;
	private bool _jumpPressed = false;
	private Vector3 _moveDirection;


	public void Start()
	{
		_moveDirection = transform.forward;
		_moveAction.action.Enable();
		_jumpAction.action.Enable();

		_jumpAction.action.performed += Jump_Performed;
	}

	private void Jump_Performed(InputAction.CallbackContext obj)
	{
		_jumpPressed = true;
	}

	private void FixedUpdate()
	{
		// Gravity
		ApplyGravity();

		// Get Move input
		Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();

		// Translate input to movement
		if (moveInput.magnitude > 0f)
		{
			Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
			_characterControl.Move(moveDirection * _moveSpeed * Time.deltaTime);
			_moveDirection = moveDirection;
		}

		// Handle Jump
		if (_jumpPressed)
		{
			if (_characterControl.isGrounded)
			{
				_verticalSpeed = _jumpSpeed;
			}
			_jumpPressed = false;
		}


		// Auto rotate
		Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);

	}



	void ApplyGravity()
	{
		_verticalSpeed += Time.fixedDeltaTime * Physics.gravity.y;
		_characterControl.Move(_verticalSpeed * Time.fixedDeltaTime * Vector3.up);
		if (_characterControl.isGrounded)
		{
			_verticalSpeed = 0f;
		}
	}
}
