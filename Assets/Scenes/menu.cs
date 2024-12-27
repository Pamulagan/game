using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public void StartGame()
    {
        // Load the main game scene (replace "GameScene" with your actual scene name)
        SceneManager.LoadScene("GameScene");
    }

    public void OpenOptions()
    {
        // Display options menu (implement this as needed)
        Debug.Log("Options Menu Opened");
    }

    public void QuitGame()
    {
        // Quit the game
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
