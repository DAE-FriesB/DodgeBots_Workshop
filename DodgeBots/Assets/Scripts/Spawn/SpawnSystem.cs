using System;
using Unity.Netcode;
using UnityEngine;

public class SpawnSystem : NetworkBehaviour
{
	[SerializeField]
	private SpawnLocation[] _spawnLocations;

	[SerializeField]
	private GameObject _playerAvatarPrefab;


	private void Awake()
	{
		foreach (var spawnLocation in _spawnLocations)
		{
			spawnLocation.Clicked += SpawnLocation_Clicked;

		}
		SetLocationsVisible(false);
	}

	public override void OnNetworkSpawn()
	{
		SetLocationsVisible(true);
	}

	public void SetLocationsVisible(bool visible)
	{
		foreach (var location in _spawnLocations)
		{
			location.gameObject.SetActive(visible);
		}
	}


	private void SpawnLocation_Clicked(object sender, System.EventArgs e)
	{
		//find spawn location
		SpawnLocation clickedLocation = sender as SpawnLocation;

		int spawnLocationIndex = Array.FindIndex(_spawnLocations, (s) => s == clickedLocation);

		SpawnPlayerRpc(spawnLocationIndex, NetworkManager.Singleton.LocalClientId);

		//hide spawn locations
		SetLocationsVisible(false);
	}

	[Rpc(SendTo.Server)]
	void SpawnPlayerRpc(int spawnIndex, ulong clientId)
	{
		Transform spawnLocation = _spawnLocations[spawnIndex].transform;

		GameObject instance = Instantiate(_playerAvatarPrefab, spawnLocation.position, spawnLocation.rotation);
		instance.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);
	}
}
