using UnityEngine;

public class TankShooter : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;   // chỗ gắn nòng súng
    public float bulletSpeed = 10f;
    public float fireCooldown = 0.5f;

    private float fireTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Xử lý bắn
        fireTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireCooldown;
        }
    }
    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = firePoint.up * bulletSpeed; // bắn theo hướng nòng súng
    }
}
