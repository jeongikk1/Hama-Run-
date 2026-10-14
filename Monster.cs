using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{
    public int hp = 3;
    public float speed = 2f;
    public float moveDistance = 2f;

    private float startX;
    private int direction = -1;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isDead = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        startX = transform.position.x;
    }

    void Update()
    {
        if (isDead) return;

        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if (transform.position.x <= startX - moveDistance)
        {
            direction = 1;
        }
        else if (transform.position.x >= startX + moveDistance)
        {
            direction = -1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Mathf.Abs(collision.contacts[0].normal.x) > 0.5f)
        {
            direction *= -1;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.Die();
            }
        }
    }

    public void TakeDamage()
    {
        if (isDead) return;

        hp--;

        if (hp <= 0)
        {
            isDead = true;
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(HitEffectRoutine());
        }
    }

    IEnumerator HitEffectRoutine()
    {
        spriteRenderer.color = Color.white;
        Vector3 originalPos = transform.position;

        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Random.Range(-0.1f, 0.1f);
            float offsetY = Random.Range(-0.1f, 0.1f);
            transform.position = new Vector3(originalPos.x + offsetX, originalPos.y + offsetY, originalPos.z);
            yield return null;
        }

        transform.position = originalPos;
        spriteRenderer.color = originalColor;
    }
}