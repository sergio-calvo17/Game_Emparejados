using UnityEngine;
using UnityEngine.UI;

public class Nube_Consejo : MonoBehaviour
{
    [Header("Imagen donde se mostrará la nube")]
    public Image nubeImage;  // Image de 'Rules'

    [Header("Sprites de las diferentes nubecitas")]
    public Sprite[] nubecitas;  // Reglas, Controles, Consejo...

    private int index = 0;

    private void Start()
    {
        if (nubeImage == null)
            nubeImage = GetComponent<Image>();

        if (nubecitas != null && nubecitas.Length > 0)
        {
            index = 0;
            nubeImage.sprite = nubecitas[index];
        }
    }

    public void NextNube()
    {
        if (nubecitas == null || nubecitas.Length == 0) return;

        index++;

        if (index >= nubecitas.Length)
            index = 0;

        nubeImage.sprite = nubecitas[index];
        Debug.Log("NextNube -> index: " + index);
    }

    public void PrevNube()
    {
        if (nubecitas == null || nubecitas.Length == 0) return;

        index--;

        if (index < 0)
            index = nubecitas.Length - 1;

        nubeImage.sprite = nubecitas[index];
        Debug.Log("PrevNube -> index: " + index);
    }
}