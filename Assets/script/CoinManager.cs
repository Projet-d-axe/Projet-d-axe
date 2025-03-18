using UnityEngine;
using UnityEngine.UI; // Nécessaire pour utiliser l'UI

public class CoinManager : MonoBehaviour
{
    public int coinCount = 0;
    public Text coinText; // Référence au texte UI pour afficher le score

    // Méthode pour collecter une pièce
    public void CollectCoin()
    {
        coinCount++; // Incrémenter le compteur de pièces
        UpdateCoinText(); // Mettre à jour le texte UI
        Debug.Log("Pièce collectée ! Total : " + coinCount);
    }

    // Méthode pour mettre à jour le texte UI
    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Pièces : " + coinCount;
        }
    }
}