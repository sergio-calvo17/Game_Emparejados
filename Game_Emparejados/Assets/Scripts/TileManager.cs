using UnityEngine;
using System.Collections;

public class TileManager : MonoBehaviour
{
    private TileFlipOnJump firstTile = null;
    private TileFlipOnJump secondTile = null;

    private bool checking = false; // Evita activar más de dos baldosas a la vez

    // Método que llama cada baldosa al voltearse
    public void RegisterTile(TileFlipOnJump tile)
    {
        if (checking) return; // Evita interferencias

        // Si es una baldosa trampa, la manejamos después
        if (tile.isTrap)
        {
            Debug.Log("Se piso una baldosa trampa.");
            return;
        }

        // Primera baldosa destapada
        if (firstTile == null)
        {
            firstTile = tile;
            Debug.Log("Primera baldosa: " + firstTile.tileName);
            return;
        }

        // Segunda baldosa destapada
        if (secondTile == null)
        {
            secondTile = tile;
            Debug.Log("Segunda baldosa: " + secondTile.tileName);

            // Bloqueamos nuevas activaciones mientras comparamos
            checking = true;
            StartCoroutine(CheckMatchDelayed());
        }
    }

    // Espera un pequeño tiempo para que la animación termine antes de comparar
    private IEnumerator CheckMatchDelayed()
    {
        yield return new WaitForSeconds(0.5f);

        CheckMatch();

        checking = false;
    }

    // Compara las dos baldosas
    private void CheckMatch()
    {
        if (firstTile.tileName == secondTile.tileName)
        {
            Debug.Log("Son iguales. Quedan destapadas.");

            firstTile.LockTile();
            secondTile.LockTile();
        }
        else
        {
            Debug.Log("Diferentes. Se vuelven a tapar.");

            firstTile.ResetTile();
            secondTile.ResetTile();
        }

        firstTile = null;
        secondTile = null;
    }
}
