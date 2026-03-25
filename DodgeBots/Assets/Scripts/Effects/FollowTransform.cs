using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
	private List<Transform> _children = new List<Transform>();
	public void AddChild(Transform child)
	{
		_children.Add(child);
	}
	public void RemoveChild(Transform child)
	{
		_children.Remove(child);
	}
	private void LateUpdate()
	{
		foreach (Transform child in _children)
		{
			child.position = transform.position;
			child.rotation = transform.rotation;
		}
	}
}
