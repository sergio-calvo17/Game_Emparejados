using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Configuración del juego")]
    public float gameTime = 60f; // tiempo total en segundos
    public Text timerText;
    public Text scoreText;

    [Header("Referencias")]
    public List<TileController> allTiles; // todas las baldosas en la escena

    private TileController firstTile = null;
    private TileController secondTile = null;

    private int score = 0;
    private float currentTime;
    private bool gameActive = true;

    private void Start()
    {
        currentTime = gameTime;
        UpdateUI();
    }

    private void Update()
    {
        if (!gameActive) return;

        // Contador de tiempo
        currentTime -= Time.deltaTime;
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            GameOver(false);
        }

        UpdateUI();
    }

    // Actualiza texto de UI
    private void UpdateUI()
    {
        if (timerText != null)
            timerText.text = "Tiempo: " + Mathf.CeilToInt(currentTime);
        if (scoreText != null)
            scoreText.text = "Puntos: " + score;
    }

    // Llamado desde TileController cuando se selecciona una baldosa
    public void TileSelected(TileController tile)
    {
        if (!gameActive) return;

        if (firstTile == null)
        {
            firstTile = tile;
        }
        else if (secondTile == null && tile != firstTile)
        {
            secondTile = tile;
            StartCoroutine(CheckMatch());
        }
    }

    // Comparar dos baldosas
    private System.Collections.IEnumerator CheckMatch()
    {
        // Espera un momento para que la animación de volteo se vea
        yield return new WaitForSeconds(0.5f);

        if (firstTile.tileType == secondTile.tileType)
        {
            firstTile.Disappear();
            secondTile.Disappear();
            score += 1;

            // Verificar si todas las baldosas están emparejadas
            if (CheckAllMatched())
            {
                GameOver(true);
            }
        }
        else
        {
            firstTile.FlipDown();
            secondTile.FlipDown();

            // 🔹 Permitir volver a voltearlas después del error
            firstTile.ResetTrigger();
            secondTile.ResetTrigger();
        }

        firstTile = null;
        secondTile = null;
        UpdateUI();
    }

    // Verifica si todas las baldosas están emparejadas
    private bool CheckAllMatched()
    {
        foreach (TileController tile in allTiles)
        {
            if (!tile.isMatched && tile.tileType != TileType.Trampa)
                return false;
        }
        return true;
    }

    // Se llama si el jugador pisa una trampa
    public void PlayerStumble()
    {
        Debug.Log("Jugador pisó una trampa!");
        // Aquí puedes agregar efectos de sonido, animación, etc.
    }

    // Fin del juego
    private void GameOver(bool won)
    {
        gameActive = false;
        if (won)
            Debug.Log("¡Ganaste!");
        else
            Debug.Log("¡Perdiste!");
    }
}
