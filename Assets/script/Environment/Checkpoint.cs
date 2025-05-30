using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visual Effects")]
    public bool useVisualEffects = true;
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;
    public ParticleSystem activationEffect;

    [Header("Audio")]
    public AudioClip activationSound;
    [Range(0, 1)] public float volume = 0.7f;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isActivated = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null && activationSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = volume;
        }

        // Définir la couleur initiale
        if (useVisualEffects && spriteRenderer != null)
        {
            spriteRenderer.color = inactiveColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        // Désactiver tous les autres checkpoints
        Checkpoint[] allCheckpoints = FindObjectsOfType<Checkpoint>();
        foreach (Checkpoint checkpoint in allCheckpoints)
        {
            if (checkpoint != this)
            {
                checkpoint.Deactivate();
            }
        }

        // Activer ce checkpoint
        isActivated = true;

        // Sauvegarder la position
        CheckpointManager.Instance.SetCurrentCheckpoint(transform.position);

        // Effets visuels
        if (useVisualEffects)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = activeColor;
            }

            if (activationEffect != null)
            {
                activationEffect.Play();
            }
        }

        // Effet sonore
        if (activationSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(activationSound, volume);
        }
    }

    private void Deactivate()
    {
        isActivated = false;
        if (useVisualEffects && spriteRenderer != null)
        {
            spriteRenderer.color = inactiveColor;
        }
    }
} 