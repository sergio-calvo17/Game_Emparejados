using UnityEngine;
using System.Collections;

public class TileFlipOnJump : MonoBehaviour
{
    public float flipSpeed = 2f;
    public string tileName;
    public bool isTrap = false;

    private TileManager tileManager;
    private bool isFlipped = false;
    private bool isLocked = false;

    private Quaternion startRotation;
    private Quaternion flippedRotation;

    public GameObject explosionPrefab;

    // Nuevo: audio de la baldosa trampa
    private AudioSource audioSource;

    private void Start()
    {
        tileManager = FindAnyObjectByType<TileManager>();
        tileName = gameObject.name;

        startRotation = transform.rotation;
        flippedRotation = Quaternion.Euler(startRotation.eulerAngles + new Vector3(180f, 0f, 0f));

        // Obtener AudioSource si existe
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null || !pc.isJumping) return;

        if (isTrap)
        {
            StartCoroutine(TriggerTrap(pc));
            return;
        }

        if (isLocked) return;

        if (!isFlipped)
        {
            StartCoroutine(FlipAnimation(startRotation, flippedRotation));
            isFlipped = true;

            if (tileManager != null)
                tileManager.RegisterTile(this);
        }
    }

    private IEnumerator TriggerTrap(PlayerController pc)
    {
        pc.hasLost = true;

        // Reproducir sonido de explosión de la baldosa
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Instanciar efecto visual de explosión
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        // Esperar un momento antes de mostrar derrota
        yield return new WaitForSeconds(0.6f);

        GameManager.Instance.MostrarDerrota();
    }

    public void ResetTile()
    {
        if (isLocked) return;

        StartCoroutine(FlipAnimation(flippedRotation, startRotation));
        isFlipped = false;
    }

    public void LockTile()
    {
        isLocked = true;
        isFlipped = true;
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
