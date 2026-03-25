using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupBall : NetworkBehaviour
{
	private BallScript _ballToPickup;

	[SerializeField]
	private InputActionReference _pickupAction;

	[SerializeField]
	private Transform _ballAnchor;
	private FollowTransform _followTransform;

	public BallScript GrabbedBall { get; private set; }


	private NetworkVariable<ulong> _grabbedBallVariable = new NetworkVariable<ulong>(ulong.MaxValue);

	private void Awake()
	{
		_followTransform = _ballAnchor.AddComponent<FollowTransform>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Ball"))
		{
			_ballToPickup = other.GetComponent<BallScript>();
		}
	}

	public override void OnNetworkSpawn()
	{
		if (IsOwner)
		{
			_pickupAction.action.Enable();
			_pickupAction.action.performed += Action_performed;
		}

		_grabbedBallVariable.OnValueChanged += GrabbedBall_Changed;

		//Late joining: 
		if (_grabbedBallVariable.Value != ulong.MaxValue)
		{
			PickupBallObject(_grabbedBallVariable.Value);
		}
	}

	private void GrabbedBall_Changed(ulong previousValue, ulong newValue)
	{
		PickupBallObject(newValue);
	}

	public override void OnNetworkDespawn()
	{
		if (IsOwner)
		{
			_pickupAction.action.performed -= Action_performed;
		}
	}

	private void Action_performed(InputAction.CallbackContext context)
	{
		if (_ballToPickup != null)
		{
			RequestPickupRpc(_ballToPickup.NetworkObjectId);
		}
	}

	[Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
	private void RequestPickupRpc(ulong objectId)
	{
		BallScript ballToPickup = NetworkManager.SpawnManager.SpawnedObjects[objectId]
										.GetComponent<BallScript>();
		//Server side validation
		//Check distance of player to ball

		_grabbedBallVariable.Value = objectId;

	}

	void PickupBallObject(ulong objectId)
	{

		BallScript ballToPickup = NetworkManager.SpawnManager.SpawnedObjects[objectId]
										.GetComponent<BallScript>();

		_followTransform.AddChild(ballToPickup.transform);
		GrabbedBall = ballToPickup;
		//ballscript.transform.SetParent(_ballAnchor);
		//ballscript.transform.localPosition = Vector3.zero;
	}

}
