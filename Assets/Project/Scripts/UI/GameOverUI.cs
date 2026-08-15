using System;
using Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class GameOverUI : ContentUi, IInitializable, IDisposable
{
    [SerializeField]
    private Button _restartButton;

    public void Initialize()
    {
        _restartButton.onClick.AddListener(RestartGame);
    }

    public void Dispose()
    {
        _restartButton.onClick.RemoveListener(RestartGame);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}