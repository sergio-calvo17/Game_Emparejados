using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        // Obtiene el componente Animator del Player
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Detecta si hay movimiento horizontal (flechas o A/D)
        float move = Input.GetAxisRaw("Horizontal");

        // Si el valor es distinto de 0, activa caminar
        anim.SetBool("isWalking", move != 0);
    }
}
