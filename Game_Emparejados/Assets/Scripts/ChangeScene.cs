using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadGameScene()
    {
        // Cambia "GameScene" por el nombre EXACTO de tu escena de juego
        SceneManager.LoadScene("GameScene");
    }

    // Este método se asigna al botón "Salir" del menú principal
    public void QuitGame()
    {
        // Cierra la aplicación cuando está compilada
        Application.Quit();

        // Solo para pruebas en el editor (no se ejecuta en build final)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
