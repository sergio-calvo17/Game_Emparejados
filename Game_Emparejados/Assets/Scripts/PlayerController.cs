using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;      // Velocidad de movimiento
    public float rotationSpeed = 10f; // Velocidad de giro
    public float gravity = -9.81f;    // Gravedad simple

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
        // Detectar movimiento en los ejes horizontal (A/D) y vertical (W/S)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Crear vector de direccion segun camara o mundo
        Vector3 direction = new Vector3(x, 0f, z).normalized;

        // Hay movimiento?
        bool isMoving = direction.magnitude > 0.1f;

        // Actualizar animacion
        anim.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            // Calcular hacia donde mirar (rotacion suave)
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Mover en la direccion
            controller.Move(direction * moveSpeed * Time.deltaTime);
        }

        // Aplicar gravedad (mantener pegado al suelo)
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
