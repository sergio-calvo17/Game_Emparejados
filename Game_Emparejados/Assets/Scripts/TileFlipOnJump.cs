using UnityEngine;
using System.Collections;

public class TileFlipOnJump : MonoBehaviour
{
    public float flipSpeed = 2f;

    // Nombre de la baldosa (se toma el nombre del GameObject)
    public string tileName;

    // Si es una baldosa trampa
    public bool isTrap = false;

    // Referencia al TileManager
    private TileManager tileManager;

    // Estados internos
    private bool isFlipped = false;  // Baldosa actualmente volteada
    private bool isLocked = false;   // Baldosa que no se puede volver a girar

    private Quaternion startRotation;
    private Quaternion flippedRotation;

    void Start()
    {
        tileManager = FindAnyObjectByType<TileManager>();

        tileName = gameObject.name;

        startRotation = transform.rotation;
        flippedRotation = Quaternion.Euler(startRotation.eulerAngles + new Vector3(180f, 0f, 0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null)
            return;

        if (!pc.isJumping)
            return;

        if (isLocked)  // Si ya quedó bloqueada, no se voltea
            return;

        if (!isFlipped)
        {
            StartCoroutine(FlipAnimation(startRotation, flippedRotation));
            isFlipped = true;

            if (tileManager != null)
                tileManager.RegisterTile(this);
        }
    }

    // Resetea la baldosa solo si NO está bloqueada
    public void ResetTile()
    {
        if (isLocked)
            return;

        StartCoroutine(FlipAnimation(flippedRotation, startRotation));
        isFlipped = false;
    }

    // Bloquea la baldosa para que quede destapada permanentemente
    public void LockTile()
    {
        isLocked = true;
        isFlipped = true; // Aseguramos que quede volteada
    }

    private IEnumerator FlipAnimation(Quaternion fromRot, Quaternion toRot)
    {
        float t = 0f;
        while (t < 1f)
        {
            transform.rotation = Quaternion.Lerp(fromRot, toRot, t);
            t += Time.deltaTime * flipSpeed;
            yield return null;
        }
        transform.rotation = toRot;
    }
}
