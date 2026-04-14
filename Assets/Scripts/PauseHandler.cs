using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;
    [SerializeField] private GoalHandler goalHandler;

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        if(isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }


    public void MainMenu()
    {
        PlayerPrefs.SetInt("HowMoneyAdds", 0);
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        TogglePause();
        goalHandler.RestartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
