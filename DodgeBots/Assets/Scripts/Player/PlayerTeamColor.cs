using UnityEngine;

public class PlayerTeamColor : MonoBehaviour
{
	public enum TeamColor
	{
		Blue,
		Red
	}

	[SerializeField]
	private TeamColor _defaultTeamColor;


	[SerializeField]
	private Material _redMaterial, _blueMaterial;
	[SerializeField]
	private Renderer[] _teamColorObjects;

	private void Awake()
	{
		SetPlayerColor(_defaultTeamColor);
	}

	public void SetPlayerColor(TeamColor color)
	{
		Material material = _redMaterial;
		if (color == TeamColor.Blue)
		{
			material = _blueMaterial;
		}

		foreach (var renderer in _teamColorObjects)
		{
			renderer.sharedMaterial = material;
		}
	}



}
