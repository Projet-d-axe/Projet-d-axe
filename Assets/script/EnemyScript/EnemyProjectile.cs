using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float moveSpeed;
    private int damage;

    [SerializeField] private float trajectoryMaxHigh;
    private float trajectoryRelativeMaxHigh;
    [SerializeField] private AnimationCurve trajectory;
    [SerializeField] private AnimationCurve axisCorrection;

    private Vector3 trajectoryStartPoint;
    private Vector3 trajectoryEndPoint;
    private Vector3 trajectoryRange;
    private float direction;

    private iDamageable targetDamageable;

    private void Start()
    {
        trajectoryRange = trajectoryEndPoint - trajectoryStartPoint;
        direction = Mathf.Sign(trajectoryRange.x);
        trajectoryRelativeMaxHigh = Mathf.Abs(trajectoryRange.x) * trajectoryMaxHigh;
    }

    private void Update()
    {
        UpdateProjectilePosition();

        if (Vector3.Distance(transform.position, trajectoryEndPoint) < 1f)
        {
            if (targetDamageable != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, ((MonoBehaviour)targetDamageable).transform.position);

                if (distanceToTarget < 1f) // Adjust radius if needed
                {
                    targetDamageable.Damage(damage);
                }
            }

            Destroy(gameObject);
        }
    }

    private void UpdateProjectilePosition()
    {
        float nextPositionX = transform.position.x + moveSpeed * direction * Time.deltaTime;
        float t = Mathf.InverseLerp(trajectoryStartPoint.x, trajectoryEndPoint.x, nextPositionX);

        float heightOffset = trajectory.Evaluate(t) * trajectoryRelativeMaxHigh;
        float yCorrection = axisCorrection.Evaluate(t) * trajectoryRange.y;

        float nextPositionY = trajectoryStartPoint.y + heightOffset + yCorrection;

        Vector3 newPos = new Vector3(nextPositionX, nextPositionY, 0);
        transform.position = newPos;
    }

    public void InitializeProjectile(Transform target, float moveSpeed, int damage)
    {
        this.moveSpeed = moveSpeed;
        this.damage = damage;

        trajectoryStartPoint = transform.position;
        trajectoryEndPoint = target.position;
        targetDamageable = target.GetComponent<iDamageable>();
    }
}
