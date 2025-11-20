using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // =======================================================================
    // VARIABLES AJUSTABLES
    // =======================================================================
    [Header("Movimiento")]
    public float moveSpeed = 4f;    
    public float rotationSpeed = 10f; 
    
    [Header("Gravedad y Salto")] 
    public float gravity = -50f;    // Valor de gravedad ajustado para que el personaje caiga rápido.
    public float jumpForce = 5f;    // Fuerza de salto baja para poca altura.
    public int maxJumps = 2;        // Número máximo de saltos (doble salto).
    private int jumpCount;          // Cuenta de saltos para el doble salto.
    
    [Header("Referencias")]
    public Animator anim;           // Referencia al Animator para animaciones
    private CharacterController controller;  // Componente CharacterController
    private Vector3 velocity;        // Para manejar el salto y la gravedad

    [Header("Configuración del suelo")]
    public float groundCheckDistance = 0.5f; // Distancia para comprobar si el personaje está tocando el suelo
    public LayerMask groundMask;  // Capa del suelo (debe incluir el plano o terreno)

    private bool isPaused = false; // Para controlar el estado de pausa
    
    // =======================================================================
    // START
    // =======================================================================
    void Start()
    {
        controller = GetComponent<CharacterController>(); 
        if (!anim) anim = GetComponentInChildren<Animator>(); 

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // =======================================================================
    // UPDATE
    // =======================================================================
    void Update()
    {
        CheckPauseInput();

        if (isPaused)
        {
            return; // Si el juego está pausado, no procesamos más
        }

        // 1. Entrada de teclas para movimiento y animaciones
        float x = 0f, z = 0f;
        bool jumpPressed = false;
        bool interactPressed = false;

        #if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed) z += 1f;
            if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed) z -= 1f;

            jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;  // Salto con la tecla Espacio
            interactPressed = Keyboard.current.enterKey.wasPressedThisFrame; // Interacción con Enter
        }
        #else
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetKeyDown(KeyCode.Space);  // Salto con la tecla Espacio
        interactPressed = Input.GetKeyDown(KeyCode.Return); // Interacción con Enter
        #endif

        Vector3 dir = new Vector3(x, 0f, z).normalized;
        bool isMoving = dir.sqrMagnitude > 0.01f; 
        anim.SetBool("caminando", isMoving); // Activamos la animación de caminar cuando el jugador se mueve

        // 2. Movimiento y Rotación
        if (isMoving)
        {
            Quaternion target = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }

        controller.Move(dir * moveSpeed * Time.deltaTime);

        // 3. Verificación del suelo y aplicación de gravedad
        bool isGrounded = controller.isGrounded; 

        if (isGrounded)
        {
            jumpCount = 0;  // Restablecemos la cuenta de saltos cuando tocamos el suelo
            if (velocity.y < 0f)
                velocity.y = -2f;  // Para mantener al personaje pegado al suelo, evitamos que se quede flotando
        }

        // 4. Saltar con la tecla Espacio
        if (jumpPressed && jumpCount < maxJumps)  // Si el jugador presiona espacio y hay saltos disponibles
        {
            anim.SetTrigger("salto");  // Activamos la animación de salto
            velocity.y = jumpForce;     // Aplicamos la fuerza de salto
            jumpCount++;                // Incrementamos la cuenta de saltos
        }

        // 5. Aplicar la gravedad
        velocity.y += gravity * Time.deltaTime;  // Aplicamos gravedad para que el personaje caiga

        controller.Move(velocity * Time.deltaTime); // Movemos al personaje con la gravedad aplicada

        // 6. Interacción con cartas o objetos
        if (interactPressed)
        {
            FlipCard();
        }
    }

    // =======================================================================
    // MÉTODOS DE ACCIÓN Y PAUSA
    // =======================================================================
    
    // Método para voltear la carta al presionar Enter
    void FlipCard()
    {
        Debug.Log("Intentando voltear carta...");
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.5f)) // Limita el rango de interacción
        {
            CardController card = hit.transform.GetComponent<CardController>();
            if (card != null)
            {
                card.AttemptFlip(); // Llamamos a la función de voltear la carta
            }
        }
    }

    // Método para controlar la pausa del juego con la tecla P
    void CheckPauseInput()
    {
        bool pausePressed = 
        #if ENABLE_INPUT_SYSTEM
            Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame;
        #else
            Input.GetKeyDown(KeyCode.P); 
        #endif

        if (pausePressed)
        {
            TogglePause(); // Llamamos a la función para pausar o reanudar el juego
        }
    }

    // Método que pausa o reanuda el juego
    void TogglePause()
    {
        isPaused = !isPaused; 
        
        if (isPaused)
        {
            Time.timeScale = 0f;  // Detenemos el tiempo en el juego
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;  // Reanudamos el tiempo del juego
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
