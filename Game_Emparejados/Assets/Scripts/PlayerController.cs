using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float rotationSpeed = 10f;
    public float gravity = 0.01f;
    public float jumpForce = 4f;

    CharacterController controller;
    Animator anim;
    Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // NO tocar la posición aquí por ahora.
        // Asegúrate en el Inspector que el Player esté en (0, 1.2, 0) aprox.
    }

    void Update()
    {
        float x = 0f, z = 0f;
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed)  x -= 1f;
            if (Keyboard.current.rightArrowKey.isPressed) x += 1f;
            if (Keyboard.current.upArrowKey.isPressed)    z += 1f;
            if (Keyboard.current.downArrowKey.isPressed)  z -= 1f;

            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.wKey.isPressed) z += 1f;
            if (Keyboard.current.sKey.isPressed) z -= 1f;
        }
#else
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
#endif
        Vector3 dir = new Vector3(x, 0f, z).normalized;
        bool moving = dir.sqrMagnitude > 0.01f;
        if (anim) anim.SetBool("caminando", moving);

        if (moving)
        {
            Quaternion target = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
            controller.Move(dir * moveSpeed * Time.deltaTime);
        }

        // Salto (Espacio)
        bool jumpPressed =
#if ENABLE_INPUT_SYSTEM
            Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
#else
            Input.GetKeyDown(KeyCode.Space);
#endif
        if (jumpPressed && controller.isGrounded)
        {
            if (anim) anim.SetTrigger("salto");
            velocity.y = jumpForce;
        }

        // Gravedad
        if (controller.isGrounded && velocity.y < 0f) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
