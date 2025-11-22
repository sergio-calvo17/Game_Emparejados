using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadGameScene()
    {
        // Cambia "GameScene" por el nombre EXACTO de tu escena de juego
        SceneManager.LoadScene("GameScene");
    }
}