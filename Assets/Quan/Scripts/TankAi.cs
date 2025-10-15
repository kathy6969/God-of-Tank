using UnityEngine;

public class TankAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 180f;
    public float detectionRadius = 10f;
    public float dodgeRadius = 5f;
    public float dodgeForce = 5f;
    public LayerMask projectileLayer;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    [Header("Fire Settings")]
    public Transform gunBarrel; // ✅ đầu nòng súng
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float minRandomFireTime = 2f;
    public float maxRandomFireTime = 5f;
    public float minFireTime = 0.8f;

    [Header("Player Detection")]
    public Transform player;
    public LayerMask playerLayer;
    public float aimSmooth = 5f;

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
        MoveRandomly();
        RotateTowardsTarget();
        DodgeIncomingProjectiles();
        HandleFiring();
    }

    void MoveRandomly()
    {
        if (Random.value < 0.01f)
            moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
    }

    void RotateTowardsTarget()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * aimSmooth);
        rb.MoveRotation(angle);
    }

    void DodgeIncomingProjectiles()
    {
        if (intelligenceLevel < 2) return;

        Collider2D[] projectiles = Physics2D.OverlapCircleAll(transform.position, dodgeRadius, projectileLayer);
        foreach (Collider2D proj in projectiles)
        {
            Vector2 dodgeDir = (Vector2)(transform.position - proj.transform.position).normalized;
            rb.AddForce(dodgeDir * dodgeForce, ForceMode2D.Impulse);
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
            bulletRb.linearVelocity = gunBarrel.up * bulletSpeed; // hướng bắn của nòng
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, dodgeRadius);
    }
}
