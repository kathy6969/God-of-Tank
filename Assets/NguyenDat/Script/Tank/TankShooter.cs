using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class TankShooter : MonoBehaviour
{
    public enum ShootMode
    {
        Single,                 // Bắn 1 viên
        Cone,                   // Bắn hình nón
        Line,                   // Bắn theo hàng dọc
        Bomb_ExplodeArea,       // 1 viên chậm, sau 2s nổ vùng cố định
        Bomb_Split4,            // 1 viên chậm, sau 2s tách ra 4 viên (tồn tại 2s)
        Bomb_LargeSplit6,       // 1 viên lớn tại vị trí bắn, sau 3s tách ra 6 viên
        Laser                   // Bắn tia laser
    }

    [Header("Bắn đạn")]
    public GameObject bulletPrefab;         // Prefab của đạn thường
    public Transform firePoint;             // Vị trí bắn
    public float bulletSpeed = 10f;         // Tốc độ đạn
    public bool doubleShoot = false;      // Bắn đôi

    [Header("Thời gian hồi chiêu cho từng loại đạn")]
    public float singleCooldown = 0.5f;
    public float coneCooldown = 0.8f;
    public float lineCooldown = 1.0f;
    public float bombExplodeAreaCooldown = 1.5f;
    public float bombSplit4Cooldown = 2.0f;
    public float bombLargeSplit6Cooldown = 2.5f;
    public float laserCooldown = 1.2f;

    public ShootMode shootMode = ShootMode.Single; // Chế độ bắn
    public float cooldownModifier = 1f; // Hệ số mặc định (1 = bình thường)

    [Header("Cài đặt bắn hình nón")]
    public int coneBulletCount = 3;         // Số lượng đạn trong hình nón
    public float coneAngle = 30f;           // Góc hình nón
    public float coneLifeTime = 3f;         // Thời gian tồn tại của đạn hình nón

    [Header("Cài đặt bắn theo hàng")]
    public float lineSpacing = 0.5f;        // Khoảng cách giữa các viên đạn
    public float lineLifeTime = 4f;         // Thời gian tồn tại của đạn hàng dọc

    [Header("Cài đặt Bomb (gán prefab)")]
    public GameObject bombPrefab;           // Prefab bomb (gắn script BombBullet)
    public GameObject splitChildPrefab;     // Prefab cho đạn con khi tách (4 hoặc 6)
    public GameObject explosionPrefab;      // Hiệu ứng nổ (nếu có)

    public float bombInitialSpeed = 2f;     // Tốc độ ban đầu của bomb khi bắn (0 nếu muốn đứng yên)
    public float bombExplodeDelay = 2f;     // Thời gian chờ nổ (mặc định 2s, dùng cho explodeArea & split4)
    public float bombSplit4ChildLife = 2f;  // Thời gian tồn tại của đạn con khi tách ra (2s)
    public float largeBombDelay = 3f;       // Thời gian chờ nổ của bomb lớn (3s cho large split6)

    [Header("Cài đặt Laser")]
    public LineRenderer laserLine;          // LineRenderer dùng cho laser
    public float laserLength = 10f;         // Độ dài tia laser
    public float laserDuration = 0.1f;      // Thời gian tồn tại của laser (0.1s)
    public LayerMask hitMask;               // Layer để raycast (Player có layer là Tank)
    public LayerMask wallMask;              // Layer để raycast (Tường có layer là Wall)
    private bool laserActive = false;       // Trạng thái laser đang hoạt động
    private float laserTimer = 0f;          // Bộ đếm thời gian cho laser

    public float fireTimer = 0f;           // Bộ đếm thời gian hồi bắn

    void Start()
    {
        // Lấy lựa chọn từ UI
        shootMode = (ShootMode)PlayerPrefs.GetInt("SelectedWeapon", 0);
    }
    void Update()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer < 0f) fireTimer = 0f;
        if (Input.GetKeyDown(KeyCode.Space) && fireTimer <= 0f)
        {
            if (doubleShoot == true)
            {
                StartCoroutine(DoubleShoot());
            }
            else
            {
                ShootOneTime();
            }
            fireTimer = GetCurrentCooldown() * cooldownModifier;
        }

        // Nếu laser đang hoạt động thì cập nhật vị trí mỗi frame
        if (laserActive)
        {
            laserTimer -= Time.deltaTime;

            Vector3 endPos = firePoint.position + firePoint.up * laserLength;

            // 1. Raycast check Wall trước
            RaycastHit2D wallHit = Physics2D.Raycast(firePoint.position, firePoint.up, laserLength, wallMask);

            if (wallHit.collider != null)
            {
                // Nếu trúng Wall thì dừng tại đó
                endPos = wallHit.point;
            }
            else
            {
                // 2. Nếu không trúng Wall thì mới check enemy
                RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.up, laserLength, hitMask);
                if (hit.collider != null)
                {
                    endPos = hit.point;
                    // hit.collider.GetComponent<EnemyHealth>()?.TakeDamage(laserDamage);
                }
            }

            // update line
            laserLine.SetPosition(0, firePoint.position);
            laserLine.SetPosition(1, endPos);

            if (laserTimer <= 0f)
            {
                DisableLaser();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("TestChoseBullet");
        }

    }

    public float GetCurrentCooldown()
    {
        float baseCooldown;
        switch (shootMode)
        {
            case ShootMode.Single: return singleCooldown;
            case ShootMode.Cone: return coneCooldown;
            case ShootMode.Line: return lineCooldown;
            case ShootMode.Bomb_ExplodeArea: return bombExplodeAreaCooldown;
            case ShootMode.Bomb_Split4: return bombSplit4Cooldown;
            case ShootMode.Bomb_LargeSplit6: return bombLargeSplit6Cooldown;
            case ShootMode.Laser: return laserCooldown;
            default: baseCooldown = 0.5f; break;
        }
        return baseCooldown;
    }
    // Hàm để thiết lập hệ số cooldown tạm thời
    public void SetCooldownModifier(float modifier, float duration)
    {
        StartCoroutine(ResetModifierAfterTime(modifier, duration));
    }
    private IEnumerator ResetModifierAfterTime(float modifier, float duration)
    {
        cooldownModifier = modifier;
        yield return new WaitForSeconds(duration);
        cooldownModifier = 1f; // trở lại bình thường
    }
    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;
        switch (shootMode)
        {
            case ShootMode.Single:
                SpawnBullet(firePoint.position, firePoint.rotation, bulletSpeed, -1f);
                break;

            case ShootMode.Cone:
                ShootCone();
                break;

            case ShootMode.Line:
                ShootLine();
                break;

            case ShootMode.Bomb_ExplodeArea:
                SpawnBomb(BombBullet.BombType.ExplodeArea, bombExplodeDelay, bombInitialSpeed, 0f, explosionPrefab, null);
                break;

            case ShootMode.Bomb_Split4:
                SpawnBomb(BombBullet.BombType.Split4, bombExplodeDelay, bombInitialSpeed, bombSplit4ChildLife, explosionPrefab, splitChildPrefab);
                break;

            case ShootMode.Bomb_LargeSplit6:
                SpawnBomb(BombBullet.BombType.LargeSplit6, largeBombDelay, 0f, -1f, explosionPrefab, splitChildPrefab);
                break;
            case ShootMode.Laser:
                ShootLaser();
                break;
        }
    }

    void ShootOneTime()
    {
        Shoot();
    }
    IEnumerator DoubleShoot()
    {
        Shoot();
        yield return new WaitForSeconds(0.2f);
        Shoot();
    }
    void ShootCone()
    {
        float startAngle = -coneAngle / 2f;
        float angleStep = coneBulletCount > 1 ? coneAngle / (coneBulletCount - 1) : 0f;

        for (int i = 0; i < coneBulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Quaternion rot = firePoint.rotation * Quaternion.Euler(0f, 0f, angle);
            SpawnBullet(firePoint.position, rot, bulletSpeed, coneLifeTime);
        }
    }

    void ShootLine()
    {
        int count = 3;
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = firePoint.up * (i * lineSpacing);
            Vector3 spawnPos = firePoint.position + offset;
            SpawnBullet(spawnPos, firePoint.rotation, bulletSpeed, lineLifeTime);
        }
    }

    void SpawnBullet(Vector3 pos, Quaternion rot, float speed, float lifeTime)
    {
        GameObject bullet = Instantiate(bulletPrefab, pos, rot);
        if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
        {
            // Sử dụng rb.linearVelocity theo Unity 6
            rb.linearVelocity = rot * Vector3.up * speed;
        }

        if (lifeTime > 0f)
            Destroy(bullet, lifeTime);
    }

    void SpawnBomb(BombBullet.BombType type, float delay, float initialSpeed, float childLifeTime, GameObject explosionFx, GameObject childPrefab)
    {
        if (bombPrefab == null)
        {
            Debug.LogWarning("Chưa gán bombPrefab!");
            return;
        }

        Vector3 spawnPos = firePoint.position;
        Quaternion spawnRot = firePoint.rotation;

        GameObject bomb = Instantiate(bombPrefab, spawnPos, spawnRot);

        // Cấu hình component BombBullet của bomb
        if (bomb.TryGetComponent<BombBullet>(out var bombScript))
        {
            bombScript.bombType = type;
            bombScript.delay = delay;
            bombScript.childPrefab = childPrefab;
            bombScript.explosionPrefab = explosionFx;
            bombScript.childLifeTime = childLifeTime;
            bombScript.childSpeed = bulletSpeed; // Tốc độ cho đạn con (có thể chỉnh)
            // Thiết lập vận tốc ban đầu
            if (bomb.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = spawnRot * Vector3.up * initialSpeed;
            }
        }
        else
        {
            Debug.LogWarning("bombPrefab không chứa script BombBullet.");
        }
    }

    // Bắn laser sử dụng LineRenderer và Raycast
    void ShootLaser()
    {
        if (laserLine == null) return;

        laserActive = true;
        laserTimer = laserDuration;

        laserLine.enabled = true;
    }

    void DisableLaser()
    {
        laserActive = false;
        if (laserLine != null)
            laserLine.enabled = false;
    }
}
