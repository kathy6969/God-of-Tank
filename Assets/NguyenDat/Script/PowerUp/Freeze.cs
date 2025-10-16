using UnityEngine;
using System.Collections;

public class Freeze : MonoBehaviour
{
    private TankController tankController;
    private TankShooter tankShooter;
    private bool isInside = false;   // kiểm tra tank còn trong vùng không
    private float timer = 0f;
    private bool isFrozen = false;

    void Start()
    {
        Destroy(gameObject, 8f);
    }

    void Update()
    {
        if (isInside && tankController != null && tankShooter != null)
        {
            timer += Time.deltaTime;

            if (!isFrozen && timer >= 2f) // sau 2s thì đóng băng
            {
                StartCoroutine(FreezeTank());
            }
        }
    }

    private IEnumerator FreezeTank()
    {
        isFrozen = true;
        timer = 0f;

        // Tắt control
        tankController.enabled = false;
        tankShooter.enabled = false;

        yield return new WaitForSeconds(1f); // đóng băng 1 giây

        // Bật lại
        tankController.enabled = true;
        tankShooter.enabled = true;

        isFrozen = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tank"))
        {
            tankController = collision.GetComponent<TankController>();
            tankShooter = collision.GetComponent<TankShooter>();
            isInside = true;
            timer = 0f; // reset khi vừa vào vùng
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tank"))
        {
            isInside = false;
            timer = 0f;
            StopAllCoroutines(); // dừng nếu đang đóng băng dở
            if (tankController != null) tankController.enabled = true;
            if (tankShooter != null) tankShooter.enabled = true;
            tankController = null;
            tankShooter = null;
        }
    }
}
