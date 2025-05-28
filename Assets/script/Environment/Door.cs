using UnityEngine;

public class Door : MonoBehaviour
{
    public void Open()
    {
        gameObject.SetActive(false); // Désactive la porte
        // OU: GetComponent<Collider2D>().enabled = false;
        // OU: Animation/autres effets...
    }
}