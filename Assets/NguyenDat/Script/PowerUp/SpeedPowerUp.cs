using UnityEngine;

public class SpeedPowerUp : MonoBehaviour
{
    public int speedMultiplier = 2; // Hệ số nhân tốc độ
    public float duration = 5f;     // Thời gian hiệu lực (giây)
    public SpriteRenderer spriteRenderer;
    public CircleCollider2D circleCollider2D;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (circleCollider2D == null)
        {
            circleCollider2D = GetComponent<CircleCollider2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tank"))
        {
            TankController tank = collision.GetComponent<TankController>();
            if (tank != null)
            {
                StartCoroutine(SpeedBoost(tank));
                // Vô hiệu hóa hiển thị và collider để tránh việc thu thập lại
                spriteRenderer.enabled = false;
                circleCollider2D.enabled = false;
            }
        }
    }
    private System.Collections.IEnumerator SpeedBoost(TankController tank)
    {
        tank.moveSpeed *= speedMultiplier; // Tăng tốc độ
        yield return new WaitForSeconds(duration); // Chờ trong thời gian hiệu lực
        tank.moveSpeed /= speedMultiplier; // Khôi phục tốc độ ban đầu
        Destroy(gameObject); // Hủy PowerUp sau khi sử dụng
    }
    
}
