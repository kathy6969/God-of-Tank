using UnityEngine;

public class TankController : MonoBehaviour
{
    public float moveSpeed = 5f;      // tốc độ di chuyển
    public float rotationSpeed = 150f; // tốc độ xoay (độ/giây)

    void Start()
    {
        
    }

    void Update()
    {
        // Input di chuyển tiến/lùi
        float moveInput = Input.GetAxisRaw("Vertical");   // W/S
        // Input xoay
        float rotateInput = Input.GetAxisRaw("Horizontal"); // A/D

        // Xoay tank quanh trục Z (2D)
        transform.Rotate(0, 0, -rotateInput * rotationSpeed * Time.deltaTime);

        // Di chuyển tiến/lùi theo hướng "forward" (trục lên của tank trong 2D là transform.up)
        transform.position += transform.up * moveInput * moveSpeed * Time.deltaTime;
    }
}
