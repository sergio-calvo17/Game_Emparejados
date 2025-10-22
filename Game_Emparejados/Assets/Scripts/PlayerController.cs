using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f, rotationSpeed = 12f, gravity = -9.81f;
    public float jumpForce = 4.5f;

    CharacterController controller;
    Animator anim;
    Vector3 vel;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // -------- entrada flechas/WASD --------
        float x = 0f, z = 0f;
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            x += Keyboard.current.leftArrowKey.isPressed ? -1 : 0;
            x += Keyboard.current.rightArrowKey.isPressed ? 1 : 0;
            z += Keyboard.current.downArrowKey.isPressed ? -1 : 0;
            z += Keyboard.current.upArrowKey.isPressed ? 1 : 0;

            x += Keyboard.current.aKey.isPressed ? -1 : 0;
            x += Keyboard.current.dKey.isPressed ? 1 : 0;
            z += Keyboard.current.sKey.isPressed ? -1 : 0;
            z += Keyboard.current.wKey.isPressed ? 1 : 0;
        }
#else
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
#endif
        Vector3 dir = new Vector3(x, 0, z).normalized;
        bool moving = dir.sqrMagnitude > 0.01f;

        // anim caminar
        anim.SetBool("caminando", moving);

        // rotar y mover
        if (moving)
        {
            Quaternion target = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
            controller.Move(dir * moveSpeed * Time.deltaTime);
        }

        // salto (espacio) solo animación; el impulso físico es opcional
        bool jumpPressed =
#if ENABLE_INPUT_SYSTEM
            Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
#else
            Input.GetKeyDown(KeyCode.Space);
#endif
        if (jumpPressed && controller.isGrounded)
        {
            anim.SetTrigger("salto");
            vel.y = jumpForce; // quita esta línea si no quieres salto físico
        }

        // gravedad
        if (controller.isGrounded && vel.y < 0) vel.y = -2f;
        vel.y += gravity * Time.deltaTime;
        controller.Move(vel * Time.deltaTime);

        // giros / victoria / derrota (teclas de prueba)
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame) anim.SetTrigger("giroIzq");
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) anim.SetTrigger("giroDch");
        if (Keyboard.current != null && Keyboard.current.vKey.wasPressedThisFrame) anim.SetTrigger("victoria");
        if (Keyboard.current != null && Keyboard.current.dKey.wasPressedThisFrame) anim.SetTrigger("derrota");
#else
        if (Input.GetKeyDown(KeyCode.Q)) anim.SetTrigger("giroIzq");
        if (Input.GetKeyDown(KeyCode.E)) anim.SetTrigger("giroDch");
        if (Input.GetKeyDown(KeyCode.V)) anim.SetTrigger("victoria");
        if (Input.GetKeyDown(KeyCode.D)) anim.SetTrigger("derrota");
#endif
    }
}
