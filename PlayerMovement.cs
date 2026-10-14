using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpPower = 7f;
    public AudioClip jumpSound;
    public AudioClip attackSound;
    public GameObject deathEffect;
    public GameObject swordObject;
    public TextMeshProUGUI deathText;

    private Rigidbody2D rb;
    private Collider2D coll;
    private float horizontalInput;
    private bool isGrounded = false;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isDead = false;
    private bool isAttacking = false;
    private Vector3 respawnPoint;
    private int deathCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        respawnPoint = transform.position;

        if (swordObject != null)
        {
            swordObject.SetActive(false);
        }

        UpdateDeathText();
    }

    void Update()
    {
        if (isDead) return;

        CheckGrounded();

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (anim != null)
        {
            anim.SetBool("isRunning", horizontalInput != 0);
        }

        if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
            if (swordObject != null && !isAttacking)
            {
                float swordX = Mathf.Abs(swordObject.transform.localPosition.x);
                swordObject.transform.localPosition = new Vector3(swordX, swordObject.transform.localPosition.y, 0f);
            }
        }
        else if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
            if (swordObject != null && !isAttacking)
            {
                float swordX = Mathf.Abs(swordObject.transform.localPosition.x);
                swordObject.transform.localPosition = new Vector3(-swordX, swordObject.transform.localPosition.y, 0f);
            }
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);

            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }

        if (Input.GetKeyDown(KeyCode.Z) && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }

        if (transform.position.y < -10f)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
    }

    void CheckGrounded()
    {
        isGrounded = false;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != coll && !hit.collider.isTrigger)
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            respawnPoint = collision.transform.position;
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        deathCount++;
        UpdateDeathText();

        spriteRenderer.enabled = false;
        rb.simulated = false;

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Invoke("Respawn", 1f);
    }

    void Respawn()
    {
        transform.position = respawnPoint;
        spriteRenderer.enabled = true;
        rb.simulated = true;
        isDead = false;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        if (swordObject != null)
        {
            swordObject.SetActive(true);

            float startAngle = spriteRenderer.flipX ? 45f : -45f;
            float endAngle = spriteRenderer.flipX ? 135f : -135f;

            float duration = 0.15f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float currentZ = Mathf.Lerp(startAngle, endAngle, elapsed / duration);
                swordObject.transform.localRotation = Quaternion.Euler(0, 0, currentZ);
                yield return null;
            }

            swordObject.SetActive(false);
        }
        isAttacking = false;
    }

    void UpdateDeathText()
    {
        if (deathText != null)
        {
            deathText.text = "Death : " + deathCount;
        }
    }
}