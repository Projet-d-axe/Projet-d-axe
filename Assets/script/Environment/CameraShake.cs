using UnityEngine;

public class CameraShake : MonoBehaviour
{
   [SerializeField] private float ShakeAmount = 0.0001f;
   private Vector3 initialPosition;
    void Awake()
    {
        initialPosition = transform.localPosition;
    }
    public void Shake(float duration, float amount)
    {
        ShakeAmount = amount;
        InvokeRepeating(nameof(Update), 0f, 0.01f);
        Invoke(nameof(StopShake), duration);
    }
    void Update()
    {
        transform.position = initialPosition + Random.insideUnitSphere * ShakeAmount;
    }

    private void StopShake()
    {
        CancelInvoke(nameof(Update));
        transform.localPosition = initialPosition;
    }
}
