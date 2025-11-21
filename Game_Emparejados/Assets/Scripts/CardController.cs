using UnityEngine;

public class CardController : MonoBehaviour
{
    [Header("Configuración de la carta")]
    public bool isFlipped = false;  // Estado de la carta, si está volteada o no
    public float flipSpeed = 1f;    // Velocidad del volteo

    private Vector3 startRotation;  // Rotación inicial de la carta
    private Vector3 flippedRotation; // Rotación cuando la carta esté volteada

    private void Start()
    {
        // Guardamos la rotación inicial
        startRotation = transform.eulerAngles;

        // Definimos la rotación final para cuando la carta esté volteada
        flippedRotation = startRotation + new Vector3(0f, 180f, 0f);  // Voltea la carta sobre el eje Y
    }

    private void Update()
    {
        // Si la carta está volteada, dejamos de rotarla
        if (isFlipped)
        {
            // Aquí podríamos agregar alguna lógica para hacer que la carta revele algo, como una imagen o texto
            // Se puede cambiar el material de la carta para que revele la figura oculta
        }
    }

    // Método para intentar voltear la carta
    public void AttemptFlip()
    {
        if (!isFlipped)
        {
            // Iniciamos el volteo de la carta
            StartCoroutine(FlipCard());
        }
    }

    // Método para realizar el flip de la carta
    private System.Collections.IEnumerator FlipCard()
    {
        float timeElapsed = 0f;

        // Mientras no se haya completado el volteo de la carta
        while (timeElapsed < 1f)
        {
            // Interpolamos entre la rotación inicial y la rotación volteada
            transform.eulerAngles = Vector3.Lerp(startRotation, flippedRotation, timeElapsed);

            timeElapsed += Time.deltaTime * flipSpeed;

            // Esperamos el siguiente frame
            yield return null;
        }

        // Aseguramos que la carta esté completamente volteada
        transform.eulerAngles = flippedRotation;
        
        // Marcamos la carta como volteada
        isFlipped = true;
    }

    // Método para resetear la carta (voltearla de vuelta)
    public void ResetCard()
    {
        isFlipped = false;
        StartCoroutine(FlipCardBack());
    }

    // Método para realizar el regreso de la carta a su estado inicial
    private System.Collections.IEnumerator FlipCardBack()
    {
        float timeElapsed = 0f;

        while (timeElapsed < 1f)
        {
            // Interpolamos entre la rotación volteada y la rotación inicial
            transform.eulerAngles = Vector3.Lerp(flippedRotation, startRotation, timeElapsed);

            timeElapsed += Time.deltaTime * flipSpeed;

            // Esperamos el siguiente frame
            yield return null;
        }

        // Aseguramos que la carta esté completamente en su posición inicial
        transform.eulerAngles = startRotation;
    }
}