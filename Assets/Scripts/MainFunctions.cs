using UnityEngine;
using UnityEngine.SceneManagement;

public class MainFunctions : MonoBehaviour
{
    public string gameSceneName = "main";

    public void OnePlayer()
    {
        LoadLevel(GameMode.OnePlayer);
    }

    public void TwoPlayers()
    {
        LoadLevel(GameMode.TwoPlayers);
    }

    void LoadLevel(GameMode mode)
    {
        Board.gameMode = mode;
        SceneManager.LoadScene(gameSceneName);
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
