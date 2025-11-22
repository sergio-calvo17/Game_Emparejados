using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("Asignar en Inspector")]
    public GameObject pauseMenuUI;  
    private PlayerController player;

    [Header("Audio")]
    public AudioSource musicSource;   // música del juego

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Cursor siempre visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuUI) pauseMenuUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Desactivar movimiento del player
        if (player != null)
            player.enabled = false;

        // PAUSAR MÚSICA 
        if (musicSource != null)
            musicSource.Pause();
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuUI) pauseMenuUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Rehabilitar al player
        if (player != null)
            player.enabled = true;

        // REANUDAR MÚSICA 
        if (musicSource != null)
            musicSource.UnPause();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
