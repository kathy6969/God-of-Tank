using UnityEngine;

public class ShieldPowerUp : MonoBehaviour
{
    public GameObject shieldPrefab; // Prefab của lá chắn
    public float duration = 5f;
    public SpriteRenderer spriteRenderer;
    public CircleCollider2D circleCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tank"))
        {
            Transform tankTransform = collision.transform;
            // Tạo lá chắn tại vị trí của tank
            GameObject shield = Instantiate(shieldPrefab, tankTransform.position, Quaternion.identity);
            // Gắn lá chắn làm con của tank để nó di chuyển cùng tank
            shield.transform.SetParent(tankTransform);
            // Vô hiệu hóa hiển thị và collider của Power-Up
            spriteRenderer.enabled = false;
            circleCollider.enabled = false;
            // Bắt đầu hủy Power-Up sau một khoảng thời gian
            Destroy(gameObject, duration);
        }
    }
}
