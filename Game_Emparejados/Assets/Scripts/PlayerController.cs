using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 15f;
    public float rotationSpeed = 500f;

    [Header("Gravedad y Salto")]
    public float gravity = -25f;
    public float jumpForce = 8f;
    private float ySpeed = 0f;
    private bool isJumping = false; // NUEVO: flag de salto

    [Header("Referencias")]
    public Animator anim;
    private CharacterController controller;
    private Vector3 velocity;

    private bool isPaused = false;

    [Header("Baldosas")]
    public float rayDistance = 1.0f; // distancia del Raycast hacia abajo

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (!anim) anim = GetComponentInChildren<Animator>();

        if (controller == null || anim == null)
            Debug.LogError("Error: Faltan componentes CharacterController o Animator.");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CheckPauseInput();
        if (isPaused) return;

        HandleInput();
        HandleMovement();
        HandleGravity();
        DetectTileBelow();
        HandleCardFlip();
    }

    void HandleInput()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard.current;
        float x = 0f, z = 0f;
        bool jumpPressed = false;
        bool interactPressed = false;

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
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool interactPressed = Input.GetKeyDown(KeyCode.Return);
#endif

        // Movimiento horizontal
        velocity.x = x * moveSpeed;
        velocity.z = z * moveSpeed;

        // SALTO
        if (jumpPressed && controller.isGrounded)
        {
            anim.SetTrigger("salto");
            anim.SetBool("enElAire", true);
            ySpeed = jumpForce;
            velocity.x = 0;
            velocity.z = 0;

            isJumping = true; // <-- Player inició salto
        }

        // Rotación solo si hay input horizontal/vertical
        Vector3 dir = new Vector3(x, 0f, z).normalized;
        if (dir.sqrMagnitude > 0.01f && controller.isGrounded)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        anim.SetBool("caminando", dir.sqrMagnitude > 0.01f && controller.isGrounded);
    }

    void HandleMovement()
    {
        // Reseteo velocidad vertical al estar en el suelo
        if (controller.isGrounded && ySpeed < 0)
        {
            ySpeed = -2f;

            if (isJumping)
                isJumping = false; // Player ya tocó el suelo
        }

        velocity.y = ySpeed;
        controller.Move(velocity * Time.deltaTime);
        anim.SetBool("enElAire", !controller.isGrounded);
    }

    void HandleGravity()
    {
        ySpeed += gravity * Time.deltaTime;
    }

    // --- Detecta baldosas solo mientras el Player está en el aire ---
    void DetectTileBelow()
    {
        if (!isJumping) return; // no hacer nada si no está saltando

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
        {
            TileController tile = hit.collider.GetComponent<TileController>();
            if (tile != null)
            {
                tile.TryFlipFromPlayer(ySpeed);
            }
        }
    }

    void HandleCardFlip()
    {
#if ENABLE_INPUT_SYSTEM
        bool interactPressed = Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame;
#else
        bool interactPressed = Input.GetKeyDown(KeyCode.Return);
#endif
        if (interactPressed)
            FlipCard();
    }

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
