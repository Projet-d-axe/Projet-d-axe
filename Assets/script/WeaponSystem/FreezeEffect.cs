using UnityEngine;
using System.Collections;

public class FreezeEffect : MonoBehaviour
{
    [Header("Freeze Settings")]
    public Material frozenMaterial;
    public Color freezeColor = Color.cyan;
    
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Color originalColor;
    private Rigidbody2D rb;
    private MonoBehaviour[] scripts;
    private bool isFrozen = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
            originalColor = spriteRenderer.color;
        }
        rb = GetComponent<Rigidbody2D>();
        scripts = GetComponents<MonoBehaviour>();
    }

    public void Freeze(float duration)
    {
        if (isFrozen) return;
        isFrozen = true;

        // Désactiver le Rigidbody2D
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Changer l'apparence
        if (spriteRenderer != null)
        {
            if (frozenMaterial != null)
            {
                spriteRenderer.material = frozenMaterial;
            }
            spriteRenderer.color = freezeColor;
        }

        // Désactiver tous les scripts sauf celui-ci
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }

        StartCoroutine(UnfreezeAfterDelay(duration));
    }

    private IEnumerator UnfreezeAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);
        Unfreeze();
    }

    private void Unfreeze()
    {
        if (!isFrozen) return;
        isFrozen = false;

        // Réactiver le Rigidbody2D
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Restaurer l'apparence
        if (spriteRenderer != null)
        {
            spriteRenderer.material = originalMaterial;
            spriteRenderer.color = originalColor;
        }

        // Réactiver tous les scripts
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = true;
            }
        }
    }
} 