using UnityEngine;
using System.Collections;

public class RetractableSpike : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 3f;
    public LayerMask playerLayer;
    public float detectionInterval = 0.1f;
    public int numberOfRays = 8;
    
    [Header("Direction Settings")]
    [Range(0, 360)] public float detectionAngle = 180f;  // Angle total de détection
    [Range(-180, 180)] public float detectionDirection = 90f;  // 90 = haut, 0 = droite, -90 = bas, 180/-180 = gauche
    public bool showDetectionArea = true;

    [Header("Attack Settings")]
    public int damage = 20;
    public float extendSpeed = 10f;
    public float retractSpeed = 5f;
    public float extendHeight = 1f;  // Hauteur maximale du pic
    public float attackCooldown = 2f;

    [Header("Visual & Audio")]
    public GameObject spikeVisual;
    public ParticleSystem extendEffect;
    public AudioSource audioSource;
    public AudioClip extendSound;
    public AudioClip retractSound;

    private Vector3 initialPosition;
    private Vector3 extendedPosition;
    private bool isExtended = false;
    private bool isMoving = false;
    private float lastAttackTime;

    private void Start()
    {
        initialPosition = spikeVisual.transform.localPosition;
        StartCoroutine(DetectionRoutine());
        
        // Réinitialiser et appliquer la rotation
        spikeVisual.transform.localRotation = Quaternion.identity;
        float finalRotation = detectionDirection + 90f; // Ajout de 90° pour corriger l'orientation
        spikeVisual.transform.localRotation = Quaternion.Euler(0, 0, finalRotation);
        
        Debug.Log($"[RetractableSpike] Rotation appliquée : {finalRotation}");
    }

    private IEnumerator DetectionRoutine()
    {
        while (true)
        {
            if (!isMoving && Time.time >= lastAttackTime + attackCooldown)
            {
                DetectPlayer();
            }
            yield return new WaitForSeconds(detectionInterval);
        }
    }

    private void DetectPlayer()
    {
        float startAngle = detectionDirection - (detectionAngle / 2);
        float angleStep = detectionAngle / numberOfRays;
        
        for (int i = 0; i < numberOfRays; i++)
        {
            float currentAngle = startAngle + (i * angleStep);
            Vector2 direction = Quaternion.Euler(0, 0, currentAngle) * Vector2.right;
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, playerLayer);
            
            if (showDetectionArea)
            {
                Debug.DrawRay(transform.position, direction * detectionRange, Color.red, detectionInterval);
            }

            if (hit.collider != null)
            {
                StartCoroutine(ExtendSpike());
                break;
            }
        }
    }

    private IEnumerator ExtendSpike()
    {
        if (isMoving || isExtended) yield break;

        isMoving = true;
        
        // Calculer la direction d'extension
        Vector3 extendDirection = Quaternion.Euler(0, 0, detectionDirection) * Vector2.right;
        extendedPosition = initialPosition + extendDirection * extendHeight;

        // Effets d'extension
        if (extendEffect != null) extendEffect.Play();
        if (audioSource != null && extendSound != null) audioSource.PlayOneShot(extendSound);

        // Extension
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * extendSpeed;
            spikeVisual.transform.localPosition = Vector3.Lerp(initialPosition, extendedPosition, t);
            
            // Vérifier les collisions pendant l'extension
            Collider2D[] hits = Physics2D.OverlapCircleAll(spikeVisual.transform.position, 0.5f, playerLayer);
            foreach (Collider2D hit in hits)
            {
                iDamageable damageable = hit.GetComponent<iDamageable>();
                if (damageable != null)
                {
                    damageable.Damage(damage);
                }
            }
            
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Rétraction
        if (audioSource != null && retractSound != null) audioSource.PlayOneShot(retractSound);
        
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * retractSpeed;
            spikeVisual.transform.localPosition = Vector3.Lerp(extendedPosition, initialPosition, t);
            yield return null;
        }

        spikeVisual.transform.localPosition = initialPosition;
        lastAttackTime = Time.time;
        isMoving = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDetectionArea) return;

        // Visualiser la zone de détection dans l'éditeur
        Gizmos.color = Color.yellow;
        
        // Dessiner l'arc de détection
        Vector3 center = transform.position;
        float radius = detectionRange;
        int segments = 32;
        float angleStep = detectionAngle / segments;
        float startAngle = detectionDirection - (detectionAngle / 2);
        
        for (int i = 0; i < segments; i++)
        {
            float angle1 = startAngle + (i * angleStep);
            float angle2 = startAngle + ((i + 1) * angleStep);
            
            Vector3 point1 = center + (Quaternion.Euler(0, 0, angle1) * Vector3.right * radius);
            Vector3 point2 = center + (Quaternion.Euler(0, 0, angle2) * Vector3.right * radius);
            
            Gizmos.DrawLine(center, point1);
            Gizmos.DrawLine(point1, point2);
        }

        // Dessiner la direction d'extension
        if (spikeVisual != null)
        {
            Gizmos.color = Color.red;
            Vector3 extendDirection = Quaternion.Euler(0, 0, detectionDirection) * Vector2.right;
            Gizmos.DrawLine(spikeVisual.transform.position, 
                spikeVisual.transform.position + extendDirection * extendHeight);
        }
    }
} 