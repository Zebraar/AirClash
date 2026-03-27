using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject onlinePanel;
    [SerializeField] private GameObject botPanel;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private Text mainMenuText;
    public float rotationSpeed = 10f;
    private RectTransform rectTransform;

    void Start()
    {
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.Play();
        rectTransform = mainMenuText.GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        float currentAngle = rectTransform.localEulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360;

        if (currentAngle >= 6 || currentAngle <= -6)
        {
            rotationSpeed *= -1;
        
            float clampedAngle = Mathf.Clamp(currentAngle, -12, 12);
            rectTransform.localEulerAngles = new Vector3(0, 0, clampedAngle);
        }
    }


    public void PlayBots(string difficulty)
    {
        switch (difficulty)
        {
            case "VeryEasy":
                PlayerPrefs.SetFloat("Difficulty", 3.1415926535f);
                SceneManager.LoadScene("BotsGame");
                break;
            case "Easy":
                PlayerPrefs.SetFloat("Difficulty", 7.5f);
                SceneManager.LoadScene("BotsGame");
                break;
            case "Medium":
                PlayerPrefs.SetFloat("Difficulty", 13.5f);
                SceneManager.LoadScene("BotsGame");
                break;
            case "Hard":
                PlayerPrefs.SetFloat("Difficulty", 25f);
                SceneManager.LoadScene("BotsGame");
                break;
            case "Extreme":
                PlayerPrefs.SetFloat("Difficulty", 50f);
                SceneManager.LoadScene("BotsGame");
                break;
            default:
                PlayerPrefs.SetFloat("Difficulty", 2f);
                break;
        }
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        onlinePanel.SetActive(false);
        botPanel.SetActive(false);
    }

    public void OpenOnline()
    {
        settingsPanel.SetActive(false);
        onlinePanel.SetActive(true);
        botPanel.SetActive(false);
    }

    public void OpenBot()
    {
        settingsPanel.SetActive(false);
        onlinePanel.SetActive(false);
        botPanel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        settingsPanel.SetActive(false);
        onlinePanel.SetActive(false);
        botPanel.SetActive(false);
    }

    public void VeryEasyMode()
    {
        PlayBots("VeryEasy");
    }
    public void Easy()
    {
        PlayBots("Easy");
    }
    public void Normal()
    {
        PlayBots("Medium");
    }
    public void Hard()
    {
        PlayBots("Hard");
    }
    public void Extreme()
    {
        PlayBots("Extreme");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }


}
