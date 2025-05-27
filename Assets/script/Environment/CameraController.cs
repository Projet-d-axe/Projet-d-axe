using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public Vector3 offset;

    [Header("Shake Settings")]
    [SerializeField] private float shakeAmount = 0.02f;
    private Vector3 initialPosition;
    private bool isShaking = false;

    void Awake()
    {
        initialPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        // Suivi normal du joueur
        Vector3 targetPosition = player.position + offset;
        
        // Appliquer le shake si actif
        if (isShaking)
        {
            targetPosition += Random.insideUnitSphere * shakeAmount;
        }
        
        transform.position = targetPosition;
    }

    // Méthode pour déclencher le shake (à appeler depuis le WeaponSystem du shotgun)
    public void Shake(float duration)
    {
        isShaking = true;
        Invoke(nameof(StopShake), duration);
    }

    private void StopShake()
    {
        isShaking = false;
        transform.localPosition = initialPosition;
    }
}