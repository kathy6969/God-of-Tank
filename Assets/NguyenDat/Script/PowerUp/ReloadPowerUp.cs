using UnityEngine;

public class ReloadPowerUp : MonoBehaviour
{
    
    public float duration = 5f;         // Thời gian hiệu lực (giây
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
            TankShooter tank = collision.GetComponent<TankShooter>();
            if (tank != null)
            {
                tank.SetCooldownModifier(0.5f, duration); // Giảm thời gian hồi bắn còn 50% trong 5 giây
                spriteRenderer.enabled = false;
                circleCollider2D.enabled = false;
                Destroy(gameObject, 5f); // Xóa power-up sau 5 giây để tránh va chạm lại
            }
        }
    }
}
