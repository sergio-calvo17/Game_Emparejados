using UnityEngine;
using System.Collections.Generic; // Necesario para usar List<T>

public class GameManager : MonoBehaviour
{
    // Una referencia estática para acceder al GameManager fácilmente (Singleton patrón)
    public static GameManager Instance;
    
    [Header("Configuración de Cartas")]
    public List<CardController> allCards; // Lista de todas las cartas en el juego
    public float matchDelay = 1f;        // Tiempo de espera antes de voltear cartas incorrectas
    
    // Variables para la lógica de emparejamiento
    private CardController firstCardFlipped = null;
    private CardController secondCardFlipped = null;
    private bool isCheckingMatch = false; // Bandera para bloquear interacción mientras se chequea

    void Awake()
    {
        // Implementación básica del patrón Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método que llama la carta cuando se voltea (desde CardController.cs)
    public void CardFlipped(CardController card)
    {
        if (isCheckingMatch) return; // Ignorar si estamos esperando el resultado
        
        if (firstCardFlipped == null)
        {
            // Primera carta volteada
            firstCardFlipped = card;
        }
        else if (secondCardFlipped == null)
        {
            // Segunda carta volteada
            secondCardFlipped = card;
            isCheckingMatch = true;
            
            // Inicia la verificación después de un pequeño retraso
            Invoke("CheckMatch", matchDelay);
        }
    }

    // Lógica para comparar las dos cartas seleccionadas
    void CheckMatch()
    {
        // Comparamos el ID de las cartas
        if (firstCardFlipped.cardID == secondCardFlipped.cardID)
        {
            Debug.Log("¡Match encontrado!");
            // Destruye o deshabilita las cartas
            firstCardFlipped.MatchFound();
            secondCardFlipped.MatchFound();
            
            // TO-DO: Lógica de puntos y victoria
        }
        else
        {
            Debug.Log("No es un Match. Volviendo a voltear...");
            // Voltea las cartas de vuelta a su estado oculto
            firstCardFlipped.ResetFlip();
            secondCardFlipped.ResetFlip();
        }

        // Reinicia las variables para el siguiente turno
        firstCardFlipped = null;
        secondCardFlipped = null;
        isCheckingMatch = false;
    }
    
    // TO-DO: Agregar lógica para iniciar el juego (mezclar cartas, instanciar)
}