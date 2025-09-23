using UnityEngine;

public class RoundBullet : MonoBehaviour
{
    public float lifeTime = 5f;
    private int bounceCount = 0;
    public int maxBounce = 5;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            bounceCount++;
            if (bounceCount > maxBounce-1)
            {
                Destroy(gameObject);
            }
        }
    }
}
