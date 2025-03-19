using UnityEngine;

public class Mouvement : MonoBehaviour
{
    public float speed = 5f;
    public float jump = 8f;
    private Rigidbody2D rb;
    private Collider2D[] colls;
    private CapsuleCollider2D monColl;
    private bool grounded;
    private float rayonDetection;
    private SpriteRenderer skin;
    private Animator anim;
    private int jumpCount;
    public int maxJumpCount = 1;
    private bool isPaused = false;
    public CoinManager cm;
    public XPSystem xpSystem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D non trouvé sur l'objet !");
        }

        monColl = GetComponent<CapsuleCollider2D>();
        skin = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        jumpCount = 0;

        if (xpSystem == null)
        {
            Debug.LogError("manaSystem n'est pas assigné !");
        }
    }

    void Update()
    {
        if (isPaused)
        {
            Debug.Log("Le jeu est en pause");
            return;
        }

        // Vérifie si la touche M est pressée
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Touche M pressée");

            // Vérifie si manaSystem est assigné
            if (xpSystem == null)
            {
                Debug.LogError("manaSystem n'est pas assigné !");
                return;
            }

        }

        // Autres fonctions de mouvement
        groundCheck();
        moveCheck();
        flipCheck();
        animCheck();
    }

    void groundCheck()
    {
        grounded = false;
        rayonDetection = monColl.size.x * 0.45f;
        colls = Physics2D.OverlapCircleAll(
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
        rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.linearVelocity.y);
        if (Input.GetButtonDown("Jump") && (grounded || jumpCount < maxJumpCount))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump);
            jumpCount++;
        }
    }

    private void flipCheck()
    {
        if (Input.GetAxis("Horizontal") < 0)
            skin.flipX = true;
        else if (Input.GetAxis("Horizontal") > 0)
            skin.flipX = false;
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

    public void SetPaused(bool paused)
    {
        isPaused = paused;
        if (isPaused && rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Arrête le mouvement
        }
        Debug.Log("Jeu en pause : " + paused);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("coin") && cm != null)
        {
            Destroy(other.gameObject);
            cm.CollectCoin();
        }
    }
}