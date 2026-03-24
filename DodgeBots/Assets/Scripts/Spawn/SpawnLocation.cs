using System;
using UnityEngine;

using UnityEngine.UIElements;

public class SpawnLocation : MonoBehaviour
{
    public event EventHandler Clicked;

    [SerializeField]
    private UIDocument _spawnUIDoc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spawnUIDoc.rootVisualElement.Q<Button>().clicked += OnClicked;
    }

    // Update is called once per frame
    protected virtual void OnClicked()
    {
        Clicked?.Invoke(this, EventArgs.Empty);
    }
}
