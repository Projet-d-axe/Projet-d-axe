using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Vector3 lastCheckpointPosition;
    private bool hasCheckpoint;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[CheckpointManager] Instance créée");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        hasCheckpoint = true;
        Debug.Log($"[CheckpointManager] Nouveau checkpoint défini à {position}");
    }

    public void RespawnPlayer()
    {
        if (!hasCheckpoint)
        {
            Debug.LogWarning("[CheckpointManager] Pas de checkpoint actif !");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[CheckpointManager] Joueur non trouvé !");
            return;
        }

        Debug.Log($"[CheckpointManager] Début de la téléportation du joueur de {player.transform.position} à {lastCheckpointPosition}");

        // Désactiver temporairement les composants
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        CapsuleCollider2D collider = player.GetComponent<CapsuleCollider2D>();
        PlayerController controller = player.GetComponent<PlayerController>();

        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
            Debug.Log("[CheckpointManager] Rigidbody désactivé");
        }

        if (collider != null)
        {
            collider.enabled = false;
            Debug.Log("[CheckpointManager] Collider désactivé");
        }

        if (controller != null)
        {
            controller.enabled = false;
            Debug.Log("[CheckpointManager] Controller désactivé");
        }

        // Téléporter le joueur
        player.transform.position = lastCheckpointPosition;
        Debug.Log($"[CheckpointManager] Position du joueur mise à jour : {player.transform.position}");

        // Réactiver les composants
        if (rb != null)
        {
            rb.simulated = true;
            Debug.Log("[CheckpointManager] Rigidbody réactivé");
        }

        if (collider != null)
        {
            collider.enabled = true;
            Debug.Log("[CheckpointManager] Collider réactivé");
        }

        if (controller != null)
        {
            controller.enabled = true;
            controller.ResetState();
            Debug.Log("[CheckpointManager] Controller réactivé et réinitialisé");
        }

        // Réinitialiser la santé
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ResetHealth();
            Debug.Log("[CheckpointManager] Santé réinitialisée");
        }

        Debug.Log("[CheckpointManager] Téléportation terminée");
    }
} 