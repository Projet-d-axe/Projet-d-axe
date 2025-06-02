using UnityEngine;

public class Stalactite : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float destroyDelay = 0.5f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool showDetectionRange = true;

    [Header("Fall Settings")]
    [SerializeField] private float fallSpeed = 15f;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeTime = 0.5f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private AudioClip fallSound;

    private bool hasDamaged = false;
    private bool isFalling = false;
    private bool hasDetectedPlayer = false;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private Vector3 initialPosition;

    private void Start()
    {
        // Ajouter un AudioSource si on a des sons
        if ((impactSound != null || fallSound != null) && GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // Son 3D
            audioSource.playOnAwake = false;
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0; // Désactiver la gravité au début
        rb.constraints = RigidbodyConstraints2D.FreezeAll; // Geler la position et rotation

        initialPosition = transform.position;
    }

    private void Update()
    {
        if (!isFalling && !hasDetectedPlayer)
        {
            CheckForPlayer();
        }
    }

    private void CheckForPlayer()
    {
        // Utiliser un overlap circle pour détecter le joueur dans une zone en dessous
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position + Vector3.down * (detectionRange / 2), 
            detectionRange / 2
        );

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag(targetTag))
            {
                hasDetectedPlayer = true;
                StartFalling();
                break;
            }
        }
    }

    private void StartFalling()
    {
        if (isFalling) return;

        isFalling = true;
        
        // Jouer le son de chute
        if (fallSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(fallSound);
        }

        // Secouer légèrement avant la chute
        LeanTween.moveX(gameObject, initialPosition.x + shakeIntensity, shakeTime)
            .setEaseShake()
            .setOnComplete(() => {
                // Activer la chute
                rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
                rb.gravityScale = fallSpeed;
            });
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasDamaged) return; // Éviter les dégâts multiples

        // Vérifier si l'objet touché peut prendre des dégâts
        if (collision.gameObject.CompareTag(targetTag))
        {
            iDamageable damageable = collision.gameObject.GetComponent<iDamageable>();
            if (damageable != null)
            {
                damageable.Damage(damage);
                hasDamaged = true;
            }
        }

        // Jouer les effets
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        if (impactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(impactSound);
        }

        // Détruire la stalactite après un délai
        Destroy(gameObject, destroyDelay);
    }

    private void OnDrawGizmosSelected()
    {
        if (showDetectionRange)
        {
            Gizmos.color = Color.red;
            // Dessiner la zone de détection circulaire
            Gizmos.DrawWireSphere(transform.position + Vector3.down * (detectionRange / 2), detectionRange / 2);
        }
    }
} 