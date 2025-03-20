using UnityEngine;
using UnityEngine.UI;

public class XPSystem : MonoBehaviour
{
    public float baseMaxXP = 100f; // XP de base nécessaire pour le premier niveau
    public float currentXP; // XP actuelle
    public int currentLevel = 1; // Niveau actuel
    public int skillPoints = 0; // Points de compétences disponibles
    public float xpGainRate = 10f; // Quantité d'XP gagnée à chaque action

    public Slider xpBar; // Référence à la barre d'XP UI
    public Text levelText; // Référence au texte du niveau UI
    public Text skillPointsText; // Référence au texte des points de compétences UI
    public Text strengthText; // Référence au texte de la force UI
    public Text agilityText; // Référence au texte de l'agilité UI
    public Text fireRateText; // Référence au texte de la vitesse de tir UI

    private float maxXP; // XP nécessaire pour le niveau actuel

    // Statistiques du joueur
    public int strength = 0;
    public int agility = 0;
    public float fireRate = 1.0f; // Vitesse de tir (remplace Intelligence)

    void Start()
    {
        maxXP = baseMaxXP; // Initialise l'XP nécessaire pour le premier niveau
        currentXP = 0f; // Initialise l'XP à 0
        UpdateUI(); // Met à jour l'interface utilisateur
    }

    void Update()
    {
        // Exemple : Gagner de l'XP en appuyant sur une touche (à adapter à votre jeu)
        if (Input.GetKeyDown(KeyCode.P))
        {
            GainXP(xpGainRate);
        }
    }

    // Méthode pour gagner de l'XP
    public void GainXP(float amount)
    {
        currentXP += amount;
        Debug.Log("XP gagnée : " + amount + ", XP actuelle : " + currentXP);

        // Vérifie si le joueur a atteint le niveau suivant
        if (currentXP >= maxXP)
        {
            LevelUp();
        }

        UpdateUI(); // Met à jour l'interface utilisateur
    }

    // Méthode pour monter de niveau
    void LevelUp()
    {
        currentLevel++; // Augmente le niveau
        skillPoints++; // Donne un point de compétence
        currentXP = 0f; // Réinitialise l'XP pour le prochain niveau
        maxXP *= 1.5f; // Augmente l'XP nécessaire pour le prochain niveau (exemple : +50%)

        Debug.Log("Niveau supérieur atteint ! Niveau actuel : " + currentLevel);

        UpdateUI(); // Met à jour l'interface utilisateur
    }

    // Méthodes pour attribuer des points de stats
    public void AddStrength()
    {
        if (skillPoints > 0)
        {
            strength++;
            skillPoints--;
            Debug.Log("Force augmentée ! Force actuelle : " + strength);
            UpdateUI();
        }
    }

    public void AddAgility()
    {
        if (skillPoints > 0)
        {
            agility++;
            skillPoints--;
            Debug.Log("Agilité augmentée ! Agilité actuelle : " + agility);
            UpdateUI();
        }
    }

    public void AddFireRate() // Remplace Intelligence par Vitesse de tir
    {
        if (skillPoints > 0)
        {
            fireRate += 0.1f; // Augmente la vitesse de tir de 0.1
            skillPoints--;
            Debug.Log("Vitesse de tir augmentée ! Vitesse de tir actuelle : " + fireRate);
            UpdateUI();
        }
    }

    // Met à jour l'interface utilisateur
    void UpdateUI()
    {
        // Met à jour la barre d'XP
        if (xpBar != null)
        {
            xpBar.value = currentXP / maxXP;
        }

        // Met à jour le texte du niveau
        if (levelText != null)
        {
            levelText.text = "Niveau : " + currentLevel;
        }

        // Met à jour le texte des points de compétences
        if (skillPointsText != null)
        {
            skillPointsText.text = "Points de compétence : " + skillPoints;
        }

        // Met à jour les statistiques
        if (strengthText != null)
        {
            strengthText.text = "Force : " + strength;
        }
        if (agilityText != null)
        {
            agilityText.text = "Agilité : " + agility;
        }
        if (fireRateText != null)
        {
            fireRateText.text = "Vitesse de tir : " + fireRate.ToString("F1"); // Affiche 1 chiffre après la virgule
        }
    }
}