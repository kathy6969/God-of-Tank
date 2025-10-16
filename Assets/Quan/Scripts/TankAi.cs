using UnityEngine;

public class TankAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public LayerMask wallLayer;

    private Rigidbody2D rb;

    [Header("Fire Settings")]
    public Transform gunBarrel;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float minRandomFireTime = 2f;
    public float maxRandomFireTime = 5f;
    public float minFireTime = 0.8f;

    [Header("Detection")]
    public Transform player;
    public float detectionRadius = 6f;
    public LayerMask playerLayer;

    [Header("AI Level (1–3)")]
    [Range(1, 3)] public int intelligenceLevel = 1;

    private float nextFireTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ScheduleNextFire();
    }

    void Update()
    {
        DetectPlayer();
        MoveForward();
        HandleFiring();
    }

    void MoveForward()
    {
        rb.MovePosition(rb.position + (Vector2)(gunBarrel.up * moveSpeed * Time.deltaTime));
    }

    void DetectPlayer()
    {
        Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (playerInRange != null)
        {
            // Có player -> xoay nòng về hướng player
            Vector2 direction = (player.position - gunBarrel.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float smooth = intelligenceLevel >= 2 ? 5f : 2f;
            float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * smooth);
            rb.MoveRotation(angle);
        }
    }

    void HandleFiring()
    {
        if (Time.time >= nextFireTime)
        {
            Fire();
            ScheduleNextFire();
        }
    }

    void Fire()
    {
        if (bulletPrefab == null || gunBarrel == null) return;

        GameObject bullet = Instantiate(bulletPrefab, gunBarrel.position, gunBarrel.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb != null)
        {
            bulletRb.linearVelocity = gunBarrel.up * bulletSpeed;
            bullet.transform.up = bulletRb.linearVelocity.normalized;
        }
    }

    void ScheduleNextFire()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRadius)
            nextFireTime = Time.time + minFireTime;
        else
            nextFireTime = Time.time + Random.Range(minRandomFireTime, maxRandomFireTime);
    }

    // 🔥 Khi va chạm với tường -> đổi hướng 0 / 90 / 180 / 270
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            float[] angles = { 0f, 90f, 180f, 270f };
            float randomAngle = angles[Random.Range(0, angles.Length)];
            transform.rotation = Quaternion.Euler(0, 0, randomAngle);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
