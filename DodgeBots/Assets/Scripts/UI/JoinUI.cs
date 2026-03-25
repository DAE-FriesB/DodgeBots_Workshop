using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class JoinUI : MonoBehaviour
{
	private void Awake()
	{
		UIDocument doc = GetComponent<UIDocument>();
		Button joinHostButton = doc.rootVisualElement.Q<Button>("JoinHostBtn");
		Button joinClientButton = doc.rootVisualElement.Q<Button>("JoinClientBtn");

		joinHostButton.clicked += JoinHostButton_clicked;
		joinClientButton.clicked += JoinClientButton_clicked;
	}

	private void JoinHostButton_clicked()
	{
		//TODO: Join as Host
		NetworkManager.Singleton.StartHost();

		//Hide UI
		gameObject.SetActive(false);
	}

	private void JoinClientButton_clicked()
	{
		//TODO: Join as Client
		NetworkManager.Singleton.StartClient();

		//Hide UI
		gameObject.SetActive(false);
	}
}
