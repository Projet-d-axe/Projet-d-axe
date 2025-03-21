using UnityEngine;

public class Mouvement : MonoBehaviour
{
    public float speed = 5f;
    public float jump = 8f;
    private Rigidbody2D rb;
    private CapsuleCollider2D monColl;
    private bool grounded;
    private float rayonDetection;
    private SpriteRenderer skin;
    private Animator anim;
    private int jumpCount;
    public int maxJumpCount = 1;
    private Vector3 monScale;
    public float dash = 10f;

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
        
        groundCheck();
        moveCheck();
        flipCheck();
        animCheck();
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
    rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y); // Utilisez rb.velocity au lieu de rb.linearVelocity

    if (Input.GetButtonDown("Jump") && (grounded || jumpCount < maxJumpCount))
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump); // Appliquer la vitesse de saut
        jumpCount++;
    }
}

void dashCheck()
{
    if (Input.GetKeyDown(KeyCode.M))
    {
        float dashSpeed = 20f; // Vitesse fixe pour le dash
        rb.linearVelocity = new Vector2(dashSpeed * Mathf.Sign(rb.linearVelocity.x), rb.linearVelocity.y); // Conserve la direction actuelle
    }
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
    private void OnGUI()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"Jump = {jump}");
        GUILayout.Label($"Jump = {jumpCount}");
        GUILayout.Label($"Speed = {speed}");
        GUILayout.Label($"dash = {dash}");
        GUILayout.Label($"Grounded = {grounded}");
        GUILayout.Label($"keydash = {Input.GetKeyDown(KeyCode.M)}");
        GUILayout.EndVertical();

    }
}