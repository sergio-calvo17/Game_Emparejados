using UnityEngine;
using System.Collections;

public class TileManager : MonoBehaviour
{
    private TileFlipOnJump firstTile = null;
    private TileFlipOnJump secondTile = null;

    private bool checking = false;

    public int pairsFound = 0;
    public int totalPairs = 4;

    private GameManager gameManager;

    // -------------------------------
    // Sonidos
    // -------------------------------
    [Header("Sonidos de juego")]
    public AudioSource sonidoVoltear;
    public AudioSource sonidoAcierto;
    public AudioSource sonidoFallo;

    // -------------------------------
    // Explosión para piso trampa
    // -------------------------------
    [Header("Explosión baldosa trampa")]
    public GameObject explosionPrefab;

    // Ajuste automático
    public float explosionMultiplier = 0.9f;
    // (1 = igual de grande que la baldosa. 0.9 la hace un poquito más pequeña)

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    // ---------------------------------------------------------
    // Se llama cuando una baldosa se voltea
    // ---------------------------------------------------------
    public void RegisterTile(TileFlipOnJump tile)
    {
        if (checking) return;

        if (sonidoVoltear != null)
            sonidoVoltear.Play();

        // --------- MANEJO DE BALDOSA TRAMPA ----------
        if (tile.isTrap)
        {
            Debug.Log("Se pisó una baldosa trampa!");

            CrearExplosion(tile);

            StartCoroutine(DelayGameOver());
            return;
        }
        // -------------------------------------------------

        if (firstTile == null)
        {
            firstTile = tile;
            return;
        }

        if (secondTile == null)
        {
            secondTile = tile;
            checking = true;
            StartCoroutine(CheckMatchDelayed());
        }
    }

    private IEnumerator CheckMatchDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        CheckMatch();
        checking = false;
    }

    private void CheckMatch()
    {
        if (AreTilesPair(firstTile.tileName, secondTile.tileName))
        {
            firstTile.LockTile();
            secondTile.LockTile();

            pairsFound++;

            if (sonidoAcierto != null)
                sonidoAcierto.Play();

            if (pairsFound >= totalPairs)
            {
                gameManager.MostrarVictoria();
            }
        }
        else
        {
            if (sonidoFallo != null)
                sonidoFallo.Play();

            firstTile.ResetTile();
            secondTile.ResetTile();
        }

        firstTile = null;
        secondTile = null;
    }

    private bool AreTilesPair(string name1, string name2)
    {
        if ((name1 == "piso circulo" && name2 == "piso circulo (1)") ||
            (name1 == "piso circulo (1)" && name2 == "piso circulo"))
            return true;

        if ((name1 == "piso cuadrado" && name2 == "piso cuadrado (1)") ||
            (name1 == "piso cuadrado (1)" && name2 == "piso cuadrado"))
            return true;

        if ((name1 == "piso estrella" && name2 == "piso estrella (1)") ||
            (name1 == "piso estrella (1)" && name2 == "piso estrella"))
            return true;

        if ((name1 == "piso triangulo" && name2 == "piso triangulo (1)") ||
            (name1 == "piso triangulo (1)" && name2 == "piso triangulo"))
            return true;

        return false;
    }

    public bool AllPairsFound()
    {
        return pairsFound >= totalPairs;
    }

    // ---------------------------------------------------------
    // Crear EXPLOSIÓN del tamaño de la baldosa
    // ---------------------------------------------------------
    private void CrearExplosion(TileFlipOnJump tile)
    {
        if (explosionPrefab == null) return;

        GameObject fx = Instantiate(explosionPrefab, tile.transform.position, Quaternion.identity);

        // Tamaño REAL de la baldosa
        Vector3 tileScale = tile.transform.localScale;

        // Escala final
        float escala = Mathf.Max(tileScale.x, tileScale.y) * explosionMultiplier;

        fx.transform.localScale = new Vector3(escala, escala, escala);

        // Si es ParticleSystem, escalar correctamente
        ParticleSystem ps = fx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;

            // Tamaño de partículas
            main.startSizeMultiplier = escala;

            // Velocidad proporcional al tamaño
            main.startSpeedMultiplier = escala * 2f;
        }

        // Destrucción automática basada en la duración real del sistema de partículas
        float lifetime = 2.5f;

        if (ps != null)
        {
            lifetime = ps.main.duration + ps.main.startLifetime.constantMax;
        }

        Destroy(fx, lifetime);
    }

    // ---------------------------------------------------------
    // Delay antes de mostrar derrota (explosión)
    // ---------------------------------------------------------
    private IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(0.6f);
        gameManager.MostrarDerrota();
    }
}
