using UnityEngine;

public class move : MonoBehaviour
{
    public float speed = 5f;
    public float jump = 8f;
    public int maxJumpCount = 1;
    public float dashSpeed = 20f; // Vitesse fixe pour le dash
    public float dashCooldown = 1f; // Temps de recharge du dash
    private Rigidbody2D rb;
    private CapsuleCollider2D monColl;
    private bool grounded;
    private float rayonDetection;
    private SpriteRenderer skin;
    private Animator anim;
    private int jumpCount;
    private Vector3 monScale;
    private float lastDashTime = -Mathf.Infinity; // Temps du dernier dash

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        monColl = GetComponent<CapsuleCollider2D>();
        skin = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        monScale = transform.localScale; // Initialiser l'échelle d'origine
    }

    void Update()
    {
        dashCheck();
        groundCheck();
        moveCheck();
        flipCheck();
        animCheck();
    }

    void groundCheck()
    {
        grounded = false;
        rayonDetection = monColl.size.x * 0.45f;
        Collider2D[] colls = Physics2D.OverlapCircleAll(
            (Vector2)transform.position + Vector2.up * (monColl.offset.y + rayonDetection * 0.8f - (monColl.size.y / 2)),
            rayonDetection
        );
        foreach (Collider2D coll in colls)
        {
            if (coll != monColl && !coll.isTrigger)
            {
                grounded = true;
                jumpCount = 0;
                break;
            }
        }
    }

    void moveCheck()
{
    float moveInput = Input.GetAxis("Horizontal");
    rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y); // Use rb.velocity

    if (Input.GetButtonDown("Jump") && (grounded || jumpCount < maxJumpCount))
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump); // Apply jump velocity
        jumpCount++;
        Debug.Log("Jump performed. Remaining jumps: " + (maxJumpCount - jumpCount));
    }
}

    void dashCheck()
    {
    if ((Input.GetKeyDown(KeyCode.LeftShift)) && (Time.time >= lastDashTime + dashCooldown))
    {
        float direction = Mathf.Sign(rb.linearVelocity.x); // Current direction of the player
        if (direction == 0) direction = 1; // Default to dash right if the player is stationary
        rb.linearVelocity = new Vector2(dashSpeed * direction, rb.linearVelocity.y); // Apply dash
        lastDashTime = Time.time; // Record the time of the last dash
        Debug.Log("Dash performed. Speed: " + rb.linearVelocity);
    }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
    {
        Debug.Log("Dash on cooldown. Time remaining: " + (lastDashTime + dashCooldown - Time.time));
    }
    Debug.Log("Dash performed. Speed: " + rb.linearVelocity);
    }

    private void flipCheck()
    {
        if (Input.GetAxis("Horizontal") < 0)
            transform.localScale = new Vector3(-Mathf.Abs(monScale.x), monScale.y, monScale.z); // Inversion sûre
        else if (Input.GetAxis("Horizontal") > 0)
            transform.localScale = monScale; // Rétablir l'échelle d'origine
    }

    private void animCheck()
    {
        anim.SetFloat("velocityX", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("velocityY", rb.linearVelocity.y);
        anim.SetBool("grounded", grounded);
    }

    private void OnDrawGizmos()
    {
        if (monColl == null)
            monColl = GetComponent<CapsuleCollider2D>();

        rayonDetection = monColl.size.x * 0.45f;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(
            (Vector2)transform.position + Vector2.up * (monColl.offset.y + rayonDetection * 0.8f - (monColl.size.y / 2)),
            rayonDetection
        );
    }
     private void OnGUI()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"Jump = {jump}");
        GUILayout.Label($"Jump = {jumpCount}");
        GUILayout.Label($"Speed = {speed}");
        GUILayout.Label($"dash = {dashSpeed}");
        GUILayout.Label($"dashCooldown = {dashCooldown}");
        GUILayout.Label($"Grounded = {grounded}");
        GUILayout.Label($"keydash = {Input.GetKeyDown(KeyCode.LeftShift)}");
        GUILayout.EndVertical();

    }
}