using UnityEngine;
using System.Collections;

public class DoubleFirePowerUp : MonoBehaviour
{
    public float duration = 5f; // Thời gian hiệu lực của Power-Up
    public SpriteRenderer spriteRenderer; // Tham chiếu đến SpriteRenderer để ẩn Power-Up sau khi thu thập
    public CircleCollider2D circleCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (circleCollider == null)
        {
            circleCollider = GetComponent<CircleCollider2D>();
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
            // Kích hoạt hiệu ứng Power-Up
            TankShooter tankShooter = collision.GetComponent<TankShooter>();
            if (tankShooter != null)
            {
                tankShooter.doubleShoot = true;
                // Vô hiệu hóa hiển thị và collider của Power-Up
                spriteRenderer.enabled = false;
                circleCollider.enabled = false;
                // Bắt đầu hủy Power-Up sau một khoảng thời gian
                StartCoroutine(PowerUpDuration(tankShooter));
            }
        }
    }
    private IEnumerator PowerUpDuration(TankShooter tankShooter)
    {
        yield return new WaitForSeconds(duration);
        tankShooter.doubleShoot = false;
        Destroy(gameObject);
    }
}
