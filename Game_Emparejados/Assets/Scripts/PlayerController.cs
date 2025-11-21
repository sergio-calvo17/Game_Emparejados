using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // --- Variables Configurables ---
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 500f;

    [Header("Gravedad y Salto")]
    public float gravity = -25f;
    public float jumpForce = 8f;
    private float ySpeed = 0f;

    [Header("Referencias")]
    public Animator anim;
    private CharacterController controller;
    private Vector3 velocity; // Vector ÚNICO para movimiento total

    private bool isPaused = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (!anim) anim = GetComponentInChildren<Animator>();

        if (controller == null || anim == null)
        {
            Debug.LogError("Error: Faltan componentes CharacterController o Animator.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CheckPauseInput();
        if (isPaused) return;

        // ================================
        //      INPUT
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
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
        interactPressed = Input.GetKeyDown(KeyCode.Return);
#endif

        // ================================
        //      MOVIMIENTO HORIZONTAL Y ROTACIÓN
        // ================================
        Vector3 dir = new Vector3(x, 0f, z).normalized;
        bool isMoving = dir.sqrMagnitude > 0.01f;
        
        bool isGrounded = controller.isGrounded;

        // --- LÓGICA CLAVE: RESTRICCIÓN DE MOVIMIENTO AÉREO ---
        if (isGrounded)
        {
            // Resetear velocidad vertical al tocar el suelo
            if (ySpeed < 0)
                ySpeed = -2f; 
            
            // 1. Aplicar movimiento horizontal y rotación solo en el suelo
            velocity.x = dir.x * moveSpeed;
            velocity.z = dir.z * moveSpeed;
            
            if (isMoving)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            // 2. Salto solo en el suelo
            if (jumpPressed)
            {
                anim.SetTrigger("salto");
                ySpeed = jumpForce;
            }
        } 
        else // Si está en el aire (no isGrounded)
        {
            // 1. DETENER MOVIMIENTO HORIZONTAL: Aseguramos que la velocidad horizontal sea CERO.
            velocity.x = 0; 
            velocity.z = 0; 

            // 2. NO permitir rotación.
        }
        
        // Animación: 'caminando' solo se activa si hay input de movimiento Y está en el suelo
        anim.SetBool("caminando", isMoving && isGrounded); 

        // ================================
        //      GRAVEDAD Y APLICAR MOVIMIENTO
        // ================================
        
        // Aplicar gravedad continuamente
        ySpeed += gravity * Time.deltaTime;
        
        // Asignar movimiento vertical al vector 'velocity'
        velocity.y = ySpeed; 

        // Mover el CharacterController
        controller.Move(velocity * Time.deltaTime); 

        // ================================
        //      VOLTEAR CARTA (Enter)
        // ================================
        if (interactPressed)
            FlipCard();
    }

    // --- Métodos (FlipCard, CheckPauseInput, TogglePause se mantienen igual) ---
    void FlipCard()
    {
        Debug.Log("Intentando voltear carta...");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.5f))
        {
            CardController card = hit.transform.GetComponent<CardController>();
            if (card != null) card.AttemptFlip();
        }
    }

    void CheckPauseInput()
    {
#if ENABLE_INPUT_SYSTEM
        bool pausePressed = Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame;
#else
        bool pausePressed = Input.GetKeyDown(KeyCode.P);
#endif
        if (pausePressed) TogglePause();
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }
}