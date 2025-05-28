using UnityEngine;

public class Crystal : MonoBehaviour
{
    [Header("Activation Settings")]
    public bool canBeActivated = true;
    public bool isOneTimeUse = true;
    public float activationDelay = 0f;

    [Header("Effects")]
    public ParticleSystem activationEffect;
    public AudioClip activationSound;
    public Color activatedColor = Color.blue;
    
    [Header("Objects to Activate")]
    public GameObject[] objectsToActivate;
    public MonoBehaviour[] componentsToEnable;

    private AudioSource audioSource;
    private bool isActivated = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void Activate()
    {
        if (!canBeActivated || (isOneTimeUse && isActivated)) return;

        isActivated = true;
        Invoke("PerformActivation", activationDelay);
        Debug.Log("Crystal activated: " + gameObject.name);
    }

    private void PerformActivation()
    {
        if (activationEffect != null) activationEffect.Play();
        if (activationSound != null) audioSource.PlayOneShot(activationSound);
        if (spriteRenderer != null) spriteRenderer.color = activatedColor;

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null) obj.SetActive(true);
        }

        foreach (MonoBehaviour component in componentsToEnable)
        {
            if (component != null) component.enabled = true;
        }

        if (isOneTimeUse) canBeActivated = false;
    }

    //Méthode pour réinitialiser le crystal
    public void ResetCrystal()
    {
        isActivated = false;
        canBeActivated = true;
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (var obj in objectsToActivate)
        {
            if (obj != null) Gizmos.DrawLine(transform.position, obj.transform.position);
        }
    }
}