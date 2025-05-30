using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [Header("Respawn Settings")]
    public float respawnDelay = 1f;
    public GameObject respawnEffect;

    private Vector3 currentCheckpoint;
    private bool hasCheckpoint = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentCheckpoint(Vector3 position)
    {
        currentCheckpoint = position;
        hasCheckpoint = true;
        Debug.Log($"[CheckpointManager] Nouveau checkpoint défini à {position}");
    }

    public void RespawnPlayer()
    {
        if (!hasCheckpoint)
        {
            Debug.LogWarning("[CheckpointManager] Tentative de respawn sans checkpoint actif");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            StartCoroutine(RespawnRoutine(player));
        }
        else
        {
            Debug.LogError("[CheckpointManager] Player non trouvé !");
        }
    }

    private System.Collections.IEnumerator RespawnRoutine(GameObject player)
    {
        // Désactiver les contrôles du joueur
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Attendre le délai de respawn
        yield return new WaitForSeconds(respawnDelay);

        // Téléporter le joueur au checkpoint
        player.transform.position = currentCheckpoint;

        // Effet de respawn
        if (respawnEffect != null)
        {
            Instantiate(respawnEffect, currentCheckpoint, Quaternion.identity);
        }

        // Réactiver les contrôles du joueur
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Restaurer la santé du joueur
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }
    }
} 