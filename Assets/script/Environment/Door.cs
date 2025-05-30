using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private bool isLocked = true;
    [SerializeField] private string targetSceneName = "SampleScene";
    [SerializeField] private Sprite activatedSprite;
    [SerializeField] private Sprite deactivatedSprite;
    
    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateDoorAppearance();
    }

    public void Activate()
    {
        if (isLocked)
        {
            isLocked = false;
            isActivated = true;
            UpdateDoorAppearance();
        }
    }

    private void UpdateDoorAppearance()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isActivated ? activatedSprite : deactivatedSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isLocked && isActivated)
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
        }
    }
}