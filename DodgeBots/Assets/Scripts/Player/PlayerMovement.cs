using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
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
	[SerializeField]
	private float _gravityMultiplier = 1f;

	//Private fields
	private float _verticalSpeed = 0f;
	private bool _jumpPressed = false;
	private Vector3 _moveDirection;


	public void Awake()
	{
		_moveDirection = transform.forward;

	}



	public override void OnNetworkSpawn()
	{
		if (IsOwner)
		{
			_moveAction.action.Enable();
			_jumpAction.action.Enable();
			_jumpAction.action.performed += Jump_Performed;


			GetComponent<PlayerCameraAttach>().enabled = true;
		}

		if (!IsOwnedByServer)
		{
			GetComponent<PlayerTeamColor>().SetPlayerColor(PlayerTeamColor.TeamColor.Red);
		}
	}


	public override void OnNetworkDespawn()
	{
		if (IsOwner)
		{
			_jumpAction.action.performed -= Jump_Performed;
		}

	}

	private void Jump_Performed(InputAction.CallbackContext obj)
	{
		RequestJumpRpc();
	}

	private void FixedUpdate()
	{
		if (IsOwner)
		{
			// Get Move input
			Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();

			// Translate input to movement
			if (moveInput.magnitude > 0f)
			{
				RequestMoveRpc(moveInput);
			}

		}

		if (IsServer)
		{
			// Gravity
			ApplyGravity();

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
	}

	[Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
	void RequestMoveRpc(Vector2 moveInput)
	{
		Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
		_characterControl.Move(moveDirection * _moveSpeed * Time.fixedDeltaTime);
		_moveDirection = moveDirection;
	}

	[Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
	void RequestJumpRpc()
	{
		_jumpPressed = true;
	}


	void ApplyGravity()
	{
		_verticalSpeed += Time.fixedDeltaTime * Physics.gravity.y * _gravityMultiplier;
		_characterControl.Move(_verticalSpeed * Time.fixedDeltaTime * Vector3.up);
		if (_characterControl.isGrounded)
		{
			_verticalSpeed = 0f;
		}
	}
}
