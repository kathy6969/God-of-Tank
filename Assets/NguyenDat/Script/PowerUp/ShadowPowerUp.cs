using UnityEngine;
using System.Collections;

public class ShadowPowerUp : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Tham chiếu đến SpriteRenderer để ẩn Power-Up sau khi thu thập
    public CircleCollider2D circleCollider;
    public float duration = 5f; // Thời gian hiệu lực của Power-Up
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tank"))
        {
            Collider2D tankCollier = collision.GetComponent<Collider2D>();
            if (tankCollier != null)
            {
                tankCollier.isTrigger = true;
                // Vô hiệu hóa hiển thị và collider của Power-Up
                spriteRenderer.enabled = false;
                circleCollider.enabled = false;
                // Bắt đầu hủy Power-Up sau một khoảng thời gian
                StartCoroutine(PowerUpDuration(tankCollier));
            }
        }
    }
    private IEnumerator PowerUpDuration(Collider2D tankCollier)
    {
        yield return new WaitForSeconds(duration);
        tankCollier.isTrigger = false;
        Destroy(gameObject);
    }
}
