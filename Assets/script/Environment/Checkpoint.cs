using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visual Feedback")]
    public bool activateOnTouch = true;
    public GameObject activeVisual;
    public GameObject inactiveVisual;
    public ParticleSystem activationEffect;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip activationSound;

    private bool isActivated = false;

    private void Start()
    {
        // Configuration initiale
        UpdateVisuals();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activateOnTouch || isActivated) return;

        if (other.CompareTag("Player"))
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        if (isActivated) return;

        isActivated = true;
        
        // Sauvegarder la position
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SetCheckpoint(transform.position);
            Debug.Log($"[Checkpoint] Checkpoint activé à {transform.position}");
        }
        else
        {
            Debug.LogError("[Checkpoint] CheckpointManager non trouvé !");
            return;
        }

        // Effets visuels
        UpdateVisuals();
        if (activationEffect != null)
        {
            activationEffect.Play();
        }

        // Effet sonore
        if (audioSource != null && activationSound != null)
        {
            audioSource.PlayOneShot(activationSound);
        }
    }

    private void UpdateVisuals()
    {
        if (activeVisual != null)
            activeVisual.SetActive(isActivated);
        
        if (inactiveVisual != null)
            inactiveVisual.SetActive(!isActivated);
    }

    // Pour activer/désactiver manuellement le checkpoint (utile pour les éditeurs de niveau)
    public void SetActivated(bool activated)
    {
        isActivated = activated;
        UpdateVisuals();
    }
} 