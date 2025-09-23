using UnityEngine;

public class BulletHitContact : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tank"))
        {
            // Hủy object cha (bullet) khi va chạm
            if (transform.parent != null)
            {
                Debug.Log("Hit: Tank");
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
