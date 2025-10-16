using UnityEngine;

public class TankAI_2D_Vision : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 180f;
    public float detectionRadius = 8f;     // phạm vi tìm player
    public float wallAvoidDistance = 1.2f; // khoảng cách tránh tường
    public LayerMask wallLayer;
    public string playerTag = "Player";

    [Header("Fire Settings")]
    public Transform gunBarrel;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float fireRate = 2f;
    private float nextFireTime;

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 moveDir;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject foundPlayer = GameObject.FindGameObjectWithTag(playerTag);
        if (foundPlayer != null) player = foundPlayer.transform;
    }

    void Update()
    {
        DetectPlayer();
        MoveForward();
        AvoidWalls();
        HandleFiring();
    }

    void DetectPlayer()
    {
        if (player == null) return;

        // Quét vùng xung quanh tank
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(playerTag))
            {
                Vector2 dirToPlayer = (hit.transform.position - transform.position).normalized;
                float targetAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg - 90f;

                // Quay mượt về phía player
                float angle = Mathf.LerpAngle(rb.rotation, targetAngle, Time.deltaTime * rotationSpeed);
                rb.MoveRotation(angle);

                moveDir = transform.up; // Cập nhật hướng đi
                return;
            }
        }
    }

    void MoveForward()
    {
        rb.MovePosition(rb.position + (Vector2)transform.up * moveSpeed * Time.deltaTime);
    }

    void AvoidWalls()
    {
        // Quét quanh chính thân tank, không phải đầu nòng
        Vector2 center = transform.position;
        Collider2D wallHit = Physics2D.OverlapCircle(center, wallAvoidDistance, wallLayer);

        if (wallHit != null)
        {
            // Khi va chạm tường -> xoay sang 1 hướng mới (90°, 180°, -90°)
            float[] possibleTurns = { -90f, 90f, 180f };
            float randomTurn = possibleTurns[Random.Range(0, possibleTurns.Length)];

            float newAngle = rb.rotation + randomTurn;
            rb.MoveRotation(newAngle);

            moveDir = transform.up; // Cập nhật hướng mới
        }
    }

    void HandleFiring()
    {
        if (player == null) return;

        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        if (bulletPrefab == null || gunBarrel == null) return;

        GameObject bullet = Instantiate(bulletPrefab, gunBarrel.position, gunBarrel.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
            bulletRb.linearVelocity = gunBarrel.up * bulletSpeed;
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ vùng phát hiện player
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Vẽ vùng tránh tường (quanh thân tank)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wallAvoidDistance);
    }
}
