using UnityEngine;

public class BulletHitContact : MonoBehaviour
{
    private RoundBullet parentBullet;

    void Start()
    {
        parentBullet = GetComponentInParent<RoundBullet>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tank"))
        {
            // Xử lý trúng tank
            Debug.Log("Tank bị trúng đạn!");
            Destroy(parentBullet.gameObject);
        }
    }
}
