using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Loads the next scene in the build (default play)
    public void PlayGame()
    {
        SceneManager.LoadScene(1); // Assuming Outside Forest is index 1
    }

    // Loads a specific scene by name (for level select)
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
