using UnityEngine;
using UnityEngine.InputSystem; // <- NUEVO sistema

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
        // Leer WASD / flechas con el nuevo sistema
        float x = 0f, z = 0f;

        if (Keyboard.current != null) {
            x = (Keyboard.current.aKey.isPressed ? -1 : 0) + (Keyboard.current.dKey.isPressed ? 1 : 0);
            z = (Keyboard.current.sKey.isPressed ? -1 : 0) + (Keyboard.current.wKey.isPressed ? 1 : 0);
        }

        // También admite flechas:
        if (Keyboard.current != null) {
            x += (Keyboard.current.leftArrowKey.isPressed ? -1 : 0) + (Keyboard.current.rightArrowKey.isPressed ? 1 : 0);
            z += (Keyboard.current.downArrowKey.isPressed ? -1 : 0) + (Keyboard.current.upArrowKey.isPressed ? 1 : 0);
        }

        Vector3 dir = new Vector3(Mathf.Clamp(x, -1, 1), 0f, Mathf.Clamp(z, -1, 1)).normalized;
        bool isMoving = dir.sqrMagnitude > 0.01f;
        anim.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            Quaternion target = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
            controller.Move(dir * moveSpeed * Time.deltaTime);
        }

        if (controller.isGrounded && velocity.y < 0f) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
