using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour
{
    public TileType tileType;            // Tipo de baldosa
    public bool isFlipped = false;       // Está volteada?
    public bool isMatched = false;       // Ya se emparejó?

    public float flipDuration = 0.3f;    // Tiempo de animación de giro

    private bool isAnimating = false;    // Evita múltiples animaciones
    private bool triggered = false;      // Evita múltiples activaciones

    private Quaternion rotationDown;     // Rotación inicial
    private Quaternion rotationUp;       // Rotación volteada

    public GameManager gameManager;      // Referencia al GameManager

    private void Start()
    {
        rotationDown = Quaternion.Euler(90f, 90f, 0f);
        rotationUp   = Quaternion.Euler(-90f, 90f, 0f);
        transform.rotation = rotationDown;

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    // --- Llamado desde PlayerController cuando cae sobre la baldosa ---
    public void TryFlipFromPlayer(float playerYSpeed)
    {
        if (triggered || isFlipped || isMatched) return;

        // Solo voltear si el Player está cayendo
        if (playerYSpeed <= 0f)
        {
            triggered = true;
            OnTileStepped();
        }
    }

    private void OnTileStepped()
    {
        if (tileType == TileType.Trampa)
        {
            StartCoroutine(TrapFlip());
            gameManager?.PlayerStumble(); // Notificar al GameManager
        }
        else
        {
            FlipUp();

            // Notificar al GameManager que se seleccionó esta baldosa
            gameManager?.TileSelected(this);
        }
    }

    private IEnumerator TrapFlip()
    {
        FlipUp();
        yield return new WaitForSeconds(flipDuration);
        FlipDown();
        triggered = false; // permite volver a activarse
    }

    public void FlipUp()
    {
        if (isFlipped || isAnimating) return;
        isFlipped = true;
        StartCoroutine(RotateTo(rotationUp));
    }

    public void FlipDown()
    {
        if (!isFlipped || isAnimating) return;
        isFlipped = false;
        StartCoroutine(RotateTo(rotationDown));
    }

    public void Disappear()
    {
        isMatched = true;
        gameObject.SetActive(false);
    }

    private IEnumerator RotateTo(Quaternion targetRotation)
    {
        isAnimating = true;
        Quaternion startRot = transform.rotation;
        float time = 0f;

        while (time < flipDuration)
        {
            transform.rotation = Quaternion.Slerp(startRot, targetRotation, time / flipDuration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        isAnimating = false;
    }

    // Permite que la baldosa pueda activarse de nuevo
    public void ResetTrigger()
    {
        triggered = false;
    }
}
