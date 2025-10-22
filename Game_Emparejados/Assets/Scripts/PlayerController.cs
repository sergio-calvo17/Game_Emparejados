using UnityEngine;
using UnityEngine.InputSystem; // <- NUEVO sistema de input

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Animator anim;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float x = 0f, z = 0f;

        // WASD
        if (Keyboard.current != null)
        {
            x += Keyboard.current.aKey.isPressed ? -1 : 0;
            x += Keyboard.current.dKey.isPressed ?  1 : 0;
            z += Keyboard.current.sKey.isPressed ? -1 : 0;
            z += Keyboard.current.wKey.isPressed ?  1 : 0;

            // Flechas
            x += Keyboard.current.leftArrowKey.isPressed  ? -1 : 0;
            x += Keyboard.current.rightArrowKey.isPressed ?  1 : 0;
            z += Keyboard.current.downArrowKey.isPressed  ? -1 : 0;
            z += Keyboard.current.upArrowKey.isPressed    ?  1 : 0;
        }

        Vector3 dir = new Vector3(Mathf.Clamp(x, -1, 1), 0f, Mathf.Clamp(z, -1, 1)).normalized;
        bool isMoving = dir.sqrMagnitude > 0.01f;

        // Animación
        if (anim != null) anim.SetBool("isWalking", isMoving);

        // Movimiento + rotación
        if (isMoving)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            controller.Move(dir * moveSpeed * Time.deltaTime);
        }

        // Gravedad
        if (controller.isGrounded && velocity.y < 0f) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
