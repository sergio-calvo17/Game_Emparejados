using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // --- Variables Configurables ---
    [Header("Movimiento")]
    public float moveSpeed = 5f;        // Velocidad de movimiento
    public float rotationSpeed = 500f;  // Velocidad de rotación

    [Header("Gravedad y Salto")]
    public float gravity = -18f;        // Gravedad (Valor negativo para caer)
    public float jumpForce = 10f;        // Fuerza vertical del salto
    private float ySpeed = 2f;          // Velocidad vertical (gravedad/salto)

    [Header("Referencias")]
    public Animator anim;               // Referencia al Animator
    private CharacterController controller;
    private Vector3 velocity;           // Vector ÚNICO para movimiento total

    private bool isPaused = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Busca el Animator en el objeto principal o en un hijo.
        if (!anim) anim = GetComponentInChildren<Animator>();

        if (controller == null || anim == null)
        {
            Debug.LogError("Error: Faltan componentes CharacterController o Animator.");
        }

        // Bloquear cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CheckPauseInput();
        if (isPaused) return;

        // ================================
        //      INPUT (Mover / Saltar / Enter)
        // ================================
        float x = 0f, z = 0f;
        bool jumpPressed = false;
        bool interactPressed = false;

#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) x -= 1f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) x += 1f;
            if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed) z += 1f;
            if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed) z -= 1f;

            jumpPressed = keyboard.spaceKey.wasPressedThisFrame;
            interactPressed = keyboard.enterKey.wasPressedThisFrame;
        }
#else
        // Usa el sistema de Input antiguo (GetAxisRaw responde a WASD y Flechas)
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
        interactPressed = Input.GetKeyDown(KeyCode.Return);
#endif

        // ================================
        //      MOVIMIENTO HORIZONTAL + ROTACIÓN
        // ================================
        Vector3 dir = new Vector3(x, 0f, z).normalized;
        bool isMoving = dir.sqrMagnitude > 0.01f;
        
        // 1. Asignar movimiento horizontal al vector 'velocity'
        velocity = dir * moveSpeed; 

        // Rotación: El personaje mira en la dirección de su input
        if (isMoving)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // ANIMACIÓN: Activar la animación de caminar si se está moviendo
        anim.SetBool("caminando", isMoving); 

        // ================================
        //      SALTO + GRAVEDAD
        // ================================
        bool isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            if (ySpeed < 0)
                ySpeed = -2f; // Mantiene al personaje pegado al suelo

            if (jumpPressed)
            {
                anim.SetTrigger("salto");   // Activa animación de salto
                ySpeed = jumpForce;         // Aplica la fuerza de salto
            }
        }

        // Aplicar gravedad continuamente
        ySpeed += gravity * Time.deltaTime;  
        
        // 2. Asignar movimiento vertical al vector 'velocity'
        velocity.y = ySpeed; 

        // 3. ¡Mover el CharacterController una sola vez! (Combina X, Z, Y)
        controller.Move(velocity * Time.deltaTime); 

        // ================================
        //      VOLTEAR CARTA (Enter)
        // ================================
        if (interactPressed)
            FlipCard();
    }

    // --- Métodos de Acción y Pausa ---
    
    void FlipCard()
    {
        Debug.Log("Intentando voltear carta...");
        RaycastHit hit;
        // Raycast limitado a 1.5 unidades de distancia
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.5f))
        {
            CardController card = hit.transform.GetComponent<CardController>();
            if (card != null)
                card.AttemptFlip();
        }
    }

    void CheckPauseInput()
    {
#if ENABLE_INPUT_SYSTEM
        bool pausePressed = Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame;
#else
        bool pausePressed = Input.GetKeyDown(KeyCode.P);
#endif

        if (pausePressed)
            TogglePause();
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}