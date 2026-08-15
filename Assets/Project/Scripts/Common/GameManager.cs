public enum GameState
{
    Playing,
    GameOver,
}

public class GameManager
{
    private readonly GameOverUI _gameOverUI;

    private GameState _state = GameState.Playing;

    public GameManager(GameOverUI gameOverUI)
    {
        _gameOverUI = gameOverUI;
    }

    public GameState State => _state;

    public bool IsGameOver => _state == GameState.GameOver;

    public void SetGameOver()
    {
        if (_state == GameState.GameOver)
            return;

        _state = GameState.GameOver;
        _gameOverUI.Show();
    }
}
