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

    private float maxXP; // XP nécessaire pour le niveau actuel

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

    // Méthode pour utiliser un point de compétence
    public void UseSkillPoint()
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            Debug.Log("Point de compétence utilisé. Points restants : " + skillPoints);
            // Ajoutez ici la logique pour améliorer les compétences du joueur
        }
        else
        {
            Debug.Log("Pas de points de compétence disponibles !");
        }

        UpdateUI(); // Met à jour l'interface utilisateur
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
    }
}