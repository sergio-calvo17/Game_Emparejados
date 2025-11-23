using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  

    [Header("Paneles UI")]
    public GameObject panelVictoria;
    public GameObject panelDerrota;

    [Header("Sonidos")]
    public AudioSource musicaFondo;
    public AudioSource sonidoVictoria;
    public AudioSource sonidoDerrota;

    private bool juegoFinalizado = false;

    void Awake()
    {
        // Configurar Singleton (solo existe 1 GameManager)
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // ------------------------------
    //           DERROTA
    // ------------------------------
    public void MostrarDerrota()
    {
        if (juegoFinalizado) return;
        juegoFinalizado = true;

        // Mostrar el panel
        panelDerrota.SetActive(true);

        // Pausar música
        if (musicaFondo != null) musicaFondo.Pause();

        // Reproducir sonido
        if (sonidoDerrota != null) sonidoDerrota.Play();

        // Pausar el juego
        Time.timeScale = 0f;
    }

    // ------------------------------
    //           VICTORIA
    // ------------------------------
    public void MostrarVictoria()
    {
        if (juegoFinalizado) return;
        juegoFinalizado = true;

        panelVictoria.SetActive(true);

        if (musicaFondo != null) musicaFondo.Pause();
        if (sonidoVictoria != null) sonidoVictoria.Play();

        Time.timeScale = 0f;
    }
}
