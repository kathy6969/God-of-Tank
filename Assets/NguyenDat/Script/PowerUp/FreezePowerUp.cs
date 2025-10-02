using UnityEngine;

public class FreezePowerUp : MonoBehaviour
{
    public float duration = 5f;
    public GameObject freezeEffect;
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
            Instantiate(freezeEffect,transform.position, Quaternion.identity);
            // Vô hiệu hóa hiển thị và collider của Power-Up
            spriteRenderer.enabled = false;
            circleCollider.enabled = false;
            // Bắt đầu hủy Power-Up sau một khoảng thời gian
            Destroy(gameObject, duration);
        }
    }
}
