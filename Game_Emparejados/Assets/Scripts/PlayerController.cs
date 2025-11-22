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

    [Header("Referencias")]
    public Animator anim;
    private CharacterController controller;
    private Vector3 velocity;

    public bool isJumping = false;
   

    // ⭐ Estados del jugador
    public bool hasWon = false;
    public bool hasLost = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (!anim) anim = GetComponentInChildren<Animator>();

        
    }

    void Update()
    {
        // ⭐ Bloquear movimiento si ganó o perdió
        if (hasWon || hasLost)
        {
            velocity = Vector3.zero;
            controller.Move(Vector3.zero);
            return;
        }


        // ================================
        // INPUT
        // ================================
        float x = 0f, z = 0f;
        bool jumpPressed = false;

#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) x -= 1f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) x += 1f;
            if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed) z += 1f;
            if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed) z -= 1f;

            jumpPressed = keyboard.spaceKey.wasPressedThisFrame;

            // ⭐ Teclas para probar las animaciones:
            if (keyboard.vKey.wasPressedThisFrame) TriggerVictory();
            if (keyboard.lKey.wasPressedThisFrame) TriggerDefeat();
        }
#else
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetKeyDown(KeyCode.Space);

        if (Input.GetKeyDown(KeyCode.V)) TriggerVictory();
        if (Input.GetKeyDown(KeyCode.L)) TriggerDefeat();
#endif

        Vector3 dir = new Vector3(x, 0f, z).normalized;
        bool isMoving = dir.sqrMagnitude > 0.01f;
        bool isGrounded = controller.isGrounded;

        velocity.x = 0;
        velocity.z = 0;

        if (isGrounded)
        {
            anim.SetBool("enElAire", false);

            if (ySpeed < 0)
                ySpeed = -2f;

            velocity.x = dir.x * moveSpeed;
            velocity.z = dir.z * moveSpeed;

            if (isMoving)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            if (jumpPressed)
            {
                anim.SetTrigger("salto");
                anim.SetBool("enElAire", true);
                ySpeed = jumpForce;

                isJumping = true;
                velocity.x = 0;
                velocity.z = 0;
            }
            else
            {
                isJumping = false;
            }
        }
        else
        {
            anim.SetBool("enElAire", true);
        }

        anim.SetBool("caminando", isMoving && isGrounded);

        ySpeed += gravity * Time.deltaTime;
        velocity.y = ySpeed;

        controller.Move(velocity * Time.deltaTime);
    }

    // ============================================================
    // ⭐ MÉTODOS NUEVOS PARA VICTORIA Y DERROTA
    // ============================================================

    public void TriggerVictory()
    {
        hasWon = true;
        anim.SetTrigger("victoria");
        Debug.Log("ANIMACIÓN: Victoria activada");
    }

    public void TriggerDefeat()
    {
        hasLost = true;
        anim.SetTrigger("derrota");
        Debug.Log("ANIMACIÓN: Derrota activada");
    }

    // ============================================================

}
