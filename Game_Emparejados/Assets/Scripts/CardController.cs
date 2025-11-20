using UnityEngine;

public class CardController : MonoBehaviour
{
    // Una referencia al Game Manager (será útil más adelante)
    private GameManager gameManager;
    
    // Una variable para identificar qué imagen tiene la carta
    public int cardID; 
    
    // Indica si la carta está volteada o no
    private bool isFlipped = false;

    void Start()
    {
        // Opcional: Busca el GameManager al inicio.
        // gameManager = FindObjectOfType<GameManager>(); 
    }

    /// <summary>
    /// Intenta voltear la carta y notifica al GameManager.
    /// Esta es la función que llama PlayerController.
    /// </summary>
    public void AttemptFlip()
    {
        if (!isFlipped) // Solo voltea si no está volteada ya
        {
            // Aquí iría la lógica visual o la animación para voltear la carta
            Debug.Log("Carta con ID " + cardID + " volteada.");
            
            // Lógica temporal para mostrar que funciona
            FlipCardVisuals();
            
            // TO-DO: Notificar al GameManager que esta carta fue seleccionada.
        }
    }
    
    // Temporal: Simula el cambio visual al voltear
    private void FlipCardVisuals()
    {
        // En un juego 3D, esto podría ser cambiar la rotación o cambiar el material
        isFlipped = true;
        
        // Ejemplo simple: Cambiar el color para indicar que está volteada
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.yellow; // Cambia a un color temporal
        }
    }
    
    // Función que será llamada por el GameManager para ocultar o destruir la carta
    public void MatchFound()
    {
        // Lógica para destruir la carta o hacerla invisible
        Debug.Log("¡Pareja encontrada! Carta " + cardID + " eliminada.");
        Destroy(gameObject);
    }
    
    public void ResetFlip()
    {
        // Lógica para voltear la carta de nuevo a su estado inicial
        isFlipped = false;
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.white; // Vuelve al color inicial
        }
    }
}