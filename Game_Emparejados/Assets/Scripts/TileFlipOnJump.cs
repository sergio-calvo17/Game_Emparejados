using UnityEngine;

public class TileFlipOnJump : MonoBehaviour
{
    public float flipSpeed = 2f;
    private bool alreadyFlipped = false;

    private Quaternion startRot;
    private Quaternion targetRot;

    void Start()
    {
        startRot = transform.rotation;
        targetRot = Quaternion.Euler(startRot.eulerAngles + new Vector3(180f, 0f, 0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController pc = other.GetComponent<PlayerController>();

        if (pc == null)
        {
            Debug.Log("El objeto que entró no tiene PlayerController.");
            return;
        }

        Debug.Log("Jugador saltando: " + pc.isJumping);

        if (pc.isJumping && !alreadyFlipped)
        {
            alreadyFlipped = true;
            StartCoroutine(FlipAnimation());
        }
    }

    private System.Collections.IEnumerator FlipAnimation()
    {
        float t = 0f;
        Quaternion initial = transform.rotation;

        while (t < 1f)
        {
            transform.rotation = Quaternion.Lerp(initial, targetRot, t);
            t += Time.deltaTime * flipSpeed;
            yield return null;
        }

        transform.rotation = targetRot;
    }
}
