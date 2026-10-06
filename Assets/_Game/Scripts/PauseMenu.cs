using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using ithappy.Animals_FREE;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false;

    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Blur Volume")]
    [SerializeField] private Volume pauseBlurVolume;

    [Header("Player Controls")]
    [SerializeField] private MovePlayerInput playerMove;
    [SerializeField] private FirstPersonLook playerLook;

    private void Start()
    {
        ResumeGame();
    }

    private void Update()
    {
        // Check for pause toggle input
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (optionsPanel.activeSelf)
            {
                CloseOptions();
                return;
            }

            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (IsPaused)
        {
            if (Cursor.lockState != CursorLockMode.None)
                Cursor.lockState = CursorLockMode.None;

            if (!Cursor.visible)
                Cursor.visible = true;
        }
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        pausePanel.SetActive(true);
        optionsPanel.SetActive(false);
        pauseBlurVolume.weight = 1f;

        playerLook.canLook = false;
        playerMove.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        pauseBlurVolume.weight = 0f;

        playerLook.canLook = true;
        playerMove.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenOptions()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}