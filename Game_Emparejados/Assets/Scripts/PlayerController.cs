using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // --- Variables Configurables ---
    [Header("Movimiento")]
    public float moveSpeed = 15f; 
    public float rotationSpeed = 500f;

    [Header("Gravedad y Salto")]
    public float gravity = -25f;
    public float jumpForce = 8f;
    private float ySpeed = 0f;

    [Header("Referencias")]
    public Animator anim;
    private CharacterController controller;
    private Vector3 velocity; // Vector de movimiento

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

        // =====================================
        //  INPUT
        // =====================================
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

        Vector3 dir = new Vector3(x, 0f, z).normalized;
        bool isMoving = dir.sqrMagnitude > 0.01f;
        bool isGrounded = controller.isGrounded;

        // ⭐ CORRECCIÓN CLAVE: Reseteamos la velocidad horizontal al inicio del frame.
        // Solo se rellenará si estamos en el suelo (ver abajo).
        velocity.x = 0;
        velocity.z = 0;

        // =====================================
        //  BLOQUEAR MOVIMIENTO EN EL AIRE
        // =====================================
        if (isGrounded)
        {
            anim.SetBool("enElAire", false);

            if (ySpeed < 0)
                ySpeed = -2f;

            // Movimiento horizontal y rotación SOLO en el suelo
            velocity.x = dir.x * moveSpeed; // Se establece la velocidad horizontal aquí
            velocity.z = dir.z * moveSpeed; // Y solo aquí.

            if (isMoving)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            // SALTO
            if (jumpPressed)
            {
                anim.SetTrigger("salto");
                anim.SetBool("enElAire", true); 
                ySpeed = jumpForce;

                // ⭐ EXTRA: Forzamos la detención si aún hay inercia después de un frame.
                velocity.x = 0; 
                velocity.z = 0; 
            }
        }
        else // isGrounded == false (en el aire)
        {
            anim.SetBool("enElAire", true);
            // velocity.x y velocity.z ya son 0 por el reseteo al inicio del Update.
        }

        // Animación caminar: solo en suelo y con input
        anim.SetBool("caminando", isMoving && isGrounded);

        // =====================================
        //  GRAVEDAD Y MOVIMIENTO VERTICAL
        // =====================================
        ySpeed += gravity * Time.deltaTime;
        velocity.y = ySpeed;

        controller.Move(velocity * Time.deltaTime);

        // =====================================
        //  VOLTEAR CARTA (Enter)
        // =====================================
        if (interactPressed)
            FlipCard();
    }

    // --- Métodos auxiliares ---
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