using UnityEngine;

public class move : MonoBehaviour
{
    public float speed = 5f;
    public float jump = 8f;
    public int maxJumpCount = 1;
    public float dashSpeed = 20f; // Vitesse fixe pour le dash
    public float dashDuration = 0.2f; // Durée du dash
    public float dashCooldown = 1f; // Temps de recharge du dash
    public float crouchSpeedMultiplier = 0.5f; // Multiplicateur de vitesse en accroupi
    private Rigidbody2D rb;
    private CapsuleCollider2D monColl;
    private bool grounded;
    private float rayonDetection;
    private SpriteRenderer skin;
    private Animator anim;
    private int jumpCount;
    private Vector3 monScale;
    private float lastDashTime = -Mathf.Infinity; // Temps du dernier dash
    private bool isDashing = false; // Indique si le joueur est en train de dash
    private bool isCrouching = false; // Indique si le joueur est accroupi

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
        if (!isDashing) // Empêche les autres mouvements pendant le dash
        {
            groundCheck();
            moveCheck();
            flipCheck();
            animCheck();
            crouchCheck(); // Vérifie l'état de l'accroupissement
        }
        dashCheck();
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

        // Applique le multiplicateur de vitesse si le joueur est accroupi
        float currentSpeed = isCrouching ? speed * crouchSpeedMultiplier : speed;

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump") && (grounded || jumpCount < maxJumpCount))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump); // Applique la vitesse du saut
            jumpCount++;
            Debug.Log("Jump performed. Remaining jumps: " + (maxJumpCount - jumpCount));
        }
    }

    void crouchCheck()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.LeftControl)) // Touche pour s'accroupir
        {
            isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.LeftControl)) // Touche pour se relever
        {
            isCrouching = false;
        }
    }

    void dashCheck()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && (Time.time >= lastDashTime + dashCooldown))
        {
            float direction = Input.GetAxis("Horizontal"); // Direction actuelle du joueur
            if (direction == 0) direction = 1; // Par défaut, dash vers la droite si le joueur est immobile

            StartCoroutine(PerformDash(direction)); // Démarre le dash
        }
    }

    private System.Collections.IEnumerator PerformDash(float direction)
    {
        isDashing = true; // Active le dash
        lastDashTime = Time.time; // Enregistre le temps du dernier dash

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            rb.linearVelocity = new Vector2(dashSpeed * direction, rb.linearVelocity.y); // Applique la vitesse du dash
            yield return null; // Attend la prochaine frame
        }

        isDashing = false; // Désactive le dash
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
        anim.SetBool("isCrouching", isCrouching); // Ajoutez un paramètre "isCrouching" dans votre Animator
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
        GUILayout.Label($"isCrouching = {isCrouching}");
        GUILayout.EndVertical();
    }
}